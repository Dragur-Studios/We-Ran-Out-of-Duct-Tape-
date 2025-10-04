using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
public class SoundWaveManager : MonoBehaviour
{
    static SoundWaveManager _instance;
    public static SoundWaveManager Instance {  get { return _instance; } }
    //{
    //    get
    //    {
    //        if (_instance == null) 
    //        {
    //            _instance = new GameObject("Sound Wave Manager(SIMULATION MODE)").AddComponent<SoundWaveManager>();
    //        }
    //        return _instance;
    //    }
    //}

    List<SoundWaveEmitter> emitters = new List<SoundWaveEmitter>();
    List<SoundWaveListener> listeners = new List<SoundWaveListener>();
    List<SoundWave> activeWaves = new List<SoundWave>();

    float temperature;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        EnviormentManager.Instance.OnBiomeChanged += (biome) =>
        {
            temperature = biome.Temperature;
        };
    }

    private void Update()
    {
        for (int i = activeWaves.Count - 1; i >= 0; i--)
        {
            if (activeWaves[i].Update(Time.deltaTime, listeners))
            {
                activeWaves.RemoveAt(i);
            }
        }
    }

    public static void EmitSound(SoundWaveEmitter emitter, Vector3 pos, float radius, string tag)
    {
        var tempK = PhysicsUtils.FahrenheitToKelvin(Instance.temperature);
        var speed = PhysicsUtils.SpeedOfSound(1.4f, tempK, 0.02897f);

        Instance.activeWaves.Add(new SoundWave(pos, radius, speed, tag));
    }

    public static void AddListener(SoundWaveListener listener) => Instance.listeners.Add(listener);
    public static void RemoveListener(SoundWaveListener listener)
    {
        if (!Instance.listeners.Contains(listener))
            return;

        Instance.listeners.Remove(listener);
    }
    public static void AddEmitter(SoundWaveEmitter emitter) => Instance.emitters.Add(emitter);
    public static void RemoveEmitter(SoundWaveEmitter emitter) => Instance.emitters.Remove(emitter);
}

