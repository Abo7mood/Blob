using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
public class CollisionHelper : MonoBehaviour
{
    #region gameObjects
    public GameObject blob;// the blob
    public GameObject center;// the center
    public GameObject collision; // the conacted collision && I made this attribute to hide it from the inspector
    #endregion

    #region colliders
    [SerializeField] CircleCollider2D[] allSoftBodyColliders; // all the colliders reference, to avoid stucking problems
    [SerializeField] CircleCollider2D fix; // here is a quick collider, to avoid scale probelms, wired fix , but it works
    #endregion

    
    #region References
    private CollisionHelper collisionHelper;//reference
    private Player collisionBlobPlayer; //the player script for the second blob , when they contacted.

    private Player player; //reference
    private CollisionHandler CollisionHandler; //reference
    #endregion

    #region booleans
    public bool IsMainBone = false;

    public bool isCombining; // check if the blobs is touching each other or not 
    public bool isGrouping; // check if the blobs is touching each other or not 

    private bool isSpilit; //check if we are spiliting or not , to avoid multiple spiliting , like if we have 20 playercount , we will spilit 20 times, if we did not put this boolean , we will spilit depend on how much collision helper we have , so it will be 180 , and we just need 20
    private bool isDone; // just to avoid problem with scaling when we create the blob, I can explain it for you if you want
    #endregion

    #region buttons
    
    #endregion

    /// <summary>
    ///  // SerializeField mean , 
    ///  it will be inside the inspector , 
    ///  but it will not be public,
    ///  and you can't change it from other classes like : ColliisonHelper.cs
    /// </summary>
    [SerializeField] Vector2[] scales; // useless


    #region Events
    private UnityEvent OnCollisionEnter = new UnityEvent();
    private UnityEvent OnCollisionExit = new UnityEvent();
    public UnityEvent OnCombineEnter = new UnityEvent(); // collision to combine and split 
    public UnityEvent OnCombineExit = new UnityEvent(); // collision to combine and split 

    private UnityEvent OnMerge = new UnityEvent(); // the combine 
    private UnityEvent OnSpilit = new UnityEvent(); // the spilit 
    #endregion


    private void Awake() // this function happen before the Start And On Enable
    {
        player = blob.GetComponent<Player>(); // reference
        if (IsMainBone)
        {
            for (int i = 0; i < allSoftBodyColliders.Length; i++) allSoftBodyColliders[i].enabled = false; // disable all soft colliders
            StartCoroutine(ScaleDown(.5f)); 
        }
    }
    private void OnEnable()
    {
       
        isGrouping = true; // to avoid two combine happen at the same time 

        Debug.Log(gameObject.transform.root.name);
        CollisionHandler = gameObject.transform.root.GetComponent<CollisionHandler>();

        if (IsMainBone)
        {
            OnCollisionEnter.AddListener(CollisionHandler.MainBoneGrounded);
            OnCollisionExit.AddListener(CollisionHandler.MainBoneNotGrounded);


        }
        else
        {

            OnCollisionEnter.AddListener(CollisionHandler.IsGrounded);
            OnCollisionExit.AddListener(CollisionHandler.IsNotGrounded);
            ///the combining handlers
            OnCombineEnter.AddListener(isCombiningFunction);
            OnCombineExit.AddListener(isNotCombiningFunction);

            OnMerge.AddListener(Merge);
        }
        OnSpilit.AddListener(Spilit);


    }

    private void OnDisable()
    {
        if (IsMainBone)
        {
            OnCollisionEnter.RemoveListener(CollisionHandler.MainBoneGrounded);
            OnCollisionExit.RemoveListener(CollisionHandler.MainBoneNotGrounded);
        }
        else
        {
            OnCollisionEnter.RemoveListener(CollisionHandler.IsGrounded);
            OnCollisionExit.RemoveListener(CollisionHandler.IsNotGrounded);

            OnCombineEnter.RemoveListener(isCombiningFunction);
            OnCombineExit.RemoveListener(isNotCombiningFunction);

            OnMerge.RemoveListener(Merge);
        }
        OnSpilit.AddListener(Spilit);

        CollisionHandler = null;
    }

