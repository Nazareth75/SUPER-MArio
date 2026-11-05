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

    public static void ValidateBatch()
    {
        Validate();
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