using System.Collections.Generic;
using UnityEngine;

/// <summary>Audio de la partida; AdventureAudio conserva las referencias de las escenas existentes.</summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioClip music, jump, coin, hurt, finish, life, pipe, powerup, stomp, brick, flag;
    public AudioClip death;
    public bool loopMusic = true;
    [Range(0f, 1f)] public float musicVolume = 0.18f;
    [Range(0f, 1f)] public float effectsVolume = 0.4f;
    [Tooltip("Evita llamadas repetidas al mismo clip en un intervalo muy corto.")]
    [Range(0f, 0.2f)] public float duplicateWindow = 0.05f;
    private AudioSource background;
    private AudioSource effects;
    private readonly Dictionary<AudioClip, float> lastPlayed = new Dictionary<AudioClip, float>();

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            Destroy(this);
            return;
        }
        Instance = this;
        background = gameObject.AddComponent<AudioSource>();
        effects = gameObject.AddComponent<AudioSource>();
        background.playOnAwake = effects.playOnAwake = false;
        background.spatialBlend = effects.spatialBlend = 0f;
        if (!jump) jump = Resources.Load<AudioClip>("DilanAudio/Jump");
        if (!powerup) powerup = Resources.Load<AudioClip>("DilanAudio/PowerUp");
        // Antes hurt contenia el sonido de muerte; ahora cada accion tiene su clip.
        hurt = Resources.Load<AudioClip>("DilanAudio/Damage") ?? hurt;
        if (!death) death = Resources.Load<AudioClip>("DilanAudio/Death");
    }

    protected virtual void Start()
    {
        SetVolumes(musicVolume, effectsVolume);
        PlayMusic(music, loopMusic);
    }

    public void SetVolumes(float musicLevel, float effectsLevel)
    {
        musicVolume = Mathf.Clamp01(musicLevel);
        effectsVolume = Mathf.Clamp01(effectsLevel);
        if (background) background.volume = musicVolume;
        if (effects) effects.volume = effectsVolume;
    }

    protected virtual void OnValidate()
    {
        SetVolumes(musicVolume, effectsVolume);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (!background || !clip) return;
        if (background.isPlaying && background.clip == clip && background.loop == loop) return;
        background.clip = clip;
        background.loop = loop;
        background.Play();
    }

    public void Play(AudioClip clip)
    {
        if (!clip || !effects || !isActiveAndEnabled) return;
        float now = Time.unscaledTime;
        if (lastPlayed.TryGetValue(clip, out float previous) && now - previous <= duplicateWindow) return;
        lastPlayed[clip] = now;
        effects.PlayOneShot(clip);
    }

    public void StopMusic() { if (background) background.Stop(); }
    [ContextMenu("Probar audio/Jump (Play Mode)")]
    public void PlayJump() { Play(jump); }
    [ContextMenu("Probar audio/Stomp (Play Mode)")]
    public void PlayStomp() { Play(stomp); }
    [ContextMenu("Probar audio/PowerUp (Play Mode)")]
    public void PlayPowerUp() { Play(powerup); }
    [ContextMenu("Probar audio/Damage (Play Mode)")]
    public void PlayDamage() { Play(hurt); }
    [ContextMenu("Probar audio/Death (Play Mode)")]
    public void PlayDeath() { StopMusic(); Play(death); }

    protected virtual void OnDestroy()
    {
        if (Instance != this) return;
        if (background) Destroy(background);
        if (effects) Destroy(effects);
        Instance = null;
    }
}