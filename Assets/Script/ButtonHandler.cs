using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

[System.Serializable]
public class StateData{
    public string state = "";
    public int now_player = 1;
}

public class ButtonHandler : MonoBehaviourPunCallbacks
{
    public int count = 0;
    private int player;
    int raise_money;
    public InputField raiseMoneyP1;
    public InputField raiseMoneyP2;
    public StateData data = new StateData();

    public bool p1_agree = false;
    public bool p2_agree = false;

    public bool is_restart1 = false;
    public bool is_restart2 = false;

    public void Update(){
        if(p1_agree && p2_agree){
            Next_turn();
        }
    }
    public void set_player(int player_num){
        player = player_num;
    }

    public void Next_turn(){
        if(count == 3){
            GameObject.Find("Scripts").GetComponent<game_rule>().ready();
        }
        else if(count == 0){
            StartCoroutine(GameObject.Find("cards").GetComponent<open_card>().base_card_split());
            StartCoroutine(GameObject.Find("cards").GetComponent<open_card>().base_card_split());
            StartCoroutine(GameObject.Find("cards").GetComponent<open_card>().base_card_split());
        }
        else if(count > 0 && count < 3) StartCoroutine(GameObject.Find("cards").GetComponent<open_card>().base_card_split());
        count++;
        p1_agree = false;
        p2_agree = false;
    }

    public void Call(){
        if(data.now_player == player){
            Debug.Log("Call");
            photonView.RPC("set_agree_true", RpcTarget.All, player, false);
            GameObject.Find("Scripts").GetComponent<Money>().call(player);
            if(data.state == "Raise"){
                photonView.RPC("Raise_call", RpcTarget.Others);
            }
            photonView.RPC("set_state", RpcTarget.Others, "Call");
            data.now_player = -1;
        }
    }

    public void Check(){
        if(data.now_player == player && count == 0){
            Debug.Log("Check");
            photonView.RPC("set_agree_true", RpcTarget.All, player, false);
            GameObject.Find("Scripts").GetComponent<Money>().check(player);
            photonView.RPC("set_state", RpcTarget.Others, "Check");
            data.now_player = -1;
        }
    }

    public void Raise(){
        if(data.now_player == player){
            Debug.Log("Raise");
            photonView.RPC("set_agree_true", RpcTarget.All, player, true);
            if(player == 1) raise_money = Convert.ToInt32(raiseMoneyP1.text);
            else if(player > 1) raise_money = Convert.ToInt32(raiseMoneyP2.text);
            photonView.RPC("set_state", RpcTarget.Others, "Raise");
            data.now_player = -1;
        }
    }

    public void Fold(){
        if(data.now_player == player){
            Debug.Log("Fold");
            GameObject.Find("Scripts").GetComponent<Money>().fold(player);
            photonView.RPC("set_state", RpcTarget.Others, "Fold");
            data.now_player = -1;
        }
    }

    public void ReStart(){
        photonView.RPC("set_restart_flag", RpcTarget.All, player);
    }

    [PunRPC]
    void Raise_call(){
        GameObject.Find("Scripts").GetComponent<Money>().raise(player, raise_money);
    }

    [PunRPC]
    void set_state(string state){
        data.state = state;
        data.now_player = player;
    }

    [PunRPC]
    void set_agree_true(int id, bool is_raise){
        if(is_raise){
            if(id == 1){
                p2_agree = true;
                p1_agree = false;
            }
            else if(id > 1){
                p2_agree = false;
                p1_agree = true;
            }
        }
        else{
            if(id == 1) p2_agree = true;
            else if(id > 1) p1_agree = true;
        }
    }

    [PunRPC]
    void set_restart_flag(int id){
        if(id == 1) is_restart1 = true;
        else if(id > 1) is_restart2 = true;
    }

}