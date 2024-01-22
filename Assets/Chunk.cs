using System.Collections.Generic;
using UnityEngine;


public class Chunk : MonoBehaviour
{
    private Voxel[,,] voxels;
    private int chunkSize = 16;
    private Color gizmoColor;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Voxel.VoxelType chunkType;
    private Material chunkMaterial;

    void Start()
    {

        addMeshComponents();

        GenerateMesh(chunkType, chunkMaterial);

        
    }

    private void addMeshComponents(){
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
    }

    private void GenerateMesh(Voxel.VoxelType type, Material material)
    {

        IterateVoxels(type); // Make sure this processes all voxels

        Mesh mesh = new Mesh();
        mesh.vertices =   vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv =              uvs.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.material = material;
    }

    public void Initialize(int size, Voxel.VoxelType type, Material material)
    {
        this.chunkType = type;
        this.chunkSize = size;
        this.chunkMaterial = material;
        voxels = new Voxel[size, size, size];
        InitializeVoxels(type);
        gizmoColor = new Color(Random.value, Random.value, Random.value, 0.4f); // Semi-transparent
    }

    private void InitializeVoxels(Voxel.VoxelType type)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    Vector3 worldPos = transform.position + new Vector3(x, y, z);
                    try {
                    if (World.Instance.world[(int) worldPos.x, (int) worldPos.y, (int) worldPos.z] == TypeToInt(type)){
                        voxels[x, y, z] = new Voxel(worldPos, type, type != Voxel.VoxelType.Air);
                    }
                    } catch (System.IndexOutOfRangeException){
                        break;
                    }

                }
            }
        }
        
    }
    void OnDrawGizmos()
    {
        if (voxels != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(transform.position + new Vector3(chunkSize / 2, chunkSize / 2, chunkSize / 2), new Vector3(chunkSize, chunkSize, chunkSize));
        }
    }
    // New method to iterate through the voxel data
    public void IterateVoxels(Voxel.VoxelType type)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    ProcessVoxel(x, y, z, type);
                }
            }
        }
    }

    private void ProcessVoxel(int x, int y, int z, Voxel.VoxelType type)
    {
        // Check if the voxels array is initialized and the indices are within bounds
        if (voxels == null || x < 0 || x >= voxels.GetLength(0) || 
            y < 0 || y >= voxels.GetLength(1) || z < 0 || z >= voxels.GetLength(2))
        {
            return; // Skip processing if the array is not initialized or indices are out of bounds
        } 
        Voxel voxel = voxels[x, y, z];
        if (voxel.isActive && voxel.type == type)
        {
            // Check each face of the voxel for visibility
            bool[] facesVisible = new bool[6];

            // Check visibility for each face
            facesVisible[0] = IsFaceVisible(x, y + 1, z); // Top
            facesVisible[1] = IsFaceVisible(x, y - 1, z); // Bottom
            facesVisible[2] = IsFaceVisible(x - 1, y, z); // Left
            facesVisible[3] = IsFaceVisible(x + 1, y, z); // Right
            facesVisible[4] = IsFaceVisible(x, y, z + 1); // Front
            facesVisible[5] = IsFaceVisible(x, y, z - 1); // Back

            for (int i = 0; i < facesVisible.Length; i++)
            {
                if (facesVisible[i])
                    AddFaceData(x, y, z, i, voxel.type); // Method to add mesh data for the visible face
            }
        }
    }

    private bool IsFaceVisible(int x, int y, int z)
    {
        // Check if the neighboring voxel in the given direction is inactive or out of bounds
        if (x < 0 || x >= chunkSize || y < 0 || y >= chunkSize || z < 0 || z >= chunkSize)
            return true; // Face is at the boundary of the chunk
        return !voxels[x, y, z].isActive;
    }

    private void AddFaceData(int x, int y, int z, int faceIndex, Voxel.VoxelType type)
    {
        // Based on faceIndex, determine vertices and triangles
        // Add vertices and triangles for the visible face
        // Calculate and add corresponding UVs

        if (faceIndex == 0) // Top Face
        {
            vertices.Add(new Vector3(x,     y + 1, z    ));
            vertices.Add(new Vector3(x,     y + 1, z + 1)); 
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z    )); 
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
        }

        if (faceIndex == 1) // Bottom Face
        {
            vertices.Add(new Vector3(x,     y, z    ));
            vertices.Add(new Vector3(x + 1, y, z    )); 
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x,     y, z + 1)); 
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
        }

        if (faceIndex == 2) // Left Face
        {
            vertices.Add(new Vector3(x, y,     z    ));
            vertices.Add(new Vector3(x, y,     z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z    ));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 1));
        }

        if (faceIndex == 3) // Right Face
        {
            vertices.Add(new Vector3(x + 1, y,     z + 1));
            vertices.Add(new Vector3(x + 1, y,     z    ));
            vertices.Add(new Vector3(x + 1, y + 1, z    ));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
        }

        if (faceIndex == 4) // Front Face
        {
            vertices.Add(new Vector3(x,     y,     z + 1));
            vertices.Add(new Vector3(x + 1, y,     z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x,     y + 1, z + 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 1));
        }

        if (faceIndex == 5) // Back Face
        {
            vertices.Add(new Vector3(x + 1, y,     z    ));
            vertices.Add(new Vector3(x,     y,     z    ));
            vertices.Add(new Vector3(x,     y + 1, z    ));
            vertices.Add(new Vector3(x + 1, y + 1, z    ));
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 0));

        }
    
        AddTriangleIndices();
    }

    private void AddTriangleIndices()
    {
        int vertCount = vertices.Count;

        // First triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 3);
        triangles.Add(vertCount - 2);

        // Second triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 2);
        triangles.Add(vertCount - 1);
    }

    private int TypeToInt(Voxel.VoxelType type){
        switch(type){
            case(Voxel.VoxelType.Grass):
            return 1;
            case(Voxel.VoxelType.Dirt):
            return 2;
            case(Voxel.VoxelType.Stone):
            return 3;
            case(Voxel.VoxelType.Water):
            return 4;
        }
        return 0;
    }
}

