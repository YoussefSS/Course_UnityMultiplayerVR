using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpawnManager : MonoBehaviour
{
    [SerializeField] 
    GameObject GenericVRPlayerPrefab; // Prefab MUST be in the resources folder since it is instantiated using PhotonNetwork.Instantiate

    public Vector3 spawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        // We instantiate players in the start method
        if(PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(GenericVRPlayerPrefab.name, spawnPosition, Quaternion.identity); // Prefab MUST be in the resources folder
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
