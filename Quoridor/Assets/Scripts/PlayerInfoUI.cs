using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    public GameObject player1, player2;
    public Image arrow1, arrow2;

    public void ShowPlayerLeftPlanks(int player, int num)
    {
        string msg;
        if (player == 0) msg = "玩家一";
        else msg = "玩家二";
        msg += "\n剩余木板：" + num;

        if(player == 0)
        {
            player1.GetComponentInChildren<Text>().text = msg;
        }
        else
        {
            player2.GetComponentInChildren<Text>().text = msg;
        }
    }

    public void ShowCurrentPlayer(int player)
    {
        if(player == 0)
        {
            arrow1.color = new Color(0, 0, 0, 255);
            arrow2.color = new Color(0, 0, 0, 0);
        }
        else
        {
            arrow2.color = new Color(0, 0, 0, 255);
            arrow1.color = new Color(0, 0, 0, 0);
        }
    }

    public void ShowWinMessage(int player)
    {
        if(player == 0)
        {
            player1.GetComponentInChildren<Text>().text += "\n胜利！";
            player2.GetComponentInChildren<Text>().text += "\n失败！";
        }
        else
        {
            player2.GetComponentInChildren<Text>().text += "\n胜利！";
            player1.GetComponentInChildren<Text>().text += "\n失败！";
        }
    }

    public void HighlightOnPlayerInfo(int i)
    {

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
