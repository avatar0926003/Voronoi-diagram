using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORONOI
{
    static class calculate
    {
        //求兩點中點
        public static PointF mid_point(PointF p1, PointF p2)
        {
            return new PointF((p1.X+p2.X)/ 2, (p1.Y + p2.Y) / 2);
        }
        //集合點是否垂直
        public static bool vertical(List<PointF> Points)
        {
            bool flag = true;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                if (Points[i].X != Points[i + 1].X)
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }
        //集合點是否水平
        public static bool horizontal(List<PointF> Points)
        {
            return (Points[0].Y == Points[1].Y) && (Points[1].Y == Points[2].Y);
        }
        //內積
        public static int dot(Vector v1, Vector v2)
        {
            // 沒有除法，儘量避免誤差。
            return v1.x * v2.x + v1.y * v2.y;
        }
        //外積 // 向量P1P2叉積向量P1P3。大於零表示從P1P2到P1P3為逆時針旋轉。
        public static float cross(PointF P1, PointF P2, PointF P3)
        {
            return (P2.X - P1.X) * (P3.Y - P1.Y) - (P2.Y - P1.Y) * (P3.X - P1.X);
        }

        //求2點的垂直平分線在y=0的x位置
        public static PointF vertical_y0(PointF p1, PointF p2)
        {
            PointF mid_point = calculate.mid_point(p1, p2);
            return new PointF(((0 - mid_point.Y) * (p1.Y - p2.Y) * (-1) / (p1.X - p2.X)) + mid_point.X, 0);
        }
        //求2點的垂直平分線在y=600的x位置

        public static PointF vertical_y600(PointF p1, PointF p2)
        {
            PointF mid_point = calculate.mid_point(p1, p2);
            return new PointF(((600 - mid_point.Y) * (p1.Y - p2.Y) * (-1) / (p1.X - p2.X)) + mid_point.X, 600);
        }
        public static float transfer_yvar(PointF p1, PointF p2,float Y)
        {
            float df= (Y - p1.Y) * (p1.X - p2.X) / (p1.Y - p2.Y) + p1.X;
            return df;
        }
        public static PointF transfer_x600(PointF p1, PointF p2)
        {
            PointF temp = new PointF(600, (600 - p1.X) * (p1.Y - p2.Y) / (p1.X - p2.X) + p1.Y);
            return temp;
        }
        public static PointF transfer_x0(PointF p1, PointF p2)
        {
            PointF temp = new PointF(0, (0 - p1.X) * (p1.Y - p2.Y) / (p1.X - p2.X) + p1.Y);
            return temp;
        }
        //斜率
        public static float slope(PointF p1, PointF p2)
        {
            return (p1.Y - p2.Y) / (p1.X - p2.X);
        }
        public static bool cmp_slope(PointF p1, PointF p2,PointF p3)//p1p2斜率 是否大於p1p3
        {
            return ((p1.Y - p2.Y) / (p1.X - p2.X)) > ((p1.Y - p3.Y) / (p1.X - p3.X));
        }
        public static double distance(PointF p1, PointF p2)//p1p2斜率 是否大於p1p3
        {
            return Math.Sqrt((Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)));
        }
    }
}
