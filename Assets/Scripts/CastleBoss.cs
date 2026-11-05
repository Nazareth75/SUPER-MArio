using System.Collections;
using UnityEngine;

public class CastleBoss : MonoBehaviour
{
    public Sprite idle, idleAlternate, attack, damage, death;
    public Sprite fire;
    public AudioClip fireSound;
    public int health = 3;
    public bool defeated { get; private set; }
    private SpriteRenderer image;
    private Rigidbody2D body;
    private Vector3 origin;
    private float nextShot, invulnerableUntil, attackUntil;
    void Start()
    {
        image = GetComponent<SpriteRenderer>();
        origin = transform.position;
        health = Mathf.Max(1, health);
        body = GetComponent<Rigidbody2D>();
        if (!body) body = gameObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.useFullKinematicContacts = true;
        nextShot = Time.time + 2;
    }
    void Update()
    {
        if (defeated) return;

        image.sprite = Time.time < invulnerableUntil ? damage : Time.time < attackUntil ? attack : (int)(Time.time * 3) % 2 == 0 ? idle : idleAlternate;
        image.color = Time.time < invulnerableUntil && (int)(Time.time * 12) % 2 == 0 ? new Color(1, .4f, .4f) : Color.white;
        if (fire && Time.time >= nextShot)
        {
            nextShot = Time.time + (health == 1 ? 1.7f : 2.5f);
            attackUntil = Time.time + .5f;
            var shot = new GameObject("Fuego de Bowser");
            shot.transform.position = transform.position + new Vector3(-1.6f, .1f);
            var render = shot.AddComponent<SpriteRenderer>(); render.sprite = fire; render.sortingOrder = 4;
            shot.transform.localScale = Vector3.one * (.85f / fire.bounds.size.x);
            var hitbox = shot.AddComponent<BoxCollider2D>();
            hitbox.size = fire.bounds.size * 0.65f;
            hitbox.offset = fire.bounds.center;
            hitbox.isTrigger = true;
            var shotBody = shot.AddComponent<Rigidbody2D>();
            shotBody.bodyType = RigidbodyType2D.Kinematic;
            shotBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            var hazard = shot.AddComponent<CastleHazard>(); hazard.driftSpeed = 5; hazard.lifetime = 8;
            if (AdventureAudio.Instance) AdventureAudio.Instance.Play(fireSound);
        }
    }
    void FixedUpdate()
    {
        if (!defeated && body)
            body.MovePosition((Vector2)origin + Vector2.right * Mathf.Sin(Time.fixedTime * .75f) * 1.5f);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (!player || player.dead || defeated) return;
        if (player.starpower || CombatContacts.IsStomp(player, GetComponent<Collider2D>(), collision))
        {
            if (Time.time < invulnerableUntil) return;
            health--;
            invulnerableUntil = Time.time + 1.2f;
            if (AdventureAudio.Instance) AdventureAudio.Instance.Play(AdventureAudio.Instance.stomp);
            if (health <= 0) StartCoroutine(Defeat());
        }
        else if (Time.time >= invulnerableUntil) player.Hit();
    }
    IEnumerator Defeat()
    {
        defeated = true;
        GetComponent<Collider2D>().enabled = false;
        image.sprite = death; image.color = Color.white;
        if (AdventureAudio.Instance) { AdventureAudio.Instance.StopMusic(); AdventureAudio.Instance.Play(AdventureAudio.Instance.finish); }
        for (float t = 0; t < 1.5f; t += Time.deltaTime)
        {
            image.color = new Color(1, 1, 1, 1 - t / 1.5f);
            transform.position += Vector3.down * Time.deltaTime;
            yield return null;
        }
        image.enabled = false;
    }
    void OnGUI()
    {
        if (!Camera.main || Camera.main.transform.position.x < 80 || defeated) return;
        var text = new GUIStyle(GUI.skin.box) { fontSize = Mathf.Max(15, Screen.height / 35), alignment = TextAnchor.MiddleCenter };
        GUI.Box(new Rect(Screen.width * .2f, Screen.height * .14f, Screen.width * .6f, Screen.height * .09f), $"BOWSER   {new string('♥', health)}   ·   Salta sobre su cabeza", text);
    }
}
