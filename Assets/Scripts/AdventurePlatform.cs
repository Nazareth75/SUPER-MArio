using UnityEngine;

public class AdventurePlatform : MonoBehaviour
{
    public float travel = 3;
    private Vector3 origin, previous;
    private Transform rider;
    void Start() { origin = previous = transform.position; }
    void FixedUpdate()
    {
        Vector3 position = origin + Vector3.up * Mathf.Sin(Time.fixedTime * .8f) * travel;
        if (rider) rider.position += position - previous;
        transform.position = previous = position;
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.transform.position.y > transform.position.y + .3f) rider = collision.transform;
    }
    void OnCollisionExit2D(Collision2D collision) { if (collision.transform == rider) rider = null; }
}
