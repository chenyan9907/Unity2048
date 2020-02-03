using Console2048;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MoveDirection = Console2048.MoveDirection;

/// <summary>
/// 游戏控制器
/// </summary>
public class GameController : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private GameCore core;
    public NumberSprite[,] spriteActionArray;
    private void Start()
    {
        core = new GameCore();
        spriteActionArray = new NumberSprite[4, 4];
        Init();
        GenerateNewNumber();
        GenerateNewNumber();
    }

    private void Init()
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                CreateSprite(r, c);
            }
        }
    }

    private void CreateSprite(int r, int c)
    {
        //Instantiate();
        GameObject go = new GameObject(r.ToString() + "," + c.ToString());
        go.AddComponent<Image>();
        NumberSprite action = go.AddComponent<NumberSprite>();  //Awake立即执行 Start下一帧执行
        //将引用存储到二维数组中
        spriteActionArray[r, c] = action;
        action.SetImage(0);
        //创建的游戏对象，scale默认为1
        go.transform.SetParent(this.transform, false);
    }

    private void GenerateNewNumber()
    {
        Location loc;
        int newNumber;
        core.GenerateNumber(out loc, out newNumber);
        spriteActionArray[loc.RIndex, loc.CIndex].SetImage(newNumber);
        //播放生成效果
        spriteActionArray[loc.RIndex, loc.CIndex].CreateEffect();
    }


    private void Update()
    {
        if (core.IsChange)
        {
            //更新界面
            UpdateMap();
            //产生新数字
            GenerateNewNumber();
            //移动效果
            //合并效果

            //判断游戏是否结束
            if (core.IsOver())
            {

            }
            core.IsChange = false;
        }
    }

    private void UpdateMap()
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                spriteActionArray[r, c].SetImage(core.Map[r, c]);
            }
        }

        //根据合并位置，执行相应合并动画
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (core.IsMerged[i, j] == true)
                {
                    spriteActionArray[i, j].MergeEffect();
                }
            }
        }
    }

    private Vector2 startPoint;
    private bool isDown = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        startPoint = eventData.position;
        isDown = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDown == false)
            return;

        Vector3 offset = eventData.position - startPoint;

        float x = Mathf.Abs(offset.x);
        float y = Mathf.Abs(offset.y);

        MoveDirection? dir = null;
        //水平移动
        if (x > y && x >= 50)
        {
            dir = offset.x > 0 ? MoveDirection.Right : MoveDirection.Left;
        }
        //垂直移动
        if (x < y && y >= 50)
        {
            dir = offset.y > 0 ? MoveDirection.Up : MoveDirection.Down;
        }
        if (dir != null)
        {
            core.Move(dir.Value);
            isDown = false;
        }
    }
}
