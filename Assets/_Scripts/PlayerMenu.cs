using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class PlayerMenu : MonoBehaviour
{
    public GameObject navigationStart;
    [SerializeField] protected Player player;
    [SerializeField] protected PlayerUIManager playerUIManager;
}
