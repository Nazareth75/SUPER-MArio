using UnityEngine;

public class LevelMenu : MonoBehaviour
{
    private readonly string[] names = { "Superficie", "Subterráneo", "Acuático", "Castillo rosado de Bowser" };
    void Update()
    {
        for (int i = 0; i < 4; i++)
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i))) GameManager.Instance.StartLevel(i + 1);
    }
    void OnGUI()
    {
        float scale = Mathf.Min(Screen.width / 960f, Screen.height / 540f);
        GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width - 960 * scale) / 2, (Screen.height - 540 * scale) / 2), Quaternion.identity, Vector3.one * scale);
        GUI.Box(new Rect(150, 35, 660, 470), "");
        var title = new GUIStyle(GUI.skin.label) { fontSize = 38, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        title.normal.textColor = new Color(1, .75f, .86f);
        GUI.Label(new Rect(170, 55, 620, 60), "MARIO · LA AVENTURA", title);
        title.fontSize = 18;
        GUI.Label(new Rect(170, 113, 620, 35), "Elige dónde quieres empezar", title);
        var button = new GUIStyle(GUI.skin.button) { fontSize = 23, alignment = TextAnchor.MiddleLeft, padding = new RectOffset(28, 10, 10, 10) };
        for (int i = 0; i < names.Length; i++)
            if (GUI.Button(new Rect(230, 166 + i * 68, 500, 54), $"Ir a nivel {i + 1}   ·   {names[i]}", button)) GameManager.Instance.StartLevel(i + 1);
        title.fontSize = 15;
        GUI.Label(new Rect(190, 450, 580, 35), "También puedes pulsar 1, 2, 3 o 4 · Cada partida comienza con 3 vidas", title);
    }
}
