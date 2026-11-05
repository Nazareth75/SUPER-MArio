using UnityEngine;

public class CastleHazard : MonoBehaviour
{
    public Sprite[] frames;
    public float driftSpeed;
    public float lifetime;
    private SpriteRenderer image;
    private Rigidbody2D body;
    void Awake() { image = GetComponent<SpriteRenderer>(); }
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        if (lifetime > 0) Destroy(gameObject, lifetime);
    }
    void Update()
    {
        if (!image) return;
        if (frames != null && frames.Length > 0) image.sprite = frames[(int)(Time.time * 6) % frames.Length];
        if (lifetime == 0) image.color = new Color(1, .85f + Mathf.Sin(Time.time * 5) * .15f, 1);
    }
    void FixedUpdate()
    {
        if (driftSpeed == 0) return;
        Vector2 step = Vector2.left * driftSpeed * Time.fixedDeltaTime;
        if (body) body.MovePosition(body.position + step);
        else transform.position += (Vector3)step;
    }
    void OnTriggerEnter2D(Collider2D other) { Touch(other); }
    void OnTriggerStay2D(Collider2D other) { Touch(other); }
    private void Touch(Collider2D other)
    {
        var player = other.GetComponentInParent<Player>();
        if (player && !player.dead)
        {
            player.Hit();
            if (lifetime > 0) Destroy(gameObject);
        }
        else if (lifetime > 0 && !other.isTrigger && other.gameObject.layer == LayerMask.NameToLayer("Default"))
            Destroy(gameObject);
    }
}