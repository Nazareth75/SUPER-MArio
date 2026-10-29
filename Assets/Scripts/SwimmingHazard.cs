using UnityEngine;

public class SwimmingHazard : MonoBehaviour
{
    private Vector3 origin;
    public float distance = 3, speed = 1.5f, phase;
    void Start() { origin = transform.position; }
    void Update()
    {
        float t = Time.time * speed + phase;
        transform.position = origin + new Vector3(Mathf.Sin(t) * distance, Mathf.Sin(t * 1.7f) * 0.6f, 0);
        GetComponent<SpriteRenderer>().flipX = Mathf.Cos(t) > 0;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player && !player.dead) player.Hit();
    }
}
