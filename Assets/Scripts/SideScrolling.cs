using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SideScrolling : MonoBehaviour
{
    private new Camera camera;
    private Transform player;

    public float height = 6.5f;
    public float undergroundHeight = -9.5f;
    public float undergroundThreshold = 0f;
    public bool bounded;
    public float leftBound = 9, rightBound = 150;
    private float followVelocity;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        player = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        // track the player moving to the right
        Vector3 cameraPosition = transform.position;
        if (!player) return;
        cameraPosition.x = bounded
            ? Mathf.SmoothDamp(cameraPosition.x, Mathf.Clamp(player.position.x + 2, leftBound, rightBound), ref followVelocity, 0.18f)
            : Mathf.Max(cameraPosition.x, player.position.x);
        transform.position = cameraPosition;
    }

    public void SetUnderground(bool underground)
    {
        // set underground height offset
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;
    }

}
