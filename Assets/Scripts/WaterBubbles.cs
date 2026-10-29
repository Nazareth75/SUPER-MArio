using UnityEngine;

public class WaterBubbles : MonoBehaviour
{
    public float ceiling = 12;
    private Vector3 origin;
    void Start() { origin = transform.position; }
    void Update()
    {
        float y = Mathf.Repeat(Time.time * 1.3f + origin.x, ceiling);
        transform.position = new Vector3(origin.x + Mathf.Sin(Time.time * 2 + origin.x) * 0.25f, y, 0);
    }
}
