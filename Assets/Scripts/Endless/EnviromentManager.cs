using System.Collections;
using System.Collections.Generic; //to use a list that includes the generated tiles
using UnityEngine;

public class EnviromentManager : MonoBehaviour
{

    public GameObject[] envPrefabs;

    private Transform playerTransform;     //to follow the player position
    private float spawnZ = 159.85f;        //where to set the starting position of the spawn on the Z axis    /// for the starting parking lot it is 159.85f  
    private float envLength = 292.5f;     // set the distance from one object to the next.                   /// the single road module is 15.0; the 15 degree curved one is 290.0f
    private float safeZone = 500.0f;       // the safety distance at which the tiles are not destroyed
    private int amnEnvOnScreen = 10;     // The number of tiles to spawn at any given time
    private int lastPrefabIndex = 0;

    private List<GameObject> activeEnv; //make a list of active tiles

    //Use this for initialization
    private void Start()
    {

        activeEnv = new List<GameObject>(); //the active tiles are the one in the list of gameobjects
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // to trigger the spawning of the tiles

        for (int i = 0; i < amnEnvOnScreen; i++)
        {
            if (i < 1)               //when creating the tile number 1 in the game
                SpawnEnv(0);        //spawn a specific tile as the first one
            else                     //for all the following
                SpawnEnv();         //just pick a random one
        }

    }

    // Update is called once per frame
    private void Update()
    {
        if (playerTransform.position.z - safeZone > (spawnZ - amnEnvOnScreen * envLength))  // to activate when the player pass a certain distance on Z axis
        {
            SpawnEnv(); //create a new tile
            DeleteEnv();   // delete the oldest Tile to limit the elements on screen and avoid memory issues

        }
    }


    private void SpawnEnv(int prefabIndex = -1)  //just pick a tile
    {
        GameObject go;
        if (prefabIndex == -1)
            go = Instantiate(envPrefabs[RandomPrefabsIndex()]) as GameObject; //create the object following the randomized object index
        else
            go = Instantiate(envPrefabs[prefabIndex]) as GameObject;          //create a specific first tile

        go.transform.SetParent(transform);   //make the new tile in a parent relation with the old ones
        go.transform.position = Vector3.forward * spawnZ;
        spawnZ += envLength;
        activeEnv.Add(go);  //add the new tile to the active list


    }

    private void DeleteEnv()
    {
        Destroy(activeEnv[0]);  //destroy the fist on the list (oldest)
        activeEnv.RemoveAt(0);
    }

    private int RandomPrefabsIndex()
    {
        if (envPrefabs.Length <= 1)
            return 0;

        int randomIndex = lastPrefabIndex;
        while (randomIndex == lastPrefabIndex)
        {
            randomIndex = Random.Range(0, envPrefabs.Length);
        }

        lastPrefabIndex = randomIndex;
        return randomIndex;
    }

    /*

    // Spawn exit tiles
    public void SpawnExit()
    {

    }

    */
}