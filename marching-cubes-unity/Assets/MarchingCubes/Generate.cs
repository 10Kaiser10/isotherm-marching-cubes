using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

using ProceduralNoiseProject;
using Common.Unity.Drawing;
using System.Linq;

using UnityEditor;

namespace MarchingCubesProject
{

    public enum MARCHING_MODE {  CUBES, TETRAHEDRON };


    public class Generate : MonoBehaviour
    {

        public Material material;

        public MARCHING_MODE mode = MARCHING_MODE.TETRAHEDRON;

        public int seed = 0;

        public bool smoothNormals = true;

        public bool drawNormals = false;

        public Color segmentColor = Color.white;

        public Volume volume;

        public int gridResolutionXZ = 128;
        public int gridResolutionY = 128;

        private List<GameObject> meshes = new List<GameObject>();

        private NormalRenderer normalRenderer;

        public void LoadMesh()
        {
            UnityEngine.Object[] loadedMeshes = AssetDatabase.LoadAllAssetsAtPath("Assets/GeneratedMeshes/");

            foreach (var meshObj in loadedMeshes)
            {

                meshes.Add((GameObject) meshObj);
            }
        }

        public void SaveMesh()
        {
            foreach (var meshObj in meshes)
            {
                string name = meshObj.transform.parent.name;

                Mesh mesh = meshObj.GetComponent<MeshFilter>().sharedMesh;

                //AssetDatabase.CreateFolder(string "Assets/GeneratedMeshes/", string name)
                AssetDatabase.CreateAsset(mesh, "Assets/GeneratedMeshes/" + name);
                AssetDatabase.SaveAssets();
            }
        }

        public void createMesh()
        {
            float temp = Time.realtimeSinceStartup;
            //INoise perlin = new PerlinNoise(seed, 1.0f);
            //FractalNoise fractal = new FractalNoise(perlin, 3, 1.0f);

            //Volume volume = GetComponent<Volume>();
            //volume.loadImgs();

            //Set the mode used to create the mesh.
            //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface.
            Marching marching = null;
            if(mode == MARCHING_MODE.TETRAHEDRON)
                marching = new MarchingTertrahedron();
            else
                marching = new MarchingCubes();

            //Surface is the value that represents the surface of mesh
            //For example the perlin noise has a range of -1 to 1 so the mid point is where we want the surface to cut through.
            //The target value does not have to be the mid point it can be any value with in the range.
            marching.Surface = 0.5f;

            //The size of voxel array.
            int width = gridResolutionXZ;
            int height = gridResolutionXZ;
            int depth = gridResolutionY;

            var voxels = new VoxelArray(width, height, depth);
            voxels.FlipNormals = false;


            //Fill voxels with values. Im using perlin noise but any method to create voxels will work.
            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        for (int z = 0; z < depth; z++)
            //        {
            //            float u = x / (width - 1.0f);
            //            float v = y / (height - 1.0f);
            //            float w = z / (depth - 1.0f);

            //            voxels[x, y, z] = (float)volume.Sample(u, v, w, segmentColor);
            //        }
            //    }
            //}

            for (int z = 0; z < depth; z++)
            {
                float w = z / (depth - 1.0f);
                volume.SampleFill(voxels, z, width, height, w, segmentColor);
            }

            //Debug.Log("1: " + (Time.realtimeSinceStartup - temp).ToString());
            temp = Time.realtimeSinceStartup;

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> indices = new List<int>();

            //The mesh produced is not optimal. There is one vert for each index.
            //Would need to weld vertices for better quality mesh.
            marching.Generate(voxels.Voxels, verts, indices);

            //Create the normals from the voxel.

            if (smoothNormals)
            {
                for (int i = 0; i < verts.Count; i++)
                {
                    //Presumes the vertex is in local space where
                    //the min value is 0 and max is width/height/depth.
                    Vector3 p = verts[i];

                    float u = p.x / (width - 1.0f);
                    float v = p.y / (height - 1.0f);
                    float w = p.z / (depth - 1.0f);

                    Vector3 n = voxels.GetNormal(u, v, w);

                    normals.Add(n);
                }

                normalRenderer = new NormalRenderer();
                normalRenderer.DefaultColor = Color.red;
                normalRenderer.Length = 0.25f;
                normalRenderer.Load(verts, normals);
            }

            var position = new Vector3(-width / 2, -height / 2, -depth / 2);

            CreateMesh32(verts, normals, indices, position);
            //Debug.Log("2: " + (Time.realtimeSinceStartup - temp).ToString());
        }

        private void CreateMesh32(List<Vector3> verts, List<Vector3> normals, List<int> indices, Vector3 position)
        {
            Mesh mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;

            mesh.SetVertices(verts);
            mesh.SetTriangles(indices, 0);

            if (normals.Count > 0)
                mesh.SetNormals(normals);
            else
                mesh.RecalculateNormals();

            mesh.RecalculateBounds();

            GameObject go = new GameObject("Mesh");
            go.transform.parent = transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = material;
            go.GetComponent<MeshFilter>().mesh = mesh;
            go.transform.localPosition = position;

            mesh.triangles = mesh.triangles.Reverse().ToArray();

            meshes.Add(go);
        }

        /// <summary>
        /// UPDATE - Unity now supports 32 bit indices so the method is optional.
        /// 
        /// A mesh in unity can only be made up of 65000 verts.
        /// Need to split the verts between multiple meshes.
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="normals"></param>
        /// <param name="indices"></param>
        /// <param name="position"></param>
        private void CreateMesh16(List<Vector3> verts, List<Vector3> normals, List<int> indices, Vector3 position)
        {

            int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle
            int numMeshes = verts.Count / maxVertsPerMesh + 1;

            for (int i = 0; i < numMeshes; i++)
            {
                List<Vector3> splitVerts = new List<Vector3>();
                List<Vector3> splitNormals = new List<Vector3>();
                List<int> splitIndices = new List<int>();

                for (int j = 0; j < maxVertsPerMesh; j++)
                {
                    int idx = i * maxVertsPerMesh + j;

                    if (idx < verts.Count)
                    {
                        splitVerts.Add(verts[idx]);
                        splitIndices.Add(j);

                        if(normals.Count != 0)
                            splitNormals.Add(normals[idx]);
                    }
                }

                if (splitVerts.Count == 0) continue;

                Mesh mesh = new Mesh();
                mesh.indexFormat = IndexFormat.UInt16;
                mesh.SetVertices(splitVerts);
                mesh.SetTriangles(splitIndices, 0);

                if(splitNormals.Count > 0)
                    mesh.SetNormals(splitNormals);
                else
                    mesh.RecalculateNormals();

                mesh.RecalculateBounds();

                GameObject go = new GameObject("Mesh");
                go.transform.parent = transform;
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                go.GetComponent<Renderer>().material = material;
                go.GetComponent<MeshFilter>().mesh = mesh;
                go.transform.localPosition = position;

                meshes.Add(go);
            }
        }

        private void Update()
        {
            //transform.Rotate(Vector3.up, 10.0f * Time.deltaTime);
        }

        private void OnRenderObject()
        {
            if(normalRenderer != null && meshes.Count > 0 && drawNormals)
            {
                var m = meshes[0].transform.localToWorldMatrix;

                normalRenderer.LocalToWorld = m;
                normalRenderer.Draw();
            }
            
        }

    }

}