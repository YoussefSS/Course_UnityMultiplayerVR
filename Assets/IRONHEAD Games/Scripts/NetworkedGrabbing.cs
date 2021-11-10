using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkedGrabbing : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks // Contains callbacks for pun ownership
{
    PhotonView m_photonView;

    private void Awake()
    {
        m_photonView = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TransferOwnership()
    {
        m_photonView.RequestOwnership(); // This is called after we try to grab, ownership is now requested. OnOwnershipRequest callback should be called 
    }

    public void OnSelectEntered()
    {
        Debug.Log("Grabbed");
        TransferOwnership();
    }

    public void OnSelectExited()
    {
        Debug.Log("Released");
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
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
}
