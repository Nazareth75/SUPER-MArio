using UnityEngine;
using TMPro;
public class LevelTimer : MonoBehaviour
{
    [SerializeField] float tiempoInicial = 400f;
    [SerializeField] TextMeshProUGUI textoTiempo;
    public float TiempoRestante { get; private set; }
    bool activo = true;
    Player player;
    void Start()
    {
        TiempoRestante = tiempoInicial;
        player = FindFirstObjectByType<Player>();
        if (textoTiempo && FindFirstObjectByType<GameHud>())
            textoTiempo.gameObject.SetActive(false);
    }
    void Update()
    {
        if (!activo) return;
        if (player && (player.dead || !player.GetComponent<PlayerMovement>().enabled)) return;
        TiempoRestante = Mathf.Max(0f, TiempoRestante - Time.deltaTime);
        if (TiempoRestante <= 0f)
        {
            activo = false;
            if (player) player.Death();
        }
        if (textoTiempo && textoTiempo.gameObject.activeSelf)
            textoTiempo.text = "TIME\n" + Mathf.CeilToInt(TiempoRestante).ToString("000");
    }
    public void Detener() => activo = false;
}