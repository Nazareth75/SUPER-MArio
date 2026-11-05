#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityMovement : MonoBehaviour
{
    public float speed = 1f;
    public Vector2 direction = Vector2.left;

    private new Rigidbody2D rigidbody;
    private Vector2 velocity;
    private bool stopped;
    private Collider2D solidCollider;
    private readonly RaycastHit2D[] obstacles = new RaycastHit2D[16];
    public void StopMoving() { stopped = true; enabled = false; }
    public void ResumeMoving() { stopped = false; enabled = true; }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        foreach (var hitbox in GetComponents<Collider2D>())
            if (!hitbox.isTrigger) { solidCollider = hitbox; break; }
        enabled = false;
    }

    private void OnBecameVisible()
    {
        if (stopped || (GetComponent<DeathAnimation>() && GetComponent<DeathAnimation>().enabled)) return;
        #if UNITY_EDITOR
        enabled = !EditorApplication.isPaused;
        #else
        enabled = true;
        #endif
    }

    private void OnBecameInvisible()
    {
        enabled = false;
    }

    private void OnEnable()
    {
        velocity = Vector2.zero;
        rigidbody.WakeUp();
    }

    private void OnDisable()
    {
        rigidbody.linearVelocity = Vector2.zero;
        rigidbody.Sleep();
    }

    private bool IsBlockedAhead()
    {
        if (!solidCollider || !solidCollider.enabled || Mathf.Abs(direction.x) < 0.01f) return false;
        int mask = LayerMask.GetMask("Default");
        // Enemigos se dan la vuelta entre si; hongos y caparazones atraviesan enemigos.
        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
            mask |= LayerMask.GetMask("Enemy");
        var filter = new ContactFilter2D();
        filter.SetLayerMask(mask);
        filter.useTriggers = false;
        Vector2 heading = direction.x > 0 ? Vector2.right : Vector2.left;
        float distance = Mathf.Max(0.06f, speed * Time.fixedDeltaTime + 0.01f);
        int count = solidCollider.Cast(heading, filter, obstacles, distance);
        for (int i = 0; i < count; i++)
        {
            var hit = obstacles[i];
            if (!hit.collider || hit.rigidbody == rigidbody) continue;
            // No confundir el suelo con una pared ni girar por un objeto que ya queda atras.
            if (hit.collider.bounds.max.y <= solidCollider.bounds.min.y + 0.03f) continue;
            if ((hit.collider.bounds.center.x - solidCollider.bounds.center.x) * heading.x < -0.01f) continue;
            if (Mathf.Abs(hit.normal.x) > 0.5f && Vector2.Dot(hit.normal, heading) < -0.5f)
                return true;
        }
        return false;
    }

    private void FixedUpdate()
    {
        if (rigidbody.isKinematic || !solidCollider || !solidCollider.enabled) return;
        // Consultar antes de avanzar evita quedarse empujando la pared.
        if (IsBlockedAhead()) direction = new Vector2(-direction.x, direction.y);
        velocity.x = direction.x * speed;
        velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;
        if (rigidbody.Raycast(Vector2.down)) velocity.y = Mathf.Max(velocity.y, 0f);
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);

        if (direction.x > 0f) transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        else if (direction.x < 0f) transform.localEulerAngles = Vector3.zero;
    }
}