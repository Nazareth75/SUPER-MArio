using System;
using UnityEngine;

/// <summary>Preferencias globales independientes del diseno del menu.</summary>
public static class AudioVolumeSettings
{
    private const string MusicKey = "Dilan.Audio.Music";
    private const string EffectsKey = "Dilan.Audio.Effects";
    public static event Action<float, float> Changed;
    public static float Music => Mathf.Clamp01(PlayerPrefs.GetFloat(MusicKey, 0.18f));
    public static float Effects => Mathf.Clamp01(PlayerPrefs.GetFloat(EffectsKey, 0.4f));

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetListeners() { Changed = null; }

    public static void SetMusic(float value) { Set(value, Effects); }
    public static void SetEffects(float value) { Set(Music, value); }
    public static void Set(float music, float effects)
    {
        PlayerPrefs.SetFloat(MusicKey, Mathf.Clamp01(music));
        PlayerPrefs.SetFloat(EffectsKey, Mathf.Clamp01(effects));
        Changed?.Invoke(Music, Effects);
    }
    public static void Save() { PlayerPrefs.Save(); }
}