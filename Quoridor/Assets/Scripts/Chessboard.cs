using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class Chessboard : MonoBehaviour
{
    private GameObject tilesPanel;

    public GameObject tileButton;
    //private List<TileScript> tileList;
    private TileScript[,] tileArray;

    public GameObject plankButton;
    //private List<PlankScript> plankList;
    private PlankScript[,] plankArray_vertical, plankArray_horizontal;


    public GameObject chessPrefab;
    public int players = 2;
    public ChessScript[] playerArray;
    public int currentPlayer = 0;

    public bool isGaming = false;


    public PlayerInfoUI playerInfo;
    
    // Start is called before the first frame update
    void Start()
    {

        tileArray = new TileScript[9, 9];
        plankArray_horizontal = new PlankScript[9, 9];
        plankArray_vertical = new PlankScript[9, 9];


        tilesPanel = GameObject.Find("Tiles");

        CreateTiles();
        CreatePlanks();
        //CreateChesses();

        /*
        isGaming = true;
        CheckTileAvailability(playerArray[currentPlayer].pos);
        playerInfo.ShowCurrentPlayer(currentPlayer);*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        DestroyPlanks();
        CreatePlanks();
        CreateChesses();
        isGaming = true;
        currentPlayer = 0;
        CheckTileAvailability(playerArray[currentPlayer].pos);
        playerInfo.ShowCurrentPlayer(currentPlayer);
        playerInfo.ShowPlayerLeftPlanks(0, 10);
        playerInfo.ShowPlayerLeftPlanks(1, 10);


    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Menu");
    }

    public void DestroyPlanks()
    {
        foreach(PlankScript  p in plankArray_horizontal)
        {
            if(p!=null)
                Destroy(p.gameObject);
        }
        plankArray_horizontal = new PlankScript[9,9];
        foreach (PlankScript p in plankArray_vertical)
        {
            if (p != null)
                Destroy(p.gameObject);
        }
        plankArray_vertical = new PlankScript[9, 9];
    }

    public int CheckWin()
    {
        for (int i = 0; i < players; i++)
        {
            if (playerArray[i].ReachGoal())
            {
                isGaming = false;
                playerInfo.ShowWinMessage(currentPlayer);
                Debug.Log(currentPlayer + "赢了");
                return i;
            }
                
        }
        return -1;
    }


    public void NextTurn()
    {
        if (currentPlayer == players - 1)
            currentPlayer = 0;
        else currentPlayer++;
        playerInfo.ShowCurrentPlayer(currentPlayer);
        CheckTileAvailability(playerArray[currentPlayer].pos);
        Debug.Log("开始下一回合");
    }


    public void CreateChesses()
    {
        if (playerArray.Length!=0)
        {
            for (int i = 0; i < playerArray .Length ; i++)
            {
                if (playerArray[i] != null)
                {
                    Destroy(playerArray[i].gameObject);
                }
            }
        }
        playerArray = new ChessScript[players];


        GameObject newPlayer0 = Instantiate(chessPrefab , tilesPanel.transform);
        ChessScript pl0 = newPlayer0 .GetComponent<ChessScript>();
        pl0.GetComponent<Image>().color = Color.white;
        pl0.SetPosition(4, 0);
        pl0.fraction = 0;
        pl0.goal = 8;
        playerArray[0] = pl0;

        GameObject newPlayer1 = Instantiate(chessPrefab, tilesPanel.transform);
        ChessScript pl1 = newPlayer1.GetComponent<ChessScript>();
        pl1.SetPosition(4, 8);
        pl1.GetComponent<Image>().color = Color.black;
        pl1.fraction = 1;
        pl1.goal = 0;
        playerArray[1] = pl1;

        
    }

    public void CreateTiles()
    {
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                GameObject  newTile = Instantiate(tileButton, tilesPanel.transform );
                TileScript ts = newTile.GetComponent<TileScript>();
                ts.SetPosition(i, j);
                tileArray[i, j] = ts;
               
            }
        }
    }

    public void CreatePlanks()
    {

        for(int i = 1; i < 9; i++)
        {
            for (int j = 1; j < 9; j++)
            {
                GameObject newPlank_horizontal = Instantiate(plankButton, tilesPanel.transform);
                PlankScript ps1 = newPlank_horizontal.GetComponent<PlankScript>();
                ps1.SetPosition(i, j);
                ps1.SetVertical(false);
                plankArray_horizontal[i, j] = ps1;

                GameObject newPlank_vertical = Instantiate(plankButton, tilesPanel.transform);
                PlankScript ps2 = newPlank_vertical.GetComponent<PlankScript>();
                ps2.SetPosition(i, j);
                ps2.SetVertical(true);
                plankArray_vertical[i, j] = ps2;

            }
        }
    }

    public void PlacePlank(PlankScript ps)
    {
        if (!isGaming) return;

        if (playerArray[currentPlayer].leftPlanks < 1) return;

        
        //ps.SetFixed(true);
        ps.isFixed = true; //**假设的过程

        RefreshTileConnection();
        
        for(int i = 0; i < players; i++)
        {
            //如果任何一个玩家无法到达终点
            if(CanReach(playerArray [i].pos , playerArray [i].goal) == -1)
            {
                Debug.Log("不能这样放");
                ps.isFixed = false;         //不能这样放
                return;
            }
        }

        playerArray[currentPlayer].leftPlanks--;
        playerInfo.ShowPlayerLeftPlanks(currentPlayer, playerArray[currentPlayer].leftPlanks);
        ps.SetFixed(true);

        int x = ps.x;int y = ps.y;


        if(ps.isVertical)
        {
            //重叠的木板不能摆放
            plankArray_horizontal[x, y].SetAvailable(false);
            if (y > 1)
            {
                plankArray_vertical[x, y-1].SetAvailable(false);
            }
            if (y <8)
            {
                plankArray_vertical[x, y+1].SetAvailable(false);
            }

            //改变附近格子不能走这个方向
            /*
            tileArray[x, y].toLeft = false;
            if (y > 0)
            {
                tileArray[x, y-1].toLeft = false;
                if (x > 0)
                {
                    tileArray[x-1, y - 1].toRight = false;
                }
            }
            if (x > 0)
            {
                tileArray[x - 1, y].toRight = false;
            }
            */
        }
        else
        {
            plankArray_vertical[x, y].SetAvailable(false);
            if (x > 1)
            {
                plankArray_horizontal[x-1, y].SetAvailable(false);
            }
            if (x < 8)
            {
                plankArray_horizontal[x+1, y].SetAvailable(false);
            }

            //改变附近格子不能走这个方向
            /*
            tileArray[x, y].toDown = false;
            if (y > 0)
            {
                tileArray[x, y - 1].toUp = false;
                if (x > 0)
                {
                    tileArray[x - 1, y - 1].toUp = false;
                }
            }
            if (x > 0)
            {
                tileArray[x - 1, y].toDown = false;
            }*/
        }

        NextTurn();
    }

    public void MoveToChess(TileScript ts)
    {
        if (!isGaming) return;
        if (!ts.isAvailable) return;

        for(int i = 0; i < players; i++)
        {
            if (playerArray[i].pos == ts.pos)
                return;
        }


        playerArray[currentPlayer].SetPosition(ts.pos);
     
        CheckWin();
        if(isGaming)
        {
            NextTurn();
        }
            
       
        /*
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                tileArray[i, j].isAvailable = false;
            }
        }
        int x = ts.pos.x; int y = ts.pos.y;
        if (ts.toLeft && x > 0) tileArray[x - 1, y].isAvailable = true;
        if (ts.toRight && x <8) tileArray[x + 1, y].isAvailable = true;
        if (ts.toUp && y<8) tileArray[x, y+1].isAvailable = true;
        if (ts.toDown && y > 0) tileArray[x, y-1].isAvailable = true;*/
    }

    /*
     * 根据木板位置，判定所有格子是否与周围格连通
     */
    public void RefreshTileConnection()
    {
        //先开放所有通道
        foreach (TileScript t in tileArray)
        {
            t.toLeft = true;
            t.toRight = true;
            t.toUp = true;
            t.toDown = true;

        }
        //水平方向木板
        for(int i = 1; i < 9; i++)
        {
            for (int j = 1; j < 9; j++)
            {
                PlankScript p = plankArray_horizontal[i,j];

                if (!p.isFixed) continue;
                int x = p.pos.x;
                int y = p.pos.y;
                //改变附近格子不能走这个方向
                tileArray[x, y].toDown = false;
                if (y > 0)
                {
                    tileArray[x, y - 1].toUp = false;
                    if (x > 0)
                    {
                        tileArray[x - 1, y - 1].toUp = false;
                    }
                }
                if (x > 0)
                {
                    tileArray[x - 1, y].toDown = false;
                }
            }
        }

        //垂直方向木板
        for (int i = 1; i < 9; i++)
        {
            for (int j = 1; j < 9; j++)
            {
                PlankScript p = plankArray_vertical[i, j];

                if (!p.isFixed) continue;
                int x = p.pos.x;
                int y = p.pos.y;
                tileArray[x, y].toLeft = false;
                if (y > 0)
                {
                    tileArray[x, y - 1].toLeft = false;
                    if (x > 0)
                    {
                        tileArray[x - 1, y - 1].toRight = false;
                    }
                }
                if (x > 0)
                {
                    tileArray[x - 1, y].toRight = false;
                }
            }
        }
    }

    /**
     * 检测棋子周围四个方向是否能移动
     */
    public void CheckTileAvailability(Vector2Int pos)
    {
        RefreshTileConnection();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                tileArray[i, j].isAvailable = false;
            }
        }
        TileScript ts = tileArray[pos.x, pos.y];
        int x = pos.x; int y = pos.y;
        if (ts.toLeft && x > 0) tileArray[x - 1, y].isAvailable = true;
        if (ts.toRight && x < 8) tileArray[x + 1, y].isAvailable = true;
        if (ts.toUp && y < 8) tileArray[x, y + 1].isAvailable = true;
        if (ts.toDown && y > 0) tileArray[x, y - 1].isAvailable = true;

        /*
        if (x > 0) tileArray[x - 1, y].isAvailable = true;
        if (x < 8) tileArray[x + 1, y].isAvailable = true;
        if (y < 8) tileArray[x, y + 1].isAvailable = true;
        if (y > 0) tileArray[x, y - 1].isAvailable = true;*/
    }

    /**
     * 利用BFS算法，求出某个位置是否能通向终点
     * TODO:记录最短路径
     */
    public int CanReach(Vector2Int pos, int y)
    {

        List<TileScript> open, close;
        open = new List<TileScript>();
        close = new List<TileScript>();

        TileScript start = tileArray[pos.x, pos.y];
        open.Add(start);
        //TileScript t1, t2;

        while (open.Count >0)
        {
            for(int i = 0;i<open.Count;i++)
            {
                TileScript t1 = open[i];
                TileScript t2;

                if (t1.pos.y == y)
                {
                    return 1;
                }

                if(t1.toDown&& t1.pos.y> 0)
                {
                    t2 = tileArray[t1.pos.x, t1.pos.y-1];
                    if(!close.Contains(t2))
                    {
                        open.Add(t2);
                    }
                }
                if (t1.toLeft&& t1.pos.x >0)
                {
                    t2 = tileArray[t1.pos.x-1, t1.pos.y];
                    if (!close.Contains(t2))
                    {
                        open.Add(t2);
                    }
                }
                if (t1.toRight&& t1.pos.x<8)
                {
                    t2 = tileArray[t1.pos.x+1, t1.pos.y];
                    if (!close.Contains(t2))
                    {
                        open.Add(t2);
                    }
                }
                if (t1.toUp&& t1.pos.y<8)
                {
                    t2 = tileArray[t1.pos.x, t1.pos.y+1];
                    if (!close.Contains(t2))
                    {
                        open.Add(t2);
                    }
                }

                open.Remove(t1);
                close.Add(t1);
            }
        }



        return -1;
    }
}
