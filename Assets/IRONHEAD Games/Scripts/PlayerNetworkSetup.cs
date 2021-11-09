using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject LocalXRRigGameobject; // The XR Rig under GenericVRPlayer prefab

    public GameObject AvatarHeadGameobject;
    public GameObject AvatarBodyGameobject;

    // Start is called before the first frame update
    void Start()
    {
        // We setup the player in the Start method


        /* What we are doing is every player keep their XR Rig as active, while others don't
         * This ensures that there is only one XR Rig active in the room
         * By doing this, every player will control their own XR Rig locally
         * Others will have their own rig, and to see them moving, their transform data will be transmitted
         * */
        if(photonView.IsMine) // VERY IMPORTANT: Tells us if the instantiated player is ours or not. 
        {
            // If IsMine, then this player is actually me, meaning it is the local player
            // If the player is local, we will keep the XR Rig active in the scene, if not we will disable it (in GenericVRPlayer)
            LocalXRRigGameobject.SetActive(true);

            // If photonview is mine, I will set the avatar head and body to local
            SetLayerRecursively(AvatarHeadGameobject,6); // 6 is the LocalAvatarHead layer number in the unity layers
            SetLayerRecursively(AvatarBodyGameobject,7); // 6 is the LocalAvatarBody

            TeleportationArea[] teleportationAreas = GameObject.FindObjectsOfType<TeleportationArea>();
            if(teleportationAreas.Length > 0)
            {
                Debug.Log("Found " + teleportationAreas.Length + " teleporation area. ");
                foreach(var item in teleportationAreas)
                {
                    item.teleportationProvider = LocalXRRigGameobject.GetComponent<TeleportationProvider>();
                }
            }

        }
        else // Photonview is not mine, so the player is the remote player
        {
            
            LocalXRRigGameobject.SetActive(false);

            // If the player is remote, set its avatar to the default layer
            SetLayerRecursively(AvatarHeadGameobject, 0); 
            SetLayerRecursively(AvatarBodyGameobject, 0); 
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    // This method helps us set the layers of the gameobjects
    void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

}
