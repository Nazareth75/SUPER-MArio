using UnityEngine;

public class CastleHazard : MonoBehaviour
{
    public Sprite[] frames;
    public float driftSpeed;
    public float lifetime;
    private SpriteRenderer image;
    void Awake() { image = GetComponent<SpriteRenderer>(); }
    void Start() { if (lifetime > 0) Destroy(gameObject, lifetime); }
    void Update()
    {
        if (frames != null && frames.Length > 0) image.sprite = frames[(int)(Time.time * 6) % frames.Length];
        if (driftSpeed != 0) transform.position += Vector3.left * driftSpeed * Time.deltaTime;
        if (lifetime == 0) image.color = new Color(1, .85f + Mathf.Sin(Time.time * 5) * .15f, 1);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player && !player.dead) player.Hit();
    }
}
