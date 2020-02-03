using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Console2048
{
    /// <summary>
    /// 游戏核心算法类，与平台无关
    /// </summary>
    class GameCore
    {
        private int[,] map;
        private int[] mergeArray;
        private int[] removeZeroArray;
        private int[,] originalMap;
        private bool[,] isMerged;

        public bool[,] IsMerged
        {
            get 
            { return isMerged; }
        }


        public int[,] Map
        {
            get
            { return map; }
        }


        public GameCore()
        {
            //实例化4*4
            map = new int[4, 4];
            //new 数组  每行有多少个
            mergeArray = new int[map.GetLength(0)];
            //去零数组
            removeZeroArray = new int[4];
            //布局
            emptyLOC = new List<Location>(16);
            //随机数
            random = new Random();
            //原来的二维数组
            originalMap = new int[4, 4];
            //合并单元格数组
            isMerged = new bool[4, 4];
        }

        private void RemoveZero()
        {

            Array.Clear(removeZeroArray, 0, 4);

            int index = 0;
            for (int i = 0; i < mergeArray.Length; i++)
            {
                if (mergeArray[i] != 0)
                    removeZeroArray[index++] = mergeArray[i];//1
            }

            removeZeroArray.CopyTo(mergeArray, 0);
        }

        private void Merge(ref bool[,] isMerged, int r, int c, MoveDirection dir)
        {
            RemoveZero();//调用去零
            for (int i = 0; i < mergeArray.Length - 1; i++)
            {
                if (mergeArray[i] != 0 && mergeArray[i] == mergeArray[i + 1])
                {
                    mergeArray[i] += mergeArray[i + 1];
                    mergeArray[i + 1] = 0;
                    //记录合并位置
                    if(dir==MoveDirection.Up)
                    {
                        isMerged[i, c] = true;
                    }
                    else if(dir == MoveDirection.Down)
                    {
                        isMerged[3 - i, c] = true;
                    }
                    else if (dir == MoveDirection.Left)
                    {
                        isMerged[r, i] = true;
                    }
                    else if (dir == MoveDirection.Right)
                    {
                        isMerged[r, 3 - i] = true;
                    }
                }
            }
            RemoveZero();//调用去零
        }

        private void MoveUp()
        {
            for (int c = 0; c < map.GetLength(1); c++)
            {
                for (int r = 0; r < map.GetLength(0); r++)
                    mergeArray[r] = map[r, c];

                Merge(ref isMerged, -1, c, MoveDirection.Up);

                for (int r = 0; r < map.GetLength(0); r++)
                    map[r, c] = mergeArray[r];
            }
        }

        private void MoveDown()
        {
            for (int c = 0; c < map.GetLength(1); c++)
            {
                for (int r = map.GetLength(0) - 1; r >= 0; r--)
                {
                    mergeArray[3 - r] = map[r, c];
                }

                Merge(ref isMerged, -1, c, MoveDirection.Down);

                for (int r = map.GetLength(0) - 1; r >= 0; r--)
                {
                    map[r, c] = mergeArray[3 - r];
                }
            }
        }

        private void MoveLeft()
        {
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                    mergeArray[c] = map[r, c];

                Merge(ref isMerged, r, -1, MoveDirection.Left);

                for (int c = 0; c < 4; c++)
                    map[r, c] = mergeArray[c];
            }
        }

        private void MoveRight()
        {
            for (int r = 0; r < 4; r++)
            {
                for (int c = 3; c >= 0; c--)
                    mergeArray[3 - c] = map[r, c];

                Merge(ref isMerged, r, -1, MoveDirection.Right);

                for (int c = 3; c >= 0; c--)
                    map[r, c] = mergeArray[3 - c];
            }
        }

        /// <summary>
        /// 地图是否发生改变
        /// </summary>
        public bool IsChange { get; set; }

        public void Move(MoveDirection direction)
        {
            //移动前记录Map   
            Array.Copy(map, originalMap, map.Length);
            IsChange = false;//假设没有发生改变

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    isMerged[i, j] = false;
                }
            }

            switch (direction)
            {
                case MoveDirection.Up: MoveUp(); break;
                case MoveDirection.Down: MoveDown(); break;
                case MoveDirection.Left: MoveLeft(); break;
                case MoveDirection.Right: MoveRight(); break;
            }

            //移动后对比  重构 --> 提取方法
            CheckMapChange();
        }


        private void CheckMapChange()
        {
            for (int r = 0; r < map.GetLength(0); r++)
            {
                for (int c = 0; c < map.GetLength(1); c++)
                {
                    if (map[r, c] != originalMap[r, c])
                    {
                        IsChange = true;//发生改变
                        return;
                    }
                }
            }
        }

        /*
            在空白位置上， 随机生成数字(2 (90%)     4(10%))
         * 1.计算空白位置
         * 2.随机选择位置
         * 3.随机生成数字
         */

        private List<Location> emptyLOC;//布局
        private void CalculateEmpty()
        {

            emptyLOC.Clear();
            for (int r = 0; r < map.GetLength(0); r++)
            {
                for (int c = 0; c < map.GetLength(1); c++)
                {
                    if (map[r, c] == 0)
                    {
                        emptyLOC.Add(new Location(r, c));
                    }
                }
            }

        }

        private Random random;

        /// <summary>
        /// 生成新数字
        /// </summary>
        public void GenerateNumber(out Location loc, out int newNumber)
        {
            CalculateEmpty();

            if (emptyLOC.Count > 0)
            {
                int emptyLocIndex = random.Next(0, emptyLOC.Count);//0,15

                loc = emptyLOC[emptyLocIndex];//有空位置的list  3

                newNumber = map[loc.RIndex, loc.CIndex] = random.Next(0, 10) == 1 ? 4 : 2;

                //将该位置清除
                emptyLOC.RemoveAt(emptyLocIndex);
            }
            else
            {
                //int?是C#的可空值类型 如需获取值，需要a.Value
                newNumber = -1;
                loc = new Location(-1, -1);
            }
        }

        /// <summary>
        /// 游戏是否结束
        /// </summary>
        public bool IsOver()
        {

            if (emptyLOC.Count > 0) return false;
            #region 省略
            //水平
            //for (int r = 0; r < 4; r++)
            //{
            //    for (int c = 0; c < 3; c++)
            //    {
            //        if (map[r, c] == map[r, c + 1])
            //            return false;
            //    } 
            //}
            ////垂直
            //for (int c = 0; c < 4; c++)
            //{
            //    for (int r = 0; r < 3; r++)
            //    {
            //        if (map[r, c] == map[r + 1, c])
            //            return false;
            //    } 
            //}
            #endregion
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (map[r, c] == map[r, c + 1] || map[c, r] == map[c + 1, r])
                        return false;
                }
            }

            return true;//游戏结束 
        }
    }
}
