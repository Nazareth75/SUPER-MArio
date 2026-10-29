using UnityEngine;

public class CastleGlow : MonoBehaviour
{
    private SpriteRenderer image;
    void Awake() { image = GetComponent<SpriteRenderer>(); }
    void Update() { image.color = new Color(1, .91f + Mathf.Sin(Time.time * 4 + transform.position.x) * .09f, 1); }
}
