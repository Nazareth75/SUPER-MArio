using UnityEngine;

public class Goomba : MonoBehaviour
{
    private bool defeated;
    public Sprite flatSprite;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!defeated && collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player.starpower) {
                Hit();
            } else if (CombatContacts.IsStomp(player, GetComponent<Collider2D>(), collision)) {
                Flatten();
            } else {
                player.Hit();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shell")) {
            Hit();
        }
    }

    private void Flatten()
    {
        if (defeated) return;
        defeated = true;
        if (AdventureAudio.Instance) AdventureAudio.Instance.Play(AdventureAudio.Instance.stomp);
        ScoreManager.AddEnemyPoints(this);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EntityMovement>().StopMoving();
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = flatSprite;
        Destroy(gameObject, 0.5f);
    }

    private void Hit()
    {
        if (defeated) return;
        defeated = true;
        ScoreManager.AddEnemyPoints(this);
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<DeathAnimation>().enabled = true;
        Destroy(gameObject, 3f);
    }

}
