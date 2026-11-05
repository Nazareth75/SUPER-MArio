using UnityEngine;

/// <summary>Pisoton basado en el contacto real, no en el angulo entre los centros.</summary>
public static class CombatContacts
{
    public static bool IsStomp(Player player, Collider2D enemy, Collision2D collision)
    {
        if (!player || !enemy || player.dead) return false;
        Bounds body = player.capsuleCollider.bounds;
        Bounds target = enemy.bounds;
        if (body.center.y <= target.center.y || body.min.y < target.max.y - 0.25f) return false;
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            if (Mathf.Abs(contact.normal.y) > 0.5f && contact.point.y >= target.max.y - 0.25f)
                return true;
        }
        return false;
    }
}