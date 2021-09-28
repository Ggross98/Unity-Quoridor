using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject networkPanel;


    public const int PLAY_COMPUTER = 100, PLAY_PLAYER = 101, PLAY_NETWORK = 103;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void StartGame(int i)
    {
        switch (i)
        {
            case PLAY_PLAYER :
                SceneManager.LoadScene("Game_VS_PLAYER");
                break;
            case PLAY_NETWORK:
                SceneManager.LoadScene("Game_VS_NETWORK");
                break;
            case PLAY_COMPUTER:
                SceneManager.LoadScene("Game_VS_COMPUTER");
                break;

        }
    }

    public void ShowRules()
    {

    }

    public void ShowNetworkPanel(bool s)
    {
        networkPanel.gameObject.SetActive(s);
    }
}
