using System.Collections;
using UnityEngine;

public class LevelMenu : MonoBehaviour
{
    private readonly string[] characters = { "Mario", "Peach" };
    private readonly string[] levels = { "Superficie", "Subterráneo", "Acuático", "Castillo rosado de Bowser" };

    public static bool PlayAsPeach { get; private set; }
    private int page, focused;
    private bool starting;

    private void Update()
    {
        if (starting) return;
        string[] options = Options();
        int direction = Input.GetKeyDown(KeyCode.DownArrow) ? 1 : Input.GetKeyDown(KeyCode.UpArrow) ? -1 : 0;
        if (direction != 0)
        {
            focused = (focused + direction + options.Length) % options.Length;
            if (AdventureAudio.Instance) AdventureAudio.Instance.Play(AdventureAudio.Instance.coin);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) Activate(focused);
        else if (page == 2)
        {
            for (int i = 0; i < levels.Length; i++)
                if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i))) Activate(i);
        }
    }

    private string[] Options()
    {
        if (page == 0) return new[] { "JUGAR" };
        if (page == 1) return new[] { characters[0], characters[1], "Volver" };
        return new[] { levels[0], levels[1], levels[2], levels[3], "Volver" };
    }

    private void Activate(int index)
    {
        if (starting) return;
        var audio = AdventureAudio.Instance;
        if (page == 2 && index < levels.Length)
        {
            if (audio) audio.Play(audio.flag);
            StartCoroutine(StartAfterCue(index + 1));
            return;
        }

        if (page == 1 && index < characters.Length)
        {
            PlayAsPeach = index == 1;
            if (audio) audio.Play(PlayAsPeach ? audio.powerup : audio.life);
            page = 2;
        }
        else
        {
            if (audio) audio.Play(audio.pipe);
            page = index == 0 ? 1 : page - 1;
        }
        focused = 0;
    }

    private IEnumerator StartAfterCue(int level)
    {
        starting = true;
        yield return new WaitForSecondsRealtime(.28f);
        GameManager.Instance.StartLevel(level);
    }

    private void OnGUI()
    {
        float scale = Mathf.Min(Screen.width / 960f, Screen.height / 540f);
        GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width - 960 * scale) / 2, (Screen.height - 540 * scale) / 2), Quaternion.identity, Vector3.one * scale);
        GUI.Box(new Rect(150, 35, 660, 470), "");
        var title = new GUIStyle(GUI.skin.label) { fontSize = 38, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        title.normal.textColor = new Color(1, .75f, .86f);
        GUI.Label(new Rect(170, 55, 620, 60), "MARIO · LA AVENTURA", title);
        title.fontSize = 18;
        string heading = page == 0 ? "Cuatro niveles · Tres vidas" : page == 1 ? "Selecciona tu personaje" : $"Jugar como {(PlayAsPeach ? "Peach" : "Mario")}";
        GUI.Label(new Rect(170, 113, 620, 35), heading, title);
        var button = new GUIStyle(GUI.skin.button) { fontSize = 23, alignment = TextAnchor.MiddleCenter };
        string[] options = Options();
        GUI.enabled = !starting;
        for (int i = 0; i < options.Length; i++)
        {
            GUI.backgroundColor = i == focused ? new Color(1f, .65f, .82f) : Color.white;
            string label = page == 2 && i < levels.Length ? $"Ir a nivel {i + 1} · {options[i]}" : options[i];
            if (GUI.Button(new Rect(230, 165 + i * 57, 500, 47), label, button)) Activate(i);
        }
        GUI.enabled = true;
        GUI.backgroundColor = Color.white;
        title.fontSize = 15;
        GUI.Label(new Rect(190, 455, 580, 35), "Flechas y Enter, o usa el ratón", title);
    }
}
