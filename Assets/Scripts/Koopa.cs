using UnityEngine;

public class Koopa : MonoBehaviour
{
    private bool defeated;
    public Sprite shellSprite;
    public float shellSpeed = 12f;

    private bool shelled;
    private bool pushed;
    private float shellGraceUntil;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!defeated && !shelled && collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player.starpower) {
                Hit();
            } else if (CombatContacts.IsStomp(player, GetComponent<Collider2D>(), collision)) {
                EnterShell();
            }  else {
                player.Hit();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!defeated && shelled && other.CompareTag("Player"))
        {
            var touchingPlayer = other.GetComponent<Player>();
            if (!touchingPlayer || touchingPlayer.dead) return;
            if (touchingPlayer.starpower) { Hit(); return; }
            if (Time.time < shellGraceUntil) return;
            if (!pushed)
            {
                Vector2 direction = new Vector2(transform.position.x - other.transform.position.x, 0f);
                PushShell(direction);
            }
            else
            {
                Player player = other.GetComponent<Player>();

                if (player.starpower) {
                    Hit();
                } else {
                    player.Hit();
                }
            }
        }
        else if (!shelled && other.gameObject.layer == LayerMask.NameToLayer("Shell"))
        {
            Hit();
        }
    }

    private void EnterShell()
    {
        shelled = true;
        shellGraceUntil = Time.time + 0.2f;
        var circle = GetComponent<CircleCollider2D>();
        if (circle && shellSprite)
        {
            float bottom = circle.offset.y - circle.radius;
            circle.radius = Mathf.Min(shellSprite.bounds.size.x, shellSprite.bounds.size.y) * 0.45f;
            circle.offset = new Vector2(circle.offset.x, bottom + circle.radius);
            foreach (var box in GetComponents<BoxCollider2D>())
                if (box.isTrigger) { box.size = Vector2.one * circle.radius * 2f; box.offset = circle.offset; }
        }
        if (AudioManager.Instance) AudioManager.Instance.PlayStomp();

        GetComponent<SpriteRenderer>().sprite = shellSprite;
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<EntityMovement>().StopMoving();
    }

    private void PushShell(Vector2 direction)
    {
        pushed = true;
        shellGraceUntil = Time.time + 0.2f;

        GetComponent<Rigidbody2D>().isKinematic = false;

        EntityMovement movement = GetComponent<EntityMovement>();
        movement.direction = direction.x >= 0f ? Vector2.right : Vector2.left;
        movement.speed = shellSpeed;
        movement.ResumeMoving();

        gameObject.layer = LayerMask.NameToLayer("Shell");
    }

    private void Hit()
    {
        if (defeated) return;
        defeated = true;
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<DeathAnimation>().enabled = true;
        Destroy(gameObject, 3f);
    }

    private void OnBecameInvisible()
    {
        if (pushed) {
            Destroy(gameObject);
        }
    }

}
