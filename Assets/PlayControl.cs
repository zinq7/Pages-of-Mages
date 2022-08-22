using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayControl : MonoBehaviour
{
    public void ExitPress()
    {
        Application.Quit(); 
    }

    public void PassPress()
    {
        SceneManager.LoadScene("PassAndPlay", LoadSceneMode.Single);
    }

    public void BotsPress()
    {
        //do other things
    }

    public void EditPress()
    {

    }
}
