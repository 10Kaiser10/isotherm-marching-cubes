using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotateSpeed = 1.0f;
    public IsothermViz isoViz;

    private int size = 1;

    private void Start()
    {
        size = isoViz.gridResolutionXZ;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.RotateAround(new Vector3(size/2, 0, -size/2), Vector3.up, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.RotateAround(new Vector3(size / 2, 0, -size / 2), Vector3.up, -rotateSpeed * Time.deltaTime);
        }
    }
}
