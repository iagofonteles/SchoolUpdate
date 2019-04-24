using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public static BattleController self;
    public static bool isInBattle { get; private set; }

    public static Battler[] enemy = new Battler[1];
    public static Battler[] ally = new Battler[1];

    private GameObject enemyObject;
    private Timer e_atk_timer = new Timer();
    private Image e_charging_power_img;
    private int e_charging_power_id;
    public static Vector3 aa_ppoint, ee_ppoint;
    protected static BattleUiPower aa_power, ee_power;

    private GameObject prefabPower;
    private PowerButton[] btn_power;

    private void Start()
    {
        self = this;

        btn_power = new PowerButton[3] { new PowerButton(0), new PowerButton(1), new PowerButton(2) };
        prefabPower = Resources.Load<GameObject>("prefabPower");

        aa_ppoint = transform.Find("spr player").position;
        ee_ppoint = transform.Find("spr enemy").position;
        e_charging_power_img = transform.Find("img holder").GetChild(0).GetComponent<Image>();
        e_charging_power_id = iPower.GetRandom;
        e_charging_power_img.sprite = iPower.icon[e_charging_power_id];
        gameObject.SetActive(false);
    }

    private void Update()
    {
        //if (!isInBattle) return;

        if (e_atk_timer[3]) {
            Attack(1, e_charging_power_id, 80, 1);
            e_charging_power_id = iPower.GetRandom;
            e_charging_power_img.sprite = iPower.icon[e_charging_power_id];
        }
        e_charging_power_img.color = new Color(1, 1, 1, e_atk_timer.time / 3);

        for (var i = 0; i < btn_power.Length; i++) btn_power[i].Decrease();

        // end of battle
        if (enemy[0].cur_hp < 1 || ally[0].cur_hp<2) EndBattle();
    }

    public static void StartBattle(GameObject enemyObject)
    {
        self.enemyObject = enemyObject;
        self.gameObject.GetComponent<Image>().sprite = MenuLayout.menuBackground.sprite();
        enemy = new Battler[] { new Battler(1000, 80, Random.Range(0, 3), Resources.Load<Texture2D>("sprites/spr_enemy1")) };
        ally = new Battler[] { PlayerController.battler };
        isInBattle = true;
        self.gameObject.SetActive(true);
    }

    public void EndBattle() {
        PlayerStats.power_experience[Random.Range(0, 9)] += Random.Range(8, 24);
        isInBattle = false;
        gameObject.SetActive(false);
        Destroy(enemyObject);
        var h = transform.Find("power holder");
        while (h.childCount > 0) Destroy(h.GetChild(0).gameObject);
    }

    public void PowerClick(int btnId) {
        var pow = btn_power[btnId].Increase();
        if (pow>=0) Attack(0, pow, PlayerStats.magic_power, 1);
    }

    private void OnGUI()
    {
        //if (!isInBattle) return;

        // draw teams
        transform.Find("bar player").GetComponent<RectTransform>().sizeDelta = new Vector2(140*ally[0].cur_hp / ally[0].max_hp, 22);
        transform.Find("bar enemy").GetComponent<RectTransform>().sizeDelta = new Vector2(140*enemy[0].cur_hp / enemy[0].max_hp, 22);
    }

    private class PowerButton
    {
        private int powerType;
        private int powerID = 0;
        private float progress = 0;
        private float depletion = 3;
        private float gain = .25f;

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
