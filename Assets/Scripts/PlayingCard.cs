/*
 * Description: The control for the visual aspect and interactable aspect of a playing card
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayingCard : MonoBehaviour
{
    public string team;
    public int id;
    public bool clicked;
    public Sprite back;
    private Sprite front;
    private Image sprt;
    private GameObject card;
    private bool needStart = true;
    private List<Transform> children = new List<Transform>();

    // Start is called before the first frame update
    void Create()
    {
        card = transform.GetChild(0).gameObject;
        sprt = card.GetComponent<Image>();
        front = sprt.sprite;

        for (int i = 0; i < transform.childCount + 1; i++)
        {
            children.Add(card.transform.GetChild(i));
        }
        needStart = false;
    }
    public void TurnCardToBack()
    {
        if (needStart) Create();
        sprt.sprite = back;
        card.GetComponent<Button>().interactable = false;
        foreach (Transform child in children)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void TurnCardToFront()
    {
        if (needStart) Create();
        sprt.sprite = front;
        card.GetComponent<Button>().interactable = true;
        foreach (Transform child in children)
        {
            child.gameObject.SetActive(true);
        }
    }
}
