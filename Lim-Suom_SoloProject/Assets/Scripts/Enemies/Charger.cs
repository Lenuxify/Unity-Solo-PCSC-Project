using UnityEngine;
using UnityEngine.AI;

public class Charger : Enemy
{
    public void Awake()
    {
        damageDealt = 30;
    }

    // explode after touching player
    public new void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.tag == "Player")
            Destroy(gameObject);
    }
}
