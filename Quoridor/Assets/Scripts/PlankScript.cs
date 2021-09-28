using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlankScript : MonoBehaviour
{
    public bool isVertical = false;
    public bool isAvailable = true;
    public bool isFixed = false;

    public Vector2Int pos;
    public int x, y;

    public Button button;
    private RectTransform rectTransform;
    public const int FIXED = 1, AVAILABLE = 0, UNAVAILABLE = -1;

    public Vector2 SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
        pos = new Vector2Int(x, y);
        gameObject.name = "p "+isVertical +" " + x + ", " + y;
        rectTransform.anchoredPosition = Utils.AnchoredPosInGrid(x, y);

        return pos;
    }

    public void ChangeState(int state)
    {
        switch (state)
        {
            case AVAILABLE:
                this.isFixed = false;
                this.isAvailable = true;
                break;
            case UNAVAILABLE:
                SetAvailable(false);
                break;
            case FIXED:
                SetFixed(true);
                break;
        }
    }

    public int GetState()
    {
        if (isFixed) return FIXED;
        else if (!isAvailable) return UNAVAILABLE;
        return AVAILABLE;
    }

    public void SetVertical(bool a)
    {
        isVertical = a;

        if (a)
        {
            rectTransform.SetPositionAndRotation(rectTransform .position , Quaternion.Euler(new Vector3(0, 0, 90)));
        }
        else
        {
            rectTransform.SetPositionAndRotation(rectTransform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
        }
    }

    public void SetAvailable(bool a)
    {
        isAvailable = a;

        if (!a)
        {
            ColorBlock cb = new ColorBlock();
            cb.disabledColor= new Color(0,0,0,255);
            button.colors = cb;
            button.interactable = false;
        }
    }

    public void SetFixed(bool f)
    {
        isFixed = f;

        if (f)
        {
            //SetAvailable(false);
            isAvailable = false;
            button.interactable = false;

        }
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
