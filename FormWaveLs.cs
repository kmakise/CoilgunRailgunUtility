using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RailgunCalcWin
{
    public partial class FormWaveLs : Form
    {
        private static bool IsDrag = false;
        private int enterX;
        private int enterY;

        private float X;//窗体宽度
        private float Y;//窗体高度


        public FormWaveLs()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false
        }

        private void setControls(float newx, float newy, Control cons)
        {
            //遍历窗体中的控件，重新设置控件的值
            foreach (Control con in cons.Controls)
            {
                string[] mytag = con.Tag.ToString().Split(new char[] { ':' });//获取控件的Tag属性值，并分割后存储字符串数组
                float a = Convert.ToSingle(mytag[0]) * newx;//根据窗体缩放比例确定控件的值，宽度
                con.Width = (int)a;//宽度
                a = Convert.ToSingle(mytag[1]) * newy;//高度
                con.Height = (int)(a);
                a = Convert.ToSingle(mytag[2]) * newx;//左边距离
                con.Left = (int)(a);
                a = Convert.ToSingle(mytag[3]) * newy;//上边缘距离
                con.Top = (int)(a);
                Single currentSize = Convert.ToSingle(mytag[4]) * (newx + newy) / 2;//字体大小
                con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);
                }
            }
            DrawWaveForm();
        }

        private void setTag(Control cons)   //遍历修改窗口中空间的大小
        {
            //遍历窗体中的控件
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                {
                    setTag(con);
                }
            }
        }

        private void FormWaveLs_Load(object sender, EventArgs e)
        {
            this.Resize += new EventHandler(FormWaveLs_ResizeEnd);//窗体调整大小时引发事件
            X = this.Width;//获取窗体的宽度
            Y = this.Height;//获取窗体的高度
            setTag(this);//调用方法
        }


        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            IsDrag = true;
            enterX = e.Location.X;
            enterY = e.Location.Y;
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            IsDrag = false;
            enterX = 0;
            enterY = 0;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDrag)
            {
                Left += e.Location.X - enterX;
                Top += e.Location.Y - enterY;
            }
        }

        private void FormWaveLs_ResizeEnd(object sender, EventArgs e)
        {
            float newx = (this.Width) / X; //窗体宽度缩放比例
            float newy = this.Height / Y;//窗体高度缩放比例
            setControls(newx, newy, this);//随窗体改变控件大小
        }

        private void DrawWaveForm()
        {
            WaveForm wf = new WaveForm(pictureBox1);

            wf.DrawGridAxis();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            DrawWaveForm();
        }
    }

    public class WaveForm
    {
        //绘制区间
        public int Width;
        public int Height;
        public Graphics gphc;


        //坐标系偏移
        public int xOffset;
        public int yOffset;

        //坐标基准位置
        public int yBase; 
        public int xBase; 

        //坐标栅格大小
        public int yGrid;
        public int xGrid;

        //坐标范围
        public int yMax;
        public int xMax;

        public WaveForm(PictureBox pb)
        {
            this.gphc = pb.CreateGraphics();
            this.Width = pb.Size.Width;
            this.Height = pb.Size.Height;
            this.gphc.Clear(Color.Black);

        }

        public void waveFormDramLine(Pen pen,int x0,int y0,int x1,int y1)
        {
            this.gphc.DrawLine(
                pen,
                x0 + this.xOffset,
                this.Height - y0 - this.yOffset,
                x1 + xOffset,
                this.Height - y1 - this.yOffset
                );
        }

        public void waveFormDramString(string s,Font f,Brush b, int x, int y)
        {
            this.gphc.DrawString(s,f,b,x + this.xOffset, this.Height - y - this.yOffset);
        }


        public void SineWaveTest(Color color,int phase,int k,int ofst,int step)
        {
            Pen p3 = new Pen(color, 2);//画笔
            int xpoint = phase;

            int[] Sine12bit = new int[32]{
            2048,2460,2856,3218,3532,3786,3969,4072,
            4093,4031,3887,3668,3382,3042,2661,2255,
            1841,1435,1054,714 ,428 ,209 ,65  ,   3,
            24  ,127 ,310 ,564 ,878 ,1240,1636,2048
            };

            for (int i = 0; i < 300; i++)
            {
                //Sine12bit
                xpoint += step;
                
                waveFormDramLine(p3, xpoint, Sine12bit[i % 31] / k + ofst, xpoint + step, Sine12bit[i % 31 + 1] / k + ofst);

            }
        }

        public void DrawGridAxis()
        {

            //Font font = new Font("微软雅黑", 20);//字体

            //栅格大小
            this.xGrid = 20;
            this.yGrid = 20;

            this.xOffset = 10;
            this.yOffset = 10;

            //坐标范围
            this.xMax = ((this.Width - this.xOffset - 5) / xGrid) * xGrid;
            this.yMax = ((this.Height - this.yOffset - 5) / yGrid) * yGrid;


            //绘制网格
            int xstep = 0;
            int ystep = 0;

            Pen p1 = new Pen(Color.FromArgb(20, 20, 20), 1);//画笔

            for (int i = 0; xstep <= this.xMax; i++)
            {
                waveFormDramLine(p1, xstep, 0, xstep, this.yMax);
                xstep += xGrid;
            }
            for (int i = 0; ystep <= this.yMax; i++)
            {
                waveFormDramLine(p1, 0, ystep, this.xMax, ystep);
                ystep += yGrid;
            }

            //绘制坐标轴 幅度正负半轴 5:1
            this.yBase = ((this.Height / this.yGrid) / 6) * this.xGrid;
            this.xBase = this.xGrid;

            Pen Axis = new Pen(Color.FromArgb(180,180,180), 2);//画笔
            waveFormDramLine(Axis, 0, this.yBase, this.xMax, this.yBase);
            waveFormDramLine(Axis, this.xBase, 0, this.xBase, this.yMax);


            //添加字符串信息
            Font font = new Font("微软雅黑",10);
            waveFormDramString("未来道具研究所 2021 All Rights Reserved", font, Brushes.Yellow,this.xBase + 5, 20);


            SineWaveTest(Color.Blue  ,-100, 13,20,20);
            SineWaveTest(Color.Yellow, -50, 13,20,20);

        }
    }
}
