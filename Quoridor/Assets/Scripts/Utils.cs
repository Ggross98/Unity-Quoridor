using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{

    public static float PosXInChessboard(int x)
    {
        return 90f * x + 45f;

    }

    public static float PosYInChessboard(int y)
    {
        return 90f * y + 45f;

    }

    public static Vector2 AnchoredPosInChessboard(int x, int y)
    {
        return new Vector2(PosXInChessboard(x), PosYInChessboard(y));

    }


    public static float PosXInGrid(int x)
    {
        return 90f * x;

    }

    public static float PosYInGrid(int y)
    {
        return 90f * y;

    }

    public static Vector2 AnchoredPosInGrid(int x, int y)
    {
        return new Vector2(PosXInGrid(x), PosYInGrid(y));

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
