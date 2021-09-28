using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessScript : MonoBehaviour
{
    public int x, y;
    public Vector2Int pos;
    private RectTransform rectTransform;

    public int fraction = 0;
    public int goal;
    public int leftPlanks = 10;

    public Vector2Int SetPosition(int x, int y)
    {
        pos = new Vector2Int(x, y);
        this.x = x; this.y = y;
        rectTransform.anchoredPosition = Utils.AnchoredPosInChessboard(x, y);

        return pos;
    }

    public void SetFraction(int f)
    {
        fraction = f;
        if (f == 0) goal = 8;
        else goal = 0;
    }

    public void Reset()
    {
        if(goal == 0)
        {
            SetPosition(4, 8);
            leftPlanks = 10;
        }
        else
        {
            SetPosition(4, 0);
            leftPlanks = 10;
        }
    }

    public bool ReachGoal()
    {
        return y == goal;
    }


    public Vector2Int SetPosition(Vector2Int tPos)
    {
        return SetPosition(tPos.x, tPos.y);
    }


    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
