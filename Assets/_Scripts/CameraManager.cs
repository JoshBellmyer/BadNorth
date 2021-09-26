using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject cameraPrefab;

    Camera[] cameraAxis;

    // Start is called before the first frame update
    void Start()
    {
        int cameraCount = Game.instance.numPlayers;
        cameraAxis = new Camera[cameraCount];

        for(int i=0; i<cameraCount; i++)
        {
            GameObject cameraObject = Instantiate(cameraPrefab, transform);
            cameraAxis[i] = cameraObject.GetComponent<Camera>();
        }

        // Currently only supports two cameras
        cameraAxis[0].rect = new Rect(0, 0, 0.5f, 1);
        cameraAxis[1].rect = new Rect(0.5f, 0, 0.5f, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
