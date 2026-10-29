using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class CastleSetup
{
    static CastleSetup() { EditorApplication.delayCall += Prepare; }
    static void Prepare()
    {
        if (File.Exists("ProjectSettings/CastlePrepared.txt") || EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (EditorSceneManager.GetActiveScene().isDirty) return;
        CastleBuilder.Build();
        File.WriteAllText("ProjectSettings/CastlePrepared.txt", "Castillo y menú preparados.");
    }
}
