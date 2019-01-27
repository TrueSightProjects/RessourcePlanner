using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeController : MonoBehaviour
{
    public Terrain terrain; // add current terrain
    public List<GameObject> trees; // this object will be placed on terrain
    public List<GameObject> rocks; // this object will be placed on terrain
    public List<GameObject> hotspot; // this object will be placed on terrain
    private List<GameObject> objectsPlaced;
    public int numberOfObjects; // number of how many objects will be created
    public int posMin; // minimum y position
    public int posMax; // maximum x position
    public bool posMaxIsTerrainHeight; // the maximum height is the terrain height
    private int terrainWidth; // terrain size x axis
    private int terrainLength; // terrain size z axis
    private int terrainPosX; // terrain position x axis
    private int terrainPosZ; // terrain position z axis
                             // Use this for initialization
    public int surfaceIndex;

    private TerrainData terrainData;
    private Vector3 terrainPos;
    void Start()
    {
        objectsPlaced = new List<GameObject>();
        terrainData = terrain.terrainData;
        terrainPos = terrain.transform.position;
        terrainWidth = (int)terrain.terrainData.size.x; // get terrain size x
        terrainLength = (int)terrain.terrainData.size.z; // get terrain size z
        terrainPosX = (int)terrain.transform.position.x; // get terrain position x
        terrainPosZ = (int)terrain.transform.position.z; // get terrain position z
        if (posMaxIsTerrainHeight == true)
        {
            posMax = (int)terrain.terrainData.size.y;
        }
    }
    // Update is called once per frame
    void Update()
    {
        // numberOfPlacedObjects is smaller than numberOfObjects
        if (objectsPlaced.Count < numberOfObjects)
        {
            PlaceObject(); // call function placeObject
        }
        if (objectsPlaced.Count == numberOfObjects)
        {
            print("Creating objects complete!");
        }
    }
    // Create objects on the terrain with random positions
    void PlaceObject()
    {

        int posx = Random.Range(terrainPosX, terrainPosX + terrainWidth); // generate random x position
        int posz = Random.Range(terrainPosZ, terrainPosZ + terrainLength); // generate random z position
        float posy = Terrain.activeTerrain.SampleHeight(new Vector3(posx, 0, posz)); // get the terrain height at the random position
        float normX = (float)posx / terrain.terrainData.heightmapWidth;
        float normY = (float)posz / terrain.terrainData.heightmapHeight;
        // get steepness from Unity terrain at the current position
        int itemRange = Random.Range(0, 1000);
        float y_01 = (float)posy / (float)terrain.terrainData.alphamapHeight;
        float x_01 = (float)posx / (float)terrain.terrainData.alphamapHeight;
        surfaceIndex = GetMainTexture(new Vector3(posx, posy, posz));
        float steepness = terrain.terrainData.GetSteepness(normY, normX);
        string terrainTextureName = terrainData.terrainLayers[surfaceIndex].name.ToLower();
        if (terrainTextureName.Contains("rock") || terrainTextureName.Contains("mountain") || terrainTextureName.Contains("stone"))
        {
            PlaceObject();
        }
        else {
            if (posy < posMax && posy > posMin)
            {
                if (itemRange < 850)
                {
                    int treeSelect = Random.Range(0, trees.Count);
                    GameObject newObject = (GameObject)Instantiate(trees[treeSelect], new Vector3(posx, posy, posz), Quaternion.identity); // create object
                    objectsPlaced.Add(newObject);
                }
                else if (itemRange > 850 && itemRange < 980)
                {
                    int rockSelect = Random.Range(0, rocks.Count);
                    GameObject newObject = (GameObject)Instantiate(rocks[rockSelect], new Vector3(posx, posy, posz), Quaternion.identity); // create object
                    objectsPlaced.Add(newObject);
                }
                else if (itemRange > 980)
                {
                    Debug.Log("You just got lucky with a hotspot!");
                }

            }
            else
            {
                PlaceObject();
            }
        }
        }

    private float[] GetTextureMix(Vector3 WorldPos)
    {
        // returns an array containing the relative mix of textures
        // on the main terrain at this world position.

        // The number of values in the array will equal the number
        // of textures added to the terrain.

        // calculate which splat map cell the worldPos falls within (ignoring y)
        int mapX = (int)(((WorldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((WorldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

        // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        // extract the 3D array data to a 1D array:
        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

        for (int n = 0; n < cellMix.Length; n++)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }
        return cellMix;
    }

    private int GetMainTexture(Vector3 WorldPos)
    {
        // returns the zero-based index of the most dominant texture
        // on the main terrain at this world position.
        float[] mix = GetTextureMix(WorldPos);

        float maxMix = 0;
        int maxIndex = 0;

        // loop through each mix value and find the maximum
        for (int n = 0; n < mix.Length; n++)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        return maxIndex;
    }

}
