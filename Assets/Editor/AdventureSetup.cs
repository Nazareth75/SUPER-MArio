using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Importación inicial: no interrumpe una partida ni modifica una escena sin guardar.
[InitializeOnLoad]
public static class AdventureSetup
{
    static AdventureSetup() { EditorApplication.delayCall += Prepare; }
    static void Prepare()
    {
        if (System.IO.File.Exists("ProjectSettings/AdventurePrepared.txt")) return;
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().isDirty)
        {
            Debug.Log("Mario: guarda la escena y usa Mario > Crear niveles de la aventura para finalizar la preparación.");
            return;
        }
        AdventureBuilder.Build();
        System.IO.File.WriteAllText("ProjectSettings/AdventurePrepared.txt", "Escenas preparadas. Usa el menú Mario para reconstruirlas solo cuando lo necesites.");
    }
}
