using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源管理类，负责管理加载资源
/// </summary>
public class ResourceManager : MonoBehaviour
{
    //private static Sprite[] spriteArray;
    //static ResourceManager()
    //{
    //    //静态构造函数
    //    spriteArray = Resources.LoadAll<Sprite>("AtlasName");
    //}

    /// <summary>
    /// 读取数字Sprite
    /// </summary>
    /// <param name="number">Sprite数字</param>
    /// <returns>Sprite</returns>
    public static Sprite LoadSprite(int number)
    {
        //读取单个Sprite使用Load，读取Sprite图集使用LoadAll
        return Resources.Load<Sprite>(number.ToString());
    }
}
