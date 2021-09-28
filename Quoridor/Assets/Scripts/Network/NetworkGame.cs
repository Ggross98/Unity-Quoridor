using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkGame : NetworkBehaviour
{
    private GameObject tilesPanel;

    private GameObject tileButton;

    //棋盘
    private TileScript[,] tileArray;

    private GameObject plankButton;
    //同步：木板
    //用数组存储木板的情况：固定，可安放，不可安放
    private SyncListInt plankList = new SyncListInt();
    public const int FIXED = 1, AVAILABLE = 0, UNAVAILABLE = -1;
    private PlankScript[,] plankArray_vertical, plankArray_horizontal;


    private GameObject chessPrefab;

    public const int players = 2;
    //同步：棋子
    //public SyncList<ChessScript> playerList;
    //public ChessScript[] playerArray;
    [SyncVar]
    public int x1, y1, x2, y2;
    //public ChessScript player;
    private ChessScript player1, player2;

    [SyncVar]
    public int currentPlayer = 0;

    [SyncVar]
    public bool isGaming = false;

    private PlayerInfoUI playerInfo;


    private bool isMyTurn = false;



    // Start is called before the first frame update
    void Awake()
    {
        //本地：创建数组
        tileArray = new TileScript[9, 9];
        plankArray_horizontal = new PlankScript[9, 9];
        plankArray_vertical = new PlankScript[9, 9];

        //寻找预制体
        tilesPanel = GameObject.Find("Tiles");
        tileButton = GameObject.Find("TileButton");
        plankButton = GameObject.Find("PlankButton");
        chessPrefab = GameObject.Find("ChessPrefab");

        playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfoUI>();
        //创建棋盘和木板（服务器端）并同步到本地数组
        
        /*
        isGaming = true;
        CheckTileAvailability(playerArray[currentPlayer].pos);
        playerInfo.ShowCurrentPlayer(currentPlayer);*/
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        if(isGaming)
        {
            if(!isMyTurn)
            {
                if(MyTurn())
                {
                    DownloadChessInfo();
                    DownloadPlankInfo();
                    CheckTileAvailability(GetPlayerPosition ());
                    //Debug.Log(currentPlayer + "的回合");
                }
            }
            isMyTurn = MyTurn();
        }
    }

    public void StartGame()
    {
        CreateTiles();
        CreatePlanks();
        CreateChesses();

        isGaming = true;


        currentPlayer = 0;
    }

    public void Restart()
    {
        //DestroyPlanks();

        CreatePlanks();
        CreateChesses();
        isGaming = true;
        currentPlayer = 0;
        //CheckTileAvailability(playerArray[currentPlayer].pos);
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
        foreach (PlankScript p in plankArray_horizontal)
        {
            if (p != null)
                Destroy(p.gameObject);
        }
        plankArray_horizontal = new PlankScript[9, 9];
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
            if ((isServer ? player1 :player2).ReachGoal())
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
        /*
        if(MyTurn())
        {
            UploadChessInfo();
            UploadPlankInfo();
        }*/
        
        if (currentPlayer == 1)
            currentPlayer = 0;
        else currentPlayer=1;

        /*
        if(MyTurn())
        {
            DownloadPlankInfo();
            DownloadChessInfo();
            playerInfo.ShowCurrentPlayer(currentPlayer);
            CheckTileAvailability(player.pos);
        }
        */

        //Debug.Log("开始下一回合");
    }


    public void CreateChesses()
    {
        //如果已经生成过棋子
        if (false)
        {
            //player.Reset();
        }
        //否则生成棋子，设定阵营等
        else
        {
            //playerArray = new ChessScript[players];

            //在本地创建两个棋子
            GameObject newPlayer0 = Instantiate(chessPrefab, tilesPanel.transform);
            player1 = newPlayer0.GetComponent<ChessScript>();
            player1.GetComponent<Image>().color = Color.white;
            player1.SetFraction(0);
            player1.SetPosition(4, 0);

            GameObject newPlayer1 = Instantiate(chessPrefab, tilesPanel.transform);
            player2 = newPlayer1.GetComponent<ChessScript>();
            player2.GetComponent<Image>().color = Color.black;
            player2.SetFraction(1);
            player2.SetPosition(4, 8);

            //player = (isServer) ? player1 : player2;

            //上传位置信息
            /*
            if (isServer) {
                player = player1;
                //player.fraction = 0;
                x1 = player.x;
                y1 = player.y;
            }
            else {
                
                player = player2;
                //player.fraction = 1;
                x2 = player.x;
                y2 = player.y;
            }*/

            //下载棋子位置信息
            //UploadChessInfo();
            x1 = 4; y1 = 0;
            x2 = 4; y2 = 8;
        }
        /*
        GameObject newPlayer1 = Instantiate(chessPrefab, tilesPanel.transform);
        ChessScript pl1 = newPlayer1.GetComponent<ChessScript>();
        pl1.SetPosition(4, 8);
        pl1.GetComponent<Image>().color = Color.black;
        pl1.fraction = 1;
        pl1.goal = 0;
        playerArray[1] = pl1;*/


    }


    public void DownloadChessInfo()
    {
        player1.SetPosition(x1, y1);
        player2.SetPosition(x2, y2);
    }

    public void UploadChessInfo()
    {
        int _x = GetPlayerPosition().x;
        int _y = GetPlayerPosition().y;

        if(isServer)
        {
            x1 = _x;y1 = _y;
        }
        else
        {
            x2 = _x;y2 = _y;
        }
    }



    public void CreateTiles()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                GameObject newTile = Instantiate(tileButton, tilesPanel.transform);
                TileScript ts = newTile.GetComponent<TileScript>();
                ts.SetPosition(i, j);
                ts.GetComponent<Button>().onClick.AddListener(

                    delegate () { MoveToChess(ts); }
                    );
                tileArray[i, j] = ts;

            }
        }

    }

    public void CreatePlanks()
    {
        //如果已经生成，则刷新
        /*
        if(plankArray_horizontal .Length != 0)
        {
            foreach(PlankScript ps in plankArray_horizontal)
            {
                ps.ChangeState(AVAILABLE);
            }
            foreach (PlankScript ps in plankArray_vertical)
            {
                ps.ChangeState(AVAILABLE);
            }
            return;
        }*/
        //否则重新生成
        
        for(int i = 0; i < 200; i++)
        {
            plankList.Add(0);
        }

        for (int i = 1; i < 9; i++)
        {
            for (int j = 1; j < 9; j++)
            {
                GameObject newPlank_horizontal = Instantiate(plankButton, tilesPanel.transform);
                PlankScript ps1 = newPlank_horizontal.GetComponent<PlankScript>();
                ps1.SetPosition(i, j);
                ps1.SetVertical(false);
                ps1.GetComponent<Button>().onClick.AddListener(

                    delegate () { PlacePlank (ps1); }
                    );
                plankArray_horizontal[i, j] = ps1;
                //plankList.Add(ps1);

                GameObject newPlank_vertical = Instantiate(plankButton, tilesPanel.transform);
                PlankScript ps2 = newPlank_vertical.GetComponent<PlankScript>();
                ps2.SetPosition(i, j);
                ps2.SetVertical(true);
                ps2.GetComponent<Button>().onClick.AddListener(

                    delegate () { PlacePlank(ps2); }
                    );
                //plankList.Add(ps2);
                plankArray_vertical[i, j] = ps2;

            }
        }


        //同步信息
        for (int i = 0; i < 128; i++)
        {
            plankList[i] = AVAILABLE;
        }


    }


    public bool MyTurn()
    {
        int t = (isServer) ? 0 : 1;
        return currentPlayer == t;
    }
    public ChessScript GetPlayer()
    {
        return (isServer) ? player1 : player2;
    }

    public Vector2Int GetPlayerPosition()
    {
        return (isServer) ? player1.pos : player2.pos;
    }

    public void PlacePlank(PlankScript ps)
    {
        if (!isGaming) return;
        if (!MyTurn()) return;
        if (GetPlayer().leftPlanks < 1) return;


        //ps.SetFixed(true);
        ps.isFixed = true; //**假设的过程

        RefreshTileConnection();

        for (int i = 0; i < players; i++)
        {
            //如果任何一个玩家无法到达终点
            if (CanReach(GetPlayerPosition(), GetPlayer ().goal) == -1)
            {
                Debug.Log("不能这样放");
                ps.isFixed = false;         //不能这样放
                return;
            }
        }

        GetPlayer ().leftPlanks--;
        playerInfo.ShowPlayerLeftPlanks(currentPlayer, GetPlayer ().leftPlanks);
        ps.SetFixed(true);

        int x = ps.x; int y = ps.y;


        if (ps.isVertical)
        {
            //重叠的木板不能摆放
            plankArray_horizontal[x, y].SetAvailable(false);
            if (y > 1)
            {
                plankArray_vertical[x, y - 1].SetAvailable(false);
            }
            if (y < 8)
            {
                plankArray_vertical[x, y + 1].SetAvailable(false);
            }
        }
        else
        {
            plankArray_vertical[x, y].SetAvailable(false);
            if (x > 1)
            {
                plankArray_horizontal[x - 1, y].SetAvailable(false);
            }
            if (x < 8)
            {
                plankArray_horizontal[x + 1, y].SetAvailable(false);
            }

        }

        NextTurn();
    }

    public int GetPlankID(int x, int y, bool vertical)
    {


        return 0;
    }

    public void UploadPlankInfo()
    {
        int id = 0;
        for (int i = 1; i < 9; i++)
        {
            for (int j = 1; j < 9; j++)
            {
                id = i * 8 + j;
                plankList[id] = plankArray_vertical[i, j].GetState();
                id = i * 8 + j + 64;
                plankList[id] = plankArray_horizontal[i, j].GetState();
            }
        }
    }

    public void DownloadPlankInfo()
    {
        int id = 0;
        for (int i = 1; i < 9; i++)
        {
            for (int j = 1; j < 9; j++)
            {
                id = i * 8 + j;
                plankArray_vertical[i, j].ChangeState(plankList[id]);
                id = i * 8 + j + 64;
                plankArray_horizontal[i, j].ChangeState(plankList[id]);
            }
        }
    }


    public void MoveToChess(TileScript ts)
    {
        //CheckTileAvailability(player.pos );
        if (!isGaming) return;
        if (!MyTurn()) return;
        if (!ts.isAvailable) {
            Debug.Log("不能走这格");
            return;
        }

        if ((x1 == ts.pos.x&&y1 == ts.pos.y)||(x2 == ts.pos.x&&y2 == ts.pos.y))
        {
            Debug.Log("不能走这格");
            return;
        }

        GetPlayer ().SetPosition(ts.pos);
        UploadChessInfo();

        CheckWin();
        if (isGaming)
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
        
        for (int i = 1; i < 9; i++)
        {
            for (int j = 1; j < 9; j++)
            {
                PlankScript p = plankArray_horizontal[i, j];

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

        while (open.Count > 0)
        {
            for (int i = 0; i < open.Count; i++)
            {
                TileScript t1 = open[i];
                TileScript t2;

                if (t1.pos.y == y)
                {
                    return 1;
                }

                if (t1.toDown && t1.pos.y > 0)
                {
                    t2 = tileArray[t1.pos.x, t1.pos.y - 1];
                    if (!close.Contains(t2))
                    {
                        open.Add(t2);
                    }
                }
                if (t1.toLeft && t1.pos.x > 0)
                {
                    t2 = tileArray[t1.pos.x - 1, t1.pos.y];
                    if (!close.Contains(t2))
                    {
                        open.Add(t2);
                    }
                }
                if (t1.toRight && t1.pos.x < 8)
                {
                    t2 = tileArray[t1.pos.x + 1, t1.pos.y];
                    if (!close.Contains(t2))
                    {
                        open.Add(t2);
                    }
                }
                if (t1.toUp && t1.pos.y < 8)
                {
                    t2 = tileArray[t1.pos.x, t1.pos.y + 1];
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
