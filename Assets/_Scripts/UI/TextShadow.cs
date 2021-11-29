using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextShadow : MonoBehaviour
{
    Text parentText;
    Text thisText;

    private void Start()
    {
        parentText = transform.parent.GetComponent<Text>();
        if(parentText == null)
        {
            Debug.LogError("TextShadow has no parent text!");
            return;
        }

        thisText = GetComponent<Text>();
    }

    private void Update()
    {
        thisText.text = parentText.text;
    }
}
