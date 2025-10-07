using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    Transform p0;
    Transform p1;

    LineRenderer lr;


    private void Start()
    {
        p0 = transform.GetChild(0);
        p1 = transform.GetChild(1);
    
        
        lr = GetComponent<LineRenderer>();
  
    }

    private void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out var hit, 100))
        {
            p1.position = hit.point;
        }
        else
        {
            p1.position = p0.position + (p0.forward * 100);
        }

        lr.SetPositions(new Vector3[2] { p0.position, p1.position });
    }
}
