using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks//, IPunObservable
{
    public int user;
    public GameObject[] card;
    private GameObject[] base_c;
    private GameObject[] p1_c;
    private GameObject[] p2_c;

    public Camera camera1;
    public Camera camera2;

    private bool is_start_game = false;
    bool game_start_1 = false;
    bool game_start_2 = false;
    bool join_p1 = false;
    bool join_p2 = false;

    void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
        Screen.SetResolution(2000, 1000, false);
        PhotonNetwork.ConnectUsingSettings();
        //DontDestroyOnLoad(gameObject);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Entered Lobby");
        Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) //방 입장이 실패했을 때
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("No Room!!");
        RoomOptions roomOption = new RoomOptions();
        roomOption.MaxPlayers = 2;

        GameObject.Find("cards").GetComponent<open_card>().select_card();
        int[] base_c = GameObject.Find("cards").GetComponent<open_card>().card_numbers;
        int[,] p_c = GameObject.Find("cards").GetComponent<open_card>().p_card_numbers;
        roomOption.CustomRoomProperties = new Hashtable() { {"b0", base_c[0]}, { "b1", base_c[1] }, {"b2", base_c[2]}, {"b3", base_c[3]}, 
        {"b4", base_c[4]}, {"p10", p_c[0,0]}, {"p11", p_c[0,1]}, {"p20", p_c[1,0]}, {"p21", p_c[1,1]}};

        PhotonNetwork.CreateRoom("Room", roomOption, null); //방을 만들어줌 (최대 2명) 
    }

    public override void OnJoinedRoom(){
        base.OnJoinedRoom();
        Debug.Log("Enter Room");
        Hashtable CP = PhotonNetwork.CurrentRoom.CustomProperties;
        GameObject.Find("cards").GetComponent<open_card>().update_cli(CP);
        if(PhotonNetwork.IsMasterClient){
            join_p1 = true;
            Debug.Log(PhotonNetwork.IsMasterClient);
        }
        else if(!PhotonNetwork.IsMasterClient){
            join_p2 = true;
            Debug.Log(PhotonNetwork.IsMasterClient);
        }
        GameObject.Find("Scripts").GetComponent<Temp>().record_id(PhotonNetwork.LocalPlayer.ActorNumber);
        user = PhotonNetwork.LocalPlayer.ActorNumber;
    }

    void Start(){
    }
    
    void Update() {
        if(GameObject.Find("GameUI").GetComponent<GameUI>().connect && PhotonNetwork.IsMasterClient && join_p1){
            game_start_1 = true;
            photonView.RPC("p1_start_flag_change", RpcTarget.Others);
        }
        else if(GameObject.Find("GameUI").GetComponent<GameUI>().connect && !PhotonNetwork.IsMasterClient && join_p2){
            game_start_2 = true;
            photonView.RPC("p2_start_flag_change", RpcTarget.Others);
        }

        if(GameObject.Find("Scripts").GetComponent<game_rule>().is_end){
            string winner;
            GameObject.Find("Scripts").GetComponent<game_rule>().is_end = false;
            winner = GameObject.Find("Scripts").GetComponent<game_rule>().winner;
            if((PhotonNetwork.IsMasterClient && winner == "p1 win")||(!PhotonNetwork.IsMasterClient && winner == "p2 win")){
                GameObject.Find("Scripts").GetComponent<Money>().end_phase(PhotonNetwork.LocalPlayer.ActorNumber);
                GameObject.Find("Scripts").GetComponent<DataTrans>().DT();
            }
        }

        if(GameObject.Find("ButtonHandle").GetComponent<ButtonHandler>().is_restart1 && GameObject.Find("ButtonHandle").GetComponent<ButtonHandler>().is_restart2){
            bool reset = true;
            while(reset){
                reset = GameObject.Find("cards").GetComponent<open_card>().reset_pos();
            }
            GameObject.Find("ButtonHandle").GetComponent<ButtonHandler>().data.state = "";
            GameObject.Find("ButtonHandle").GetComponent<ButtonHandler>().data.now_player = 1;
            if(PhotonNetwork.IsMasterClient){
                ReStart();
            }
            GameObject.Find("ButtonHandle").GetComponent<ButtonHandler>().count = 0;
            GameObject.Find("Scripts").GetComponent<Money>().bat_money = 0;
            GameObject.Find("Scripts").GetComponent<Money>().total_money = 0;
            GameObject.Find("ButtonHandle").GetComponent<ButtonHandler>().is_restart1 = false;
            GameObject.Find("ButtonHandle").GetComponent<ButtonHandler>().is_restart2 = false;
            photonView.RPC("change_start_flag", RpcTarget.All);
        }

        if(!is_start_game && game_start_1 && game_start_2){
            is_start_game = true;
            GameObject.Find("cards").GetComponent<open_card>().Start_game();
            GameObject.Find("ButtonHandle").GetComponent<ButtonHandler>().set_player(PhotonNetwork.LocalPlayer.ActorNumber);
        }

    }

    private void ReStart(){
        Hashtable cp = PhotonNetwork.CurrentRoom.CustomProperties;
        GameObject.Find("cards").GetComponent<open_card>().select_card();
        int[] base_c = GameObject.Find("cards").GetComponent<open_card>().card_numbers;
        int[,] p_c = GameObject.Find("cards").GetComponent<open_card>().p_card_numbers;

        for(int i = 0; i < 5; i++){
            cp["b"+i.ToString()] = base_c[i];
            if(i < 2){
                cp["p1"+i.ToString()] = p_c[0,i];
                cp["p2"+i.ToString()] = p_c[1,i];
            }
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(cp);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log(PhotonNetwork.PlayerList[i]);
            PhotonNetwork.PlayerList[i].SetCustomProperties(cp);
        }

        photonView.RPC("update_cli", RpcTarget.All, cp);
    }

    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if (stream.IsWriting){
            stream.SendNext(game_start_1);
            stream.SendNext(game_start_2);
        }
        else{
            game_start_1 = (bool)stream.ReceiveNext();
            game_start_2 = (bool)stream.ReceiveNext();
        }
    }
    */

    [PunRPC]
    void p1_start_flag_change(){
        game_start_1 = true;
    }

    [PunRPC]
    void p2_start_flag_change(){
        game_start_2 = true;
    }

    [PunRPC]
    void change_start_flag(){
        is_start_game = false;
    }

    [PunRPC]
    void update_cli(Hashtable CP){
        GameObject.Find("cards").GetComponent<open_card>().update_cli(CP);
        GameObject.Find("cards").GetComponent<open_card>().copy_card();
    }

}