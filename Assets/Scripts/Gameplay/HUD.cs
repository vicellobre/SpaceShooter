using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    Slider hp;
    GameObject reset;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        hp = GameObject.FindGameObjectWithTag(TagName.HP.ToString()).GetComponent<Slider>();
        reset = GameObject.FindGameObjectWithTag(TagName.Reset.ToString());
        reset.SetActive(false);
        EventManager.AddListener(EventName.HealthChangedEvent, HandleHealthChangedEvent);
        EventManager.AddListener(EventName.GameOverEvent, HandleGameOverEvent);
    }

    void HandleHealthChangedEvent(int damage) {
        hp.value -= damage;
    }

    void HandleGameOverEvent(int zero) {

        Destroy(hp.gameObject);
        Time.timeScale = 0;
        reset.SetActive(true);        
        AudioManager.Play(AudioClipName.GameOver);
    }

    /// <summary>
    /// Reset is called when the user hits the Reset button in the Inspector's
    /// context menu or when adding the component the first time.
    /// </summary>
    public void Reset()
    {
        PoolObjects.ReturnPoolObjects(PoolObjects.PoolUFOs, TagName.UFO.ToString());
        PoolObjects.ReturnPoolObjects(PoolObjects.PoolBullets, TagName.Bullet.ToString());

        Time.timeScale = 1;
        AudioManager.Play(AudioClipName.ButtonReset);
        SceneManager.LoadScene(0);
    }
}
