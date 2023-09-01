using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using MarchingCubesProject;
using UnityEngine.UIElements;

public class Volume : MonoBehaviour
{
    public string imgFolder;

    public int l, w, minH, maxH;

    SortedList<int, Texture2D> imgs;
    List<int> temps;

    public void loadImgs()
    {
        imgs = new SortedList<int, Texture2D>();
        string[] filePaths = Directory.GetFiles(imgFolder, "*.png");
        int pathLen = imgFolder.Length;

        foreach (string file in filePaths)
        {
            byte[] fileData = File.ReadAllBytes(file);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.Apply();

            string f = file.Remove(file.Length - 4).Substring(pathLen + 1);
            int temp = int.Parse(f);

            imgs.Add(temp, tex);
        }
        
        temps = new List<int>(imgs.Keys);

        l = imgs.Values[0].height;
        w = imgs.Values[0].width;
        minH = imgs.Keys[0];
        maxH = imgs.Keys[imgs.Count - 1];

        Debug.Log(l.ToString() + ", " + w.ToString() + ", ");
    }

    private (int, int, double) getClosestImgs(double z)
    {
        int i = temps.BinarySearch((int)z);

        if (i >= 0)
        {
            if (i == temps.Count - 1)
            {
                return (i, i, 0);
            }
            return (i, i+1, 0);
        }
        else
        {
            int j = ~i;
            if (j == 0)
            {
                return (0, 0, 0);
            }
            else if(j == temps.Count)
            {
                return (j-1, j-1, 0);
            }
            else
            {
                double interp = (z - temps[j - 1]) / (temps[j] - temps[j - 1]);
                return (j - 1, j, interp);
            }
        }
    }

    public double Sample(double p, double q, double r, Color col)
    {
        //convert xyz to full space: max height, width, length
        double x = p * w;
        double y = q * l;
        double z = minH + (maxH - minH) * r;

        //get top and bottom img
        (int img1, int img2, double interp) = getClosestImgs(z);

        //get value based on color
        //Debug.Log(p.ToString() + " : " + x.ToString() + " : " + q.ToString() + " : " + y.ToString() + " : " + r.ToString() + " : " + z.ToString());

        //if ((int)x == l) x = l - 1;
        //if ((int)y == w) y = w - 1;
        Color col1 = imgs[imgs.Keys[img1]].GetPixel((int)x, (int)y);
        Color col2 = imgs[imgs.Keys[img2]].GetPixel((int)x, (int)y);

        //Debug.Log(col1.ToString() + " " + col2.ToString());

        int val1 = (col1 == col) ? 1 : 0;
        int val2 = (col2 == col) ? 1 : 0;

        double val = (1 - interp) * val1 + interp * val2;
        //Debug.Log(val1.ToString() + " " + val2.ToString() + " " + val.ToString());
        return val;
    }

    //public void SampleFill(VoxelArray vox, int zInd, int limX, int limY, double h, Color col)
    //{
    //    double z = minH + (maxH - minH) * h;
    //    (int img1, int img2, double interp) = getClosestImgs(z);

    //    for (int i = 0; i < limX; i++)
    //    {
    //        for (int j = 0; j < limY; j++)
    //        {
    //            double x = w * i / (limX - 1f);
    //            double y = l * j / (limY - 1f);

    //            Color col1 = imgs[imgs.Keys[img1]].GetPixel((int)x, (int)y);
    //            Color col2 = imgs[imgs.Keys[img2]].GetPixel((int)x, (int)y);

    //            int val1 = (col1 == col) ? 1 : 0;
    //            int val2 = (col2 == col) ? 1 : 0;
    //            //Debug.Log(i.ToString() + " " + j.ToString() + " " + zInd.ToString());
    //            vox[i, j, zInd] = (float)((1 - interp) * val1 + interp * val2);
    //        }
    //    }
    //}

    public void SampleFill(VoxelArray vox, int zInd, int limX, int limY, double h, Color col)
    {
        double z = minH + (maxH - minH) * h;
        (int img1, int img2, double interp) = getClosestImgs(z);

        ImageInterp interpolator = new ImageInterp(imgs[imgs.Keys[img1]], imgs[imgs.Keys[img2]], interp, col);
        float[,] middleImg = interpolator.interpolate();
        //Debug.Log(middleImg.GetLength(0));
        //Debug.Log(middleImg.GetLength(1));
        //Debug.Log(imgs[imgs.Keys[img1]].width);
        //Debug.Log(imgs[imgs.Keys[img1]].height);

        for (int i = 0; i < limX; i++)
        {
            for (int j = 0; j < limY; j++)
            {
                double x = w * i / (limX - 1f);
                double y = l * j / (limY - 1f);

                x = (x >= w) ? x - 1 : x;
                y = (y >= l) ? y - 1 : y;
                //Debug.Log(x.ToString() + " " + y.ToString() + " " + middleImg.GetLength(0).ToString() + " " + middleImg.GetLength(1).ToString());
                vox[i, j, zInd] = middleImg[(int)y, (int)x];
            }
        }
    }

    public Texture2D SampleAbs(int l, int w, double temp)
    {
        double z = temp;

        (int img1, int img2, double interp) = getClosestImgs(z);

        Texture2D tex1 = imgs[imgs.Keys[img1]];
        Texture2D tex2 = imgs[imgs.Keys[img2]];

        Texture2D newTex = new Texture2D(w, l);

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < l; j++)
            {
                Color col = Color.Lerp(tex1.GetPixel(i, j), tex2.GetPixel(i, j), (float) interp);
                newTex.SetPixel(i, j, col);
            }
        }

        return newTex;
    }


    private void Start()
    {
        //loadImgs();
        //Debug.Log(l.ToString() + ", " + w.ToString());
    }
}
