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

    public string chatMessage;
    Text chatText;
    ScrollRect scroll_rect = null;
    PhotonView pv;
    public void SetPlayerName()
    {
        Debug.Log(playerInput.text + "�� �Է��ϼ̽��ϴ�.");
        if(isGameStart == false )
        {
            playerName = playerInput.text;
            playerInput.text = string.Empty;
            isGameStart = true;
        }
        else
        {
            chatMessage = playerInput.text;
            pv.RPC("ChatInfo", PhotonTargets.All, chatMessage);
            playerInput.text= string.Empty;
            //ShowChat(chatMessage);
        }
        
    }
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings("MyFps 1.0");

        chatText = GameObject.Find("ChatText").GetComponent<Text>();
        scroll_rect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
    }
    public void ShowChat(string chat)
    {
        //���� �����߿��� ä�� �޽����� ó��
        chatText.text += chat + "\n";
        //��ũ�ѹ��� ��ġ�� ���� �Ʒ��� ������ 1.0�� �� ��, 0.0�� �� �Ʒ�
        scroll_rect.verticalNormalizedPosition = 0.0f;
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
        pv = GetComponent<PhotonView>();
        yield return null;
    }
    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    [PunRPC]
    public void ChatInfo(string sChat, PhotonMessageInfo info)
    {
        ShowChat(sChat);
    }

}
