using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class AdventureValidation
{
    static int step;
    static double deadline;
    static AdventureValidation()
    {
        EditorApplication.delayCall += () =>
        {
            if (File.Exists("ProjectSettings/RunAdventureCheck.txt"))
            {
                File.Delete("ProjectSettings/RunAdventureCheck.txt");
                Run();
            }
            else if (SessionState.GetBool("AdventureChecking", false)) Watch();
        };
    }
    [MenuItem("Mario/Comprobar vidas y cambios de nivel")]
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorSceneManager.GetActiveScene().isDirty) return;
        EditorSceneManager.OpenScene("Assets/Scenes/1-2.unity");
        SessionState.SetBool("AdventureChecking", true);
        EditorApplication.EnterPlaymode();
        Watch();
    }
    static void Watch()
    {
        step = 0;
        deadline = EditorApplication.timeSinceStartup + 70;
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
    }
    static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    static void Tick()
    {
        try
        {
            if (EditorApplication.timeSinceStartup > deadline) throw new Exception("Tiempo de prueba agotado");
            if (!EditorApplication.isPlaying || !GameManager.Instance) return;
            var gm = GameManager.Instance;
            var scene = SceneManager.GetActiveScene().name;
            if (step == 0 && scene == "1-2" && gm.lives == 3)
            {
                Require(!UnityEngine.Object.FindFirstObjectByType<PlayerMovement>().swimming, "Natación activa fuera del agua");
                Require(AdventureAudio.Instance.music.name == "Cueva", "Falta música de cueva");
                gm.NextLevel(); step++;
            }
            else if (step == 1 && scene == "1-3")
            {
                Require(UnityEngine.Object.FindFirstObjectByType<PlayerMovement>().swimming, "Falta natación");
                Require(AdventureAudio.Instance.music.name == "Agua", "Falta música acuática");
                gm.AddCoin(); Require(gm.coins == 1, "No suma monedas");
                gm.ResetLevel(); step++;
            }
            else if (step == 2 && gm.lives == 2) { gm.ResetLevel(); step++; }
            else if (step == 3 && gm.lives == 1) { gm.ResetLevel(); step++; }
            else if (step == 4 && scene == "GameOver")
            {
                Require(gm.lives == 0, "Game Over con vidas restantes");
                Require(UnityEngine.Object.FindFirstObjectByType<AdventureHud>().gameOver, "Falta pantalla de Game Over");
                gm.NewGame(); step++;
            }
            else if (step == 5 && scene == "1-1" && gm.lives == 3)
            {
                Require(gm.coins == 0, "Reinicio conserva monedas");
                Require(AdventureAudio.Instance.music.name == "Superficie", "Falta música de superficie");
                Finish("ADVENTURE_TEST_OK: secuencia 1-2 → 1-3, natación, música, monedas, tres vidas, Game Over y reinicio comprobados.");
            }
        }
        catch (Exception error) { Finish("ADVENTURE_TEST_FAILED: " + error); }
    }
    static void Finish(string result)
    {
        EditorApplication.update -= Tick;
        SessionState.SetBool("AdventureChecking", false);
        File.WriteAllText("ProjectSettings/AdventureCheck.txt", result);
        Debug.Log(result);
        EditorApplication.isPlaying = false;
    }
}
