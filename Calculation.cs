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
    public class RailgunCalculator
    {
        public const string STRC_MLGJT = "未来道具研究所 2021 All Rights Reserved";
        public const string STRC_VERSN = "\r\n Railgun Coilgun Calculation Utility V1.0 \r\n";
        public const string STRC_GITHB = "\r\n Home: https://kmakise.cn";
        public const string STRC_HOMEL = "\r\n Github: https://github.com/kmakise";

        //弹丸质量
        public double BLT_rad = 0;    //直径
        public double BLT_len = 0;    //长度

        public double BLT_m = 0;    //质量
        public double BLT_s = 0;    //截面积


        //功率动能
        public double PA_bltm = 0;    //弹丸质量 
        public double PA_accd = 0;    //加速距离 
        public double PA_aimv = 0;    //目标速度

        public double PA_time = 0;    //加速时间
        public double PA_accv = 0;    //加速度
        public double PA_watt = 0;    //平均功率
        public double PA_ekval = 0;    //动能

        //电流电压
        public double VC_avpw = 0;    //平均功率 
        public double VC_pwvt = 0;    //电源电压 
        public double VC_capvt = 0;    //逆变电压 
        public double VC_acctim = 0;    //加速时间 
        public double VC_nek = 0;    //转换效率 
        public double VC_ntf = 0;    //逆变效率

        public double VC_dischargeCur = 0;    //放电电流
        public double VC_powerCur = 0;    //直供电流  
        public double VC_heatout = 0;    //热耗功率
        public double VC_btmah = 0;    //电池功耗

        //电容器
        public double CPT_CapU = 0;    //电压
        public double CPT_CapI = 0;    //电流
        public double CPT_CapT = 0;    //时间

        public double CPT_energy = 0;    //储能量
        public double CPT_capval = 0;    //电容量

        //动能比计算
        public double EKP_outek = 0;    //出口动能
        public double EKP_sblt = 0;    //出口动能

        public double EKP_ekpst = 0;    //动能比


        //电感量计算
        public double COIL_Rad = 0;//半径
        public double COIL_Len = 0;//长度
        public double COIL_Thi = 0;//厚度
        public double COIL_Wrd = 0;//线直径
        public double COIL_Rpt = 0;//电阻率

        public double COIL_Cln = 0;//匝数
        public double COIL_Wln = 0;//线长
        public double COIL_Res = 0;//内阻
        public double COIL_Lin = 0;//电感

        //长冈系数
        private double[,] COIL_CGK = new double[,]{
            { 0.1, 0.96 },
            { 0.2, 0.92 },
            { 0.3, 0.88 },
            { 0.4, 0.85 },
            { 0.6, 0.79 },
            { 0.8, 0.74 },
            { 1.0, 0.69 },
            { 1.5, 0.60 },
            { 2.0, 0.52 },
            { 3.0, 0.43 },
            { 4.0, 0.37 },
            { 5.0, 0.32 },
            { 10 , 0.20 },
            { 20 , 0.12 },
        };
        //1/t系数
        private double[,] COIL_1PT = new double[,] {
            {1 ,0   },
            {5 ,0.23},
            {10,0.28},
            {20,0.31},
            {30,0.32},
        };



        public RailgunCalculator()
        {
            
        }

        //弹丸质量计算  rad直径 len长度
        public void BLT_CalcWeightData()
        {
            this.BLT_s = ((this.BLT_rad / 10) / 2) * ((this.BLT_rad / 10) / 2) * 3.1415926;
            this.BLT_m = (this.BLT_s * this.BLT_len / 10) * 7.8;//7.8g/cm3
        }

        // 功率动能计算  
        public void PA_CalcAccData()
        {

            this.PA_time = (this.PA_accd / 1000) / (this.PA_aimv / 2);         //时间 = 路程/平均速度
            this.PA_accv = this.PA_aimv / this.PA_time;                     //加速度 = 速度变化/时间
            this.PA_ekval = (this.PA_bltm / 1000) * this.PA_aimv * this.PA_aimv / 2;   //速度 :m/s  质量kg 动能定理 E=mv²/2  p = E/t  1W=1J/s
            this.PA_watt = this.PA_ekval / this.PA_time;                 //功率 = (质量x速度平方) / 2 / t
        }

        //电流电压计算 
        public void VC_CalcCurrentData()
        {
            

            this.VC_dischargeCur = (this.VC_avpw / this.VC_capvt) / (this.VC_nek / 100);// I = (P/U)/η
            this.VC_powerCur = (this.VC_avpw / this.VC_pwvt) / (this.VC_nek / 100) / (this.VC_ntf / 100);
            this.VC_heatout = this.VC_powerCur - (this.VC_avpw / this.VC_pwvt);
            this.VC_btmah = this.VC_powerCur * (this.VC_acctim / 1000) / 3600 * 1000;

        }
        //电容计算 CapU 电压 CapI电流 CapT时间
        public void CPT_CalcCapData()
        {
            this.CPT_energy = this.CPT_CapU * this.CPT_CapI * this.CPT_CapT / 1000;     // w = uit
            this.CPT_capval = 2 * this.CPT_energy / (this.CPT_CapU * this.CPT_CapU) * 1000000; //Ec=0.5CU² C=2Ec/U²
        }

        // 动能比计算 
        public void EKP_CalcOEkData()
        {
            this.EKP_ekpst = (1.0 / this.EKP_sblt) * this.EKP_outek;
        }

        //分段直线方程查表
        private double chatLinerTf(double[,] chat,int num,double input)
        {
            int index = 0;
            double x, y, x1, x2, y1, y2;
            double k, b;

            index = 0xFF;
            //find adc val from table
            for (int i = 0; i < (num - 1); i++)
            {
                if (chat[i,0] <= input && chat[i + 1,0] > input)
                {
                    index = i;
                    break;
                }
            }

            if (index == 0xFF)
            {
                return 0;
            }

            x1 = chat[index     ,0];
            x2 = chat[index + 1 ,0];
            y1 = chat[index     ,1];
            y2 = chat[index + 1 ,1];

            k = (y2 - y1) / (x2 - x1);
            b = y1 - k * x1;

            x = input;
            y = k * x + b;

            return y;
        }

        //电感计算
        public void COIL_CalcLvalData()
        {
            this.COIL_Cln = (int)(this.COIL_Len * this.COIL_Thi * Math.PI / 4 / this.COIL_Wrd);//匝数 = 长度 x 厚度x π/ 4 / 线径
            this.COIL_Wln = (this.COIL_Rad - this.COIL_Thi / 2) * 2 * Math.PI * COIL_Cln / 1000;//长度 = 2* 平均半径 * pi * n 
            this.COIL_Res = this.COIL_Rpt / ((this.COIL_Wrd / 2) * (this.COIL_Wrd / 2) * Math.PI) * this.COIL_Wln * 1000;//内阻 = 电阻率 / 截面积 * 长度

            //L = ( 4pi * 平均半径 * 总匝数^2 / 长度 ) * （pi * 平均半径 * 长冈系数 - 线圈厚度 * （0.693 + 1/t系数）） x 10^-7 H

            double avrad = ((this.COIL_Rad - this.COIL_Thi / 2) / 1000);        //平均半径
            double lenm = (this.COIL_Len / 1000);                               //长度 m
            double thim = (this.COIL_Thi / 1000);                               //厚度 m

            double x1 = (4 * Math.PI * avrad * Math.Pow(this.COIL_Cln, 2)) / lenm;      //( 4pi * 平均半径 * 总匝数^2 / 长度 )                  
            double x2 = Math.PI * avrad * chatLinerTf(COIL_CGK, 14, 2 * avrad / lenm);  //pi * 平均半径 * 长冈系数
            double x3 = thim * (0.693 + chatLinerTf(COIL_1PT, 5, lenm / thim));         //线圈厚度 * （0.693 + 1/t系数）                                          

            this.COIL_Lin =  x1 * (x2 - x3) * Math.Pow(10,-1);
        }
    }

}

