/*
 * Description: Quick script to make it easier to set the text of the child
 * Author: Ben Farmilo
 * Date: 2022/08/16
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetText : MonoBehaviour
{
    public void ChangeTexts(string newText)
    {
        GetComponentInChildren<Text>().text = newText;
    }
}
