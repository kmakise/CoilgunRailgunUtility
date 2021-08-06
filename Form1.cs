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
    public partial class FormMain : Form
    {
        private static bool IsDrag = false;
        private int enterX;
        private int enterY;

        private float X;//窗体宽度
        private float Y;//窗体高度

        public RailgunCalculator railgun = new RailgunCalculator();

        public FormMain()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;//设置该属性 为false

            OutputClear();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.Resize += new EventHandler(Form1_ResizeEnd);//窗体调整大小时引发事件
            X = this.Width;//获取窗体的宽度
            Y = this.Height;//获取窗体的高度
            setTag(this);//调用方法

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
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            float newx = (this.Width) / X; //窗体宽度缩放比例
            float newy = this.Height / Y;//窗体高度缩放比例
            setControls(newx, newy, this);//随窗体改变控件大小
            //this.Text = this.Width.ToString() + " " + this.Height.ToString();//窗体标题栏文本
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
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            IsDrag = true;
            enterX = e.Location.X;
            enterY = e.Location.Y;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            IsDrag = false;
            enterX = 0;
            enterY = 0;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDrag)
            {
                Left += e.Location.X - enterX;
                Top += e.Location.Y - enterY;
            }
        }

        /****************************************************************************************************************************/

        //弹丸质量计算  rad直径 len长度
        private void updateBLTdata()
        {
            textBoxblts.Text = railgun.BLT_s.ToString();
            textBoxbltm.Text = railgun.BLT_m.ToString();

            //输出到功率动能
            if (checkBoxMP.Checked == true)
            {
                textBoxWeight.Text = textBoxbltm.Text;
            }

            textBoxOutPut.Text += "\r\n";
            textBoxOutPut.Text += "\r\n" + "弹丸质量  : " + textBoxbltm.Text + " g";
            textBoxOutPut.Text += "\r\n" + "弹丸截面积: " + textBoxbltm.Text + " cm²";
        }

        // 功率动能计算 bltm弹丸质量 accd加速距离 aimv目标速度
        private void updatePAdata()
        {
            double time = 0;

            time = railgun.PA_time;

            time *= 1000;
            textBoxAccTime.Text     = time.ToString();                  //时间显示 ms
            textBoxAccVal.Text      = railgun.PA_accv.ToString();       //加速度
            textBoxAccPower.Text    = railgun.PA_watt.ToString();       //加速功率
            textBoxOutPower.Text    = railgun.PA_ekval.ToString();      //出口动能

            //输出到电流电压
            if (checkBoxPV.Checked == true)
            {
                textBoxAvWt.Text = textBoxAccPower.Text;
                textBoxAccTim.Text = textBoxAccTime.Text;
            }
            //输出到动能比计算
            if (checkBoxEK.Checked)
            {
                textBoxOutEk.Text = textBoxOutPower.Text;
                textBoxSblt.Text = textBoxblts.Text;
            }
            textBoxOutPut.Text += "\r\n";
            textBoxOutPut.Text += "\r\n" + "加速度时间: " + textBoxAccTime.Text + " ms";
            textBoxOutPut.Text += "\r\n" + "加速度     : " + textBoxAccVal.Text + " m/s²";
            textBoxOutPut.Text += "\r\n" + "平均功率  : " + textBoxAccPower.Text + " W";
            textBoxOutPut.Text += "\r\n" + "出口动能  : " + textBoxOutPower.Text + " J";

        }

        //电流电压计算
        private void updateVCdata()
        {
            textBoxDischargeCurrent.Text    = railgun.VC_dischargeCur.ToString();
            textBoxPowerCurrent.Text        = railgun.VC_powerCur.ToString();
            textBoxHeatOut.Text             = railgun.VC_heatout.ToString();
            textBoxBtmAh.Text               = railgun.VC_btmah.ToString();

            //输出到电容
            if (checkBoxPC.Checked == true)
            {
                textBoxCapVtg.Text = railgun.VC_capvt.ToString();
                textBoxDisCur.Text = textBoxDischargeCurrent.Text;
                textBoxAccSpd.Text = railgun.VC_acctim.ToString();
            }
            textBoxOutPut.Text += "\r\n";
            textBoxOutPut.Text += "\r\n" + "放电电流  : " + textBoxDischargeCurrent.Text + " A";
            textBoxOutPut.Text += "\r\n" + "直供电流  : " + textBoxPowerCurrent.Text + " A";
            textBoxOutPut.Text += "\r\n" + "热耗功率  : " + textBoxHeatOut.Text + " J";
            textBoxOutPut.Text += "\r\n" + "电能消耗  : " + textBoxBtmAh.Text + " mAh";
        }
        //电容计算
        private void updateCPTdata()
        {
            textBoxSumEng.Text  = railgun.CPT_energy.ToString();
            textBoxNeedCap.Text = railgun.CPT_capval.ToString();

            textBoxOutPut.Text += "\r\n";
            textBoxOutPut.Text += "\r\n" + "电容储能  : " + textBoxSumEng.Text + " J";
            textBoxOutPut.Text += "\r\n" + "所需容量  : " + textBoxNeedCap.Text + " uF";
        }

        // 动能比计算 outek出口动能 sblt弹丸截面积
        private void updateEKPdata()
        {
            textBoxEKPS.Text = railgun.EKP_ekpst.ToString();

            textBoxOutPut.Text += "\r\n";
            textBoxOutPut.Text += "\r\n" + "动能比  : " + textBoxEKPS.Text + " J/cm²";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBoxOutPut.Text += "\r\n\r\n[开始计算][通用数值]          " + 
                DateTime.Now.ToString() + 
                "\r\n==========================================";

            //弹丸质量计算
            if (!double.TryParse(textBoxRad.Text, out railgun.BLT_rad) ||
                !double.TryParse(textBoxLen.Text, out railgun.BLT_len)
                )
            {
                MessageBox.Show("弹丸质量计算:必须为数字！请从新输入！");
            }
            else
            {
                railgun.BLT_CalcWeightData();
                updateBLTdata();
            }


            //功率动能计算
            if (!double.TryParse(textBoxWeight.Text     , out railgun.PA_bltm) ||
                !double.TryParse(textBoxDistance.Text   , out railgun.PA_accd) ||
                !double.TryParse(textBoxAimSpeed.Text   , out railgun.PA_aimv) 
                )
            {
                MessageBox.Show("功率动能计算:必须为数字！请从新输入！");
            }
            else
            {
                railgun.PA_CalcAccData();
                updatePAdata();
            }


            //电压电流计算
            if (!double.TryParse(textBoxAvWt.Text,      out railgun.VC_avpw     ) ||
                !double.TryParse(textBoxPowerVol.Text,  out railgun.VC_pwvt     ) ||
                !double.TryParse(textBoxCtVol.Text,     out railgun.VC_capvt    ) ||
                !double.TryParse(textBoxAccTim.Text,    out railgun.VC_acctim   ) ||
                !double.TryParse(textBoxToEkEy.Text,    out railgun.VC_nek      ) ||
                !double.TryParse(textBoxToTfEy.Text,    out railgun.VC_ntf      )
                )
            {
                MessageBox.Show("电压电流计算:必须为数字！请从新输入！");
            }
            else
            {
                railgun.VC_CalcCurrentData();
                updateVCdata();
            }

            //电容计算
            if (!double.TryParse(textBoxCapVtg.Text, out railgun.CPT_CapU) ||
                !double.TryParse(textBoxDisCur.Text, out railgun.CPT_CapI) ||
                !double.TryParse(textBoxAccSpd.Text, out railgun.CPT_CapT) 
                )
            {
                MessageBox.Show("电容计算:必须为数字！请从新输入！");
            }
            else
            {
                railgun.CPT_CalcCapData();
                updateCPTdata();
            }

            //输出动能比计算
            if (!double.TryParse(textBoxOutEk.Text, out railgun.EKP_outek) ||
                !double.TryParse(textBoxSblt.Text,  out railgun.EKP_sblt) 
                )
            {
                MessageBox.Show("动能比计算:必须为数字！请从新输入！");
            }
            else
            {
                railgun.EKP_CalcOEkData();
                updateEKPdata();
            }

            textBoxOutPut.Text += "\r\n\r\n[计算结束][通用数值]          " + 
                DateTime.Now.ToString() + 
                "\r\n==========================================";
        }


        //功率动能计算输入变更
        private void checkBoxMP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMP.Checked)
            {
                textBoxWeight.ReadOnly = true;
                textBoxWeight.BackColor = Color.FromArgb(140, 140, 140);
            }
            else
            {
                textBoxWeight.ReadOnly = false;
                textBoxWeight.BackColor = Color.FromArgb(224, 224, 224);
            }
        }

        //电压电流输入变更
        private void checkBoxPV_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxPV.Checked)
            {
                textBoxAvWt.ReadOnly = true;
                textBoxAvWt.BackColor = Color.FromArgb(140, 140, 140);
                textBoxAccTim.ReadOnly = true;
                textBoxAccTim.BackColor = Color.FromArgb(140, 140, 140);
            }
            else
            {
                textBoxAvWt.ReadOnly = false;
                textBoxAvWt.BackColor = Color.FromArgb(224, 224, 224);
                textBoxAccTim.ReadOnly = false;
                textBoxAccTim.BackColor = Color.FromArgb(224, 224, 224);
            }
        }


        //电容器计算
        private void checkBoxPC_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxPC.Checked)
            {
                textBoxCapVtg.ReadOnly = true;
                textBoxCapVtg.BackColor = Color.FromArgb(140, 140, 140);
                textBoxDisCur.ReadOnly = true;
                textBoxDisCur.BackColor = Color.FromArgb(140, 140, 140);
                textBoxAccSpd.ReadOnly = true;
                textBoxAccSpd.BackColor = Color.FromArgb(140, 140, 140);
            }
            else
            {
                textBoxCapVtg.ReadOnly = false;
                textBoxCapVtg.BackColor = Color.FromArgb(224, 224, 224);
                textBoxDisCur.ReadOnly = false;
                textBoxDisCur.BackColor = Color.FromArgb(224, 224, 224);
                textBoxAccSpd.ReadOnly = false;
                textBoxAccSpd.BackColor = Color.FromArgb(224, 224, 224);
            }
        }
        //动能比计算
        private void checkBoEK_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEK.Checked)
            {
                textBoxOutEk.ReadOnly = true;
                textBoxOutEk.BackColor = Color.FromArgb(140, 140, 140);
                textBoxSblt.ReadOnly = true;
                textBoxSblt.BackColor = Color.FromArgb(140, 140, 140);
            }
            else
            {
                textBoxOutEk.ReadOnly = false;
                textBoxOutEk.BackColor = Color.FromArgb(224, 224, 224);
                textBoxSblt.ReadOnly = false;
                textBoxSblt.BackColor = Color.FromArgb(224, 224, 224);
            }

        }

        private void OutputClear()
        {
            textBoxOutPut.Text = "";

            textBoxOutPut.Text += RailgunCalculator.STRC_MLGJT;

            textBoxOutPut.Text += RailgunCalculator.STRC_VERSN;
            textBoxOutPut.Text += RailgunCalculator.STRC_HOMEL;
            textBoxOutPut.Text += RailgunCalculator.STRC_GITHB;
            textBoxOutPut.Text += "\r\n \r\n [就绪]";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OutputClear();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(textBoxOutPut.Text);

            textBoxOutPut.Text += "\r\n内容已经复制到剪贴板 ---- " + DateTime.Now.ToString();
        }


        private void textBoxOutPut_TextChanged(object sender, EventArgs e)
        {
            textBoxOutPut.Focus();//获取焦点
            textBoxOutPut.Select(textBoxOutPut.TextLength, 0);//光标定位到文本最后
            textBoxOutPut.ScrollToCaret();//滚动到光标处
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://kmakise.cn");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/kmakise");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://space.bilibili.com/22908638");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FormWaveLs formWaveLs = new FormWaveLs();
            formWaveLs.Show();



        }

        /****************************************************************************************************************************/


        private void updateLindata()
        {
            textBoxCOIL_Cln.Text = railgun.COIL_Cln.ToString();
            textBoxCOIL_Wln.Text = railgun.COIL_Wln.ToString();
            textBoxCOIL_Res.Text = railgun.COIL_Res.ToString();
            textBoxCOIL_Lin.Text = railgun.COIL_Lin.ToString();

            textBoxOutPut.Text += "\r\n";
            textBoxOutPut.Text += "\r\n" + "计算匝数  : " + textBoxCOIL_Cln.Text + " N";
            textBoxOutPut.Text += "\r\n" + "导线长度  : " + textBoxCOIL_Wln.Text + " m";
            textBoxOutPut.Text += "\r\n" + "线圈内阻  : " + textBoxCOIL_Res.Text + " mΩ";
            textBoxOutPut.Text += "\r\n" + "线圈电感  : " + textBoxCOIL_Lin.Text + " uH";


        }

        //开始计算 
        private void button8_Click(object sender, EventArgs e)
        {
            textBoxOutPut.Text += "\r\n\r\n[开始计算][磁阻加速器][线圈参数]          " +
                DateTime.Now.ToString() +
                "\r\n==========================================";

            //电感计算
            if (!double.TryParse(textBoxCOIL_Rad.Text, out railgun.COIL_Rad) ||
                !double.TryParse(textBoxCOIL_Len.Text, out railgun.COIL_Len) ||
                !double.TryParse(textBoxCOIL_Thi.Text, out railgun.COIL_Thi) ||
                !double.TryParse(textBoxCOIL_Wrd.Text, out railgun.COIL_Wrd) ||
                !double.TryParse(textBoxCOIL_Rpt.Text, out railgun.COIL_Rpt)
                )
            {
                MessageBox.Show("电感计算:必须为数字！请从新输入！");
            }
            else
            {
                railgun.COIL_CalcLvalData();
                updateLindata();
            }

            textBoxOutPut.Text += "\r\n\r\n[计算结束][磁阻加速器][线圈参数]          " +
                DateTime.Now.ToString() +
                "\r\n==========================================";
        }

    }
}
