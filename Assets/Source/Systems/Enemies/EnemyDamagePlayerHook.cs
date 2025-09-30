using UnityEngine;

public class EnemyDamagePlayerHook : MonoBehaviour
{
    Enemy enemy;

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();    
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach(var point in collision.contacts)
        {
            var player = collision.gameObject.GetComponent<Player>();
            if (player!= null)
            {
                player.TakeDamage(enemy.GetDamage());
                gameObject.SetActive(false);
            }
        }

    }
}
