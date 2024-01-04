/*
	Name: Form1.cs
	Copyright: Copyright © 2021
	Author: 李健廷 
    Student ID: M103040066
    Class: 資訊碩一
	Date: 
	Description: 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VORONOI
{
    public struct Vector { public int x, y; };
    public partial class Form1 : Form
    {
        //Pen pen;
        int pen_size = 2;
        int draw_point_size = 6;
        Color color_point = Color.Black;
        Color color_line = Color.Green;
        //Color color_hide_line = Color.FromArgb(240, 240, 240);

        List<PointF> points = new List<PointF>();
        int point_cnt;//終止條件
        bool finish = true;
        Thread t, t_exec;
        AutoResetEvent auto = new AutoResetEvent(false);
        ManualResetEvent stepbystep = new ManualResetEvent(true);
        Dictionary<string, int> point_dic = new Dictionary<string, int>();//檢查新加入的點是否重複

        List<Edge> test = new List<Edge>();
        public Form1()
        {
            InitializeComponent();
            init();
        }
        private void init()
        {
            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bitmap;
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 10000;
            toolTip1.InitialDelay = 500;
            toolTip1.ShowAlways = false;
            toolTip1.SetToolTip(this.button1, "讀取檔案");
            toolTip1.SetToolTip(this.button2, "下一組資料");
            toolTip1.SetToolTip(this.run, "Run");
            toolTip1.SetToolTip(this.nextstep, "Step by step");
            toolTip1.SetToolTip(this.clear, "清除");
            toolTip1.SetToolTip(this.read_draw, "讀取輸出的文字檔案");
            toolTip1.SetToolTip(this.save, "儲存目前結果");
        }
        private void main_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (t != null)
            {
                t.Abort();
                t_exec.Abort();
            }
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            PointF p = new PointF(e.X, e.Y);
            Draw_point(p, color_point);
        }
        private void openfolder(object sender, EventArgs e)
        {
          
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "請選擇輸入的文字檔案";
            dialog.Filter = "txt files|*.txt;*.ini;*.doc;*.docx";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (t != null)
                {
                    t.Abort();
                }
                this.clear.PerformClick();
                ParameterizedThreadStart par_T = new ParameterizedThreadStart(Read_infile);
                t = new Thread(par_T);
                t.Start(dialog.FileName);
            }
        }
        //讀取輸入文字檔案
        public delegate void Invoke_Addpoint(PointF point, Color color);
        public void Read_infile(object o_filename)
        {
            string filename = o_filename.ToString();
            using (StreamReader sr = new StreamReader(filename))
            {
                string Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    if (Line.Length > 0 && Line.Replace(" ", "").Substring(0, 1) != "#")
                    {
                        string pointline;
                        if (Int32.TryParse(Line, out int num))
                        {
                            for (int i = 0; i < Convert.ToInt32(num); i++)
                            {
                                pointline = sr.ReadLine();
                                string[] coord = pointline.Split(' ');
                                string dickey = coord[0] + "," + coord[1];
                                if (!point_dic.ContainsKey(dickey))
                                {
                                    point_dic.Add(dickey, 1);
                                    PointF p = new PointF(Convert.ToSingle(coord[0]), Convert.ToSingle(coord[1]));
                                    Invoke_Addpoint Invoke_Addpoint = new Invoke_Addpoint(Draw_point);
                                    this.BeginInvoke(Invoke_Addpoint, p, color_point);
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                        auto.WaitOne(-1);
                    }
                }
            }
        }

        //讀取輸出文字檔案 待確認
        public void Read_outfile(object o_filename)
        {
            string filename = o_filename.ToString();
            using (StreamReader sr = new StreamReader(filename))
            {
                string Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    if (Line.Length > 0)
                    {
                        switch (Line.Replace(" ", "").Substring(0, 1))
                        {
                            case "P":
                                string[] coord_P = Line.Split(' ');
                                PointF p = new PointF(Convert.ToSingle(coord_P[1]), Convert.ToSingle(coord_P[2]));
                                Invoke_Addpoint Invoke_Addpoint = new Invoke_Addpoint(Draw_point);
                                this.BeginInvoke(Invoke_Addpoint, p, color_point);
                                break;
                            case "E":
                                string[] coord_E = Line.Split(' ');
                                PointF p1 = new PointF(Convert.ToSingle(coord_E[1]), Convert.ToSingle(coord_E[2]));
                                PointF p2 = new PointF(Convert.ToSingle(coord_E[3]), Convert.ToSingle(coord_E[4]));
                                Draw_Edge(p1, p2, color_line);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        //待確認
        private void clear_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bitmap);
            pictureBox1.Image = bitmap;//畫布清空
            //點相關清空
            points.Clear();
            list_box.Items.Clear();
            point_dic.Clear();
            finish = true;
        }

        private void run_Click(object sender, EventArgs e)
        {
            stepbystep.Set();
            points = Sort_ListPointF(points);
            finish = true;
            exec(points);
        }

        private region exec(List<PointF> Points)
        {
            region final = new region();
            final.convex_point = new List<PointF>();
            final.perpendicular_bisector = new List<perpendicular>();
            if (Points.Count == 1)
            {
                final.convex_point.Add(Points[0]);
            }
            else if (calculate.vertical(Points))
            {
                final.convex_point.Add(Points[0]);
                for (int i = 0; i < Points.Count - 1; i++)
                {
                    PointF mid_point = calculate.mid_point(Points[i], Points[i + 1]);
                    Edge edge;
                    edge.start_vertex = new PointF(-10000, mid_point.Y);
                    edge.end_vertex = new PointF(10000, mid_point.Y);
                    perpendicular perpendicular_bisector = new perpendicular(edge, Points[i], Points[i + 1]);
                    final.perpendicular_bisector.Add(perpendicular_bisector);
                    final.convex_point.Add(Points[i + 1]);
                }
            }
            else if (Points.Count == 2)
            {
                PointF mid_point = calculate.mid_point(Points[0], Points[1]);
                PointF p1;
                PointF p2;
                /*if (Points[0].Y == Points[1].Y)
                {
                    //p1 = new PointF(mid_point.X, 0);
                    //p2 = new PointF(mid_point.X, 600);
                }
                else
                {
                    p1 = calculate.vertical_y0(Points[0], Points[1]);
                    p2 = calculate.vertical_y600(Points[0], Points[1]);
                }*/
                p1 = calculate.vertical_y0(Points[0], Points[1]);
                p2 = calculate.vertical_y600(Points[0], Points[1]);
                final.convex_point.Add(Points[0]);
                final.convex_point.Add(Points[1]);
                Edge edge;
                edge.start_vertex = p1;
                edge.end_vertex = p2;
                perpendicular perpendicular_bisector = new perpendicular(edge, Points[0], Points[1]);
                final.perpendicular_bisector.Add(perpendicular_bisector);
            }
            else
            {
                int divide_index = Get_divide_index(Points);//divide_index 為左半邊最後要包含的點
                List<PointF> l_points = new List<PointF>();
                for (int i = 0; i <= divide_index; i++)
                {
                    l_points.Add(Points[i]);
                }
                region l_region = exec(l_points);
                List<PointF> r_points = new List<PointF>();
                for (int i = divide_index + 1; i < Points.Count; i++)
                {
                    r_points.Add(Points[i]);
                }
                region r_region = exec(r_points);
                final = merge(l_region, r_region);
            }
            Draw_region(final, Points);
            if (!finish)
            {
                stepbystep.Reset();
                stepbystep.WaitOne(-1);
            }
            return final;
        }
        private int Get_divide_index(List<PointF> Points)
        {
            int mid_index = (Points.Count - 1) / 2;//左邊要包含的最後的點 預設為中間 ex:共4個 那左邊最後index預設為 (4-1)/2=1,共3個 那左邊最後index預設為 (3-1)/2=1
            float x = Points[mid_index].X;
            int back = 0;
            int front = 0;
            for (int i = mid_index + 1; i < Points.Count; i++)//後面有幾個與mid相同的x座標
            {
                if (x == Points[i].X)
                {
                    back++;
                }
                else
                {
                    break;
                }
            }
            for (int i = mid_index - 1; 0 <= i; i--)//前面有幾個與mid相同的x座標
            {
                if (x == Points[i].X)
                {
                    front++;
                }
                else
                {
                    break;
                }
            }
            //第一種左邊不含與x相同的座標
            int count1_L = mid_index - front;//左邊個數
            int count1_R = Points.Count - count1_L;//右邊個數
            int gap1 = Math.Abs(count1_L - count1_R);
            //第二種左邊含與x相同的座標
            int count2_L = mid_index + back + 1;//左邊個數
            int count2_R = Points.Count - count2_L;//右邊個數
            int gap2 = Math.Abs(count2_L - count2_R);
            if (gap1 < gap2)//不含X
            {
                return mid_index - front - 1;
            }
            else
            {
                return mid_index + back;
            }
        }
        private region merge(region l_region, region r_region)
        {
            if (r_region == null)
            {
                return l_region;
            }
            else
            {
                region final = new region();
                final.convex_point = new List<PointF>();
                final.perpendicular_bisector = new List<perpendicular>();
                //找到左半邊最右的的最下的convex
                float temp_x = l_region.convex_point[0].X;
                float temp_y = l_region.convex_point[0].Y;
                int L_index = 0;
                for (int i = 1; i < l_region.convex_point.Count; i++)
                {
                    if (l_region.convex_point[i].X > temp_x || (l_region.convex_point[i].X == temp_x && l_region.convex_point[i].Y > temp_y))
                    {
                        temp_x = l_region.convex_point[i].X;
                        temp_y = l_region.convex_point[i].Y;
                        L_index = i;
                    }
                }
                PointF l_top = l_region.convex_point[L_index];
                PointF l_down = l_region.convex_point[L_index];
                //找到右半邊最左的的最下的convex
                int R_index = 0;
                temp_x = r_region.convex_point[0].X;
                temp_y = r_region.convex_point[0].Y;
                for (int i = 1; i < r_region.convex_point.Count; i++)
                {
                    //找到左半邊最右的的最下的convex
                    if (r_region.convex_point[i].X < temp_x || (r_region.convex_point[i].X == temp_x && r_region.convex_point[i].Y > temp_y))
                    {
                        temp_x = r_region.convex_point[i].X;
                        temp_y = r_region.convex_point[i].Y;
                        R_index = i;
                    }
                }
                PointF r_top = r_region.convex_point[R_index];
                PointF r_down = r_region.convex_point[R_index];
                //找出上下切線點
                bool flag1 = true;//有無修改過
                bool flag2 = true;//有無修改過
                int l_ccw_next = 0;
                int r_cw_next = 0;
                while (flag1 || flag2)
                {
                    flag1 = false;
                    flag2 = false;
                    if (l_region.convex_point.Count > 1)
                    {
                        while (true)
                        {
                            PointF cmp_point = l_region.convex_point[(L_index + 1 + l_ccw_next) % l_region.convex_point.Count];
                            if (calculate.cmp_slope(r_top, cmp_point, l_top))
                            {
                                l_top = cmp_point;
                                l_ccw_next++;
                                flag1 = true;//有無修改過
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        flag1 = false;
                    }
                    if (r_region.convex_point.Count > 1)
                    {
                        while (true)
                        {
                            PointF cmp_point = r_region.convex_point[(R_index + r_region.convex_point.Count - 1 - r_cw_next) % r_region.convex_point.Count];
                            if (calculate.cmp_slope(l_top, r_top, cmp_point))
                            {
                                r_top = cmp_point;
                                r_cw_next++;
                                flag2 = true;//有無修改過
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        flag2 = false;
                    }
                }
                flag1 = true;//有無修改過
                flag2 = true;//有無修改過
                int l_cw_next = 0;
                int r_ccw_next = 0;
                while (flag1 || flag2)
                {
                    flag1 = false;
                    flag2 = false;
                    if (l_region.convex_point.Count > 1)
                    {
                        while (true)
                        {
                            PointF cmp_point = l_region.convex_point[(L_index + l_region.convex_point.Count - 1 - l_cw_next) % l_region.convex_point.Count];
                            if (calculate.cmp_slope(r_down, l_down, cmp_point))
                            {
                                l_down = cmp_point;
                                l_cw_next++;
                                flag1 = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        flag1 = false;
                    }
                    if (r_region.convex_point.Count > 1)
                    {
                        while (true)
                        {
                            PointF cmp_point = r_region.convex_point[(R_index + 1 + r_ccw_next) % r_region.convex_point.Count];
                            if (calculate.cmp_slope(l_down, cmp_point, r_down))
                            {
                                r_down = cmp_point;
                                r_ccw_next++;
                                flag2 = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        flag2 = false;
                    }
                }
                //找出所有convex_hull的點
                int totalcnt = 0;
                if (l_cw_next == 0 && l_ccw_next == 0)//計算總點數 //是最高點也是最低點
                {
                    totalcnt = l_region.convex_point.Count;
                }
                else
                {
                    totalcnt = l_region.convex_point.Count - (l_cw_next + l_ccw_next - 1);
                }
                for (int i = 0; i < totalcnt; i++)
                {
                    final.convex_point.Add(l_region.convex_point[(L_index + l_ccw_next + i) % l_region.convex_point.Count]);
                }
                if (r_cw_next == 0 && r_ccw_next == 0)//計算總點數 //是最高點也是最低點
                {
                    totalcnt = r_region.convex_point.Count;
                }
                else
                {
                    totalcnt = r_region.convex_point.Count - (r_cw_next + r_ccw_next - 1);
                }
                for (int i = 0; i < totalcnt; i++)
                {
                    final.convex_point.Add(r_region.convex_point[(R_index + r_ccw_next + i) % r_region.convex_point.Count]);
                }
                int Lreplace_index = -1;
                int Rreplace_index = -1;
                PointF findL = l_top;
                PointF findR = r_top;
                Edge edge;//Hyplerplan
                edge.start_vertex = calculate.vertical_y0(findL, findR);
                List<perpendicular> hyperplan = new List<perpendicular>();
                List<string> cross_edge = new List<string>();
                List<string> cross_point = new List<string>();

                float last_y = 0;
                List<string> last_select_edge = new List<string>();
                while (true)
                {
                    string cross_region = "";
                    edge.end_vertex = calculate.vertical_y600(findL, findR);
                    for (int i = 0; i < l_region.perpendicular_bisector.Count; i++)
                    {
                        if (l_region.perpendicular_bisector[i].Cut_p1 == findL || l_region.perpendicular_bisector[i].Cut_p2 == findL)
                        {
                            PointF cmp = GetIntersection(l_region.perpendicular_bisector[i].edge.start_vertex, l_region.perpendicular_bisector[i].edge.end_vertex, edge.start_vertex, edge.end_vertex);
                            if (cmp.Y > last_y && cmp.Y < edge.end_vertex.Y && !last_select_edge.Contains("L" + i) &&
                                ((cmp.X <= l_region.perpendicular_bisector[i].edge.start_vertex.X && cmp.X >= l_region.perpendicular_bisector[i].edge.end_vertex.X)
                                || (cmp.X <= l_region.perpendicular_bisector[i].edge.end_vertex.X && cmp.X >= l_region.perpendicular_bisector[i].edge.start_vertex.X)))
                            {
                                Lreplace_index = i;
                                cross_region = "L";
                                edge.end_vertex = cmp;
                            }                          
                        }
                    }
                    for (int i = 0; i < r_region.perpendicular_bisector.Count; i++)
                    {
                        if (r_region.perpendicular_bisector[i].Cut_p1 == findR || r_region.perpendicular_bisector[i].Cut_p2 == findR)
                        {
                            PointF cmp = GetIntersection(r_region.perpendicular_bisector[i].edge.start_vertex, r_region.perpendicular_bisector[i].edge.end_vertex, edge.start_vertex, edge.end_vertex);
                            if (edge.end_vertex.Y == cmp.Y)
                            {
                                Rreplace_index = i;
                                cross_region = "LR";
                            }
                            else if (cmp.Y > last_y && cmp.Y < edge.end_vertex.Y && !last_select_edge.Contains("R" + i) &&
                                 ((cmp.X <= r_region.perpendicular_bisector[i].edge.start_vertex.X && cmp.X >= r_region.perpendicular_bisector[i].edge.end_vertex.X)
                                || (cmp.X <= r_region.perpendicular_bisector[i].edge.end_vertex.X && cmp.X >= r_region.perpendicular_bisector[i].edge.start_vertex.X)))
                            {
                                Rreplace_index = i;
                                cross_region = "R";
                                edge.end_vertex = cmp;
                            }            
                        }                       
                    }
                    last_select_edge.Clear();
                    last_y = edge.end_vertex.Y;
                    if (cross_region != "")
                    {
                        perpendicular perpendicular_bisector = new perpendicular(edge, findL, findR);
                        hyperplan.Add(perpendicular_bisector);
                        final.perpendicular_bisector.Add(perpendicular_bisector);
                        if (cross_region == "L")//紀錄交點與hyplerplan用來消線
                        {
                            if (edge.end_vertex.X <= 600 && edge.end_vertex.X >= 0)
                            {
                                cross_edge.Add("L" + Lreplace_index);
                                if (left_p1_pick(edge, l_region.perpendicular_bisector[Lreplace_index].edge.start_vertex))
                                {
                                    l_region.perpendicular_bisector[Lreplace_index].edge.start_vertex = edge.end_vertex;
                                }
                                else
                                {
                                    l_region.perpendicular_bisector[Lreplace_index].edge.end_vertex = edge.end_vertex;
                                }
                            }                    
                            last_select_edge.Add("L" + Lreplace_index);
                        }
                        else if (cross_region == "R")
                        {
                            if (edge.end_vertex.X <= 600 && edge.end_vertex.X >= 0)
                            {
                                cross_edge.Add("R" + Rreplace_index);
                                if (right_p1_pick(edge, r_region.perpendicular_bisector[Rreplace_index].edge.start_vertex))
                                {
                                    r_region.perpendicular_bisector[Rreplace_index].edge.start_vertex = edge.end_vertex;
                                }
                                else
                                {
                                    r_region.perpendicular_bisector[Rreplace_index].edge.end_vertex = edge.end_vertex;
                                }
                            }                   
                            last_select_edge.Add("R" + Rreplace_index);
                        }
                        else
                        {
                            //L
                            if (edge.end_vertex.X <= 600 && edge.end_vertex.X >= 0)
                            {
                                cross_edge.Add("L" + Lreplace_index);
                                if (left_p1_pick(edge, l_region.perpendicular_bisector[Lreplace_index].edge.start_vertex))
                                {
                                    l_region.perpendicular_bisector[Lreplace_index].edge.start_vertex = edge.end_vertex;
                                }
                                else
                                {
                                    l_region.perpendicular_bisector[Lreplace_index].edge.end_vertex = edge.end_vertex;
                                }
                            }                            
                            last_select_edge.Add("L" + Lreplace_index);
                            //R
                            if (edge.end_vertex.X <= 600 && edge.end_vertex.X >= 0)
                            {
                                cross_edge.Add("R" + Lreplace_index);
                                if (right_p1_pick(edge, r_region.perpendicular_bisector[Rreplace_index].edge.start_vertex))
                                {
                                    r_region.perpendicular_bisector[Rreplace_index].edge.start_vertex = edge.end_vertex;
                                }
                                else
                                {
                                    r_region.perpendicular_bisector[Rreplace_index].edge.end_vertex = edge.end_vertex;
                                }
                            }                
                            last_select_edge.Add("R" + Rreplace_index);
                        }
                        edge.start_vertex = edge.end_vertex;
                    }
                    else
                    {                
                         if (calculate.distance(l_top, r_top) < calculate.distance(l_down, r_down))
                        {
                            edge.end_vertex = calculate.vertical_y600(l_top, r_top);
                            perpendicular perpendicular_bisector = new perpendicular(edge, l_top, r_top);
                            hyperplan.Add(perpendicular_bisector);
                            final.perpendicular_bisector.Add(perpendicular_bisector);
                            break;
                        }
                        else
                        {
                            edge.end_vertex = calculate.vertical_y600(l_down, r_down);
                            perpendicular perpendicular_bisector = new perpendicular(edge, l_down, r_down);
                            hyperplan.Add(perpendicular_bisector);
                            final.perpendicular_bisector.Add(perpendicular_bisector);
                            break;
                        }
                    }
                    if (cross_region.Contains("L"))
                    {
                        findL = l_region.perpendicular_bisector[Lreplace_index].Cut_p1 != findL ? l_region.perpendicular_bisector[Lreplace_index].Cut_p1 : l_region.perpendicular_bisector[Lreplace_index].Cut_p2;
                    }
                    if (cross_region.Contains("R"))
                    {
                        findR = r_region.perpendicular_bisector[Rreplace_index].Cut_p1 != findR ? r_region.perpendicular_bisector[Rreplace_index].Cut_p1 : r_region.perpendicular_bisector[Rreplace_index].Cut_p2;
                    }
                    if (findL == l_down && findR == r_down)
                    {
                        edge.end_vertex = calculate.vertical_y600(findL, findR);
                        perpendicular perpendicular_bisector = new perpendicular(edge, findL, findR);
                        hyperplan.Add(perpendicular_bisector);
                        final.perpendicular_bisector.Add(perpendicular_bisector);
                        break;
                    }

                }
                for (int i = 0; i < l_region.perpendicular_bisector.Count; i++)
                {
                    PointF temp1 = l_region.perpendicular_bisector[i].edge.start_vertex;
                    PointF temp2 = l_region.perpendicular_bisector[i].edge.end_vertex;
                    if (!cross_edge.Contains("L" + i))
                    {
                        bool Show = true; 
                        if (temp1.X > 600)
                        {
                            temp1 = calculate.transfer_x600(temp1, temp2);
                        }
                        else if (temp1.X < 0)
                        {
                            temp1 = calculate.transfer_x0(temp1, temp2);
                        }
                        for (int j = 0; j < hyperplan.Count; j++)
                        {
                            if (temp1.Y >= hyperplan[j].edge.start_vertex.Y && temp1.Y <= hyperplan[j].edge.end_vertex.Y)
                            {
                                if (calculate.transfer_yvar(hyperplan[j].edge.start_vertex, hyperplan[j].edge.end_vertex, temp1.Y) < temp1.X)
                                {
                                    Show = false;
                                }
                            }
                        }
                        if (Show)
                        {
                            final.perpendicular_bisector.Add(l_region.perpendicular_bisector[i]);
                        }
                    }
                    else {
                        if ((temp1.X < 600 && temp1.X>0) || (temp2.X < 600 && temp2.X > 0))
                        {
                            final.perpendicular_bisector.Add(l_region.perpendicular_bisector[i]);
                        }            
                    }
                }
                for (int i = 0; i < r_region.perpendicular_bisector.Count; i++)
                {
                    PointF temp1 = r_region.perpendicular_bisector[i].edge.start_vertex;
                    PointF temp2 = r_region.perpendicular_bisector[i].edge.end_vertex;
                    if (!cross_edge.Contains("R" + i))
                    {
                        bool Show = true;
                        if (temp1.X > 600)
                        {
                            temp1 = calculate.transfer_x600(temp1, temp2);
                        }
                        else if (temp1.X < 0)
                        {
                            temp1 = calculate.transfer_x0(temp1, temp2);
                        }
                        for (int j = 0; j < hyperplan.Count; j++)
                        {
                            if (temp1.Y >= hyperplan[j].edge.start_vertex.Y && temp1.Y <= hyperplan[j].edge.end_vertex.Y)
                            {
                                if (calculate.transfer_yvar(hyperplan[j].edge.start_vertex, hyperplan[j].edge.end_vertex, temp1.Y) > temp1.X)
                                {
                                    Show = false;
                                }
                            }
                        }
                        if (Show)
                        {
                            final.perpendicular_bisector.Add(r_region.perpendicular_bisector[i]);
                        }
                    }
                    else
                    {
                        if ((temp1.X < 600 && temp1.X > 0) || (temp2.X < 600 && temp2.X > 0))
                        {
                            final.perpendicular_bisector.Add(r_region.perpendicular_bisector[i]);
                        }
                    }
                }
                final.hyperplane = hyperplan;
                return final;
            }
        }
        public static List<perpendicular> Sort_ListEdge(List<perpendicular> Edges)
        {
            Edges = Edges.OrderBy(A => A.edge.start_vertex.X).ThenBy(A => A.edge.start_vertex.Y).ThenBy(A => A.edge.end_vertex.X).ThenBy(A => A.edge.end_vertex.Y).ToList();
            return Edges;
        }
        //計算交點
        public static PointF GetIntersection(PointF lineFirstStar, PointF lineFirstEnd, PointF lineSecondStar, PointF lineSecondEnd)
        {
            float a = 0, b = 0;
            int state = 0;
            if (lineFirstStar.X != lineFirstEnd.X)//不平行y軸
            {
                a = calculate.slope(lineFirstStar, lineFirstEnd);
                state |= 1;
            }
            if (lineSecondStar.X != lineSecondEnd.X)//不平行y軸
            {
                b = calculate.slope(lineSecondStar, lineSecondEnd);
                state |= 2;
            }
            switch (state)
            {
                case 0: //L1與L2都平行Y軸
                    {
                        return new PointF(0, 0);
                    }
                case 1:  //L1存在斜率, L2平行Y軸
                    {
                        float x = lineSecondStar.X;
                        float y = (lineFirstStar.X - x) * (-a) + lineFirstStar.Y;
                        return new PointF(x, y);
                    }
                case 2: //L1 平行Y軸，L2存在斜率
                    {
                        float x = lineFirstStar.X;
                        float y = (lineSecondStar.X - x) * (-b) + lineSecondStar.Y;
                        return new PointF(x, y);
                    }
                case 3:  //L1，L2都存在斜率
                    {
                        if (a == b)
                        {
                            return new PointF(0, 0);
                        }
                        float x = (a * lineFirstStar.X - b * lineSecondStar.X - lineFirstStar.Y + lineSecondStar.Y) / (a - b);
                        float y = a * x - a * lineFirstStar.X + lineFirstStar.Y;
                        return new PointF(x, y);
                    }
            }
            return new PointF(0, 0);
        }

        private void save_Click(object sender, EventArgs e)
        {
            string path = Directory.GetCurrentDirectory() + "//OUT.txt";
            points = Sort_ListPointF(points);
            using (StreamWriter writetext = new StreamWriter(path))
            {
                for (int i = 0; i < points.Count; i++)
                {
                    string str = "P " + points[i].X + " " + points[i].Y;
                    writetext.WriteLine(str);
                }
                //show_edge= Sort_ListEdge(show_edge);
                //for (int i = 0; i < show_edge.Count; i++)
                //{
                //   // string str = "E " + show_edge[i].vertex1.X + " " + show_edge[i].vertex1.Y +" "+show_edge[i].vertex2.X + " " + show_edge[i].vertex2.Y;
                //   // writetext.WriteLine(str);
                //}
                MessageBox.Show("匯出成功");
            }
        }
        public static List<PointF> Sort_ListPointF(List<PointF> points)
        {
            return points.OrderBy(A => A.X).ThenBy(B => B.Y).ToList();
        }
        public static List<PointF> Sort_ListPointF(List<PointF> points, string position)
        {
            if (position == "L")//找到右半邊最左的的最下的convex
            {
                return points.OrderByDescending(A => A.X).ThenByDescending(B => B.Y).ToList();
            }
            else
            {//找到左半邊最右的的最下的convex
                return points.OrderBy(A => A.X).ThenByDescending(B => B.Y).ToList();
            }
        }
        public static List<PointF> Sort_ListPointF(PointF p1, PointF p2)
        {
            List<PointF> temp = new List<PointF>();
            temp.Add(p1);
            temp.Add(p2);
            return Sort_ListPointF(temp);
        }

        private void read_draw_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "請選擇輸出的文字檔案";
            //dialog.Filter = "所有檔案(*.txt)";
            dialog.Filter = "txt files|*.txt;*.ini;*.doc;*.docx";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.clear.PerformClick();
                Read_outfile(dialog.FileName);
            }
        }

        /* public void Draw_Edge(PointF p1, PointF p2)
         {
             g.DrawLine(pen, p1, p2);
             List<PointF>sort_point = Sort_ListPointF(p1,p2);
             pictureBox1.Image = bitmap;
         }*/
        public void Draw_Edge(PointF p1, PointF p2, Color color)
        {
            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bitmap);
            Pen pen = new Pen(color, pen_size);
            g.DrawLine(pen, p1, p2);
            pictureBox1.Image = bitmap;
            //List<PointF> sort_point = Sort_ListPointF(p1, p2);
        }
        public void Draw_region(region r, List<PointF> Cur_Points)
        {
            try
            {
                Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(pictureBox1.BackColor);//bitmap清空
                pictureBox1.Image = bitmap;//畫布清空

                for (int i = 0; i < Cur_Points.Count; i++)
                {
                    SolidBrush drawBrush = new SolidBrush(color_point);
                    g.FillEllipse(drawBrush, Cur_Points[i].X, Cur_Points[i].Y, draw_point_size, draw_point_size);
                    pictureBox1.Image = bitmap;
                }
                for (int i = 0; i < r.convex_point.Count; i++)
                {
                    Pen pen = new Pen(color_line, pen_size);
                    g.DrawLine(pen, r.convex_point[i], r.convex_point[(i + 1) % r.convex_point.Count()]);
                    pictureBox1.Image = bitmap;
                }
                for (int i = 0; i < r.perpendicular_bisector.Count; i++)
                {
                    Pen pen = new Pen(Color.Blue, pen_size);
                    PointF Temp1 = r.perpendicular_bisector[i].edge.start_vertex;
                    Temp1.X = Temp1.X + 2;
                    PointF Temp2 = r.perpendicular_bisector[i].edge.end_vertex;
                    Temp2.X = Temp2.X + 2;
                    g.DrawLine(pen, Temp1, Temp2);
                    pictureBox1.Image = bitmap;
                }
                if (r.hyperplane != null)
                {
                    for (int i = 0; i < r.hyperplane.Count; i++)
                    {
                        Pen pen = new Pen(Color.Red, pen_size);
                        PointF Temp1 = r.hyperplane[i].edge.start_vertex;
                        Temp1.X = Temp1.X + 2;
                        PointF Temp2 = r.hyperplane[i].edge.end_vertex;
                        Temp2.X = Temp2.X + 2;
                        g.DrawLine(pen, Temp1, Temp2);
                        pictureBox1.Image = bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                Draw_region(r,Cur_Points);
            }
            /*for (int i = 0; i < test.Count; i++)
            {
                Pen pen = new Pen(Color.Red, pen_size);
                g.DrawLine(pen, test[i].start_vertex, test[i].end_vertex);
                pictureBox1.Image = bitmap;
            }     */
        }
        private void button3_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            string path = Directory.GetCurrentDirectory() + "//OUT" + dt.ToString("yyyyMMddhhmmss") + ".txt";
            points = Sort_ListPointF(points);
            using (StreamWriter writetext = new StreamWriter(path))
            {
                string str = points.Count.ToString();
                writetext.WriteLine(str);
                for (int i = 0; i < points.Count; i++)
                {
                    str = points[i].X + " " + points[i].Y;
                    writetext.WriteLine(str);
                }
                //show_edge = Sort_ListEdge(show_edge);
                //for (int i = 0; i < show_edge.Count; i++)
                //{
                //    // string str = "E " + show_edge[i].vertex1.X + " " + show_edge[i].vertex1.Y +" "+show_edge[i].vertex2.X + " " + show_edge[i].vertex2.Y;
                //    // writetext.WriteLine(str);
                //}
                MessageBox.Show("匯出成功");
            }
        }

        private void nextdata(object sender, EventArgs e)
        {
            this.clear.PerformClick();
            if (t_exec != null)
            {
                t_exec.Abort();
                finish = true;
            }          
            auto.Set();
        }

        private void nextstep_Click(object sender, EventArgs e)
        {
            stepbystep.Set();
            if (finish)
            {
                finish = false;
                points = Sort_ListPointF(points);
                point_cnt = points.Count;
                t_exec = new Thread(() => exec(points));
                t_exec.Start();
            }
        }
        public void Draw_point(PointF point, Color color)
        {
            Bitmap bitmap = new Bitmap(pictureBox1.Image);
            Graphics g = Graphics.FromImage(bitmap);
            points.Add(point);
            list_box.Items.Add(point.X + "," + point.Y);
            SolidBrush drawBrush = new SolidBrush(color);
            g.FillEllipse(drawBrush, point.X, point.Y, draw_point_size, draw_point_size);
            pictureBox1.Image = bitmap;
        }
        public bool left_p1_pick(Edge e, PointF P1)//選擇要被替換調的點
        {
            if (calculate.cross(e.end_vertex, e.start_vertex, P1) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool right_p1_pick(Edge e, PointF P1)
        {//選擇要被替換調調的點
            if (calculate.cross(e.end_vertex, e.start_vertex, P1) > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }

}
