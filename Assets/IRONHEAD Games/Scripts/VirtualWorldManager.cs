using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class VirtualWorldManager : MonoBehaviourPunCallbacks
{

    // Singleton pattern.. allows us to access methods and classes from outside scripts
    public static VirtualWorldManager Instance;

    // Singleton pattern
    private void Awake()
    {
        if(Instance != null && Instance !=this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    // To be called from UI
    public void LeaveRoomAndLoadHomeScene()
    {
        PhotonNetwork.LeaveRoom();
    }


    #region Photon callback methods
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to: " + PhotonNetwork.CurrentRoom.Name + " Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    // Called after calling PhotonNetwork.LeaveRoom();
    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect(); // We disconnect as it may cause problems with voice chat
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("HomeScene");
    }
    #endregion
}
