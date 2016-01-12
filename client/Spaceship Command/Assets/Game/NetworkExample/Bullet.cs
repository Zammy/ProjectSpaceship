using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        var combat = hit.GetComponent<Combat>();
        if (combat != null)
        {
            combat.TakeDamage(10);

            Destroy(gameObject);
        }
    }
}
