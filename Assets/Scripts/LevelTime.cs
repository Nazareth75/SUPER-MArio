using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] float tiempoInicial = 400f;
    [SerializeField] TextMeshProUGUI textoTiempo;

    float tiempoRestante;
    bool activo = true;
    Player player;

    void Start()
    {
        tiempoRestante = tiempoInicial;
        player = FindFirstObjectByType<Player>();
    }

    void Update()
    {
        if (!activo) return;

        // Si Mario ya murió (Goomba, vacío, etc.) el tiempo se congela
        if (player != null && player.dead) return;

        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            activo = false;
            TiempoAgotado();
        }

        textoTiempo.text = "TIME\n" + Mathf.CeilToInt(tiempoRestante).ToString("000");
    }

    void TiempoAgotado()
    {
        if (player != null)
        {
            player.Death();
        }
    }

    public void Detener() => activo = false; // llámalo al llegar a la bandera
}