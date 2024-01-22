using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class World : MonoBehaviour
{
    public int worldSize = 10;
    private int chunkSize = 16;

    private Dictionary<Voxel.VoxelType, Dictionary<Vector3, Chunk>> chunks;

    public static World Instance { get; private set; }

    public float cameraFOV = 1;

    public Material GrassMaterial;
    public Material DirtMaterial;
    public Material StoneMaterial;
    public Material WaterMaterial;

    public CinemachineVirtualCamera cam;

    public int[ , , ] world;



    void Start()
    {   
        this.world = DataManager.GetWorldData();
        chunks = new Dictionary<Voxel.VoxelType, Dictionary<Vector3, Chunk>>();
        chunks.Add(Voxel.VoxelType.Grass, new Dictionary<Vector3, Chunk>());
        chunks.Add(Voxel.VoxelType.Dirt, new Dictionary<Vector3, Chunk>());
        chunks.Add(Voxel.VoxelType.Stone, new Dictionary<Vector3, Chunk>());
        chunks.Add(Voxel.VoxelType.Water, new Dictionary<Vector3, Chunk>());

        calculateWorldCenter();

        GenerateWorld();
    }

    void calculateWorldCenter(){
        float x = (float) world.GetLength(0);
        float y = (float) world.GetLength(1);
        float z = (float) world.GetLength(2);

        cameraFOV = (((-1f) * (Mathf.Sqrt(x))) + (0.6f * x) + (0.5f)); 

        float xCenter = x / 2f;
        float yCenter = y / 2f;
        float zCenter = z / 2f;
        GameObject worldCenter = GameObject.Find("Center");
        worldCenter.transform.position = new Vector3(xCenter, yCenter, zCenter);


    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void GenerateWorld()
    {
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                for (int z = 0; z < worldSize; z++)
                {
                    CreateChunk(x, y, z, Voxel.VoxelType.Grass, GrassMaterial);
                    CreateChunk(x, y, z, Voxel.VoxelType.Dirt, DirtMaterial);
                    CreateChunk(x, y, z, Voxel.VoxelType.Stone, StoneMaterial);
                    CreateChunk(x, y, z, Voxel.VoxelType.Water, WaterMaterial);
                }
            }
        }
    }

    private void CreateChunk(int x, int y, int z, Voxel.VoxelType type, Material material)
    {
        Vector3 chunkPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
        GameObject newChunkObject = new GameObject($"Chunk_{type.ToString()}_{x}_{y}_{z}");
        newChunkObject.transform.position = chunkPosition;
        newChunkObject.transform.parent = this.transform;
        Chunk newChunk = newChunkObject.AddComponent<Chunk>();
        newChunk.Initialize(chunkSize, type, material);
        chunks[type].Add(chunkPosition, newChunk);
    }
}