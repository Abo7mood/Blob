using UnityEngine;
using System;
using System.Collections.Generic;
public class Player : MonoBehaviour
{
    [Range(-0.2f, 0.2f)]
    public float EyeLookHorizontal;

    public GameObject[] Eyes;

    public int playerCount=1; // the size for the blob


    [Tooltip("The timeout between collision interactions.")]
    public float interactionCooldown = 2f; // here is the interaction cooldown, to avoid mutliple interactions at the same time

    public float timeSinceInteraction; // the starting timer
    public bool canInteract = false; // check if they can interact or not 

   [HideInInspector] public float buttonCooldownStart = 0; // the start timer (NOTE : the HideInInspector mean : this thing willl be public , and it will not shown inside the inspector at the same time).
    [Tooltip("The maximum time to hold to get the merge")]
    public float buttonCooldown=1;
    private void Start()
    {
        playerCount = 1; // set the size for 1
        canInteract = false;
       
    }
    private void Update()
    {
       
       

        // if the timer surpasses the cooldown...
        if (timeSinceInteraction >= interactionCooldown&&buttonCooldownStart>=buttonCooldown)
        {
            // this object can interact again
            canInteract = true;
        }

        //foreach (GameObject eye in Eyes)
        //{
        //    eye.transform.localPosition = new Vector3(0 + EyeLookHorizontal, 0, eye.transform.localPosition.z);
        //}

    }
    /// <summary>
    /// just reset the timer + make it uninteractible
    /// </summary>
    public void StartInteractionTimer()
    {
        timeSinceInteraction = 0;
        buttonCooldownStart = 0;
        canInteract = false;
    }
  
}
