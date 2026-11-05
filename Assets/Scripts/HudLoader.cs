using UnityEngine;
using UnityEngine.SceneManagement;

public static class HudLoader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameHud current = Object.FindAnyObjectByType<GameHud>();
        bool gameplay = scene.name.StartsWith("1-");

        if (!gameplay) {
            if (current != null) {
                Object.Destroy(current.gameObject);
            }
            return;
        }

        if (current != null) {
            return;
        }

        GameObject prefab = Resources.Load<GameObject>("HUD/GameHud");
        if (prefab != null) {
            Object.Instantiate(prefab);
        } else {
            new GameObject("GameHud").AddComponent<GameHud>();
        }
    }
}
