using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Read_Draw_Convert : MonoBehaviour
{
    //Data before proper coversion (keeping string lines)
    public class File_Data_Lines
    {
        public List<string> Points = new List<string>();
        public List<string> Points_Index = new List<string>();
        public List<string> Texture_Points = new List<string>();
        public List<string> Texture_Points_Index = new List<string>();
    }

    //Data after proper conversion
    public class File_Data
    {
        public List<Vector3> Points = new List<Vector3>();
        public List<int> Points_Index = new List<int>();
        public List<Vector2> Texture_Points = new List<Vector2>();
        public List<int> Texture_Points_Index = new List<int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Open .wrl File
        string path = EditorUtility.OpenFilePanel("Select .wrl File", "", "wrl");

        if (path.Length != 0)
        {
            //Read File
            var dataLines = System.IO.File.ReadAllLines(path);

            //Parse and split file data (keep each line as string)
            var fileDataLines = splitData(dataLines);

            //Parse and Convert Data
            File_Data fileData = new File_Data();
            fileData.Points = getParsedDataToVector3(fileDataLines.Points);
            fileData.Points_Index = getParsedDataToListInt(fileDataLines.Points_Index);
            fileData.Texture_Points = getParsedDataToVector2(fileDataLines.Texture_Points);
            fileData.Texture_Points_Index = getParsedDataToListInt(fileDataLines.Texture_Points_Index);

            //Construct 3D Face Object
            var constructed3DFace = create3DObject(fileData);

            //Add 3D object to the Unity Scene
            constructed3DFace.transform.SetParent(this.transform);

            //Take Screenshot of the Scene
            ScreenCapture.CaptureScreenshot(path.Replace(".wrl", ".png"));
        }
    }

    //Parsing and splitting data (Keeping each line as string)
    private File_Data_Lines splitData(string [] dataLines)
    {
        File_Data_Lines fileDataLines = new File_Data_Lines();

        int stage = 0;
        for (int index = 0; index < dataLines.Length; index++)
        {
            dataLines[index] = dataLines[index].Trim().Replace(",", "");

            if (stage == 0)
            {
                if (dataLines[index] == "point [")
                    stage++;
            }
            else if (stage == 1)
            {
                if (dataLines[index] == "]")
                    stage++;
                else
                    fileDataLines.Points.Add(dataLines[index]);
            }
            else if (stage == 2)
            {
                if (dataLines[index] == "coordIndex [")
                    stage++;
            }
            else if (stage == 3)
            {
                if (dataLines[index] == "]")
                    stage++;
                else
                    fileDataLines.Points_Index.Add(dataLines[index]);
            }
            else if (stage == 4)
            {
                if (dataLines[index] == "point [")
                    stage++;
            }
            else if (stage == 5)
            {
                if (dataLines[index] == "]")
                    stage++;
                else
                    fileDataLines.Texture_Points.Add(dataLines[index]);
            }
            else if (stage == 6)
            {
                if (dataLines[index] == "texCoordIndex [")
                    stage++;
            }
            else if (stage == 7)
            {
                if (dataLines[index] == "]")
                    stage++;
                else
                    fileDataLines.Texture_Points_Index.Add(dataLines[index]);
            }
        }

        return fileDataLines;
    }

    #region Parse and Convert Lines
    private List<Vector3> getParsedDataToVector3(List<string> dataLines)
    {
        List<Vector3> vectors = new List<Vector3>();

        for (int index = 0; index < dataLines.Count; index++)
        {
            string line = dataLines[index].Trim();

            if (!string.IsNullOrEmpty(line))
            {
                var lineSplit = line.Split(new List<char>() { ' ' }.ToArray());
                float x = 0f;
                float y = 0f;
                float z = 0f;

                if (lineSplit.Length >= 3)
                {
                    x = (float)Convert.ToDouble(lineSplit[0]);
                    y = (float)Convert.ToDouble(lineSplit[1]);
                    z = (float)Convert.ToDouble(lineSplit[2]);

                    vectors.Add(new Vector3(x, y, z));
                }
            }
        }

        return vectors;
    }

    private List<int> getParsedDataToListInt(List<string> dataLines)
    {
        List<int> indices = new List<int>();

        for (int index = 0; index < dataLines.Count; index++)
        {
            string line = dataLines[index].Trim();

            if (!string.IsNullOrEmpty(line))
            {
                var lineSplit = line.Split(new List<char>() { ' ' }.ToArray());

                if (lineSplit.Length >= 3)
                {
                    indices.Add(Convert.ToInt32(lineSplit[0]));
                    indices.Add(Convert.ToInt32(lineSplit[1]));
                    indices.Add(Convert.ToInt32(lineSplit[2]));
                }
            }
        }
        return indices;
    }

    private List<Vector2> getParsedDataToVector2(List<string> dataLines)
    {
        List<Vector2> vectors = new List<Vector2>();

        for (int index = 0; index < dataLines.Count; index++)
        {
            string line = dataLines[index].Trim();

            if (!string.IsNullOrEmpty(line))
            {
                var lineSplit = line.Split(new List<char>() { ' ' }.ToArray());
                float x = 0f;
                float y = 0f;

                if (lineSplit.Length >= 2)
                {
                    x = (float)Convert.ToDouble(lineSplit[0]);
                    y = (float)Convert.ToDouble(lineSplit[1]);

                    vectors.Add(new Vector3(x, y));
                }
            }
        }

        return vectors;
    }
    #endregion Parse and Convert Lines

    //Construct 3D object to be able to render it in Unity
    private GameObject create3DObject(File_Data fileData)
    {
        GameObject face3D = new GameObject("face3D");
        face3D.AddComponent<MeshRenderer>();
        face3D.AddComponent<MeshFilter>();
        face3D.GetComponent<MeshRenderer>().materials = new List<Material>() { Resources.Load("Materials/Default_Material", typeof(Material)) as Material }.ToArray();
        face3D.GetComponent<MeshFilter>().mesh = new Mesh();
        face3D.GetComponent<MeshFilter>().mesh.vertices = fileData.Points.ToArray();
        face3D.GetComponent<MeshFilter>().mesh.uv = fileData.Texture_Points.ToArray();
        face3D.GetComponent<MeshFilter>().mesh.triangles = fileData.Points_Index.ToArray();

        face3D.GetComponent<MeshFilter>().mesh.RecalculateNormals();

        return face3D;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
