using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageInterp
{
    Texture2D img1;
    Texture2D img2;
    Color col;
    double interp;

    int sizeX;
    int sizeY;

    public ImageInterp(Texture2D i1, Texture2D i2, double z, Color c)
    {
        img1 = i1;
        img2 = i2;
        col = c;
        interp = z;

        sizeY = i1.width;
        sizeX = i1.height;
        Debug.Log(sizeX.ToString() + " " + sizeY.ToString());
    }

    float pixel(Texture2D img, int x, int y)
    {
        Color c = img.GetPixel(x, y);
        return (c == col) ? 1.0f : 0.0f;
    }

    float area(Texture2D img)
    {
        float a = 0;

        for (int i=0; i<sizeX; i++)
        {
            for (int j=0; j<sizeY; j++)
            {
                a += pixel(img, i, j);
            }
        }

        return a;
    }

    float area(float[,] arr)
    {
        float a = 0;

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                a += arr[i,j];
            }
        }

        return a;
    }

    float[,] getArr(Texture2D tex)
    {
        float[,] arr = new float[sizeX, sizeY];

        for (int i=0; i<sizeX; i++)
        {
            for (int j=0; j<sizeY; j++)
            {
                arr[i, j] = pixel(tex, j, i);
            }
        }

        return arr;
    }

    bool boundsCheck(int x, int y)
    {
        if (x < 0 | y < 0 | x >= sizeX | y >= sizeY) return false;

        return true;
    }

    float perim(float[,] img, float[,] i1, float[,] i2)
    {
        float peri = 0;

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                float sum = 0;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        int p = i + x;
                        int q = j + y;
                        if (!boundsCheck(p, q)) continue;

                        sum += img[p, q];
                    }
                }

                float im1p = i1[i, j];
                float im2p = i2[i, j];

                if (img[i,j] >= 1f && sum < 9f && (im1p + im2p) <= 1.9f)
                {
                    peri += 1f;
                }
            }
        }

        return peri;
    }

    float[,] erode(float[,] arr, float[,] i1, float[,] i2)
    {
        float[,] newArr = new float[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                float sum = 0;
                for (int x=-1; x<=1; x++)
                {
                    for (int y=-1; y<=1; y++)
                    {
                        int p = i + x;
                        int q = j + y;
                        if (!boundsCheck(p, q)) continue;

                        sum += arr[p, q];
                    }
                }

                float im1p = i1[i, j];
                float im2p = i2[i, j];

                if (arr[i, j] >= 0.5f)
                {
                    if (sum < 8.5f && (im1p + im2p) <= 1.9f)
                    {
                        newArr[i, j] = 0f;
                    }
                    else
                    {
                        newArr[i, j] = 1f;
                    }
                }
            }
        }

        return newArr;
    }

    float[,] blur(float[,] arr)
    {
        float[,] newArr = new float[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                float sum = 0;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        int p = i + x;
                        int q = j + y;
                        if (!boundsCheck(p, q)) continue;

                        sum += arr[p, q];
                    }
                }

                newArr[i, j] = sum / 9;

                //float im1p = pixel(img1, i, j);
                //float im2p = pixel(img2, i, j);


                
                //if ((im1p + im2p) >= 1.9f)
                //{
                //    newArr[i, j] = 1f;
                //}
                //else
                //{
                //    newArr[i, j] = 1f;
                //}
            }
        }

        return newArr;
    }

    public float[,] interpolate()
    {
        float a1 = area(getArr(img1));
        float a2 = area(getArr(img2));

        Texture2D img = img1;
        float bigA = a1;
        if (a2 > a1)
        {
            img = img2;
            bigA = a2;
        }

        float midA = (float)(a1 * (1 - interp) + a2 * (interp));
        //midA = a2;

        float[,] arr = getArr(img);

        //Debug.Log(a1.ToString() + " " + a2.ToString() + " " + midA.ToString() + " " + perim(arr).ToString() + " " + col.ToString());

        int count = 100;
        float[,] i1 = getArr(img1);
        float[,] i2 = getArr(img2);
        float p = perim(arr, i1, i2);
        float a = area(arr);

        while (a - p > midA && p >= 1 && count > 0)
        {
            arr = erode(arr, i1, i2);
            count--;
            p = perim(arr, i1, i2);
            a = area(arr);
        }


        arr = blur(arr);

        return arr;
    }
}
