using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UIWriter : MonoBehaviour
{
    private TextMeshProUGUI[] screenText;
    private void Start()
    {
        screenText = FindObjectsOfType<TextMeshProUGUI>();
    }
    public void SetText(int id, string text)
    {
        if (screenText == null) return;
        if (id < screenText.Length)
            screenText[id].text = text;
    }
}
