using UnityEngine;

public static class BattleController
{
    public static bool isInBattle { get; private set; }

    private static Battler[] enemy;
    private static Battler[] ally;

    private static Texture2D hpbar;

    public static void Initialize()
    {
        hpbar = Resources.Load<Texture2D>("sprites/spr_healthbar");

    }

    public static void StartBattle()
    {
        enemy = new Battler[] { new Battler(100,80,Random.Range(0,3),Resources.Load<Texture2D>("sprites/spr_enemy1")) };
        ally = new Battler[] { PlayerController.battler };
        isInBattle = true;
    }

    public static void OnGUI()
    {
        if (!isInBattle) return;

        // draw teams
        ally[0].sprite.DrawAt(0, Screen.height - ally[0].sprite.height - hpbar.height - 8);
        enemy[0].sprite.DrawAt(0, Screen.height - enemy[0].sprite.height - hpbar.height - 8);
        var r = new Rect(0, Screen.height - hpbar.height, hpbar.width * ally[0].cur_hp / ally[0].max_hp, hpbar.height);
        GUI.DrawTextureWithTexCoords(r, hpbar, new Rect(0, 0, ally[0].cur_hp / ally[0].max_hp, 1));
        r = new Rect(Screen.width, Screen.height - hpbar.height, hpbar.width * enemy[0].cur_hp / enemy[0].max_hp, hpbar.height);
        r.x -= r.width;
        GUI.DrawTextureWithTexCoords(r, hpbar, new Rect(1 - (enemy[0].cur_hp / enemy[0].max_hp), 0, 1, 1));
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
