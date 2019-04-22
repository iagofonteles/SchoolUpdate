using UnityEngine;
using UnityEngine.UI;

public static class BattleController
{
    public static bool isInBattle { get; private set; }

    public static Battler[] enemy = new Battler[1];
    public static Battler[] ally = new Battler[1];

    private static Texture2D hpbar;

    public static Transform battleScene;

    public static void Init()
    {
        hpbar = Resources.Load<Texture2D>("sprites/spr_healthbar");
        battleScene = GameObject.Find("BattleScene").transform;
        battleScene.gameObject.SetActive(false);
    }

    public static void StartBattle()
    {
        battleScene.gameObject.GetComponent<Image>().sprite = MenuLayout.menuBackground.sprite();
        enemy = new Battler[] { new Battler(100,80,Random.Range(0,3),Resources.Load<Texture2D>("sprites/spr_enemy1")) };
        //ally = new Battler[] { PlayerController.battler };
        ally[0] = PlayerController.battler;
        isInBattle = true;
        battleScene.gameObject.SetActive(true);
    }

    public static void EndBattle() {
        PlayerStats.power_experience[Random.Range(0, 9)] += Random.Range(8, 24);
        isInBattle = false;
        battleScene.gameObject.SetActive(false);
    }

    public static void OnGUI()
    {
        if (!isInBattle) return;
        //MenuLayout.DrawChessBkg();

        // draw teams
        battleScene.Find("bar player").GetComponent<RectTransform>().sizeDelta = new Vector2(140*ally[0].cur_hp / ally[0].max_hp, 22);
        battleScene.Find("bar enemy").GetComponent<RectTransform>().sizeDelta = new Vector2(140*enemy[0].cur_hp / enemy[0].max_hp, 22);
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
