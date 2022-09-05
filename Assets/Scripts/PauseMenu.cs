/*
 * Description: The control for pausing and continuing the game, with currently only EXIT   
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseWall;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject resumeButton;
    private GameManager game;
    bool resetClick;
    bool paused = false;
    List<float> animSpeeds = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        game = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape") == true)
        {
            if (paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        //turn off everything
        if (game != null)
        {
            resetClick = game.turnTransitionScreen.transform.parent.gameObject.GetComponent<InputControl>().waitingForClick;
            game.turnTransitionScreen.transform.parent.gameObject.GetComponent<InputControl>().waitingForClick = false;

            foreach (Animator anim in game.playingAnimations)
            {
                anim.speed = 0;
            }
        }

        paused = true;
        pauseWall.SetActive(true);
        pauseButton.SetActive(false);
        resumeButton.SetActive(true);

    }

    public void ResumeGame()
    {
        //turn on everythign that was turned off
        if (game != null)
        {
            game.turnTransitionScreen.transform.parent.gameObject.GetComponent<InputControl>().waitingForClick = resetClick;
            foreach (Animator anim in game.playingAnimations)
            {
                anim.speed = 1;
            }
        }
        
        paused = false;
        pauseWall.SetActive(false);
        pauseButton.SetActive(true);
        resumeButton.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
