using UnityEngine;
using UnityEngine.UI;

public class BattleUiPower : MonoBehaviour
{
    //private int _magic;
    private int _magic, _powerId, _powerType;
    public int magic { get => _magic; set { _magic = value; GetComponentInChildren<Text>().text = value.ToString(); } }
    public int powerId { get => _powerId; set {
            _powerId = value;
            GetComponentInChildren<Image>().sprite = iPower.icon[value];
            _powerType = Mathf.FloorToInt(value / 3);
        } }
    public int team;
    public Vector2 direction;
    public float speed;
    private bool slow = false;
    private ParticleSystem spark;

    private void Start()
    {
        spark = GetComponentInChildren<ParticleSystem>();
        spark.gameObject.SetActive(false);
    }

    public void FadeOut() {
        spark.gameObject.SetActive(false);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    void Update() {
        var _move = (slow ? 30 : speed) * Time.deltaTime;
        transform.Translate(_move * direction, Space.World);
        transform.GetChild(0).Rotate(0, 0, -1f * speed * Time.deltaTime);

        // hit target
        if (Vector2.Distance(transform.position, team == 0 ? BattleController.ee_ppoint : BattleController.aa_ppoint) < _move) {
            if (team == 0) BattleController.enemy.cur_hp -= magic; else BattleController.ally.cur_hp -= magic;
            FadeOut();
        }
        slow = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        var p = collision.gameObject.GetComponent<BattleUiPower>();
        if (p.team != team && p._powerType == _powerType)
            spark.gameObject.SetActive(true);
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        var p = collision.gameObject.GetComponent<BattleUiPower>();
        if (team==p.team || _powerType != p._powerType) return;

        slow = true;
        if (p.magic>0 && magic>0) {
            magic -= 1; p.magic -= 1;
        }
        if (p.magic < 1) {
            spark.gameObject.SetActive(false);
            p.FadeOut();
        }
    }
}
