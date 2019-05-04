using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public static BattleController self;
    public static bool isInBattle { get; private set; }

    public static Battler enemy, ally;

    private GameObject enemyObject;
    private Timer e_atk_timer = new Timer();
    private Image e_charging_power_img;
    private Image[] a_charge_power = new Image[3];



    private Animator a_animator, e_animator;
    private RectTransform e_healthbar, a_healthbar;
    private int e_charging_power_id;
    public static Vector3 aa_ppoint, ee_ppoint;

    private GameObject prefabPower;
    private PowerButton[] btn_power;

    private void Start()
    {
        self = this;

        btn_power = new PowerButton[3] { new PowerButton(0), new PowerButton(1), new PowerButton(2) };
        prefabPower = Resources.Load<GameObject>("prefabPower");

        var r = transform.Find("spr player").GetComponent<RectTransform>();
        aa_ppoint = r.position + (Vector3.up * r.rect.height / 2);
        r = transform.Find("spr enemy").GetComponent<RectTransform>();
        ee_ppoint = r.position + (Vector3.up * r.rect.height / 2);

        e_charging_power_img = transform.Find("img holder").GetChild(0).GetComponent<Image>();
        e_charging_power_id = iPower.GetRandom;
        e_charging_power_img.sprite = iPower.icon[e_charging_power_id];
        a_charge_power[0] = transform.Find("charge power0").GetComponent<Image>();
        a_charge_power[1] = transform.Find("charge power1").GetComponent<Image>();
        a_charge_power[2] = transform.Find("charge power2").GetComponent<Image>();
        a_healthbar = transform.Find("bar player").GetComponent<RectTransform>();
        e_healthbar = transform.Find("bar enemy").GetComponent<RectTransform>();
        a_animator = transform.Find("spr player").GetComponent<Animator>();
        e_animator = transform.Find("spr enemy").GetComponent<Animator>();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        //if (!isInBattle) return;

        // enemy attack
        if (e_atk_timer[3]) {
            e_animator.Play("attack");
            Attack(1, e_charging_power_id, 80, 1);
            e_charging_power_id = iPower.GetRandom;
            e_charging_power_img.sprite = iPower.icon[e_charging_power_id];
        }
        e_charging_power_img.color = new Color(1, 1, 1, e_atk_timer.time / 3);

        for (var i = 0; i < btn_power.Length; i++) {
            btn_power[i].Decrease();
            a_charge_power[i].fillAmount = btn_power[i].progress;
        }

        a_healthbar.sizeDelta = new Vector2(140 * ally.cur_hp / ally.max_hp, 22);
        e_healthbar.sizeDelta = new Vector2(140 * enemy.cur_hp / enemy.max_hp, 22);

        // end of battle
        if (enemy.cur_hp < 1 || ally.cur_hp<2) EndBattle();
    }

    public static void StartBattle(GameObject enemyObject)
    {
        self.enemyObject = enemyObject;
        self.gameObject.GetComponent<Image>().sprite = MenuLayout.menuBackground.sprite();
        enemy = new Battler(1000, 80, Random.Range(0, 3), Resources.Load<Texture2D>("sprites/spr_enemy1"));
        ally = PlayerController.battler;
        isInBattle = true;
        self.gameObject.SetActive(true);
    }

    public void EndBattle() {
        PlayerStats.power_experience[Random.Range(0, 9)] += Random.Range(8, 24);
        isInBattle = false;

        //clear and reset things
        var h = transform.Find("power holder");
        foreach (Transform c in h) Destroy(c.gameObject);
        foreach (PowerButton p in btn_power) p.progress = 0;
        e_atk_timer.Reset();

        Destroy(enemyObject);
        gameObject.SetActive(false);
    }

    public void PowerClick(int btnId) {
        var pow = btn_power[btnId].Increase();
        if (pow>=0) Attack(0, pow, PlayerStats.magic_power, 1);
    }

    private class PowerButton
    {
        private int powerType;
        private int powerID = 0;
        public float progress = 0; // current bar filled
        private float depletion = 3; // time in seconds for a full gauge to deplete
        private float gain = .20f; // fill amout per click

        public PowerButton(int type)
        {
            powerType = type;
            powerID = type * 3 + Random.Range(0, 3);
            GameObject.Find("btn power" + powerType).GetComponent<Image>().sprite = iPower.icon[powerID];
        }
        public void Decrease() { progress = Mathf.Max(0, progress - Time.deltaTime / depletion); }
        public int Increase()
        {
            if ((progress += gain) >= 1) {
                progress = 0;
                var _oldpower = powerID;
                powerID = Random.Range(0, 3) + powerType * 3;
                GameObject.Find("btn power" + powerType).GetComponent<Image>().sprite = iPower.icon[powerID];
                return _oldpower;
            } else return -1;
        }
    }

    public void Attack(int team, int powerId, int magic, float speed)
    {
        a_animator.Play("attack");
        var obj = Instantiate(prefabPower, transform.Find("power holder")).GetComponent<BattleUiPower>();
        // Debug.LogError("attack");
        //gameObject.SetActive(true);

        obj.team = team;
        obj.powerId = powerId;
        obj.magic = magic;

        obj.transform.position = team == 0 ? aa_ppoint : ee_ppoint;
        //obj.transform.position = Vector3.zero;
        obj.direction = (team == 0 ? ee_ppoint - aa_ppoint : aa_ppoint - ee_ppoint).normalized;
        obj.speed = speed * 300;
    }

    public struct Battler
    {
        public int max_hp;
        public int cur_hp;
        public int magic;
        public int type;

        public Texture2D sprite;

        public Battler(int hp,int magic, int type, Texture2D spr) {
            max_hp = hp; cur_hp = hp;
            this.magic = magic;
            this.type = type;
            sprite = spr;
        }
        public Battler(Battler b) { this = b; }
        
    }
}
