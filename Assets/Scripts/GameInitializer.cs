using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initializes the game
/// </summary>
public class GameInitializer : MonoBehaviour 
{
    /// <summary>
    /// Awake is called before Start
    /// </summary>
	void Awake()
    {
        // initialize screen utils
        
    }

    void Start() {
        EventManager.Initialize();
        ScreenUtils.Initialize();
        PoolObjects.InitializeUFOPool();
        PoolObjects.InitializeBulletPool();
        Instantiate(Resources.Load<GameObject>("Ship") as GameObject);
    }
}
