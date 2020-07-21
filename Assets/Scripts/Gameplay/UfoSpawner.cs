using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoSpawner : MonoBehaviour
{
    // needed for spawning
	[SerializeField]
	GameObject prefabUFO;

	// spawn control
	Timer spawnTimer;

	// spawn location support
	Vector3 location = new Vector3();

	float minSpawnY;
	float maxSpawnY;

    float spawnX;

	// collision-free spawn support
	const int MaxSpawnTries = 20;
	float ufoColliderWidth;
	float ufoColliderHalfHeight;
	Vector2 min = new Vector2();
	Vector2 max = new Vector2();

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {

        // spawn and destroy a bird to cache collider values
		GameObject tempUFO = Instantiate(prefabUFO) as GameObject;
		BoxCollider2D collider = tempUFO.GetComponent<BoxCollider2D>();
		ufoColliderWidth = collider.size.y / 4;
		ufoColliderHalfHeight = collider.size.x / 4;
        Destroy(tempUFO);

		// save spawn boundaries for efficiency
		spawnX = ScreenUtils.ScreenRight + ufoColliderWidth;
        min.x = spawnX;
        max.x = spawnX;
		minSpawnY = ScreenUtils.ScreenTop - ufoColliderHalfHeight;
		maxSpawnY = ScreenUtils.ScreenBottom + ufoColliderHalfHeight;
		location.x = spawnX;
		location.z = -Camera.main.transform.position.z;

		// create and start timer
		spawnTimer = gameObject.AddComponent<Timer>();
		spawnTimer.AddTimerFinishedEventListener(HandleSpawnTimerFinishedEvent);
		StartRandomTimer();
	}

	/// <summary>
	/// Handles the spawn timer finished event
	/// </summary>
	private void HandleSpawnTimerFinishedEvent()
    {
		// only spawn a bird if below max number
		if (GameObject.FindGameObjectsWithTag(TagName.UFO.ToString()).Length < GameConstants.MaxNumUFOs)
        {
			SpawnUFO();
		}
		// change spawn timer duration and restart
		StartRandomTimer();
	}

	/// <summary>
	/// Spawns a new bird at a random location
	/// </summary>
	void SpawnUFO()
    {		
		// generate random location 
		location.y = Random.Range(minSpawnY, maxSpawnY);
		SetMinAndMax(location);

		// make sure we don't spawn into a collision
		int spawnTries = 1;
		while (Physics2D.OverlapArea(min, max) != null &&
			spawnTries < MaxSpawnTries)
        {
			// change location and calculate new rectangle points
			location.y = Random.Range(minSpawnY, maxSpawnY);
			SetMinAndMax (location);
			spawnTries++;
		}

		// create new bird if found collision-free location
		if (Physics2D.OverlapArea(min, max) == null)
        {
			GameObject ufo = PoolObjects.Get_Object(PoolObjects.PoolUFOs, prefabUFO);
			ufo.transform.position = location;
            ufo.SetActive(true);
            ufo.GetComponent<UFO>().StartMoving();
		}
	}

	/// <summary>
	/// Starts the timer with a random duration
	/// </summary>
	void StartRandomTimer()
    {
		spawnTimer.Duration = Random.Range(GameConstants.MinSpawnDelay, GameConstants.MaxSpawnDelay);
		spawnTimer.Run();
	}

	/// <summary>
	/// Sets min and max for a bird collision rectangle
	/// </summary>
	/// <param name="location">location of the bird</param>
	void SetMinAndMax(Vector3 location)
    {
		min.y = location.y - ufoColliderHalfHeight;
		max.y = location.y + ufoColliderHalfHeight;
	}
}
