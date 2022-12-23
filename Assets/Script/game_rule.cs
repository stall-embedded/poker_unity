using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class game_rule : MonoBehaviour
{
    GameObject[] card;
    int[] card_numbers;
    int[,] p_card_numbers;

    void Scripting(){
        card = GameObject.Find("cards").GetComponent<open_card>().card;
        card_numbers = GameObject.Find("cards").GetComponent<open_card>().card_numbers;
        p_card_numbers = GameObject.Find("cards").GetComponent<open_card>().p_card_numbers;
    }
    
    static string[] Priority_shape = {"s", "d", "h", "c"}; //s = 300, d = 200, h = 100, c = 000 
    static string[] Priority_number = {
        "a", "k", "q", "j", "10", "9", "8", 
        "7", "6", "5", "4", "3", "2"
    }; //a = 14, k = 13, q = 12, j = 11

    //구조체화 시키는 작업 필요
    string[] p1_hand = new string[7];
    string[] p2_hand = new string[7];
    int[] p1_hand_int = new int[7];
    int[] p2_hand_int = new int[7];

    public string p1_hand_state;
    public string p2_hand_state;
    public string winner;
    public bool is_end = false;

    public void ready(){
        Scripting();
        make_hand();
        split_num_shape();
        p1_hand_state = judge_hand(p1_hand_int);
        p2_hand_state = judge_hand(p2_hand_int);
        winner = judge_winner(p1_hand_state, p2_hand_state);

        is_end = true;

        Debug.Log("player1 : " + p1_hand_state);
        Debug.Log("player2 : " + p2_hand_state);
        Debug.Log("Winner : " + winner);
    }

    string judge_winner(string p1, string p2){
        string[] Priority_hand = {"RSF", "BSF", "STF", "POK", "FHO", "FLU", "MOT", "BST", "STR", "TRI", "TWO", "ONE", "TOP"};
        int judge_num_p1;
        int judge_num_p2;

        if(p1.Substring(0,3) == p2.Substring(0,3)){
            //Array.IndexOf로 /위치 찾고 +1 부터 다시 문자열 생성 후 0다음 /까지 substring하면 될듯
            if(p1.Substring(5,1) == "/") judge_num_p1 = Convert.ToInt32(p1.Substring(4,1));
            else judge_num_p1 = Convert.ToInt32(p1.Substring(4,2));
            if(p2.Substring(5,1) == "/") judge_num_p2 = Convert.ToInt32(p2.Substring(4,1));
            else judge_num_p2 = Convert.ToInt32(p2.Substring(4,2));

            if(judge_num_p1 > judge_num_p2) return "p1 win";
            else if(judge_num_p1 < judge_num_p2) return "p2 win";

            return "draw";
        }
        else{
            return Array.IndexOf(Priority_hand, p1.Substring(0,3)) < Array.IndexOf(Priority_hand, p2.Substring(0,3)) ? "p1 win" : "p2 win";
        }
    }

    string judge_hand(int[] num){
        string hand_state = "\0";
        hand_state = is_RSF(num);

        if(hand_state == "N") hand_state = is_BSF(num);
        else return hand_state;

        if(hand_state == "N") hand_state = is_STF(num);
        else return hand_state;

        if(hand_state == "N") hand_state = is_POK(num);
        else return hand_state;

        if(hand_state == "N") hand_state = is_FHO(num);
        else return hand_state;

        if(hand_state == "N") hand_state = is_FLU(num);
        else return hand_state;

        if(hand_state == "N") hand_state = is_MOT(num);
        else return hand_state;

        if(hand_state == "N") hand_state = is_BST(num);
        else return hand_state;

        if(hand_state == "N") hand_state = is_STR(num);
        else return hand_state;

        if(hand_state == "N") hand_state = is_TRI(num);
        else return hand_state;

        if(hand_state == "N") hand_state = is_TWO(num);
        else return hand_state;

        if(hand_state == "N") hand_state = is_ONE(num);
        else return hand_state;
        
        if(hand_state == "N") hand_state = is_TOP(num);
        return hand_state;
    }

    string is_RSF(int[] num){
        int[] RSF = {15, 10, 5, 0};
        int shape = 0;
        int i = 0;
        int[] value = new int[5];
        foreach (var item in num)
        {
            if(item % 100 > 9 && Array.IndexOf(value, item) == -1){
                value[i] = item;
                shape += item;
                i++;
            }
        }
        if(i == 5){
            shape = (int)(shape/100);
            if(Array.IndexOf(RSF, shape) > -1) return "RSF/" + Array.IndexOf(RSF, shape).ToString();
        }
        return "N";
    }

    string is_BSF(int[] num){
        int[] BSF = {15, 10, 5, 0};
        int shape = 0;
        int i = 0;
        int[] value = new int[5];
        foreach (var item in num)
        {
            if((item % 100 < 6 || item % 100 == 14) && Array.IndexOf(value, item) == -1){
                value[i] = item;
                shape += item;
                i++;
            }
        }
        if(i == 5){
            shape = (int)(shape/100);
            if(Array.IndexOf(BSF, shape) > -1) return "BSF/" + Array.IndexOf(BSF, shape).ToString();
        }
        return "N";
    }

    string is_STF(int[] num){
        int count = 0;
        int[] STF = {3, 2, 1, 0};
        int shape = 0;
        int first = 0;
        bool flag = true;

        for(int i = 0; i < 6; i++){
            if(num[i]%100 - num[i+1]%100 == 1 && (int)((num[i] - num[i+1])/100) == 0){
                count += 1;
                shape = (int)(num[i]/100);
                if(flag){
                    first = num[i];
                    flag = false;
                }
            }
            else{
                flag = true;
                count = 0;
            }

            if(count == 4) return "STF/" + Array.IndexOf(STF, shape).ToString() + "/" + first.ToString();
        }
        return "N";
    }
    
    string is_POK(int[] num){
        int count = 0;
        int top = 0;
        bool flag = true;

        for(int i = 0; i < 6; i++){
            if(num[i]%100 == num[i+1]%100){
                count += 1;
                if(flag){
                    flag = false;
                    top = num[i];
                }
            }
            else{
                flag = true;
                count = 0;
            }

            if(count == 3) return "POK/" + top.ToString();
        }

        return "N";
    }

    string is_FHO(int[] num){
        int[] shape_p = {3, 2, 1, 0};
        int[] shape = new int[2];
        int[] count = new int[3];
        bool[] check = {false, false};
        int j = 0;

        for(int i = 0; i < 6; i++){
            if(num[i]%100 == num[i+1]%100){
                count[j] += 1;
            }
            else if(count[j] > 0){
                j++;
            }

            if(j > 2){
                break;
            }
        }

        for(int i = 0; i < 3; i++){
            if(count[i] == 2){
                check[0] = true;
                shape[0] = Array.IndexOf(shape_p, (int)(num[i]/100));
            }
            else if(count[i] == 1){
                check[1] = true;
                shape[1] = Array.IndexOf(shape_p, (int)(num[i]/100));
            }
        }

        if(check[0]&check[1]){
            return "FHO/" + shape[0].ToString() + "/" + shape[1].ToString();
        }
        return "N";
    }

    string is_FLU(int[] num){
        int[] shape_p = {3, 2, 1, 0};
        int[] temp = (int[])num.Clone();
        int count = 0;
        string str = "\0";
        Array.Sort(temp, (a, b) => (a > b) ? -1 : 1);
        for(int i = 0; i < 6; i++){
            if((int)(temp[i]/100) == (int)(temp[i+1]/100)){
                count += 1;
                str += (temp[i]%100).ToString();
            }
            else{
                str = "\0";
                count = 0;
            }

            if(count == 4){
                str += (temp[i+1]%100).ToString();
                return "FLU/" + str + "/" + Array.IndexOf(shape_p, (int)(temp[i]/100)).ToString();
            }
        }
        return "N";
    }

    string is_MOT(int[] num){
        int count = 0;
        int[] royal = {14, 13, 12, 11, 10};
        string str = "\0";
        for(int i = 0; i < 7; i++){
            if(num[i]%100 == royal[count]){
                count++;
                str += ((int)(num[i]/100)).ToString();
            }
            if(count == 5) return "MOT/" + str;
        }
        return "N";
    }

    string is_BST(int[] num){
        int[] shape_p = {3, 2, 1, 0};
        int count = 0;
        int[] back = {14, 5, 4, 3, 2};
        string str = "\0";
        bool flag = true;

        for(int i = 0; i < 7; i++){
            if(num[i]%100 == back[count]){
                count++;
                if(flag){
                    str = Array.IndexOf(shape_p, (int)(num[i]/100)).ToString();
                    flag = false;
                }
            }
            if(count == 5) return "BST/" + str;
        }
        return "N";
    }

    string is_STR(int[] num){
        int[] shape_p = {3, 2, 1, 0};
        int count = 0;
        string str = "\0";
        bool flag = true;

        for(int i = 0; i < 6; i++){
            if(num[i]%100 - num[i+1]%100 == 1){
                if(flag){
                    str = (num[i]%100).ToString() + "/" + Array.IndexOf(shape_p, (int)(num[i]/100)).ToString();
                    flag = false;
                }
                count++;
            }
            else{
                count = 0;
            }
            if(count == 4) return "STR/" + str;
        }
        return "N";
    }

    string is_TRI(int[] num){
        int[] shape_p = {3, 2, 1, 0};
        int count = 0;

        for(int i = 0; i < 6; i++){
            if(num[i]%100 == num[i+1]%100){
                count++;
            }
            else{
                count = 0;
            }
            if(count == 2) return "TRI/" + (num[i]%100).ToString();
        }
        return "N";
    }

    string is_TWO(int[] num){
        int[] shape_p = {3, 2, 1, 0};
        int[] shape = new int[2];
        int[] num_pair = new int[2];
        int count = 0;
        for(int i = 0; i < 6; i++){
            if(num[i]%100 == num[i+1]%100){
                num_pair[count] = num[i]%100;
                shape[count] = (int)(num[i]/100);
                count++;
            }
            if(count == 2) return "TWO/" + num_pair[0].ToString() + "/" + num_pair[1].ToString() + "/" + Array.IndexOf(shape_p, shape[0]).ToString();
        }
        return "N";
    }

    string is_ONE(int[] num){
        int[] shape_p = {3, 2, 1, 0};
        for(int i = 0; i < 6; i++){
            if(num[i]%100 == num[i+1]%100) return "ONE/" + (num[i]%100).ToString() + "/" + Array.IndexOf(shape_p, (int)(num[i]/100 > num[i+1]/100 ? num[i]/100 : num[i+1]/100)).ToString();
        }
        return "N";
    }

    string is_TOP(int[] num){
        int[] shape = {3, 2, 1, 0};
        return "TOP/" + (num[0]%100).ToString() + "/" + Array.IndexOf(shape, (int)(num[0]/100)).ToString();
    }


    void split_num_shape(){
        int[] temp1 = new int[7];
        int[] temp2 = new int[7];

        for(int i = 0; i < 7; i++){
            p1_hand_int[i] = 0;
            p2_hand_int[i] = 0;
            p1_hand_int[i] += (swap_str_to_num(p1_hand[i].Substring(1)) + swap_str_to_num(p1_hand[i].Substring(0, 1)));
            p2_hand_int[i] += (swap_str_to_num(p2_hand[i].Substring(1)) + swap_str_to_num(p2_hand[i].Substring(0, 1)));
        }
        Array.Sort(p1_hand_int, (a, b) => (a%100 > b%100) ? -1 : 1);
        Array.Sort(p2_hand_int, (a, b) => (a%100 > b%100) ? -1 : 1);
    }

    int swap_str_to_num(string str){
        switch (str)
        {
            case "s":
                return 300;
            case "d":
                return 200;
            case "h":
                return 100;
            case "c":
                return 000;
            case "a":
                return 14;
            case "k":
                return 13;
            case "q":
                return 12;
            case "j":
                return 11;
            default:
                return Convert.ToInt32(str);
        }
    }

    void make_hand(){
        var list1 = new List<int>();
        list1.AddRange(card_numbers);
        list1.Add(p_card_numbers[0,0]);
        list1.Add(p_card_numbers[0,1]);
        int[] arr1 = list1.ToArray();

        var list2 = new List<int>();
        list2.AddRange(card_numbers);
        list2.Add(p_card_numbers[1,0]);
        list2.Add(p_card_numbers[1,1]);
        int[] arr2 = list2.ToArray();

        for(int i = 0; i < 7; i++){
            p1_hand[i] = card[arr1[i]].name;
            p2_hand[i] = card[arr2[i]].name;
        }
    }
}
