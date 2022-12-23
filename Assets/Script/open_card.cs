using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class open_card : MonoBehaviourPunCallbacks
{
    public GameObject[] card;
    public GameObject[] base_card;
    public GameObject[] p1_card;
    public GameObject[] p2_card;
    public GameObject[] p1_back;
    public GameObject[] p2_back;
    public GameObject[] base_card_copy;

    string[] number = {
        "a", "k", "q", "j", "10", "9", "8", 
        "7", "6", "5", "4", "3", "2"
    };
    string[] shape = {"c", "s", "h", "d"};
    public int[] card_numbers = new int[5]; //베이스 카드 넘버
    public int[,] p_card_numbers = new int[2, 2]; //행은 플레이어 수, 플레이어 배분 카드 넘버
    public bool is_start_game = false;
    int[] temp_cards = new int[9];
    int base_card_num = 0; //베이스 카드 순차적 스플릿을 위한 변수, 몇 번째 카드인지
    int[] p_card_num = {0, 0}; //플레이어 카드 순차적 스플릿을 위한 변수
    int player_numbers = 2; //플레이어 수
    bool[] start_flag = new bool[5]; //베이스카드 배분 애니메이션을 위한 초기 플래그
    bool[,] p_start_flag = new bool[2, 2]; //위와 동일하지만 플레이어 카드 배분
    float x_distance = 70.0f;
    float x_start = -140.0f;
    Vector3 start_position = new Vector3(-210.0f, -500.0f, 0);
    Vector3 start_position2 = new Vector3(-210.0f, -1500.0f, 0);
    Vector3 destination = new Vector3(-140.0f, -500.0f, 0);
    Vector3 destination_copy = new Vector3(-140.0f, -1500.0f, 0);
    Vector3[] p_destination = new Vector3[2];
    Vector3[] b_destination2 = new Vector3[2];
    Vector3[] b_destination1 = new Vector3[2];
    public PhotonView PV;

    void Awake(){
        base_card = new GameObject[5];
        base_card_copy = new GameObject[5];
        p1_card = new GameObject[2];
        p2_card = new GameObject[2];
        countCard();
    }

    void Start(){
        ResetDestination();
    }

    private void ResetDestination(){
        p_destination[0] = new Vector3(-50.0f, -500.0f + -135.0f, 0);
        p_destination[1] = new Vector3(-50.0f, -1500.0f - 135.0f, 0);
        b_destination2[0] = new Vector3(-50.0f, -500.0f + 135.0f, 0);
        b_destination2[1] = new Vector3(50.0f, -500.0f + 135.0f, 0);
        b_destination1[0] = new Vector3(-50.0f, -1500.0f + 135.0f, 0);
        b_destination1[1] = new Vector3(50.0f, -1500.0f + 135.0f, 0);
    }

    public void Start_game()
    {
        StartCoroutine(reset_pos_init());
        StartCoroutine(player_card_split());
        StartCoroutine(player_card_split2());
    }

    //카드 object 만드는 작업
    void countCard(){
        card = new GameObject[52];
        p1_back = new GameObject[2];
        p2_back = new GameObject[2];
        int k = 0;
        for(int i = 0; i < number.Length; i++){
            for(int j = 0; j < shape.Length; j++){
                card[k] = GameObject.Find(shape[j]+number[i]);
                k++;
            }
        }
        p1_back[0] = GameObject.Find("backcard3");
        p1_back[1] = GameObject.Find("backcard4");
        p2_back[0] = GameObject.Find("backcard1");
        p2_back[1] = GameObject.Find("backcard2");
    }
    
    public void copy_card(){
        for(int i = 0; i < 5; i++){
            base_card_copy[i] = Instantiate(base_card[i]);
        }
    }

    public void update_cli(Hashtable CP){
        for(int i = 0; i < 5; i++){
            card_numbers[i] = (int)CP["b"+i.ToString()];
            base_card[i] = card[(int)CP["b"+i.ToString()]];
            if(i < 2){
                p_card_numbers[0, i] = (int)CP["p1"+i.ToString()];
                p_card_numbers[1, i] = (int)CP["p2"+i.ToString()];
                p1_card[i] = card[(int)CP["p1"+i.ToString()]];
                p2_card[i] = card[(int)CP["p2"+i.ToString()]];
            }
        }
    }

    //베이스 카드 뿌려놓는 작업
    public IEnumerator base_card_split(){
        while(true){
            if(Mathf.Abs(card[card_numbers[base_card_num]].transform.position.x - destination.x) < 0.001f){
                Debug.Log(base_card_num + "  card : " +card[card_numbers[base_card_num]].ToString());
                base_card_num++;
                destination = new Vector3(x_start + x_distance*base_card_num, -500, 0);
                destination_copy = new Vector3(x_start + x_distance*base_card_num, -1500, 0);
                break;
            }
            if(start_flag[base_card_num] == false){
                card[card_numbers[base_card_num]].transform.position = start_position;
                base_card_copy[base_card_num].transform.position = start_position2;
                start_flag[base_card_num] = true;
            }
            card[card_numbers[base_card_num]].transform.position = Vector3.Lerp(card[card_numbers[base_card_num]].transform.position, destination, 0.01f);
            base_card_copy[base_card_num].transform.position = Vector3.Lerp(base_card_copy[base_card_num].transform.position, destination_copy, 0.01f);
            yield return null;
        }
    }

    public IEnumerator player_card_split(){
        bool p_split_end_flag = false;
        while(true){
            if(p_split_end_flag){
                break;
            }
            for(int i = 0; i < player_numbers-1; i++){
                if(Mathf.Abs(card[p_card_numbers[i, p_card_num[i]]].transform.position.x - p_destination[i].x) < 0.001f){
                    Debug.Log(p_card_numbers[0, p_card_num[i]]+ "  card : " + card[p_card_numbers[i, p_card_num[i]]].ToString());
                    p_card_num[i]++;
                    p_destination[i] = new Vector3(50.0f, -500.0f - 135.0f, 0);
                    if(p_card_num[i] == player_numbers){
                        p_split_end_flag = true;
                        break;
                    }
                }

                if(p_start_flag[i, p_card_num[i]] == false){
                    card[p_card_numbers[i, p_card_num[i]]].transform.position = start_position;
                    p2_back[p_card_num[i]].transform.position = start_position;
                    p_start_flag[i, p_card_num[i]] = true;
                }
                p2_back[p_card_num[i]].transform.position = Vector3.Lerp(p2_back[p_card_num[i]].transform.position, b_destination2[p_card_num[i]], 0.01f);
                card[p_card_numbers[i, p_card_num[i]]].transform.position = Vector3.Lerp(card[p_card_numbers[i, p_card_num[i]]].transform.position, p_destination[i], 0.01f);
                yield return null;
            }
        }
    }


    public IEnumerator player_card_split2(){
        bool p_split_end_flag_2 = false;
        while(true){
            if(p_split_end_flag_2){
                break;
            }
            for(int i = 1; i < player_numbers; i++){
                if(Mathf.Abs(card[p_card_numbers[i, p_card_num[i]]].transform.position.x - p_destination[i].x) < 0.001f){
                    Debug.Log(p_card_numbers[0, p_card_num[i]]+ "  card : " + card[p_card_numbers[i, p_card_num[i]]].ToString());
                    p_card_num[i]++;
                    p_destination[i] = new Vector3(50.0f, -1500.0f - 135.0f, 0);
                    if(p_card_num[i] == player_numbers){
                        p_split_end_flag_2 = true;
                        break;
                    }
                }

                if(p_start_flag[i, p_card_num[i]] == false){
                    card[p_card_numbers[i, p_card_num[i]]].transform.position = start_position2;
                    p1_back[p_card_num[i]].transform.position = start_position2;
                    p_start_flag[i, p_card_num[i]] = true;
                }
                p1_back[p_card_num[i]].transform.position = Vector3.Lerp(p1_back[p_card_num[i]].transform.position, b_destination1[p_card_num[i]], 0.01f);
                card[p_card_numbers[i, p_card_num[i]]].transform.position = Vector3.Lerp(card[p_card_numbers[i, p_card_num[i]]].transform.position, p_destination[i], 0.01f);
                yield return null;
            }
        }
    }

    public void select_card(){
        System.Random rand = new System.Random();
        for(int i = 0; i < 5; i++){
            card_numbers[i] = rand.Next(number.Length*shape.Length-1);
            for(int j = 0; j < i; j++){
                while(card_numbers[i] == card_numbers[j]){
                    card_numbers[i] = rand.Next(number.Length*shape.Length-1);
                }
            }
            base_card[i] = card[card_numbers[i]];
            start_flag[i] = false;
            temp_cards[i] = card_numbers[i];
        }

        for(int i = 0; i < player_numbers; i++){
            for(int j = 0; j < 2; j++){
                rand_again:
                p_card_numbers[i,j] = rand.Next(number.Length*shape.Length-1);
                for(int k = 0; k < 5+2*i+j; k++){
                    if(p_card_numbers[i,j] == temp_cards[k]){
                        goto rand_again;
                    }
                }
                p_start_flag[i,j] = false;
                temp_cards[5+2*i+j] = p_card_numbers[i,j];

                switch(i){
                    case 0:
                    p1_card[j] = card[p_card_numbers[i,j]];
                    continue;

                    case 1:
                    p2_card[j] = card[p_card_numbers[i,j]];
                    continue;

                    default:
                    continue;
                }
            }
        }
    }

    public bool reset_pos(){
        StartCoroutine(ReStart());
        return false;
    }

    private IEnumerator reset_pos_init(){
        Vector3 init_position = new Vector3(670.0f, 360.0f, 0);
        for(int i = 0; i < 52; i++){
            card[i].transform.position = init_position;
            if(i < 5){
                base_card_copy[i].transform.position = init_position;
            }
        }
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator ReStart(){
        Vector3 init_position = new Vector3(670.0f, 360.0f, 0);
        bool[] reset_flag_base = {false, false, false, false, false};
        bool[] reset_flag_p1 = {false, false};
        bool[] reset_flag_p2 = {false, false};

        while(true){
            if(Array.IndexOf(reset_flag_base, false) == -1 && Array.IndexOf(reset_flag_p1, false) == -1 && Array.IndexOf(reset_flag_p2, false) == -1){
                break;
            }

            destination = new Vector3(-140.0f, -500.0f, 0);
            destination_copy = new Vector3(-140.0f, -1500.0f, 0);
            base_card_num = 0;
            for(int i = 0; i < 5; i++){
                start_flag[i] = false;
                card[card_numbers[i]].transform.position = init_position;
                base_card_copy[i].transform.position = init_position;
                if(i < 2){
                    p_start_flag[0, i] = false;
                    p_start_flag[1, i] = false;
                    p_card_num[i] = 0;
                    p2_back[i].transform.position = init_position;
                    p1_back[i].transform.position = init_position;
                    card[p_card_numbers[0, i]].transform.position = init_position;
                    card[p_card_numbers[1, i]].transform.position = init_position;
                }
                if(Mathf.Abs(card[card_numbers[i]].transform.position.x - init_position.x) < 0.001f){
                    reset_flag_base[i] = true;
                }
                if(i < 2){
                    if(Mathf.Abs(card[p_card_numbers[0, i]].transform.position.x - init_position.x) < 0.001f){
                        reset_flag_p1[i] = true;
                    }
                    if(Mathf.Abs(card[p_card_numbers[1, i]].transform.position.x - init_position.x) < 0.001f){
                        reset_flag_p2[i] = true;
                    }
                }
                Destroy(base_card_copy[i]);
            }
            ResetDestination();
            for(int i = 0; i < 9; i++){
                temp_cards[i] = 0;
            }
            yield return null;
        }
    }

}
