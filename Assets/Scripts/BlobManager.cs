/// here we can set up everything
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobManager : MonoBehaviour
{
    public static BlobManager instance;
    public GameObject prefab;
    public List<GameObject> blobs; // (Useless)

    private void Awake()=> instance = this;
    
}
