using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sectionsGen : MonoBehaviour
{
    [Range(100f, 1500f)]
    public float temp;
    private float lastTemp;

    public Volume volume;
    public Material mat;

    Texture2D texture;
    int l, w;

    public Transform plane;

    // Start is called before the first frame update
    void Start()
    {
        volume.loadImgs();
        l = volume.l;
        w = volume.w;

        texture = new Texture2D(w, l);
    }

    // Update is called once per frame
    void Update()
    {
        if (temp != lastTemp)
        {
            generateTexture();

            Vector3 pos1 = plane.position;
            plane.position = new Vector3(pos1.x, 64*(temp-100)/1450f, pos1.z);
        }

        lastTemp = temp;
    }

    void generateTexture()
    {
        texture = volume.SampleAbs(l, w, temp);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        mat.mainTexture = texture;
    }
}
