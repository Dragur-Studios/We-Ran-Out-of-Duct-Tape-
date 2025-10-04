using UnityEngine;
using UnityEngine.Audio;

public enum CollisionShape 
{
    None,
    Sphere,
    Box,
    Capsule,
}


[RequireComponent(typeof(Rigidbody))]
public class BulletProjectile : MonoBehaviour
{
    Rigidbody rb;
    Collider col;

    [SerializeField] float bulletSpeed = 6.0f;
    [SerializeField] int damageAmount = 15;
    [SerializeField] GameObject vfx_bloodSplatter;

    private Vector3 lastPos;

    [SerializeField] CollisionShape shape = CollisionShape.Sphere;
    CollisionShape _lastShape;

    void Start()
    {

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        //col.isTrigger = true;

        lastPos = transform.position;

        // Fire the bullet forward
        var forwardForce = (bulletSpeed * transform.forward) * 10f;
        rb.AddForce(forwardForce, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        // Track last position for raycast
        lastPos = transform.position;
    }

    private void LateUpdate()
    {
        lastPos = transform.position;
    }
    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);

            var enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DealDamage(damageAmount);
            }

            if (contact.normal != Vector3.zero)
            {
                var go = Instantiate(
                    vfx_bloodSplatter,
                    contact.point,
                    Quaternion.LookRotation(contact.normal)
                );
                if (enemy != null) go.transform.SetParent(enemy.transform);
            }
        }

        Destroy(gameObject);
    }

    public int GetDamageAmount()
    {
        return damageAmount;
    }

    public void Expend()
    {
        Destroy(gameObject);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Enemy")
    //    {


    //        //// Raycast back to find exact hit point and normal
    //        //Vector3 dir = (transform.position - lastPos).normalized;
    //        //float dist = Vector3.Distance(lastPos, transform.position);
    //        //if (Physics.Raycast(lastPos, dir, out RaycastHit hit, dist + 0.1f))
    //        //{
    //        //    if (hit.normal != Vector3.zero)
    //        //    {
    //        //        var go = Instantiate(
    //        //            vfx_bloodSplatter,
    //        //            hit.point,
    //        //            Quaternion.LookRotation(hit.normal)
    //        //        );
    //        //        if (enemy != null) go.transform.SetParent(enemy.transform);
    //        //    }
    //        //}
    //        //else
    //        //{
    //        //    if (!float.IsNaN(transform.position.x) && !float.IsInfinity(transform.position.x))
    //        //    {
    //        //        var go = Instantiate(vfx_bloodSplatter, transform.position, Quaternion.identity);
    //        //        if (enemy != null) go.transform.SetParent(enemy.transform);
    //        //    }
    //        //}


    //    }

    //    Destroy(gameObject);

    //}





}


