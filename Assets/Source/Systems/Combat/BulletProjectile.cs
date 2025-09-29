using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] float bulletSpeed = 6.0f; // UPS / Screen
    [SerializeField] int damageAmount = 15;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        var forwardForce = (bulletSpeed * transform.forward) * 10;
        rb.AddForce(forwardForce);
    }

    public int GetDamageAmount()
    {
        return damageAmount;
    }

    public void Expend()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            var enemy = other.GetComponent<Enemy>();
            enemy.DealDamage(damageAmount);
            Destroy(gameObject);
        }
    }
}
