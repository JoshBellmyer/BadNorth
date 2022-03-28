using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveComponent : MonoBehaviour
{
    [SerializeField] MonoBehaviour targetScript;
    [SerializeField] string savePath = "SaveData.dat";
    [SerializeField] bool loadOnStart = true;
    [SerializeField] bool saveOnDestroy = true;

    ISavable savable;

    // Start is called before the first frame update
    void Start()
    {
        if(!(targetScript is ISavable))
        {
            Debug.LogError("Object " + targetScript.name + " does not implement ISavable. To save/load an object from file, the object must implement ISavable");
        }
        savable = targetScript as ISavable;

        if (loadOnStart)
        {
            SaveData.Load(savable, savePath);
        }
    }

    void OnDestroy()
    {
        if (saveOnDestroy)
        {
            SaveData.Save(savable, savePath);
        }
    }
}
