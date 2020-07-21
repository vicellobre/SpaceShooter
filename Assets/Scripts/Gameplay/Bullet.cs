using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// French fries
/// </summary>
public class Bullet : IntEventInvoker {
    [SerializeField]
    GameObject prefabExplosion;

    // saved for efficiency
    Rigidbody2D rb2d;
    SpriteRenderer spriteRenderer;
    float localScaleX;

    /// <summary>
    /// Initializes object. We don't use Start for this because
    /// we want to set up the points added event when we
    /// create the object in the french fries pool
    /// </summary>
    public void Initialize() {
        //prefabExplosion = Resources.Load<GameObject>("Explosion");
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        unityEvents.Add(EventName.TakeDamageEvent, new TakeDamageEvent());
        EventManager.AddInvoker(EventName.TakeDamageEvent, this);
        localScaleX = transform.localScale.x;
    }

    /// <summary>
    /// Starts the french fries moving
    /// </summary>
    public void StartMoving(Color color) {
        // apply impulse force to get projectile moving
        spriteRenderer.color = color;

        if (spriteRenderer.color == Color.red) {
            transform.localScale = new Vector2(localScaleX, transform.localScale.y);
            rb2d.AddForce(new Vector2(GameConstants.BulletImpulseForce, 0), ForceMode2D.Impulse);
        } else if(spriteRenderer.color == Color.green) {
            transform.localScale = new Vector2(-localScaleX, transform.localScale.y);
            rb2d.AddForce(new Vector2(-GameConstants.BulletImpulseForce, 0), ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Stops the french fries
    /// </summary>
    public void StopMoving() {
        rb2d.velocity = Vector2.zero;
        rb2d.velocity.Normalize();
        //rb2d.velocity.magnitude.
    }

    /// <summary>
    /// Called when the french fries become invisible
    /// </summary>
    void OnBecameInvisible() {
        // return to the pool
        PoolObjects.Return_Object(PoolObjects.PoolBullets, gameObject);
    }

    /// <summary>
    /// Processes trigger collisions with other game objects
    /// </summary>
    /// <param name="other">information about the other collider</param>
    void OnTriggerEnter2D(Collider2D other) {
        // if colliding with teddy bear, add score, destroy teddy bear,
        // and return self to pool
        if (other.gameObject.CompareTag(TagName.Player.ToString()) && spriteRenderer.color != Color.red) {
            Instantiate(prefabExplosion, other.gameObject.transform.position, Quaternion.identity);
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
            unityEvents[EventName.TakeDamageEvent].Invoke(GameConstants.DamageBullet);
            PoolObjects.Return_Object(PoolObjects.PoolBullets, gameObject);
            //AudioManager.Play(AudioClipName.Explosion);
        } else if (other.gameObject.CompareTag(TagName.UFO.ToString()) && spriteRenderer.color == Color.red) {
            // if colliding with teddy bear projectile, destroy projectile and 
            // return self to pool
            Instantiate(prefabExplosion, other.gameObject.transform.position, Quaternion.identity);
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
            PoolObjects.Return_Object(PoolObjects.PoolBullets, gameObject);
            PoolObjects.Return_Object(PoolObjects.PoolUFOs, other.gameObject);
            //AudioManager.Play(AudioClipName.Explosion);
        } else if (other.gameObject.CompareTag(TagName.Bullet.ToString()) && spriteRenderer.color != other.gameObject.GetComponent<SpriteRenderer>().color)
        {
            Instantiate(prefabExplosion, other.gameObject.transform.position, Quaternion.identity);
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
            PoolObjects.Return_Object(PoolObjects.PoolBullets, gameObject);
            PoolObjects.Return_Object(PoolObjects.PoolBullets, other.gameObject);
            //AudioManager.Play(AudioClipName.Explosion);
        }
    }
}