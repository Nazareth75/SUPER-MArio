using UnityEngine;

public static class Extensions
{
    private static readonly RaycastHit2D[] castHits = new RaycastHit2D[8];
    private static LayerMask layerMask = LayerMask.GetMask("Default");

    // Checks if the rigidbody is colliding with an object in a given direction.
    // For example, if you want to check if the player is touching the ground,
    // you can pass Vector2.down. If you want to check if the player is running
    // into a wall, you can pass Vector2.right or Vector2.left.
    public static bool Raycast(this Rigidbody2D rigidbody, Vector2 direction)
    {
        if (rigidbody.isKinematic) {
            return false;
        }

        if (direction.sqrMagnitude < 0.0001f) return false;
        var filter = new ContactFilter2D();
        filter.SetLayerMask(layerMask);
        filter.useTriggers = false;
        // El barrido parte de la hitbox real: sirve para Mario pequeno/grande y caparazones.
        return rigidbody.Cast(direction.normalized, filter, castHits, 0.06f) > 0;
    }
    // Checks if the transform is facing another transform in a given direction.
    // For example, if you want to check if the player stomps on an enemy, you
    // would pass the player transform, the enemy transform, and Vector2.down.
    public static bool DotTest(this Transform transform, Transform other, Vector2 testDirection)
    {
        Vector2 direction = other.position - transform.position;
        return Vector2.Dot(direction.normalized, testDirection) > 0.25f;
    }

}
