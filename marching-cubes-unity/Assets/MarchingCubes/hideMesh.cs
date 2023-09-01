using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class hideMesh : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameObject obj = transform.GetChild(1).gameObject;
            obj.active = !obj.active;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameObject obj = transform.GetChild(2).gameObject;
            obj.active = !obj.active;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameObject obj = transform.GetChild(3).gameObject;
            obj.active = !obj.active;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameObject obj = transform.GetChild(4).gameObject;
            obj.active = !obj.active;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            GameObject obj = transform.GetChild(5).gameObject;
            obj.active = !obj.active;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            GameObject obj = transform.GetChild(6).gameObject;
            obj.active = !obj.active;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            GameObject obj = transform.GetChild(7).gameObject;
            obj.active = !obj.active;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            GameObject obj = transform.GetChild(8).gameObject;
            obj.active = !obj.active;
        }

    }
}
