using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CastleGoal : MonoBehaviour
{
    public CastleBoss boss;
    public GameObject gate;
    public Transform peach;
    private bool rescuing;
    void Update() { if (boss && boss.defeated && gate) gate.SetActive(false); }
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (!player || player.dead || rescuing || !boss || !boss.defeated) return;
        rescuing = true;
        StartCoroutine(Rescue(player));
    }
    IEnumerator Rescue(Player player)
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        Vector3 start = peach.position;
        for (float t = 0; t < 2; t += Time.deltaTime)
        {
            peach.position = start + Vector3.up * Mathf.Sin(t * 6) * .15f;
            yield return null;
        }
        SceneManager.LoadScene("Completed");
    }
}
