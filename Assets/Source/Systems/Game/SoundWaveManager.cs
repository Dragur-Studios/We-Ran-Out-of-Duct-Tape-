using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SoundWaveManager : MonoBehaviour
{
    public static SoundWaveManager Instance;

    List<SoundWaveEmitter> emitters = new List<SoundWaveEmitter>();
    List<SoundWaveListener> listeners = new List<SoundWaveListener>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public static void AddListener(SoundWaveListener listener) => Instance.listeners.Add(listener);
    public static void RemoveListener(SoundWaveListener listener) => Instance.listeners.Remove(listener);

    public static void AddEmitter(SoundWaveEmitter emitter) => Instance.emitters.Add(emitter);
    public static void RemoveEmitter(SoundWaveEmitter emitter) => Instance.emitters.Remove(emitter);

    public static void EmitSound(SoundWaveEmitter emitter, Vector3 pos, float radius, string tag)
    {
        foreach (var listener in Instance.listeners)
        {
            listener.OnSoundHeard(pos, radius, tag);
        }
    }
}

