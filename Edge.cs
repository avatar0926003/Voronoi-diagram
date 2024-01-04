using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORONOI
{
    public class Edge
    {
        public PointF vertex1;
        public PointF vertex2;
        public Edge(PointF vertex1, PointF vertex2)
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
        }
    }
}
