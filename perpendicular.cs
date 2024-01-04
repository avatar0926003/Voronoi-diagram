using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORONOI
{
    public class perpendicular
    {
        public Edge edge;
        public PointF Cut_p1;//被中垂線切割的2個點
        public PointF Cut_p2;
        //public int right_polygon;
        //public int left_polygon;


        public perpendicular(Edge e, PointF Cut_p1, PointF Cut_p2)
        {
            this.edge = e;
            this.Cut_p1 = Cut_p1;
            this.Cut_p2 = Cut_p2;            
        }
   }
    public struct Edge
    {
        public PointF start_vertex;//中垂線起點
        public PointF end_vertex;//節垂線結束點 
    }
}
