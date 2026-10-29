using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class CastleDelivery
{
    const string Output = "C:/Users/HP/Documents/Codex/2026-10-29/https-github-com-jesusbautistah-super-mario/outputs/Mario-Aventura-Windows";
    static int step;
    static double deadline, nextStomp;
    static int previousHealth;
    static CastleDelivery()
    {
        EditorApplication.delayCall += () =>
        {
            if (SessionState.GetBool("CastleDeliveryTesting", false)) Watch();
            else if (File.Exists("ProjectSettings/CastleDeliveryRequest.txt"))
            {
                File.Delete("ProjectSettings/CastleDeliveryRequest.txt");
                Prepare();
            }
        };
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.EnteredEditMode && SessionState.GetBool("CastleDeliveryBuild", false))
            {
                SessionState.SetBool("CastleDeliveryBuild", false);
                EditorApplication.delayCall += BuildWindows;
            }
        };
    }
    [MenuItem("Mario/Probar y generar ejecutable de Windows")]
    public static void Prepare()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new Exception("Sal del modo Play antes de generar el juego.");
        if (EditorSceneManager.GetActiveScene().isDirty) EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        CastleBuilder.Build();
        File.WriteAllText("ProjectSettings/CastlePrepared.txt", "Castillo y menú preparados.");
        SessionState.SetBool("CastleDeliveryTesting", true);
        EditorApplication.EnterPlaymode();
        Watch();
    }
    static void Watch()
    {
        step = 0; previousHealth = 3; nextStomp = 0;
        deadline = EditorApplication.timeSinceStartup + 100;
        EditorApplication.update -= Tick; EditorApplication.update += Tick;
    }
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    static void Tick()
    {
        try
        {
            if (EditorApplication.timeSinceStartup > deadline) throw new Exception("La prueba del castillo agotó el tiempo.");
            if (!EditorApplication.isPlaying || !GameManager.Instance) return;
            var gm = GameManager.Instance;
            string scene = SceneManager.GetActiveScene().name;
            if (step == 0 && scene == "Menu")
            {
                Check(UnityEngine.Object.FindFirstObjectByType<LevelMenu>(), "Falta selector de niveles");
                gm.StartLevel(1); step++;
            }
            else if (step >= 1 && step <= 3 && scene == "1-" + step && gm.stage == step)
            {
                Check(gm.lives == 3, "El nivel no comienza con tres vidas");
                if (step == 3) { gm.NextLevel(); step = 4; }
                else { step++; gm.StartLevel(step); }
            }
            else if (step == 4 && scene == "1-4")
            {
                Check(gm.stage == 4, "El nivel 4 no conserva su número");
                Check(UnityEngine.Object.FindFirstObjectByType<CastleBoss>(), "Falta Bowser");
                Check(AdventureAudio.Instance.music.name == "Castillo", "Falta música del castillo");
                gm.ShowMenu(); step++;
            }
            else if (step == 5 && scene == "Menu") { gm.StartLevel(4); step++; }
            else if (step == 6 && scene == "1-4")
            {
                var boss = UnityEngine.Object.FindFirstObjectByType<CastleBoss>();
                var player = UnityEngine.Object.FindFirstObjectByType<Player>();
                if (boss.defeated)
                {
                    Check(boss.health == 0, "Bowser cae con vida restante");
                    Camera.main.transform.position = new Vector3(104, 6, -10);
                    player.transform.position = new Vector3(109, 1.2f, 0);
                    player.GetComponent<Rigidbody2D>().position = player.transform.position;
                    step++;
                }
                else if (EditorApplication.timeSinceStartup >= nextStomp)
                {
                    Check(boss.health <= previousHealth, "La vida del jefe aumentó");
                    previousHealth = boss.health;
                    nextStomp = EditorApplication.timeSinceStartup + 2.2;
                    Camera.main.transform.position = new Vector3(98, 6, -10);
                    Vector3 landing = new Vector3(boss.transform.position.x, boss.GetComponent<Collider2D>().bounds.max.y + 2, 0);
                    player.transform.position = landing;
                    player.GetComponent<Rigidbody2D>().position = landing;
                }
            }
            else if (step == 7 && scene == "Completed")
            {
                gm.ShowMenu(); step++;
            }
            else if (step == 8 && scene == "Menu")
            {
                Finish("CASTLE_TEST_OK: menú, niveles 1/2/3/4, agua → castillo, tres golpes reales a Bowser, rescate de Peach y regreso al menú.", true);
            }
        }
        catch (Exception error) { Finish("CASTLE_TEST_FAILED: " + error, false); }
    }
    static void Finish(string result, bool success)
    {
        EditorApplication.update -= Tick;
        SessionState.SetBool("CastleDeliveryTesting", false);
        File.WriteAllText("ProjectSettings/CastleCheck.txt", result);
        Debug.Log(result);
        SessionState.SetBool("CastleDeliveryBuild", success);
        EditorApplication.isPlaying = false;
    }
    [MenuItem("Mario/Generar ejecutable de Windows")]
    public static void BuildWindows()
    {
        Directory.CreateDirectory(Output);
        PlayerSettings.productName = "Mario - La aventura del castillo rosado";
        PlayerSettings.companyName = "Nazareth";
        PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = Output + "/MarioAventura.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        });
        string result = "WINDOWS_BUILD_" + report.summary.result + ": " + report.summary.totalErrors + " errores. " + Output;
        File.WriteAllText("ProjectSettings/WindowsBuild.txt", result);
        Debug.Log(result);
        if (report.summary.result != BuildResult.Succeeded) throw new Exception(result);
    }
}
