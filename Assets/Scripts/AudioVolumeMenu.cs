using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Boton y panel autonomos; los metodos publicos sirven para cualquier UI.</summary>
public class AudioVolumeMenu : MonoBehaviour
{
    [Tooltip("Desactivar si el menu del equipo ya tiene sus propios botones y sliders.")]
    public bool drawBuiltInControls = true;
    public bool IsOpen { get; private set; }
    public void Open() { IsOpen = true; }
    public void Close() { IsOpen = false; AudioVolumeSettings.Save(); }
    public void Toggle() { if (IsOpen) Close(); else Open(); }
    public void SetMusicVolume(float value) { AudioVolumeSettings.SetMusic(value); }
    public void SetEffectsVolume(float value) { AudioVolumeSettings.SetEffects(value); }
    private void OnDisable() { AudioVolumeSettings.Save(); }
    private void OnApplicationPause(bool paused) { if (paused) AudioVolumeSettings.Save(); }
    private void OnApplicationQuit() { AudioVolumeSettings.Save(); }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        SceneManager.sceneLoaded -= AddToMenu;
        SceneManager.sceneLoaded += AddToMenu;
    }

    private static void AddToMenu(Scene scene, LoadSceneMode mode)
    {
        bool isMenu = scene.name == "Menu" || scene.name == "MainMenu";
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.GetComponentInChildren<AudioVolumeMenu>(true)) return;
            if (root.GetComponentInChildren<LevelMenu>(true)) isMenu = true;
        }
        if (!isMenu) return;
        var obj = new GameObject("Dilan - Controles de volumen");
        SceneManager.MoveGameObjectToScene(obj, scene);
        obj.AddComponent<AudioVolumeMenu>();
    }

    private void OnGUI()
    {
        if (!drawBuiltInControls) return;
        Matrix4x4 oldMatrix = GUI.matrix;
        int oldDepth = GUI.depth;
        Color oldColor = GUI.color;
        try
        {
            float scale = Mathf.Min(Screen.width / 960f, Screen.height / 540f);
            GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width - 960 * scale) / 2,
                (Screen.height - 540 * scale) / 2), Quaternion.identity, Vector3.one * scale);
            GUI.color = Color.white;
            GUI.depth = -100;
            if (!IsOpen)
            {
                if (GUI.Button(new Rect(790, 480, 150, 42), "Volumen")) Open();
                return;
            }
            GUI.ModalWindow(GetInstanceID(), new Rect(260, 150, 440, 240), DrawPanel, "VOLUMEN");
        }
        finally { GUI.matrix = oldMatrix; GUI.depth = oldDepth; GUI.color = oldColor; }
    }

    private void DrawPanel(int id)
    {
        GUI.Label(new Rect(25, 40, 390, 25), "Musica de fondo: " + Mathf.RoundToInt(AudioVolumeSettings.Music * 100) + "%");
        float music = GUI.HorizontalSlider(new Rect(25, 75, 390, 25), AudioVolumeSettings.Music, 0f, 1f);
        if (!Mathf.Approximately(music, AudioVolumeSettings.Music)) SetMusicVolume(music);
        GUI.Label(new Rect(25, 105, 390, 25), "Efectos de sonido: " + Mathf.RoundToInt(AudioVolumeSettings.Effects * 100) + "%");
        float effects = GUI.HorizontalSlider(new Rect(25, 140, 390, 25), AudioVolumeSettings.Effects, 0f, 1f);
        if (!Mathf.Approximately(effects, AudioVolumeSettings.Effects)) SetEffectsVolume(effects);
        if (GUI.Button(new Rect(145, 185, 150, 35), "Guardar y cerrar")) Close();
    }
}