/*
 * Description: The control for pausing and continuing the game, with currently only EXIT   
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        resetClick = game.turnTransitionScreen.transform.parent.gameObject.GetComponent<InputControl>().waitingForClick;
        game.turnTransitionScreen.transform.parent.gameObject.GetComponent<InputControl>().waitingForClick = false;
        paused = true;
        pauseWall.SetActive(true);
        pauseButton.SetActive(false);
        resumeButton.SetActive(true);

        foreach (Animator anim in game.playingAnimations)
        {
            anim.speed = 0;
        }
    }

    public void ResumeGame()
    {
        //turn on everythign that was turned off
        game.turnTransitionScreen.transform.parent.gameObject.GetComponent<InputControl>().waitingForClick = resetClick;
        paused = false;
        pauseWall.SetActive(false);
        pauseButton.SetActive(true);
        resumeButton.SetActive(false);
        foreach (Animator anim in game.playingAnimations)
        {
            anim.speed = 1;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
