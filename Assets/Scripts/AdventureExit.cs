using System.Collections;
using UnityEngine;

public class AdventureExit : MonoBehaviour
{
    public bool lastLevel;
    private bool entered;
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (!player || player.dead || entered) return;
        entered = true;
        StartCoroutine(Leave(player));
    }
    IEnumerator Leave(Player player)
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        if (AdventureAudio.Instance) { AdventureAudio.Instance.StopMusic(); AdventureAudio.Instance.Play(AdventureAudio.Instance.pipe); }
        Vector3 start = player.transform.position;
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            player.transform.position = start + Vector3.right * t * 2;
            player.transform.localScale = Vector3.one * (1 - t * 0.7f);
            yield return null;
        }
        if (lastLevel) UnityEngine.SceneManagement.SceneManager.LoadScene("Completed");
        else GameManager.Instance.NextLevel();
    }
}
