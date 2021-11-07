using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class LoginManager : MonoBehaviourPunCallbacks // We use this instead of MonoBehaviour, to use PunCallbacks
{
    public TMP_InputField PlayerName_InputName;

    #region Unity Methods
    void Start()
    {
       
    }


    void Update()
    {
        
    }
    #endregion


    #region UI Callback Methods
    public void ConnectAnonymously()
    {
        PhotonNetwork.ConnectUsingSettings(); // This connects us to Photon
    }

    public void ConnectToPhotonServer()
    {
        if(PlayerName_InputName != null)
        {
            PhotonNetwork.NickName = PlayerName_InputName.text;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    #endregion

    #region Photon Callback Methods
    public override void OnConnected() // Called when the internet connection is established (first method that is called when we initiate the connection process)
    {
        //base.OnConnected(); // don't need it

        Debug.Log("OnConnected is called. The server is available!");
    }

    public override void OnConnectedToMaster() // Called when the user is successfully connected to the photon server
    {
        // base.OnConnectedToMaster();

        Debug.Log("Connected to Master Server with player name: "+PhotonNetwork.NickName);
        PhotonNetwork.LoadLevel("HomeScene");
    }

    #endregion
}
