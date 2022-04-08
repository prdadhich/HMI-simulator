using System.Collections;
using System.Collections.Generic; //to use a list that includes the generated tiles
using UnityEngine;

public class CarManager : MonoBehaviour
{

    public GameObject[] AIcarPrefabs;

    private Transform playerTransform;     //to follow the player position
    private float spawnZ = 159.85f;        //where to set the starting position of the spawn on the Z axis    /// for the starting parking lot it is 159.85f  
    private float spawnX = 10.0f;         //where to set the starting position on X axis
    private float carLength = 92.5f;      // set the distance from one object to the next.                   /// the single road module is 15.0; the 15 degree curved one is 290.0f
    private float safeZone = 500.0f;       // the safety distance at which the tiles are not destroyed
    private int amnCarOnScreen = 10;       // The number of tiles to spawn at any given time
    private int lastPrefabIndex = 0;

    private List<GameObject> activeCar;   //make a list of active tiles

    //Use this for initialization
    private void Start()
    {

        activeCar = new List<GameObject>(); //the active tiles are the one in the list of gameobjects
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // to trigger the spawning of the tiles

        for (int i = 0; i < amnCarOnScreen; i++)
        {
            if (i < 1)               //when creating the tile number 1 in the game
                SpawnCar(0);        //spawn a specific tile as the first one
            else                     //for all the following
                SpawnCar();         //just pick a random one
        }

    }

    // Update is called once per frame
    private void Update()
    {
        if (playerTransform.position.z - safeZone > (spawnZ - amnCarOnScreen * carLength))  // to activate when the player pass a certain distance on Z axis
        {
            SpawnCar(); //create a new tile
            DeleteCar();   // delete the oldest Tile to limit the elements on screen and avoid memory issues

        }
    }


    private void SpawnCar(int prefabIndex = -1)  //just pick a tile
    {
        GameObject go;
        if (prefabIndex == -1)
            go = Instantiate(AIcarPrefabs[RandomPrefabsIndex()]) as GameObject; //create the object following the randomized object index
        else
            go = Instantiate(AIcarPrefabs[prefabIndex]) as GameObject;          //create a specific first tile

        go.transform.SetParent(transform);   //make the new tile in a parent relation with the old ones
        go.transform.position = (Vector3.forward * spawnZ) ;
        //go.transform.position = Vector3.left * spawnX;
        spawnZ += carLength;
        //spawnX += carLength;
        activeCar.Add(go);  //add the new tile to the active list


    }

    private void DeleteCar()
    {
        Destroy(activeCar[0]);  //destroy the fist on the list (oldest)
        activeCar.RemoveAt(0);
    }

    private int RandomPrefabsIndex()
    {
        if (AIcarPrefabs.Length <= 1)
            return 0;

        int randomIndex = lastPrefabIndex;
        while (randomIndex == lastPrefabIndex)
        {
            randomIndex = Random.Range(0, AIcarPrefabs.Length);
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