using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingCubesProject;

public class IsothermViz : MonoBehaviour
{
    public int gridResolutionXZ = 128;
    public int gridResolutionY = 128;
    public List<Material> materials;
    private List<GameObject> meshes = new List<GameObject>();
    public GameObject axes;
    public Transform parentObj;

    public void initScene()
    {
        Volume volume = GetComponent<Volume>();
        volume.loadImgs();

        foreach (Material mat in materials)
        {
            GameObject newObj = new GameObject(mat.name);
            newObj.transform.parent = parentObj;

            newObj.AddComponent(typeof(Generate));
            Generate genScr = newObj.GetComponent<Generate>();
            genScr.mode = MARCHING_MODE.TETRAHEDRON;
            genScr.material = mat;
            genScr.segmentColor = mat.color;
            genScr.volume = volume;
            genScr.gridResolutionXZ = gridResolutionXZ;
            genScr.gridResolutionY = gridResolutionY;
            genScr.createMesh();
            newObj.transform.GetChild(0).transform.position = Vector3.zero;
            newObj.transform.GetChild(0).transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
            genScr.SaveMesh();
        }

        createAxes();
    }

    void Start()
    {
        //initScene();
    }

    void createAxes()
    {
        float z1 = -(12f / 350f) * gridResolutionXZ;
        float z2 = -(335f / 350f) * gridResolutionXZ;
        float x1 = (10f / 400f) * gridResolutionXZ;
        float x2 = (195f / 400f) * gridResolutionXZ;
        float x3 = (383f / 400f) * gridResolutionXZ;

        LineRenderer bottom = axes.transform.GetChild(0).GetComponent<LineRenderer>();
        bottom.SetPosition(0, new Vector3(x1, 0, z1));
        bottom.SetPosition(1, new Vector3(x3, 0, z1));
        bottom.SetPosition(2, new Vector3(x2, 0, z2));
        bottom.SetPosition(3, new Vector3(x1, 0, z1));

        LineRenderer one = axes.transform.GetChild(1).GetComponent<LineRenderer>();
        one.SetPosition(0, new Vector3(x1, 0, z1));
        one.SetPosition(1, new Vector3(x1, gridResolutionY, z1));

        LineRenderer two = axes.transform.GetChild(2).GetComponent<LineRenderer>();
        two.SetPosition(0, new Vector3(x3, 0, z1));
        two.SetPosition(1, new Vector3(x3, gridResolutionY, z1));

        LineRenderer three = axes.transform.GetChild(3).GetComponent<LineRenderer>();
        three.SetPosition(0, new Vector3(x2, 0, z2));
        three.SetPosition(1, new Vector3(x2, gridResolutionY, z2));
    }
}
