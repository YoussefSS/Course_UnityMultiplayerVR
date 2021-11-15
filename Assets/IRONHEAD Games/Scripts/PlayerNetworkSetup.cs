using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject LocalXRRigGameobject; // The XR Rig under GenericVRPlayer prefab
    public GameObject MainAvatarGameobject;

    public GameObject AvatarHeadGameobject;
    public GameObject AvatarBodyGameobject;
    
    public GameObject[] AvatarModelPrefabs;

    public TextMeshProUGUI PlayerName_Text;

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

            // Getting the Avatar Selection data so that the correct avatar model can be instantiated
            object avatarSelectionNumber;
            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber)) // Getting the value
            {
                Debug.Log("Avatar selection number: " + (int)avatarSelectionNumber);
                photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, (int)avatarSelectionNumber);
            }


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

            MainAvatarGameobject.AddComponent<AudioListener>();

        }
        else // Photonview is not mine, so the player is the remote player
        {
            
            LocalXRRigGameobject.SetActive(false);

            // If the player is remote, set its avatar to the default layer
            SetLayerRecursively(AvatarHeadGameobject, 0); 
            SetLayerRecursively(AvatarBodyGameobject, 0); 
        }

        if(PlayerName_Text != null)
        {
            PlayerName_Text.text = photonView.Owner.NickName;
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

    [PunRPC]
    public void InitializeSelectedAvatarModel(int avatarSelectionNumber)
    {
        GameObject selectedAvatarGameobject = Instantiate(AvatarModelPrefabs[avatarSelectionNumber], LocalXRRigGameobject.transform);

        AvatarInputConverter avatarInputConverter = LocalXRRigGameobject.GetComponent<AvatarInputConverter>();
        AvatarHolder avatarHolder = selectedAvatarGameobject.GetComponent<AvatarHolder>();
        SetUpAvatarGameobject(avatarHolder.HeadTransform, avatarInputConverter.AvatarHead);
        SetUpAvatarGameobject(avatarHolder.BodyTransform, avatarInputConverter.AvatarBody);
        SetUpAvatarGameobject(avatarHolder.HandLeftTransform, avatarInputConverter.AvatarHand_Left);
        SetUpAvatarGameobject(avatarHolder.HandRightTransform, avatarInputConverter.AvatarHand_Right);
    }

    void SetUpAvatarGameobject(Transform avatarModelTransform, Transform mainAvatarTransform)
    {
        avatarModelTransform.SetParent(mainAvatarTransform);
        avatarModelTransform.localPosition = Vector3.zero;
        avatarModelTransform.localRotation = Quaternion.identity;
    }
}
