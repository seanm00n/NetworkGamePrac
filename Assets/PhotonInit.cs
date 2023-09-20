using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

public class PhotonInit : Photon.PunBehaviour
{
    public InputField playerInput;
    bool isGameStart = false;
    string playerName = "";

    public void SetPlayerName()
    {
        Debug.Log(playerInput.text + "를 입력하셨습니다.");

        playerName  = playerInput.text;
        isGameStart = true;
    }
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings("MyFps 1.0");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        PhotonNetwork.JoinRandomRoom();
    }
    
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("No Room");
        PhotonNetwork.CreateRoom("My Room");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Finish make a room");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        StartCoroutine(this.CreatePlayer());
        
    }

    IEnumerator CreatePlayer()
    {
        while (!isGameStart)
        {
            yield return new WaitForSeconds(0.5f);
        }
            
        GameObject tempPlayer =  PhotonNetwork.Instantiate("Player", new Vector3(0,0,0), Quaternion.identity, 0);
        tempPlayer.GetComponent<PlayerCtrl>().SetPlayerName(playerName);
        yield return null;
    }
    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
