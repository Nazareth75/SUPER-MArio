using UnityEngine;

public class AdventureAudio : MonoBehaviour
{
    public AudioClip music, jump, coin, hurt, finish, life, pipe, powerup, stomp, brick, flag;
    public bool loopMusic = true;
    private AudioSource effects;
    private AudioSource background;
    public static AdventureAudio Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        background = gameObject.AddComponent<AudioSource>();
        background.clip = music;
        background.loop = loopMusic;
        background.volume = 0.18f;
        background.Play();
        effects = gameObject.AddComponent<AudioSource>();
        effects.volume = 0.4f;
    }
    public void Play(AudioClip clip) { if (clip) effects.PlayOneShot(clip); }
    public void StopMusic() { background.Stop(); }
    void OnDestroy() { if (Instance == this) Instance = null; }
}
