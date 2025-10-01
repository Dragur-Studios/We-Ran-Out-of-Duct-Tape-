using System.Collections.Generic;
using UnityEngine;

public class SoundWave
{
    public Vector3 origin;
    public float maxRadius;
    public float speed;
    public string tag;

    private float currentRadius = 0f;
    private HashSet<SoundWaveListener> notified = new HashSet<SoundWaveListener>();

    public SoundWave(Vector3 origin, float maxRadius, float speed, string tag)
    {
        this.origin = origin;
        this.maxRadius = maxRadius;
        this.speed = speed;
        this.tag = tag;
    }

    public bool Update(float deltaTime, List<SoundWaveListener> listeners)
    {
        currentRadius += speed * deltaTime;

        foreach (var listener in listeners)
        {
            if (!notified.Contains(listener))
            {
                float dist = Vector3.Distance(origin, listener.transform.position);
                if (dist <= currentRadius)
                {
                    listener.OnSoundHeard(origin, currentRadius, tag);
                    notified.Add(listener);
                }
            }
        }

        return currentRadius >= maxRadius; // true = finished
    }
}

