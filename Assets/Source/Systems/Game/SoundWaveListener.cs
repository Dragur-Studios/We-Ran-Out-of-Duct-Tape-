using System;
using UnityEngine;

public class SoundWaveListener : MonoBehaviour
{
    [SerializeField] float sensitivity = 1f; // how well this listener hears

    protected virtual void OnEnable() => SoundWaveManager.AddListener(this);
    protected virtual void OnDisable() => SoundWaveManager.RemoveListener(this);
    public void OnSoundHeard(Vector3 sourcePos, float intensity, string tag)
    {
        ReactToSoundHeard(sourcePos, tag);
    }

    protected virtual void ReactToSoundHeard(Vector3 sourcePos, string tag)
    {
        Debug.Log($"{name} heard sound: {sourcePos} from {tag}.");
    }
}

