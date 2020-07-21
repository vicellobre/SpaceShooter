using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// French fries
/// </summary>
public class UFO : IntEventInvoker {
    [SerializeField]
    GameObject prefabExplosion;

    // saved for efficiency
    Rigidbody2D rb2d;
    Timer cooldownTimer;
    bool canShoot = false;

    const float PositionOffset = 0.2f;

    /// <summary>
    /// Initializes object. We don't use Start for this because
    /// we want to set up the points added event when we
    /// create the object in the french fries pool
    /// </summary>
    public void Initialize() {
        rb2d = GetComponent<Rigidbody2D>();
        cooldownTimer = gameObject.AddComponent<Timer>();
    }

    /// <summary>
    /// Starts the french fries moving
    /// </summary>
    public void StartMoving() {
        // apply impulse force to get projectile moving
        rb2d.AddForce(new Vector2(GameConstants.UfoImpulseForce, 0), ForceMode2D.Impulse);
        canShoot = true;
        cooldownTimer.Duration = Random.Range(GameConstants.UfoMinShotDelay, GameConstants.UfoMaxShotDelay);
        cooldownTimer.Run();
    }


    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

        if (!cooldownTimer.Running && canShoot) {
            //Save position
            Vector3 position = transform.position;
            position.x -= PositionOffset;
            // Load a bullet in the pool
            GameObject bullet = PoolObjects.Get_Object(PoolObjects.PoolBullets, PoolObjects.PrefabBullet);
            // Locate the butte on the Ship
            bullet.transform.position = position;
            // Enable in the scene
            bullet.SetActive(true);
            // Start to move the bullet
            bullet.GetComponent<Bullet>().StartMoving(Color.green);
            //AudioManager.Play(AudioClipName.Shoot);

            //Start the cooldown
            cooldownTimer.Duration = Random.Range(GameConstants.UfoMinShotDelay, GameConstants.UfoMaxShotDelay);
            cooldownTimer.Run();
            canShoot = false;
        }

        if (!canShoot && cooldownTimer.Running)
        {
            canShoot = true;
        }
    }
    /// <summary>
    /// Stops the french fries
    /// </summary>
    public void StopMoving() {
        rb2d.velocity = Vector2.zero;
        canShoot = false;
        cooldownTimer.Stop();
    }

    /// <summary>
    /// Called when the french fries become invisible
    /// </summary>
    void OnBecameInvisible() {
        // return to the pool
        PoolObjects.Return_Object(PoolObjects.PoolUFOs, gameObject);
    }

    /// <summary>
    /// Processes trigger collisions with other game objects
    /// </summary>
    /// <param name="other">information about the other collider</param>
    void OnTriggerEnter2D(Collider2D other) {
        // if colliding with teddy bear, add score, destroy teddy bear,
        // and return self to pool
        if (other.gameObject.CompareTag(TagName.Player.ToString())) {
            //unityEvents[EventName.PointsAddedEvent].Invoke(ConfigurationUtils.BearPoints);
            Instantiate(prefabExplosion, other.gameObject.transform.position, Quaternion.identity);
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
            PoolObjects.Return_Object(PoolObjects.PoolUFOs, gameObject);
        }
    }
}