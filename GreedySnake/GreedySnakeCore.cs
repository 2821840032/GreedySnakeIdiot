using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreedySnake
{
    public class GreedySnakeCore
    {
        Graphics Graphic = null;

        /// <summary>
        /// 边长
        /// </summary>
        const int SideLength = 20;

        /// <summary>
        /// 落点管理
        /// </summary>
        LandingPointCore LandingPointCores { get; set; }

        /// <summary>
        /// 蛇身
        /// </summary>
        List<BodyPoint> SnakeBodys { get; set; }

        /// <summary>
        /// 蛇头
        /// </summary>
        BodyPoint SnakeHead { get; set; }

        /// <summary>
        /// 运动方向X
        /// </summary>
        DirectionX SnakeDirectionx { get; set; }

        /// <summary>
        /// 运动方向Y
        /// </summary>
        DirectionY SnakeDirectiony { get; set; }

        /// <summary>
        /// 食物
        /// </summary>
        List<BodyPoint> Foods { get; set; }

        /// <summary>
        /// 吃到食物事件
        /// </summary>
        public event Action EatFoodEvent;


        public GreedySnakeCore(Graphics Graphic) {
            this.Graphic = Graphic;
            LandingPointCores = new LandingPointCore(Graphic.VisibleClipBounds.Width, Graphic.VisibleClipBounds.Height, SideLength);
            SnakeBodys = new List<BodyPoint>();
            Foods = new List<BodyPoint>();

            ///初始化蛇
            var snakeBodys = LandingPointCores.GetAllLandingPoints().Take(8).ToList();
            SnakeBodys.AddRange(snakeBodys);
            SnakeHead = snakeBodys[0];
            SnakeDirectionx = DirectionX.Right;
            SnakeDirectiony = DirectionY.Wait;

            ObtainFoods();
            DrawSnake();

        }
        /// <summary>
        /// 随机获取食物
        /// </summary>
        public void ObtainFoods() {
            Random ra = new Random();
            for (int i = 0; i < ra.Next(1,2); i++)
            {
                var food = LandingPointCores.GetAllLandingPoints().OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                if (IsOpenSpace(food.X,food.Y))
                {
                    Foods.Add(food);
                }
                else {
                    i--;
                }
             
            }
        }

        /// <summary>
        /// 游戏绘制
        /// </summary>
        public void DrawSnake() {
            Graphic.Clear(Color.Black);
            foreach (var item in LandingPointCores.ExchangePoint(SnakeBodys))
            {
                Rectangle r = new Rectangle(item.PointX, item.PointY, SideLength, SideLength);
                Graphic.DrawRectangle(Pens.White, r);
                Graphic.FillRectangle(Brushes.White, r);
            }
            foreach (var item in LandingPointCores.ExchangePoint(Foods))
            {
                Rectangle r = new Rectangle(item.PointX, item.PointY, SideLength, SideLength);
                Graphic.DrawRectangle(Pens.Yellow, r);
                Graphic.FillRectangle(Brushes.Yellow, r);
            }
        }

        /// <summary>
        /// 时钟前进
        /// </summary>
        public void ClockForward() {
            ModifyDirection(Wheregonext());
            var snakeHead = LandingPointCores.XYExchangePoint(SnakeHead.X + (int)SnakeDirectionx, SnakeHead.Y + (int)SnakeDirectiony);
            if (snakeHead == null)
            {
                //撞墙 游戏结束
                return;
            }
            if (SnakeBodys.Contains(snakeHead))
            {
                //撞身体 游戏结束
                return;
            }
            //判断是否吃到食物
            if (Foods.Contains(snakeHead))
            {
                SnakeHead = snakeHead;
                SnakeBodys.Insert(0, SnakeHead);
                Foods.Remove(snakeHead);
                EatFoodEvent?.Invoke();


            }
            if (Foods.Count <= 0)
                ObtainFoods();
            else {
                SnakeHead = snakeHead;
                SnakeBodys.Insert(0, SnakeHead);
                SnakeBodys.RemoveAt(SnakeBodys.Count - 1);
            }
          

            DrawSnake();
        }
        /// <summary>
        /// 修改方向
        /// </summary>
        /// <param name="key"></param>
        public void ModifyDirection(Keys key) {


            //计算得出第二截相对于第一截的位置
            DirectionX directionX =(DirectionX)(SnakeHead.X - SnakeBodys[1].X);

            DirectionY directionY = (DirectionY)(SnakeHead.Y - SnakeBodys[1].Y);

            if (key == Keys.Up && directionY != DirectionY.Down)
            {
                SnakeDirectionx = DirectionX.Wait;
                SnakeDirectiony = DirectionY.UP;
            }

            if (key == Keys.Down && directionY != DirectionY.UP)
            {
                SnakeDirectionx = DirectionX.Wait;
                SnakeDirectiony = DirectionY.Down;
            }
               
            if (key == Keys.Right && directionX!= DirectionX.Left)
            {
                SnakeDirectiony = DirectionY.Wait;
                SnakeDirectionx = DirectionX.Right;
            }
               
            if (key == Keys.Left && directionX != DirectionX.Right)
            {
                SnakeDirectiony = DirectionY.Wait;
                SnakeDirectionx = DirectionX.Left;
            }
              

        }

        /// <summary>
        /// 下一步去哪？
        /// </summary>
        /// <returns></returns>
        public Keys Wheregonext()
        {
            //蛇头位置
            var hx = SnakeHead.X;
            var hy = SnakeHead.Y;
            //食物距离
            var fx = Foods.FirstOrDefault().X;
            var fy = Foods.FirstOrDefault().Y;

            //distance结构 方向，与食物的距离，转向后可用步数
            Dictionary<Keys, Tuple<int, int>> distance = new Dictionary<Keys, Tuple<int, int>>();
            distance.Add(Keys.Left, new Tuple<int, int>(hx - fx, DarkEcho(SnakeHead.X - 1, SnakeHead.Y)));
            distance.Add(Keys.Right, new Tuple<int, int>(fx - hx, DarkEcho(SnakeHead.X + 1, SnakeHead.Y)));
            distance.Add(Keys.Up, new Tuple<int, int>(hy - fy, DarkEcho(SnakeHead.X, SnakeHead.Y - 1)));
            distance.Add(Keys.Down, new Tuple<int, int>(fy - hy, DarkEcho(SnakeHead.X, SnakeHead.Y + 1)));

            //预测不能走动的方向
            var availabledistance = distance.Where(x => x.Value.Item2 > (SnakeBodys.Count)).ToList();
            if (availabledistance.Count == 0)
            {
                //如果没有可用方向则按可用步数倒序选取第一个方向
                return distance.OrderByDescending(x => x.Value.Item2).FirstOrDefault().Key;
            }
            //选择食物最小距离
            return availabledistance.OrderByDescending(x => x.Value.Item1).FirstOrDefault().Key;
        }

        /// <summary>
        /// 回声探路
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns></returns>
        public int DarkEcho(int x, int y)
        {
            List<BodyPoint> points = new List<BodyPoint>();
            DarkEcho(x, y, points);
            return points.Count;
        }
        /// <summary>
        /// 回声探路 递归
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns></returns>
        public void DarkEcho(int x,int y, List<BodyPoint> points) {
            if (IsOpenSpace(x, y)&& points.Where(c=>c.X==x&&c.Y==y).Count()==0)
            {
                points.Add(new BodyPoint() { X = x, Y = y });
                DarkEcho(x - 1, y, points);
                
                DarkEcho(x + 1, y, points);

                DarkEcho(x , y - 1, points);

                DarkEcho(x , y + 1, points);
            }
        }

        /// <summary>
        /// 判断是否为空地
        /// </summary>
        /// <returns></returns>
        bool IsOpenSpace(int x,int y)
        {
            //空地判断
            var changPoint =  LandingPointCores.XYExchangePoint(x, y);
            if (changPoint == null)
                return false ;
            if (SnakeBodys.Contains(changPoint))
            {
                return false;
            }
            return true;


        }
    }
    /// <summary>
    /// 方向Y
    /// </summary>
    public enum DirectionY { 
    
        UP=-1,
        Down= 1,
        Wait=0

    }
    /// <summary>
    /// 方向X
    /// </summary>
    public enum DirectionX
    {

        Wait = 0,
        Right = 1,
        Left = -1

    }


}
