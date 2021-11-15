using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sand : MonoBehaviour
{
    [SerializeField] float risingSpeed;

    public static float height;

    // Start is called before the first frame update
    void Start()
    {
        Clock.instance.clockFinished += OnClockFinished;
    }

    private void Update () {
        height = transform.position.y - 0.7f;
    }

    private void OnClockFinished()
    {
        StartCoroutine("RaiseSand");
    }


    public IEnumerator RaiseSand()
    {
        while (true)
        {
            transform.position += new Vector3(0, risingSpeed * Time.deltaTime, 0);
            yield return null;
        }
    }
}
