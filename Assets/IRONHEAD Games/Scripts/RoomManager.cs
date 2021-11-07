using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class RoomManager : MonoBehaviourPunCallbacks
{
    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI callback methods
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnCreatedRoom() // Called by Photon when a room is created
    {
        Debug.Log("A room is created with the name: " + PhotonNetwork.CurrentRoom.Name);
    }
    
    /*
    * Called only for the local player (me). So if a player other than me joins the room, OnJoinedRoom will not be called.
    * */
    public override void OnJoinedRoom() 
    {
        Debug.Log("The local player : " + PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    /*
    * Called when a new player joins the room that we are in. 
    * We can access the data of Player newPlayer
    * Not called at the player that joins, but everyone else
    * */
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to: " + PhotonNetwork.CurrentRoom.Name + " Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    #endregion


    #region Photon Callback Methods
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateAndJoinRoom();
    }
    #endregion


    #region Private Methods
    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room_" + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions(); // Needed as input parameter for PhotonNetwork.CreateRoom
        roomOptions.MaxPlayers = 20; // max players is in bytes so keep that in mind

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions); // We can also specify lobby but it is already created in the room creation/joining process
    }
    #endregion
}
