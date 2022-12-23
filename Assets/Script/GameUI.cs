using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { set; get; }
    public Camera camera1;
    public Camera camera2;
    public bool connect;
    public InputField email;
    public InputField password;
    public string state;

    private void Awake(){
        Instance = this;
    }

    public void OnStartGameButton(){
        camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();
        camera2 = GameObject.Find("Sub Camera").GetComponent<Camera>();
        GameObject LoginMenu = GameObject.Find("LoginMenu");
        Vector3 LoginP = new Vector3(LoginMenu.transform.position.x, LoginMenu.transform.position.y, -1);
        camera1.transform.position = LoginP;
        camera2.transform.position = LoginP;
    }

    public void OnlineConnectButton(){
        camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();
        camera2 = GameObject.Find("Sub Camera").GetComponent<Camera>();

        connect = true;

        if(GameObject.Find("NetworkManager").GetComponent<NetworkManager>().user == 1){
            camera1.enabled = true;
            camera2.enabled = false;
            GameObject Game1 = GameObject.Find("background1");
            Vector3 GameP1 = new Vector3(Game1.transform.position.x, Game1.transform.position.y, -1);
            camera1.orthographicSize = 200;
            camera1.transform.position = GameP1;
        }
        else if(GameObject.Find("NetworkManager").GetComponent<NetworkManager>().user > 1){
            camera1.enabled = false;
            camera2.enabled = true;
            GameObject Game2 = GameObject.Find("background2");
            Vector3 GameP2 = new Vector3(Game2.transform.position.x, Game2.transform.position.y, -1);
            camera2.orthographicSize = 200;
            camera2.transform.position = GameP2;
        }

        GameObject.Find("Scripts").GetComponent<Temp>().get_tokens();
        GameObject.Find("cards").GetComponent<open_card>().copy_card();
    }

    public void OnlineBackButton(){
        camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();
        camera2 = GameObject.Find("Sub Camera").GetComponent<Camera>();
        GameObject.Find("Scripts").GetComponent<DataTrans>().logout();
        GameObject LoginMenu = GameObject.Find("LoginMenu");
        Vector3 LoginP = new Vector3(LoginMenu.transform.position.x, LoginMenu.transform.position.y, -1);
        camera1.transform.position = LoginP;
        camera2.transform.position = LoginP;
    }

    public void OnLoginButton(){
        GameObject.Find("Scripts").GetComponent<DataTrans>().login(email.text, password.text);
    }

    public void ChangeCameraViewConnect(){
        camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();
        camera2 = GameObject.Find("Sub Camera").GetComponent<Camera>();
        GameObject OnlineMenu = GameObject.Find("OnlineMenu");
        Vector3 OnlineP = new Vector3(OnlineMenu.transform.position.x, OnlineMenu.transform.position.y, -1);
        camera1.transform.position = OnlineP;
        camera2.transform.position = OnlineP;
        
        GameObject.Find("Scripts").GetComponent<DataTrans>().get_tokens();
    }

    public void OnSignUpButton(){
        GameObject.Find("Scripts").GetComponent<DataTrans>().sign_up(email.text, password.text);
    }
}
