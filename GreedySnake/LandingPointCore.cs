using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreedySnake
{
    /// <summary>
    /// 落点管理
    /// </summary>
    public class LandingPointCore
    {
        /// <summary>
        /// 游戏落地矩阵
        /// </summary>
        List<LandingPoints> LandingPoint { get; set; }
        public LandingPointCore(float DpiX, float DpiY,int SideLength)
        {
            LandingPoint = new List<LandingPoints>();

            int SideLengthInterval = SideLength / 3;
            int LatticeDistance= SideLength + SideLengthInterval;

            //得到游戏面积
            for (int x = 1; x < (DpiX/ LatticeDistance) -1; x++)
            {
                for (int y = 1; y < (DpiY/ LatticeDistance) -1; y++)
                {
                    LandingPoint.Add(new LandingPoints()
                    {
                        PointX = (x * LatticeDistance)- SideLengthInterval,
                        PointY=(y * LatticeDistance)- SideLengthInterval, 
                        X=x,
                        Y=y
                    }) ;
                }
            }

        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <returns></returns>
        public List<LandingPoints> GetAllLandingPoints() {
            return LandingPoint;
        }

        /// <summary>
        /// 转换成拥有画布大小的坐标轴
        /// </summary>
        /// <param name="bodies"></param>
        /// <returns></returns>
        public IEnumerable<LandingPoints> ExchangePoint(List<BodyPoint> bodies)
        {
            return bodies.Select(x=>(LandingPoints)x);
        }
        /// <summary>
        /// 游戏 X Y换算成画布对象
        /// 如果为空则表示没有该位置 撞墙了
        /// </summary>
        /// <returns></returns>
        public BodyPoint XYExchangePoint(int x,int y) {
           return LandingPoint.FirstOrDefault(c => c.X == x && c.Y == y);
        }
    }
    /// <summary>
    /// 游戏位置与画布位置
    /// </summary>
    public class LandingPoints: BodyPoint
    {
        /// <summary>
        /// 对应像素点X
        /// </summary>
        public int PointX { get; set; }
        /// <summary>
        /// 对应像素点Y
        /// </summary>
        public int PointY { get; set; }


    }

    /// <summary>
    /// 游戏落点
    /// </summary>
    public class BodyPoint
    {

        /// <summary>
        /// 游戏落点X
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 游戏落点Y
        /// </summary>
        public int Y { get; set; }
    }
}
