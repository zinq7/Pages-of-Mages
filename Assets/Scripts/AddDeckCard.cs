using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddDeckCard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(() => { this.Game(); });

        Debug.Log("AWAKE");
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Game()
    {

    }
}
