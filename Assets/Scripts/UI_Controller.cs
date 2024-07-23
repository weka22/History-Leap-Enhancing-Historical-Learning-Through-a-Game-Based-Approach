using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Controller : MonoBehaviour
{
    public Slider _musicSlider;

    public GameObject finishMenu;
    public GameObject quizMenu;

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    }

    public void Restart()
    {
        Scenes_controller.Restart();
        Time.timeScale = 1f;
    }

    public void NextLevel()
    {
        Scenes_controller.NextLevel();
        Time.timeScale = 1f;
    }

    public void SceneLoad(int sceneIndex)
    {
        Scenes_controller.LoadScene(sceneIndex);
        Time.timeScale = 1f;
    }

    public void openFinishMenu()
    {
        finishMenu.SetActive(true);
        quizMenu.SetActive(false);
    }

    public void exitGame()
    {

    }
}
