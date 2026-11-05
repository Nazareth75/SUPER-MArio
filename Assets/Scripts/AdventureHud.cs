using UnityEngine;
using UnityEngine.SceneManagement;

public class AdventureHud : MonoBehaviour
{
    public string title;
    public bool gameOver, completed;
    private float enteredAt;
    void OnEnable() { enteredAt = Time.unscaledTime; }
    void OnGUI()
    {
        float scale = Mathf.Min(Screen.width / 960f, Screen.height / 540f);
        GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width - 960 * scale) / 2, (Screen.height - 540 * scale) / 2), Quaternion.identity, Vector3.one * scale);
        var text = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        text.normal.textColor = Color.white;
        if (gameOver || completed)
        {
            GUI.color = new Color(1, 1, 1, Mathf.Clamp01((Time.unscaledTime - enteredAt) * 1.5f));
            GUI.Box(new Rect(210, 110, 540, 330), "");
            text.fontSize = 40;
            GUI.Label(new Rect(220, 135, 520, 65), gameOver ? "GAME OVER" : "¡NIVELES COMPLETADOS!", text);
            text.fontSize = 19;
            GUI.Label(new Rect(230, 212, 500, 65), gameOver ? "Mario se quedó sin vidas.\nTu próxima aventura empieza aquí." : "¡Rescataste a Peach del castillo rosado!\n¡Gracias por jugar!", text);
            var button = new GUIStyle(GUI.skin.button) { fontSize = 21 };
            if (GUI.Button(new Rect(330, 308, 300, 52), "Volver a jugar", button)) GameManager.Instance.NewGame();
            if (GUI.Button(new Rect(330, 373, 300, 40), "Elegir nivel", button)) GameManager.Instance.ShowMenu();
        }
        else
        {
            text.fontSize = 15;
            GUI.Label(new Rect(70, 506, 820, 26), SceneManager.GetActiveScene().name == "1-3" ? "A / D o flechas: nadar   ·   Espacio: impulso   ·   Llega a la tubería de salida" : "A / D o flechas: moverse   ·   Espacio: saltar   ·   Llega a la tubería de salida", text);
        }
    }
}
