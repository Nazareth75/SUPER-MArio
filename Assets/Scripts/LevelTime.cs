using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] float tiempoInicial = 400f;
    [SerializeField] TextMeshProUGUI textoTiempo;

    float tiempoRestante;
    bool activo = true;

    void Start()
    {
        tiempoRestante = tiempoInicial;
    }

    void Update()
    {
        if (!activo) return;

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
        // Aquí matas a Mario, por ejemplo:
        // FindObjectOfType<Player>().Morir();
    }

    public void Detener() => activo = false; // llámalo al llegar a la bandera
}
