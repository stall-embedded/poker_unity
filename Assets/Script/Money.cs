using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviourPunCallbacks
{
    public int p1_wallet = 0;
    public int p2_wallet = 0;
    public int blind = 100;
    public int bat_money = 0;
    public int total_money = 0;

    private void Start(){
        p1_wallet = 0;
        p2_wallet = 0;
    }

    public void init_wallet(int id, int tokens){
        if(id == 1){
            p1_wallet = tokens;
            photonView.RPC("sync_wallet", RpcTarget.Others, id, tokens);
        }
        else if(id > 1){
            p2_wallet = tokens;
            photonView.RPC("sync_wallet", RpcTarget.Others, id, tokens);
        }
    }

    public void call(int id){
        if(id == 1) p1_wallet -= bat_money;
        else if(id > 1) p2_wallet -= bat_money;
        total_money += bat_money;
        photonView.RPC("sync_money", RpcTarget.All, total_money, bat_money);
    }

    public void check(int id){
        if(id == 1){
            p1_wallet -= blind;
            bat_money += blind;
        }
        else if(id > 1){
            p2_wallet -= blind;
            bat_money += blind;
        }
        total_money += bat_money;
        photonView.RPC("sync_money", RpcTarget.All, total_money, bat_money);
    }

    public void raise(int id, int Money){
        if(id == 1){
            p1_wallet -= Money;
            bat_money += Money;
        }
        else if(id > 1){
            p2_wallet -= Money;
            bat_money += Money;
        }
        total_money += bat_money;
        photonView.RPC("sync_money", RpcTarget.All, total_money, bat_money);
    }

    public void fold(int id){
        if(id == 1){
            GameObject.Find("Scripts").GetComponent<game_rule>().winner = "p1 win";
        }
        else if(id > 1){
            GameObject.Find("Scripts").GetComponent<game_rule>().winner = "p2 win";
        }
        GameObject.Find("Scripts").GetComponent<game_rule>().is_end = true;
    }

    public void end_phase(int winner_id){
        if(winner_id == 1) p1_wallet += total_money;
        else if(winner_id > 1) p2_wallet += total_money;

        photonView.RPC("local_sync_wallet", RpcTarget.All, winner_id, total_money);
    }

    [PunRPC]
    void sync_wallet(int id, int tokens){
        if(id == 1) p1_wallet = tokens;
        else if(id > 1) p2_wallet = tokens;
    }

    [PunRPC]
    void local_sync_wallet(int id, int money){
        if(id == 1){
            p1_wallet += money;
            p2_wallet -= money;
        }
        else if(id > 1){
            p1_wallet -= money;
            p2_wallet += money;
        }
    }

    [PunRPC]
    void sync_money(int TM, int BM){
        total_money = TM;
        bat_money = BM;
    }

}
