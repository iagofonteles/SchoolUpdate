using UnityEngine;
using UnityEngine.UI;

public class BattleUiPower : MonoBehaviour
{
    //private int _magic;
    private int _magic;
    private int magic { get => _magic; set { _magic = value; GetComponentInChildren<Text>().text = value.ToString(); } }
    private int _powerId;
    private int powerId { get => _powerId; set { _powerId = value; GetComponentInChildren<Image>().sprite = PlayerStats.power_icons[value]; } }
    private int team;
    private Vector2 direction;
    private float speed;
    bool slow = false;
    private ParticleSystem spark;

    private void Start()
    {
        spark = GetComponentInChildren<ParticleSystem>();
        spark.gameObject.SetActive(false);
    }

    public void Attack(int team, int powerId, int magic, Vector2 origin, Vector2 direction, float speed)
    {
        gameObject.SetActive(true);
        this.team = team;
        this.powerId = powerId;
        this.magic = magic;
        //GetComponentInChildren<Image>().sprite = PlayerStats.power_icons[powerId];

        transform.position = origin;
        this.direction = direction;
        this.speed = speed * 300;
    }

    public void FadeOut() {
        spark.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    void Update() {
        if (magic < 1) FadeOut();
        transform.Translate((slow ? 30 : speed) * direction * Time.deltaTime, Space.World);
        transform.GetChild(0).Rotate(0, 0, -1f * speed * Time.deltaTime);

        // hit target
        /*
        if (Vector2.Distance(transform.position, team == 0 ? BattleController.ee_ppoint : BattleController.aa_ppoint) <= speed) {
            if (team == 0) BattleController.enemy[0].cur_hp -= magic; else BattleController.ally[0].cur_hp -= magic;
            FadeOut();
        }*/
        slow = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) => spark.gameObject.SetActive(true); 

    private void OnTriggerStay2D(Collider2D collision)
    {
        slow = true;
        //if (gameObject.tag == "enemy") return;

        var p = collision.gameObject.GetComponent<BattleUiPower>();
        if (p.magic>0 && magic>0) {
            magic -= 1;
            p.magic -= 1;
        }
        if (p.magic < 1) spark.gameObject.SetActive(false);
        //if (p.magic <= 0) p.FadeOut();
        //if (magic <= 0) FadeOut();
    }
}
