using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game constants
/// </summary>
public static class GameConstants 
{
    public const int DamageBullet = 5;
	public const int DamageEnemy = 20;
    public const int UnitsPerSecond = 10;
    public const float TimeForShoot = 0.5f;
    public const float BulletImpulseForce = 10f;
    public const float UfoImpulseForce = -3f;
    public const int MaxNumUFOs = 20;
    public const float MaxSpawnDelay = 1f;//2
    public const float MinSpawnDelay = 0.25f;//0.5
    public const float UfoMinShotDelay = 0.5f;
    public const float UfoMaxShotDelay = 1.5f;
}