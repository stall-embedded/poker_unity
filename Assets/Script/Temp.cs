using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Temp : MonoBehaviour
{
    private int token_num;
    private int id;
    public void temp_wallet(string tokens){
        token_num = Convert.ToInt32(tokens.Substring(13, tokens.Length-15));
        Debug.Log(token_num);
    }
    public void record_id(int client_id){
        id = client_id;
    }
    public void get_tokens(){
        GameObject.Find("Scripts").GetComponent<Money>().init_wallet(id, token_num);
    }
}
