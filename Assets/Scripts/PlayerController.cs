using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    public static BattleController.Battler battler;

    private void Start()
    {
        PlayerStats.Init();
        QuestionsDB.Init();

        battler = new BattleController.Battler(1000, 140, 3, Resources.Load<Texture2D>("sprites/spr_avatar"));
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
            BattleController.StartBattle();
    }

}
