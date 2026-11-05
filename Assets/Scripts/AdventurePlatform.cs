using UnityEngine;

// Compute the lift's displacement before the player submits its physics move.
[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Collider2D))]
public class AdventurePlatform : MonoBehaviour
{
    public float travel = 3;
    private Rigidbody2D body;
    private Vector2 origin;
    private float startedAt;
    public Vector2 Displacement { get; private set; }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        if (!body) body = gameObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        origin = body.position;
        startedAt = Time.fixedTime;
    }

    private void FixedUpdate()
    {
        float phase = (Time.fixedTime - startedAt) * .8f;
        Vector2 target = origin + Vector2.up * (Mathf.Sin(phase) * travel);
        Displacement = target - body.position;
        body.MovePosition(target);
    }
}
