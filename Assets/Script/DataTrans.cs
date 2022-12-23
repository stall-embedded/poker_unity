using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Data;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class Data
{
    public string recipient;
    public int n_tokens;
}

[System.Serializable]
public class LoginData{
    public string email;
    public string password;
}

public class DataTrans : MonoBehaviourPunCallbacks
{
    string player_email;
    public void DT(){
        photonView.RPC("TokenTransfer", RpcTarget.Others, player_email);
    }

    public void sign_up(string id, string pw){
        LoginData logindata = new LoginData();
        logindata.email = id;
        logindata.password = pw;
        string json = JsonUtility.ToJson(logindata);
        StartCoroutine(UnityWebRequestPost("http://192.168.56.1:8000/users/signup", json));
    }

    public void login(string id, string pw){
        LoginData logindata = new LoginData();
        logindata.email = id;
        logindata.password = pw;
        string json = JsonUtility.ToJson(logindata);
        StartCoroutine(UnityWebRequestPost("http://192.168.56.1:8000/users/login", json, true));

        player_email = id;
    }

    public void logout(){
        StartCoroutine(UnityWebRequestGET("http://192.168.56.1:8000/users/logout"));
    }

    public void get_tokens(){
        StartCoroutine(UnityWebRequestGET("http://192.168.56.1:8000/tokens/"));
    }

    IEnumerator UnityWebRequestPost(string URL, string json, bool isLogin = false){
        using (UnityWebRequest request = UnityWebRequest.Post(URL, json)){
            
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler.Dispose();
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if(request.error == null){
                Debug.Log(request.downloadHandler.text);
                if(request.downloadHandler.text.Substring(10) == "true}" && isLogin){
                    GameObject.Find("GameUI").GetComponent<GameUI>().ChangeCameraViewConnect();
                }
            }
            else{
                Debug.Log(request.error);
            }
            request.Dispose();
            
        }
    }

    IEnumerator UnityWebRequestGET(string URL)
    {
		// UnityWebRequest에 내장되있는 GET 메소드를 사용한다.
        UnityWebRequest request = UnityWebRequest.Get(URL);

        yield return request.SendWebRequest();  // 응답이 올때까지 대기한다.

        if (request.error == null)  // 에러가 나지 않으면 동작.
        {
            Debug.Log(request.downloadHandler.text);
            if(URL.Substring(URL.Length - 7) == "tokens/"){
                GameObject.Find("Scripts").GetComponent<Temp>().temp_wallet(request.downloadHandler.text);
            }
        }
        else
        {
            Debug.Log(request.error);
        }
    }

    /*
    public IEnumerator Upload()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", "testid");
        form.AddField("password", "1234");

        using (UnityWebRequest www = UnityWebRequest.Post("http://192.168.56.1:8000/users/signup", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
    */
    [PunRPC]
    void TokenTransfer(string other_email){
        Data data = new Data();
        data.recipient = other_email;
        data.n_tokens = GameObject.Find("Scripts").GetComponent<Money>().total_money;
        string json = JsonUtility.ToJson(data);
        Debug.Log(json);
        StartCoroutine(UnityWebRequestPost("http://192.168.56.1:8000/tokens/transfer", json));
    }

}
