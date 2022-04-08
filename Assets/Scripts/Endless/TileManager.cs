using System.Collections;
using System.Collections.Generic; //to use a collection list of game objects that includes the generated tiles
using UnityEngine;

public class TileManager : MonoBehaviour
{

    [Header("Standard Tiles")]                 //the title of the section on the component
    public GameObject[] tilePrefabs;

    [Header("Event Tiles")]                    //the title of the section on the component
    public GameObject exitTile;                //the tile to end the game selected by the event

    [Header("Cars")]                           //the title of the section on the component
    public GameObject[] carsPrefabs;

    [Header("Speeding Cars")]                  //the title of the section on the component
    public GameObject[] speedingCar;           //the selected event tiles

    private Transform playerTransform;         //to follow the player position
    private float spawnZ = 159.85f;            //where to set the starting position of the spawn on the Z axis    /// for the starting parking lot it is 159.85f  
    private float tileLength = 292.5f;         // set the distance from one object to the next.                   /// the single road module is 15.0; the 15 degree curved one is 292.5f
    private float safeZone = 900.0f;           // the safety distance at which the tiles are not destroyed
    private int amnTilesOnScreen = 11;         // The number of tiles to spawn at any given time
    private int lastPrefabIndex = 0;
    private float spawnCarOffset = 600.0f;     // Spawn cars at 700m from the player position
    public float spawnCarProbability = 0.8f;   // The probability to spawn a car in a random position at each frame (nico's original was 0.3f)
    private static System.Random rnd = new System.Random();

    // Use this in other classes which need to know the current event state to alter their behaviour
    private EventManager EventManager = null;


    public List<float> activeTileZ; //make a list of active tiles
    public Dictionary<float, GameObject> zToTile = new Dictionary<float, GameObject>(); // Translates Z values to tile references

    //Use this for initialization
    private void Start()
    {
        // Get reference to another script just by using the owner gameobject's tag
        EventManager = GameObject.FindWithTag("EventManager").GetComponent<EventManager>();

        activeTileZ = new List<float>();   // Save the Z offset of every tile created
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // to trigger the spawning of the tiles

        for (int i = 0; i < amnTilesOnScreen; i++)
        {
            if (i < 1)               //when creating the tile number 1 in the game
                SpawnTile(0);        //spawn a specific tile as the first one
            else                     //for all the following
                SpawnTile();         //just pick a random one
        }
        // After 2 seconds, spwan a car every 2 seconds
        if (GlobalVariables.Trial_type != GlobalVariables.TrialType.ADAPTATION)
            InvokeRepeating("SpawnCars", 2.0f, 2f);
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerTransform.position.z - safeZone > (spawnZ - amnTilesOnScreen * tileLength))  // to activate when the player pass a certain distance on Z axis
        {
            SpawnTile();    //create a new tile
            if (GlobalVariables.Trial_type != GlobalVariables.TrialType.ADAPTATION)
                SpawnCars();    //create a new car
            DeleteTile();   // delete the oldest Tile to limit the elements on screen and avoid memory issues
        }
    }


    private void SpawnTile(int prefabIndex = -1)  //just pick a tile
    {
        // [TODO] Toss a random coin to select between exit tile and normal tiles.
        float coin = Random.Range(0.0f, 1.0f);
        // Set the 30% of probability of getting an exit path only if we are in the exit state
        float target = (EventManager.state == EventManager.EventStates.Exit) ? 0.3f : 0.0f;
        // Positive case (spawn exit tile)
        if (false)//(coin < target)
        {
            GameObject go = Instantiate(exitTile) as GameObject;     //create a specific first tile
            go.transform.SetParent(transform);                       //make the new tile in a parent relation with the old ones

            zToTile[spawnZ] = go;
            activeTileZ.Add(spawnZ);

            go.transform.position = Vector3.forward * spawnZ;
            spawnZ += tileLength;

        }
        else // Spawn normal tiles
        {
            GameObject tile;
            if (prefabIndex == -1)
                tile = Instantiate(tilePrefabs[RandomPrefabsIndex()]) as GameObject; //create the object following the randomized object index
            else
                tile = Instantiate(tilePrefabs[prefabIndex]) as GameObject;          //create a specific first tile

            tile.transform.SetParent(transform);   //make the new tile in a parent relation with the old ones
            tile.transform.position = Vector3.forward * spawnZ;
            zToTile[spawnZ] = tile;
            activeTileZ.Add(spawnZ);
            spawnZ += tileLength;

            // Spawn a car
            //Transform firstNode = tile.transform.Find("Path1").GetComponent(typeof(Path)).nodes[1]; // get the position of the nodes (better if a random one from the list)
            // MAKE IT SPAWN
            //m_Cars = cars[Random.Range(0, cars.Count)];
        }
    }

    private void SpawnCars()  //just pick a car
    {
        float spawnCarCoin = Random.Range(0.0f, 1.0f);
        if (spawnCarCoin < spawnCarProbability)
        {
            GameObject carToSpawn = null;

            // Toss a random coin to select between speeding car and normal car.
            float carTypeCoin = Random.Range(0.0f, 1.0f);
            // Set the 20% of probability of getting a speeding car only if we are in the Overtake state
            float carTypeTarget = (EventManager.state == EventManager.EventStates.Overtake) ? 0.2f : 0.0f;
            // Positive case (spawn a speeding car)
            if (carTypeCoin < carTypeTarget)
                carToSpawn = speedingCar[rnd.Next(speedingCar.Length)]; //it use to be a single type of speeding car
            else
                carToSpawn = carsPrefabs[rnd.Next(carsPrefabs.Length)];

            // [TODO] Select between normal and speeding car
            GameObject go = Instantiate(carToSpawn) as GameObject;      //create a specific car
            go.transform.SetParent(transform);                                                      //make the new tile in a parent relation with the old ones

            // Set transform to a random node from a given distance from the player
            bool found = false;
            float foundTileZ = 0.0f;
            // [TODO] Refactor this algorithm as binary search
            // This finds the first tile with a given distance to the player
            foreach (float tileZ in activeTileZ)
            {
                if (tileZ > (GlobalVariables.playerZ + spawnCarOffset))
                {
                    found = true;
                    foundTileZ = tileZ;
                    break;
                }
            }
            if (!found)
                return;

            GameObject tile = zToTile[foundTileZ];
            // Change "Path1" to let the car pick the nodes from a different lane
            Path pathScript = (Path)tile.transform.Find("Path2").GetComponent(typeof(Path));
            // Pick a random node from the track
            go.transform.position = pathScript.nodes[rnd.Next(pathScript.nodes.Count)].position;
        }
    }

    private void DeleteTile()
    {
        Destroy(zToTile[activeTileZ[0]]);            // destroy the fist on the list (oldest)
        zToTile.Remove(activeTileZ[0]);              // remove the tile reference from the map
        activeTileZ.RemoveAt(0);                     // remove the tile z from the list
    }

    private int RandomPrefabsIndex()
    {
        if (tilePrefabs.Length <= 1)
            return 0;

        int randomIndex = lastPrefabIndex;
        while (randomIndex == lastPrefabIndex)
        {
            randomIndex = Random.Range(0, tilePrefabs.Length);
        }

        lastPrefabIndex = randomIndex;
        return randomIndex;
    }

    // Spawn exit tiles
    public void SpawnExit()
    {

    }
}