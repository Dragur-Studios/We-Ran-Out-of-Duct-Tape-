using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;

using System;


public class SoundWaveEmitter : MonoBehaviour
{
    [SerializeField] float radius = 10f;   // how far the sound travels
    [SerializeField] float loudness = 1f;  // multiplier for intensity
    [SerializeField] string soundTag = "Generic"; // footsteps, gunshot, etc.

    // sound is dependent on temperature.

    private void OnEnable() => SoundWaveManager.AddEmitter(this);
    private void OnDisable() => SoundWaveManager.RemoveEmitter(this);

    public void Emit()
    {

        var pos = transform.position;
        pos.y = 0;
        Debug.Log($"Emitting Sound! {pos} {loudness} {tag}");

        SoundWaveManager.EmitSound(this,  pos, radius * loudness, soundTag);
    }
}

