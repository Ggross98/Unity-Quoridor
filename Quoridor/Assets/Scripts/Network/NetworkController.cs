using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkController : NetworkBehaviour
{

    NetworkManager nm;

    

    // Start is called before the first frame update
    void Awake()
    {
        nm = GetComponent<NetworkManager>();

        GameObject.Find("NetState").GetComponentInChildren<Text>().text = "未连接";

    }

    void OnStartLocalPlayer()
    {
        GameObject.Find("NetState").GetComponent<Text>().text = "已连接";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
