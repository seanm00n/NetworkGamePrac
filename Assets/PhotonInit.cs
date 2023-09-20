using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

public class PhotonInit : Photon.PunBehaviour
{
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings("MyFps 1.0");
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined Lobby");
        //PhotonNetwork.CreateRoom("MyRoom");
        PhotonNetwork.JoinRandomRoom();
        //PhotonNetwork.JoinRoom("MyRoom");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        base.OnPhotonJoinRoomFailed(codeAndMsg);
        Debug.Log("No Room");
        PhotonNetwork.CreateRoom("MyRoom");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Finish make a room");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room");

        StartCoroutine(CreatePlayer());
    }

    IEnumerator CreatePlayer()
    {
        PhotonNetwork.Instantiate("Player",
                                    new Vector3(0, 0, 0),
                                    Quaternion.identity,
                                    0);
        yield return null;
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }


}
