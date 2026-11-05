using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DilanAudioValidation
{
    [MenuItem("Dilan/Validar recursos de audio")]
    public static void Validate()
    {
        foreach (string name in new[] { "Jump", "PowerUp", "Damage", "Death" })
        {
            var clip = Resources.Load<AudioClip>("DilanAudio/" + name);
            if (!clip || clip.length <= 0) throw new Exception("Audio ausente o vacio: " + name);
        }
        if (!AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Pisoton.wav"))
            throw new Exception("Falta Pisoton.wav");
        Debug.Log("DILAN_AUDIO_OK: cuatro recursos importados y pisoton disponible.");
    }

    public static void ValidateVolumes()
    {
        bool hadMusic = PlayerPrefs.HasKey("Dilan.Audio.Music");
        bool hadEffects = PlayerPrefs.HasKey("Dilan.Audio.Effects");
        float music = AudioVolumeSettings.Music, effects = AudioVolumeSettings.Effects;
        try
        {
            AudioVolumeSettings.Set(0.25f, 0.75f);
            AudioVolumeSettings.SetMusic(-1f);
            if (AudioVolumeSettings.Music != 0f || AudioVolumeSettings.Effects != 0.75f)
                throw new Exception("Musica no es independiente de efectos");
            AudioVolumeSettings.SetEffects(2f);
            if (AudioVolumeSettings.Effects != 1f || AudioVolumeSettings.Music != 0f)
                throw new Exception("Efectos no son independientes de musica");
            AudioVolumeSettings.Save();
            if (PlayerPrefs.GetFloat("Dilan.Audio.Effects") != 1f)
                throw new Exception("Preferencias de volumen no guardadas");
            Debug.Log("DILAN_VOLUME_OK: controles independientes, limites y preferencias.");
        }
        finally
        {
            AudioVolumeSettings.Set(music, effects);
            if (!hadMusic) PlayerPrefs.DeleteKey("Dilan.Audio.Music");
            if (!hadEffects) PlayerPrefs.DeleteKey("Dilan.Audio.Effects");
            AudioVolumeSettings.Save();
        }
    }
    public static void ValidateBatch()
    {
        Validate();
        ValidateVolumes();
        foreach (string name in new[] { "1-1", "1-2", "1-3", "1-4" })
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + name + ".unity");
            var audio = UnityEngine.Object.FindFirstObjectByType<AdventureAudio>();
            if (!audio || !audio.music || !audio.jump || !audio.stomp || !audio.powerup)
                throw new Exception("Referencias de audio incompletas en " + name);
        }
        Debug.Log("DILAN_SCENES_OK: referencias heredadas conservadas en los cuatro niveles.");
    }
}