using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform soccerField; // a reference to our soccer field
    public Vector3 moveArea; // the size of our area where we can move 
    public Transform arCamera; // the reference to our AR Camera

    public GameObject soccerballPrefab; // a reference to the soccer ball in our scene.
    private GameObject currentSoccerBallInstance; // the current soccernall that has been spawned in
    public Transform aRContentParent; // reference to the overall parent of the AR Content.

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("new RandomPosition of:" + ReturnRandomPositionOnField());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Returns a random position within our move area/
    /// </summary>
    /// <returns></returns>
    public Vector3 ReturnRandomPositionOnField()
    {
        float xPosition = Random.Range(-moveArea.x / 2, moveArea.x / 2); // random number between negative moveArea X and positive moveArea X
        float yPosition = soccerField.position.y; // our soccer field y transform position (use that as our height)? 
        float zPosition = Random.Range(-moveArea.z / 2, moveArea.z / 2); // random number between negative moveArea Z and positive moveArea Z

        return new Vector3(xPosition, yPosition, zPosition);
    }

    /// <summary>
    /// this is a debug function, it lets us draw objects in our scene view, it's not viewable in the game view
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // if the user hasn't put a soccer field in, just get out of this function
       if(soccerField==null)
        {
            return;
        }
        Gizmos.color = Color.red; // sets my gizmo to red 
        Gizmos.DrawCube(soccerField.position + new Vector3(0,0.05f,0), moveArea); // draws a cube at the soccer fields position the size of our move area
    }

    /// <summary>
    /// Return true or false if we are too close or not close enough to the AR Camera
    /// </summary>
    /// <param name="character"></param>
    /// <param name="distanceThreshold"></param>
    /// <returns></returns>
    public bool IsPlayerToCloseToCharacter(Transform character, float distanceThreshold)
    {
        if (Vector3.Distance(arCamera.position, character.position) <= distanceThreshold)
        {
            // returns true if we are too close
            return true;
        }
        else
        {
            // returns false if we are too far away
            return false;
        }
    }
    
    /// <summary>
    /// Spawns in a new soccer ball based on the position provided. If a soccer ball already exists in the world we just want to move it to that new position
    /// </summary>
    /// <param name="positionToSpawn"></param>
    public void SpawnOrMoveSoccerBall(Vector3 positionToSpawn)
    {
        if(soccerballPrefab == null)
        {
            Debug.LogError("Something is wrong there is no soccerball assigned in the inspector");
            return;
        }

        // if the soccer ball isn't spawned into the world yet
        if (currentSoccerBallInstance = null)
        {
            // spawn in and store a reference to our soccer ball, and parent it to our ar content parent
            currentSoccerBallInstance = Instantiate(soccerballPrefab, positionToSpawn, soccerballPrefab.transform.rotation, aRContentParent);
            currentSoccerBallInstance.GetComponent<Rigidbody>().velocity = Vector3.zero; // sets the velocity of the soccer ball to 0
            currentSoccerBallInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // sets the angular velocity of the soccer ball to 0
        }
        else
        {
            // the soccer ball already exists, so lets just move it.
            currentSoccerBallInstance.transform.position = positionToSpawn; // move our soccer ball to the position to spawn
            currentSoccerBallInstance.GetComponent<Rigidbody>().velocity = Vector3.zero; // sets the velocity of the soccer ball to 0
            currentSoccerBallInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // sets the angular velocity of the soccer ball to 0
        }
    }

}
