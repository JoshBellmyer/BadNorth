using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour
{
    protected TitleUIManager manager;
    public GameObject firstSelected;

    private void Awake()
    {
        manager = FindObjectOfType<TitleUIManager>();
    }
}