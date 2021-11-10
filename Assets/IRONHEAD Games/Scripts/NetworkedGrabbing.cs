using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkedGrabbing : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks // Contains callbacks for pun ownership
{
    PhotonView m_photonView;

    Rigidbody rb;

    bool isBeingHeld = false;

    private void Awake()
    {
        m_photonView = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isBeingHeld) // Object is being grabbed
        {
            rb.isKinematic = true;
            gameObject.layer = 11; // InHand layer, we don't want others to grab it

        }
        else
        {
            rb.isKinematic = false;
            gameObject.layer = 9; // Interactable layer, now others can grab it
        }
    }

    private void TransferOwnership()
    {
        m_photonView.RequestOwnership(); // This is called after we try to grab, ownership is now requested. OnOwnershipRequest callback should be called 
    }

    public void OnSelectEntered()
    {
        Debug.Log("Grabbed");
        m_photonView.RPC("StartNetworkGrabbing", RpcTarget.AllBuffered);

        if(m_photonView.Owner == PhotonNetwork.LocalPlayer)
        {
            Debug.Log("We don't request ownership because it is already mine");
        }
        else
        {
            TransferOwnership();
        }
    }

    public void OnSelectExited()
    {
        Debug.Log("Released");
        m_photonView.RPC("StopNetworkGrabbing", RpcTarget.AllBuffered);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        // If there are many of the same object, there may be a problem as OnOwnershipRequest callback will be called for all of them
        // This method should be called only for hte grabbed object 
        if(targetView != m_photonView)
        {
            return;
        }


        Debug.Log("Ownership requested for: " + targetView.name + " from " + requestingPlayer.NickName);

        // Now we will transfer ownership
        m_photonView.TransferOwnership(requestingPlayer); // Once done, the OnOwnershipTransfered callback should be called
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log("OnOwnership Transferred to " + targetView.name + "from " + previousOwner.NickName);
    }

    // Fails for objects with "Takeover" Ownership transfer setting
    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        
    }

    
    [PunRPC]
    public void StartNetworkGrabbing()
    {
        isBeingHeld = true;
    }

    [PunRPC]
    public void StopNetworkGrabbing()
    {
        isBeingHeld = false;
    }
}
