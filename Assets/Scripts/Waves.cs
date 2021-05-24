using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public int dimensions = 10;

    protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;
    public Material[] material;
    protected Mesh mesh;
    public Octave[] octaves;
    public UI[] uiTools;

    void Start()
    {
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.name = gameObject.name;

        mesh.vertices = GenerateVertices();
        mesh.triangles = GenerateTriangles();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    private Vector3[] GenerateVertices()
    {
        var vertices = new Vector3[(dimensions + 1) * (dimensions + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        int count = 0;
        for (int x = 0; x <= dimensions; x++)
        {
            for (int z = 0; z <= dimensions; z++)
            {
                vertices[Index(x, z)] = new Vector3(x, 0, z);
                uv[count] = new Vector2((float)x / dimensions, (float)z / dimensions);
                tangents[count] = tangent;
                count++;
            }
        }
        mesh.uv = uv;
        mesh.tangents = tangents;
        return vertices;
    }

    private int Index(int x, int z)
    {
        return x * (dimensions + 1) + z;
    }

    private int[] GenerateTriangles()
    {
        var triangles = new int[mesh.vertices.Length * 6];
        for (int x = 0; x < dimensions; x++)
        {
            for (int z = 0; z < dimensions; z++)
            {
                triangles[Index(x, z) * 6 + 0] = Index(x, z);
                triangles[Index(x, z) * 6 + 1] = Index(x + 1, z + 1);
                triangles[Index(x, z) * 6 + 2] = Index(x + 1, z);

                triangles[Index(x, z) * 6 + 3] = Index(x, z);
                triangles[Index(x, z) * 6 + 4] = Index(x, z + 1);
                triangles[Index(x, z) * 6 + 5] = Index(x + 1, z + 1);
            }
        }
        return triangles;
    }

    // Update is called once per frame
    void Update()
    {
        var vertices = mesh.vertices;

        foreach (Octave octave in octaves)
        {
            octave.k = 2 * Mathf.PI / octave.wavelength;
            octave.c = Mathf.Sqrt(9.8f / octave.k);
            octave.direction = octave.direction.normalized;
        }
        for (int x = 0; x <= dimensions; x++)
        {
            for (int z = 0; z <= dimensions; z++)
            {

                Vector3 p = vertices[Index(x, z)];
                Vector3 combinedVector = new Vector3(x, 0, z);
                foreach (Octave octave in octaves)
                {
                    combinedVector = combinedVector + GerstnerWave(x, z, p, octave);
                }
                
                vertices[Index(x, z)] = new Vector3(combinedVector.x, combinedVector.y, combinedVector.z);
            }
        }
        mesh.vertices = vertices;
    }

    private Vector3 GerstnerWave(int x, int z, Vector3 p, Octave octave)
    {
        float f = octave.k * (Vector2.Dot(octave.direction, new Vector2(p.x, p.z)) - octave.c * Time.time);
        float a = octave.steepness / octave.k;
        p.x = x + octave.direction.x * (a * Mathf.Cos(f));
        p.y = a * Mathf.Sin(f);
        p.z = z + octave.direction.y * (a * Mathf.Cos(f));
        return p;
    }

    [Serializable]
    public class Octave
    {
        public Vector2 direction;
        [Range(0, 1)]
        public float steepness;
        public float wavelength;
        [HideInInspector]
        public float k;
        [HideInInspector]
        public float c;
    }

    public void SetOctaves(System.Single octaveNumber)
    {
        switch (octaveNumber)
        {
            case 1:
                UpdateOctave(new Vector2(1, 1), 0.25f, 60f, 0);
                UpdateOctave(new Vector2(1, 0.6f), 0.25f, 31f, 1);
                UpdateOctave(new Vector2(0.6f, 0.8f), 0.25f, 18f, 2);
                break;
            case 2:
                UpdateOctave(new Vector2(1, 1), 0.25f, 60f, 0);
                UpdateOctave(new Vector2(1, 0f), 0.25f, 60f, 1);
                UpdateOctave(new Vector2(-1f, 1f), 0.25f, 60f, 2);
                break;
            case 3:
                UpdateOctave(new Vector2(1, 1), 0.5f, 60f, 0);
                UpdateOctave(new Vector2(0.6f, 0.8f), 0.3f, 36f, 1);
                UpdateOctave(new Vector2(0.45f, 0.9f), 0.28f, 47f, 2);
                break;
            case 4:
                UpdateOctave(new Vector2(1f, 0f), 1f, 60f, 0);
                UpdateOctave(new Vector2(0.6f, 0.8f), 0f, 36f, 1);
                UpdateOctave(new Vector2(0.45f, 0.9f), 0f, 47f, 2);
                break;
        }
    }

    public Octave GetOctave(int octaveNumber)
    {
        return octaves[octaveNumber];
    }

    public void SetMaterial(System.Single matNumber)
    {
        switch (matNumber)
        {
            case 1:
                meshRenderer.material = material[0];
                break;
            case 2:
                meshRenderer.material = material[1];
                break;
            case 3:
                meshRenderer.material = material[2];
                break;
        }
    }

    public void UpdateSteepness(float _steepness, int waveNumber)
    {
        octaves[waveNumber].steepness = _steepness;
    }

    public void UpdateWavelength(float _wavelength, int waveNumber)
    {
        octaves[waveNumber].wavelength = _wavelength;
    }

    public void UpdateVector(Vector2 _vector, int waveNumber)
    {
        octaves[waveNumber].direction = _vector;
    }

    public void UpdateOctave(Vector2 _direction, float _steepness, float _wavelength, int waveNumber)
    {
        octaves[waveNumber].direction = _direction;
        octaves[waveNumber].steepness = _steepness;
        octaves[waveNumber].wavelength = _wavelength;
        uiTools[waveNumber].UpdateAllUIValues(_direction, _wavelength, _steepness);
    }
}
