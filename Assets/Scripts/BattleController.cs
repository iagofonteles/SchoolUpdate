using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public static BattleController self;
    public static bool isInBattle { get; private set; }

    public static Battler[] enemy = new Battler[1];
    public static Battler[] ally = new Battler[1];

    Timer a_atk_timer = new Timer(), e_atk_timer = new Timer();
    public static Vector3 aa_ppoint, ee_ppoint;
    BattleUiPower aa_power, ee_power;

    private void Start()
    {
        self = this;

        aa_ppoint = transform.Find("a power point").position;
        ee_ppoint = transform.Find("e power point").position;
        aa_power = transform.Find("a power").GetComponent<BattleUiPower>();
        ee_power = transform.Find("e power").GetComponent<BattleUiPower>();
        aa_power.gameObject.SetActive(false);
        ee_power.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    public static void StartBattle()
    {
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
    }

    private void Update() {
        if (!isInBattle) return;

        if(e_atk_timer[3]) ee_power.Attack(1, Random.Range(0, 9), 80, ee_ppoint, (aa_ppoint - ee_ppoint).normalized, 1);
        if(a_atk_timer[3]) aa_power.Attack(0, Random.Range(0, 9), PlayerStats.magic_power, aa_ppoint, (ee_ppoint - aa_ppoint).normalized, 1);
    }

    private void OnGUI()
    {
        if (!isInBattle) return;

        // draw teams
        transform.Find("bar player").GetComponent<RectTransform>().sizeDelta = new Vector2(140*ally[0].cur_hp / ally[0].max_hp, 22);
        transform.Find("bar enemy").GetComponent<RectTransform>().sizeDelta = new Vector2(140*enemy[0].cur_hp / enemy[0].max_hp, 22);
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
