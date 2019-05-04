using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    public static BattleController.Battler battler;
    public static PlayerController self;

    private void Awake()
    {
        iPower.Init();
        QuestionsDB.Init();
    }

    private void Start()
    {
        battler = new BattleController.Battler(1000, 140, 3, Resources.Load<Texture2D>("sprites/spr_avatar"));
        self = this;
    }

    void Update()
    {
        var v = new Vector3(
            Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0, 0,
            Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0);
        if(!BattleController.isInBattle) transform.Translate(v.normalized * moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "enemy")
            BattleController.StartBattle(collision.gameObject);
    }

    public static Vector3 hDirection(Transform t) {
        var d = (self.transform.position - t.position).normalized;
        d.y = 0; return d;
    }
    }