    private void Update()
    {
        Debug.Log(player.buttonCooldownStart + " + Cooldown");

        if (Input.GetMouseButton(0)) // lets say we are making the combine / merge function here , if we hold the merge function will activated.
        {
           player.timeSinceInteraction += Time.deltaTime;
            player.buttonCooldownStart += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0)&&player.playerCount>1&&!isSpilit&&IsMainBone&& player.buttonCooldownStart< player.buttonCooldown)  // call the spilit function , and check if we are pressing fast or slow
        {
            Debug.Log("WOrk");
            OnSpilit?.Invoke();
        }

        if (fix.enabled&&isDone) // if the collider is enable + this wired boolean to avoid problems, idk why I'm calling it isDone
        {
            StartCoroutine(ScaleUp(0.1f)); // just for stucking problems and these stuff, I'll explain it for you in chat
        }
        if (Input.GetMouseButtonUp(0)) player.buttonCooldownStart = 0;

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
       
         if (collision.gameObject.CompareTag("Player") && collision.transform.parent != transform.parent&&isGrouping) // check if the collider is touching another collider + make sure it isn't the same object brrr
        {
            this.collision = collision.gameObject; // this is just to reference it outside the oncollsion void 
           

            collisionHelper = this.collision.GetComponent<CollisionHelper>(); // reference
            collisionBlobPlayer = this.collision.GetComponent<CollisionHelper>().blob.GetComponent<Player>();//reference

            OnCombineEnter?.Invoke(); // I'll explain the (?) for you if you want .	

            if (IsBigger()) // check which one is bigger
            {
                OnMerge?.Invoke(); // do the merge / combine 
            }
           
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            OnCollisionEnter.Invoke();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            OnCollisionExit.Invoke();
        }
        if (collision.gameObject.CompareTag("Player") && collision.transform.parent != transform.parent) // check if the collider is touching another collider + make sure it isn't the same object brrr
        {
            OnCombineExit?.Invoke(); // I'll explain the (?) for you if you want .
        }
    }


    #region combiningSection
    /// <summary>
    /// // the => make it easier to read + it just take 1 block , I'll explain it more if u want
    /// </summary>
    public void isCombiningFunction() => isCombining = true;

    public void isNotCombiningFunction() => isCombining = false;

    private void Merge()
    {
        collisionHelper.isGrouping = false; // to avoid 2 merge at the same time
        if (isGrouping && player.canInteract)
        {
            player.playerCount += collisionBlobPlayer.playerCount; // add the size for the first blob, so 2+2=4 for instance

            Destroy(collisionBlobPlayer.gameObject); // destroy the second blob , since we eat it ? right?????

            player.StartInteractionTimer(); // start the timer 
            fix.enabled = true; // enable the collider
             
        }
    }

    private void Spilit()
    {
        for (int i = 0; i < player.playerCount; i++)
        {
            float rand = Random.value; // random value between 0 and 1 , to make some randomization for the posiiton when we create the blobs , COOL
            GameObject newBlob = Instantiate(BlobManager.instance.prefab, new Vector2(center.transform.position.x + rand, center.transform.position.y + rand), Quaternion.identity, null);// make a for loop to walk through all blobs and create them
        }
        isSpilit = true;
        Destroy(player.gameObject); // destroy the player 
    }
    #region this is just for fixing
    IEnumerator ScaleUp(float time)
    {
        yield return new WaitForSeconds(time);
        center.transform.localScale = new Vector2(player.playerCount, player.playerCount); // change the scale
        fix.enabled = false;
        for (int i = 0; i < allSoftBodyColliders.Length; i++)    allSoftBodyColliders[i].enabled = true; // enable all soft colliders

    }
    IEnumerator ScaleDown(float time)
    {
        yield return new WaitForSeconds(time);
        fix.enabled = false;
        isDone = true;
        for (int i = 0; i < allSoftBodyColliders.Length; i++) allSoftBodyColliders[i].enabled = true; // enable all soft colliders

    }

    #endregion

   
    #region varibles
    private bool IsBigger() => player.playerCount >= collisionBlobPlayer.playerCount;
    #endregion
    #endregion


}
