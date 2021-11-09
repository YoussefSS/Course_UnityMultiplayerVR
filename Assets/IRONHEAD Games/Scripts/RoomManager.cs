using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private string mapType;

    public TextMeshProUGUI OccupancyRateText_ForSchool;
    public TextMeshProUGUI OccupancyRateText_ForOutdoor;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // Scenes will now be synchronized for all players.. ie other players will load the same game scene when they join the same room we are in. Scene will be loaded before OnJoinedRoom callback method is called

        if(!PhotonNetwork.IsConnectedAndReady) // This is how we check if we are connected to the servers or not
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }

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

    public void OnEnterButtonClicked_OutDoor() // Check the school function for documentations
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR;

        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0); 
    }

    public void OnEnterButtonClicked_School()
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL;

        /* Documentation https://doc.photonengine.com/en-us/realtime/current/lobby-and-matchmaking/matchmaking-and-lobby
        * Under Filtering Room Properties In Join Random
        * */
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { {MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };

        /* Documentation https://doc-api.photonengine.com/en/pun/v2/class_photon_1_1_pun_1_1_photon_network.html#a497af0481f21c6a4647b886989e63a32
         * */
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0); // 0 means any max player value
    }
    #endregion


    #region Photon Callback Methods

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

        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(MultiplayerVRConstants.MAP_TYPE_KEY))
        {
            object mapType;
            if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.MAP_TYPE_KEY,out mapType)) // Try get value takes key and has a object type as output
            {
                // Checking to see which map we joined
                Debug.Log("Joined room with the map: " + (string)mapType);

                // We load the map based on which mapType we get from the properties
                if((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL)
                {
                    // Load the school scene, since we enabled scene synchronization, other players will load the same game scene when they join the same room we are in
                    PhotonNetwork.LoadLevel("World_School");
                }
                else if ((string) mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR)
                {
                    // Load outdoor scene
                    PhotonNetwork.LoadLevel("World_Outdoor");
                }
            }
        }
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

    /*
     * Called when someone joins / creates room or changes room properties
     * Called while in a lobby, so we need to connect to the default lobby to get the room list updates, we do that in the Start method
     */
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(roomList.Count == 0)
        {
            // There is no room at all
            OccupancyRateText_ForSchool.text = 0 + "/" + 20;
            OccupancyRateText_ForOutdoor.text = 0 + "/" + 20;
        }

        foreach(RoomInfo room in roomList)
        {
            Debug.Log(room.Name);

            // We cannot access the custom properties of a room if we are not inside that room, we can only access the CustomRoomPropertiesForLobby, but that only contains the map key, and does not tell us the map value

            if(room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR))
            {
                // Update the outdoor room occupancy field
                Debug.Log("Room is a Outdoor map. Player count is: " + room.PlayerCount);
                OccupancyRateText_ForOutdoor.text = room.PlayerCount + "/" + 20;
            }
            else if(room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL))
            {
                // Update the school room occupancy field
                Debug.Log("Room is a School map. Player count is: " + room.PlayerCount);
                OccupancyRateText_ForSchool.text = room.PlayerCount + "/" + 20;
            }
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateAndJoinRoom();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to servers again");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined the Lobby.");
    }

    #endregion


    #region Private Methods

    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room_" + mapType + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions(); // Needed as input parameter for PhotonNetwork.CreateRoom
        roomOptions.MaxPlayers = 20; // max players is in bytes so keep that in mind

        /* props means properties
         * A Lobby is where photon organizes the rooms
         * There are 3 types of lobbies.. default, SQL and async. We will use default
         * explained here https://doc.photonengine.com/en-us/realtime/current/lobby-and-matchmaking/matchmaking-and-lobby under Exposing Some Properties In The Lobby
         * */
        string[] roomPropsInLobby = { MultiplayerVRConstants.MAP_TYPE_KEY };

        /* We have to different maps
         * 1. Outdoor = "outdoor"
         * 2. School = "school"
         * We set them up in a custom script we created called MultiplayerVRConstants
         * */
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { {MultiplayerVRConstants.MAP_TYPE_KEY, mapType } }; // mapType is set in OnEnterButtonClicked functions
        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby; // this will set the room properties that will be shown in the lobby
        roomOptions.CustomRoomProperties = customRoomProperties; // This is where we can set properties for our room


        PhotonNetwork.CreateRoom(randomRoomName, roomOptions); // We can also specify lobby but it is already created in the room creation/joining process
    }


    #endregion
}
