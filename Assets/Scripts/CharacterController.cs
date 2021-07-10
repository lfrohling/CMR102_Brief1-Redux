using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterController : MonoBehaviour
{

    /// <summary>
    /// The different states that our character can be in
    /// </summary>
    public enum CharacterStates {Idle,Roaming,Waving,Playing,Fleeing}

    public CharacterStates currentCharacterState; // the current state our character is in.


    
    
    public GameManager gameManager; // a reference to our game manager
    public Rigidbody rigidBody; // reference to Rigidbody

    // roaming state variables
    private Vector3 currentTargetPosition; // the target we are currently heading towards
    private Vector3 previousTargetPosition; // the last target we were heading towards.
    public float moveSpeed = 3; // how fast our character is moving.
    public float minDistanceToTarget = 1; // how close should we get to our target?

    // Idle state variables
    public float idleTime = 2; // once we reach our target position how longshould we wait till we get another position?
    private float currentIdleWaitTime; // the time we are waiting till we can move again. 

    // waving state variables
    public float waveTime = 2; // the time spent waving
    private float currentWaveTime; // the current time to wave till
    public float distanceToStartWavingFrom = 4f; // the distance that will be checking to see if we are in range to wave at another
    private CharacterController[] allCharactersInScene; // a collection of references to all characters in our scene.
    public float timeBetweenWaves = 5; // the time between we are able to wave again
    private float currentTimeBetweenWaves; // the current time for our next wave to start


    /// <summary>
    /// Returns the currentTargetPosition
    /// And sets the new current position
    /// </summary>
    private Vector3 CurrentTargetPosition
    {
        get
        {
            return currentTargetPosition; // gets the current value
        }
        set
        {
            previousTargetPosition = currentTargetPosition; // assign our current position to our previous target position
            currentTargetPosition = value; //assign the new value to our current target position
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentTargetPosition = gameManager.ReturnRandomPositionOnField(); // get a random starting position
        allCharactersInScene = FindObjectOfType<CharacterController>(); // find the references to all characers in our scene. 
        currentCharacterState = CharacterStates.Roaming; // set the character by default to start roaming
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Current Time is: " + Time.time);
        LookAtTargetPosition(); // always look towards the position we are aiming for.
        HandleRoamingState(); // call our roaming state function
        HandleIdleState(); // call our idle state function
        HandleWavingState(); // call out waving state fucntion
        HandleFleeingState(); // call our fleeing state function
        HandlePlayingState(); // call our playing state function
    }

   /// <summary>
   /// Handles the roaming state of our character
   /// </summary>
    private void HandleRoamingState()
    {
        /// if we are still too far away move closer
        if (currentCharacterState == CharacterStates.Roaming && Vector3.Distance(transform.position, CurrentTargetPosition) > minDistanceToTarget)
        {
            Vector3 targetPosition = new Vector3(CurrentTargetPosition.x, transform.position.y, CurrentTargetPosition.z); // the position we want to move forwards
            Vector3 nextMovePosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime); // the amount we should move towards that position
            rigidBody.MovePosition(nextMovePosition);
            currentIdleWaitTime = Time.time + idleTime;
        }
        else if (currentCharacterState == CharacterStates.Roaming) // so check to see if we're roaming
        {
            currentCharacterState = CharacterStates.Idle; // start idling
        }
    }

    /// <summary>
    /// Handle the idle state of our character
    /// </summary>
    private void HandleIdleState()
    {
        if (currentCharacterState == CharacterStates.Idle)
        {
            // we must be close enough to our target position
            // we wait a couple of seconds
            // then find a new position to move to.
            if (Time.time > currentIdleWaitTime)
            {
                //lets find a new position
                CurrentTargetPosition = gameManager.ReturnRandomPositionOnField();
                currentCharacterState = CharacterStates.Roaming; // start roaming again
            }
        }
    }

    /// <summary>
    /// Handles the fleeing state of our character
    /// </summary>
    private void HandleFleeingState()
    {
       
    }

    /// <summary>
    /// Handles the playing state of our character
    /// </summary>
    private void HandlePlayingState()
    {

    }

    /// <summary>
    /// Handles the waving state
    /// </summary>
    private void HandleWavingState()
    {
        if (ReturnCharacterTransformToWaveAt() != null && currentCharacterState != CharacterStates.Waving && Time.time > currentTimeBetweenWaves)
        {
            // we should start waving!
            currentCharacterState = CharacterStates.Waving;
            currentWaveTime = Time.time + waveTime; // set up the time we should be waving till.
            CurrentTargetPosition = ReturnCharacterTransformToWaveAt().position; //set the current target position to the closest transform, so that way we also rotate towards it.
        }
        if (currentCharacterState == CharacterStates.Waving && Time.time > currentWaveTime)
        {
            // stop waving
            CurrentTargetPosition = previousTargetPosition; // resume moving towards our random target position
            currentTimeBetweenWaves = Time.time + timeBetweenWaves; // set the next time when we can wave again
            currentCharacterState = CharacterStates.Roaming; // start roaming again
        }

    }

    /// <summary>
    /// returns a transform if they are in range of the player to be waved at.
    /// </summary>
    /// <returns></returns>
    private Transform ReturnCharacterTransformToWaveAt()
    {
        // looping through all the characters in our scene
        for(int i =0; i<allCharactersInScene.Length; i++)
        {
            // if the current element we are up to isn't equal to this instance of our character.
            if (allCharactersInScene[i] != this)
            {
                // check the distance between them, if they are close enough return that other character
                if(Vector3.Distance(transform.position, allCharactersInScene[i].transform.position) < distanceToStartWavingFrom)
                {
                    // but also let's return the character we should be waving at.
                    return allCharactersInScene[i].transform;
                }
            }
        }
        return null; 
    }

    /// <summary>
    /// Rotates our character to always face the direction HE is heading.
    /// </summary>
    private void LookAtTargetPosition()
    {
        Vector3 directionToLookAt = CurrentTargetPosition - transform.position; // direction we should be looking at
        directionToLookAt.y = transform.position.y; // don't change the y position
        Quaternion rotationOfDirection = Quaternion.LookRotation(directionToLookAt); // get a rotation we can look towards 
        transform.rotation = rotationOfDirection; // set our current rotation to our rotation to face towards
    }

    private void OnDrawGizmosSelected()
    {
        // draw a blue sphere on the position we are moving towards.
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(CurrentTargetPosition, 0.5f);
    }
}
