using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class UnitData : ScriptableObject
{
    public UnitType type;
    public Sprite sprite;
    public float cooldown;
}
