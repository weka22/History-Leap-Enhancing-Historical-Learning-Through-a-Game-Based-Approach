using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionController : MonoBehaviour
{
    public GameObject FinishScene;
    PlayerMove movement;

    private void Start()
    {
        movement = GameObject.FindObjectOfType<PlayerMove>();

        FinishScene.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("obstacle"))
        {
            Scenes_controller.Restart();
        }
        if (collision.gameObject.CompareTag("Finish"))
        {
            FinishGoal();
            //UnlockNewLevel();
            movement.canMove = false;
            
        }
    }

    public void UnlockNewLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
    private void FinishGoal()
    {
        FinishScene.SetActive(true);
        Debug.Log("goal");
    }
}
