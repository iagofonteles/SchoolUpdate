using UnityEngine;
using UnityEngine.Networking;

public class AI_Enemy : MonoBehaviour
{
    public float speed = 1;
    public float visionRadius = 4;
    public int magic_power;
    private Cooldown fearTimer = new Cooldown();
    private Timer motionChangeTimer = new Timer();
    private Vector3 direction;

    private void Start() {
        direction = Random.rotation.eulerAngles.normalized;
    }

    void Update()
    {
        if (BattleController.isInBattle) return;

        if (!fearTimer[2]) transform.Translate(speed * -PlayerController.hDirection(transform) * Time.deltaTime);
        else if (Vector3.Distance(transform.position, PlayerController.self.transform.position) < visionRadius)
            if (PlayerStats.magic_power > magic_power) fearTimer.Reset(); // enemy is weaker
            else transform.Translate(speed * PlayerController.hDirection(transform) * Time.deltaTime);
        else transform.Translate(speed * direction * Time.deltaTime / 5);
        
        if (motionChangeTimer[2]) direction = new Vector3(Random.value,0, Random.value).normalized;
    }
}
