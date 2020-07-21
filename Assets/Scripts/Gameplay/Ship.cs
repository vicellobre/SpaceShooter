using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ship : IntEventInvoker {

    // CONSTANTS
    float PositionOffset = 0.2f;

    // Native components of Unity
    GameObject prefabExplosion;

    // Primitive variables
    float vertical, limitBottom, limitTop;
    int health = 100;
    bool canShoot = true;

    //Variables create
    Timer timerShoot;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake() {
        // Inicialize Components
        timerShoot = gameObject.AddComponent<Timer>();

        // Inicialize prefabs
        prefabExplosion = Resources.Load<GameObject>("Explosion");
    }

    // Start is called before the first frame update
    void Start() {
        // Add Invokers
        unityEvents.Add(EventName.HealthChangedEvent, new HealthChangedEvent());
        EventManager.AddInvoker(EventName.HealthChangedEvent, this);

        unityEvents.Add(EventName.GameOverEvent, new GameOverEvent());
        EventManager.AddInvoker(EventName.GameOverEvent, this);

        EventManager.AddListener(EventName.TakeDamageEvent, TakeDamage);
        
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();

        //Locate in position initial
        float size = boxCollider2D.size.x / 4;
        float positionX = ScreenUtils.ScreenLeft + size;
        transform.position = new Vector2(positionX, 0);

        //Limits Top and Bottom of the Screen
        limitBottom = ScreenUtils.ScreenBottom + size;
        limitTop = ScreenUtils.ScreenTop - size;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update() {
        if (!canShoot &&
			Input.GetAxisRaw("Fire1") == 0)
        {
  			timerShoot.Stop();
			canShoot = true;
		}

        if (Input.GetAxis("Fire1") > 0 && !timerShoot.Running) {
            //Save position
            Vector3 position = transform.position;
            position.x += PositionOffset;
            // Load a bullet in the pool
            GameObject bullet = PoolObjects.Get_Object(PoolObjects.PoolBullets, PoolObjects.PrefabBullet);
            // Locate the butte on the Ship
            bullet.transform.position = position;
            // Enable in the scene
            bullet.SetActive(true);
            // Start to move the bullet
            bullet.GetComponent<Bullet>().StartMoving(Color.red);
            AudioManager.Play(AudioClipName.Shoot);

            //Start the cooldown
            timerShoot.Duration = GameConstants.TimeForShoot;
            timerShoot.Run();
            canShoot = false;
        }
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate() {

        //Moving vertically the Ship
        vertical = Input.GetAxis("Vertical");
        if (vertical != 0) {
            float positionY = transform.position.y + vertical * GameConstants.UnitsPerSecond * Time.fixedDeltaTime;
            transform.position = new Vector2(transform.position.x, Mathf.Clamp(positionY, limitBottom, limitTop));
        }
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(TagName.UFO.ToString())) {
            TakeDamage(GameConstants.DamageEnemy);
        }
    }

    void TakeDamage(int damage) {
        health = Mathf.Max(0, health - damage);
        unityEvents[EventName.HealthChangedEvent].Invoke(damage);

        if (health == 0) {
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
            unityEvents[EventName.GameOverEvent].Invoke(0);
            Destroy(gameObject);
        }
    }
}