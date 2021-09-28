using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TileScript : MonoBehaviour
{
    private RectTransform rectTransform;

    public Vector2Int pos;
    public Button button;
    public bool isAvailable = true;

    public bool toLeft = true, toRight = true, toUp = true, toDown = true;

    public Vector2Int SetPosition(int x, int y)
    {
        pos = new Vector2Int(x, y);
        gameObject.name = "b "+x + ", " + y;
        rectTransform.anchoredPosition = Utils.AnchoredPosInChessboard(x, y);

        return pos;
    }

    public void SetAvailable(bool a)
    {
        isAvailable = a;
    }


    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
