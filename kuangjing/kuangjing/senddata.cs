using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;


using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;


using System.Globalization;

using System.Configuration;
using MySql.Data.MySqlClient;
using CCWin;
namespace kuangjing
{
    public partial class senddata : CCSkinMain
    {
        public senddata(AddOneDelegate addone)
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.addOne = addone;
        }
        private AddOneDelegate addOne;
        MySqlConn msc = new MySqlConn();
        MySqlConn msc1 = new MySqlConn();
        string data = "";
        private void getFactory(object obj)
        {
            try
            {
                comboBox1.Items.Clear();

                string sql1 = "select * from work";

                MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                while (rd1.Read())
                {
                    comboBox1.Items.Add(rd1["workid"].ToString());
                }
                rd1.Close();
                msc.Close();
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.Text = comboBox1.Items[0].ToString();
                }
                sql1 = "select * from work where workid =" + comboBox1.Text + "";

                MySqlDataReader rd2 = msc.getDataFromTable(sql1);
                while (rd2.Read())
                {
                    label2.Text = rd2["workname"].ToString();
                }
                rd2.Close();
                msc.Close();



            }
            catch (SqlException se)
            {
                MessageBox.Show("数据库异常", "提示");
            }

        }
        private void senddata_Load(object sender, System.EventArgs e)
        {
            Thread fac_thread = new Thread(getFactory);
            fac_thread.Start();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sql1 = "select * from work where workid =" + comboBox1.Text + "";

            MySqlDataReader rd2 = msc.getDataFromTable(sql1);
            while (rd2.Read())
            {
                label2.Text = rd2["workname"].ToString();
            }
            rd2.Close();
            msc.Close();
        }
        string shebei = "";
        string senior = "";
        int changdu = 0;

        int s = 0;
        int ec = 0;
        int sc = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            shebei = " 01";
            int length = 12;//数据初始长度
            string ss = "";
            string bz = "";
            int elength = 0;
            string sss = "";
            int o;
            string sql1 = "SELECT count(*) FROM equipment where workid =" + comboBox1.Text + "";

            MySqlDataReader rd2 = msc.getDataFromTable(sql1);

            while (rd2.Read())
            {
                ss = rd2["count(*)"].ToString();

            }
            rd2.Close();
            msc.Close();


            try
            { 

            bz = Ten2Hex(ss);
            o = 0;
            if (bz.Length == 2)
            {
                shebei = shebei + " " + bz;
            }
            else
            {
                shebei = shebei + " 0" + bz;
            }
            elength = int.Parse(ss);
            string qqq = bz;
            length = length + elength * 9;

            sss = "";


            string sql2 = "SELECT * FROM equipment where workid =" + comboBox1.Text + "";

            MySqlDataReader rd = msc1.getDataFromTable(sql2);

            string temp1;
            while (rd.Read())
            {
                ss = rd["Etype"].ToString();
                shebei = shebei + " 0" + ss;
                sss = rd["Eid"].ToString();
                bz = Ten2Hex(sss);
                if (bz.Length == 2)
                {
                    shebei = shebei + " " + bz;
                }
                else
                {
                    shebei = shebei + " 0" + bz;
                }
                string ss1 = shuju(comboBox1.Text, sss,ss);
                bz = Ten2Hex(ss1);
                if (bz.Length == 2)
                {
                    shebei = shebei + " " + bz;
                }
                else
                {
                    shebei = shebei + " 0" + bz;
                }

                ss = rd["caiji"].ToString();
                if (ss == "")
                {
                    shebei = shebei + " 00 00 00";

                }
                else
                {
                    string s = "";
                    int i = 0;
                    while (ss[i] != '分')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }

                    s = "";
                    i++;
                    while (ss[i] != '秒')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }
                    s = "";
                    i++;
                    while (ss[i] != '毫')
                    {
                        s = s + ss[i];
                        i++;
                    }
                        int a1 = int.Parse(s);
                        a1 = a1 / 100;
                        s = a1.ToString();
                        bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }
                    ss = rd["shangchuan"].ToString();
                    if (ss == "")
                    {
                        shebei = shebei + " 00 00 00";

                    }
                    else
                    {
                        s = "";
                        i = 0;
                        while (ss[i] != '分')
                        {
                            s = s + ss[i];
                            i++;
                        }
                        bz = Ten2Hex(s);
                        if (bz.Length == 2)
                        {
                            shebei = shebei + " " + bz;
                        }
                        else
                        {
                            shebei = shebei + " 0" + bz;
                        }

                        s = "";
                        i++;
                        while (ss[i] != '秒')
                        {
                            s = s + ss[i];
                            i++;
                        }
                        bz = Ten2Hex(s);
                        if (bz.Length == 2)
                        {
                            shebei = shebei + " " + bz;
                        }
                        else
                        {
                            shebei = shebei + " 0" + bz;
                        }
                        s = "";
                        i++;
                        while (ss[i] != '毫')
                        {
                            s = s + ss[i];
                            i++;
                        }
                             a1 = int.Parse(s);
                            a1 = a1 / 100;
                            s = a1.ToString();
                            bz = Ten2Hex(s);
                        if (bz.Length == 2)
                        {
                            shebei = shebei + " " + bz;
                        }
                        else
                        {
                            shebei = shebei + " 0" + bz;
                        }
                    }

                }


            }

            rd.Close();
            msc1.Close();

            shebei = shebei + " 02";
            sql1 = "SELECT count(*) FROM dsensor where workid =" + comboBox1.Text + "";

            MySqlDataReader rd5 = msc.getDataFromTable(sql1);

            while (rd5.Read())
            {
                ss = rd5["count(*)"].ToString();

            }
            rd5.Close();
            msc.Close();


            bz = Ten2Hex(ss);
            o = 0;
            if (bz.Length == 2)
            {
                shebei = shebei + " " + bz;
            }
            else
            {
                shebei = shebei + " 0" + bz;
            }
            //length = length + 2;

            elength = int.Parse(ss);

            length = length + elength * 16;

            sss = "";


            sql2 = "SELECT equiptype.Etype,Eid,type.bzid,Sid,Smax,Smin,caiji,shangchuan FROM equiptype,dsensor,type where equiptype.Ename=dsensor.Ename and type.type= dsensor.Sname and workid = " + comboBox1.Text + " ORDER BY Eid asc";

            MySqlDataReader rd6 = msc1.getDataFromTable(sql2);

            while (rd6.Read())
            {
                ss = rd6["Etype"].ToString();

                shebei = shebei + " 0" + ss;
                sss = rd6["Eid"].ToString();
                bz = Ten2Hex(sss);
                if (bz.Length == 2)
                {
                    shebei = shebei + " " + bz;
                }
                else
                {
                    shebei = shebei + " 0" + bz;
                }
                ss = rd6["bzid"].ToString();
                shebei = shebei + " 0" + ss;
                sss = rd6["Sid"].ToString();
                bz = Ten2Hex(sss);
                if (bz.Length == 2)
                {
                    shebei = shebei + " " + bz;
                }
                else
                {
                    shebei = shebei + " 0" + bz;
                }
                sss = rd6["Smax"].ToString();
                if (sss[0] == '-')
                {
                    shebei = shebei + " 00";

                }
                else
                {
                    shebei = shebei + " 08";
                }
                bz = Ten2Hex(sss);
                if (bz.Length == 4)
                {
                    shebei = shebei + " " + bz[0] + bz[1] + " " + bz[2] + bz[3];
                }
                else if (bz.Length == 3)
                {
                    shebei = shebei + " 0" + bz[0] + " " + bz[1] + bz[2];
                }
                else if (bz.Length == 2)
                {
                    shebei = shebei + " 00 " + bz;
                }
                else if (bz.Length == 1)
                {
                    shebei = shebei + " 00 0" + bz;
                }
                sss = rd6["Smin"].ToString();
                if (sss[0] == '-')
                {
                    shebei = shebei + " 00";

                }
                else
                {
                    shebei = shebei + " 08";
                }
                bz = Ten2Hex(sss);
                if (bz.Length == 4)
                {
                    shebei = shebei + " " + bz[0] + bz[1] + " " + bz[2] + bz[3];
                }
                else if (bz.Length == 3)
                {
                    shebei = shebei + " 0" + bz[0] + " " + bz[1] + bz[2];
                }
                else if (bz.Length == 2)
                {
                    shebei = shebei + " 00 " + bz;
                }
                else if (bz.Length == 1)
                {
                    shebei = shebei + " 00 0" + bz;
                }

                ss = rd6["caiji"].ToString();
                if (ss == "")
                {
                    shebei = shebei + " 00 00 00";

                }
                else
                {
                    string s = "";
                    int i = 0;
                    while (ss[i] != '分')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }

                    s = "";
                    i++;
                    while (ss[i] != '秒')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }
                    s = "";
                    i++;
                    while (ss[i] != '毫')
                    {
                        s = s + ss[i];
                        i++;
                    }
                        int a1 = int.Parse(s);
                        a1 = a1 / 100;
                        s = a1.ToString();
                        bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }
                    ss = rd6["shangchuan"].ToString();
                    if (ss == "")
                    {
                        shebei = shebei + " 00 00 00";

                    }
                    else
                    {
                            //Log1("ss   " + ss+"    "+ss[1]);
                            s = "";
                            i = 0;
                            while (ss[i] != '分')
                            {
                                s = s + ss[i];
                                i++;

                            }
                            bz = Ten2Hex(s);
                            if (bz.Length == 2)
                            {
                                shebei = shebei + " " + bz;
                            }
                            else
                            {
                                shebei = shebei + " 0" + bz;
                            }

                            s = "";
                            i++;
                            while (ss[i] != '秒')
                            {
                                s = s + ss[i];
                                i++;
                            }
                            bz = Ten2Hex(s);
                            if (bz.Length == 2)
                            {
                                shebei = shebei + " " + bz;
                            }
                            else
                            {
                                shebei = shebei + " 0" + bz;
                            }
                            s = "";
                            i++;
                            while (ss[i] != '毫')
                            {
                                s = s + ss[i];
                                i++;
                            }
                            a1 = int.Parse(s);
                            a1 = a1 / 100;
                            s = a1.ToString();
                            bz = Ten2Hex(s);
                            if (bz.Length == 2)
                            {
                                shebei = shebei + " " + bz;
                            }
                            else
                            {
                                shebei = shebei + " 0" + bz;
                            }
                        }

                    }

                    //Log1("shebei    " + shebei);
            }
            rd6.Close();
            msc1.Close();
            if (Ten2Hex(comboBox1.Text).Length == 1)
            {
                data = "0" + Ten2Hex(comboBox1.Text);
            }
            else
            {
                data = Ten2Hex(comboBox1.Text);
            }
            data = data + " 0F ";


            string qq = Ten2Hex(length.ToString());

            if (qq.Length == 4)
            {
                data = data + qq[0] + qq[1] + " " + qq[3] + qq[4];
            }
            if (qq.Length == 3)
            {
                data = data + "0" + qq[0] + " " + qq[1] + qq[2];
            }
            if (qq.Length == 2)
            {
                data = data + "00" + " " + qq[0] + qq[1];
            }
            if (qq.Length == 1)
            {
                data = data + "00 0" + qq[0];
            }



            data = data + " " + "00";
            sql2 = "SELECT * FROM `work` where workid = " + comboBox1.Text + " ";
            MySqlDataReader rd7 = msc1.getDataFromTable(sql2);

            while (rd7.Read())
            {
                ss = rd7["caiji"].ToString();
                if (ss == "")
                {
                    data = data + " 00 00 00";

                }
                else
                {
                    string s = "";
                    int i = 0;
                    while (ss[i] != '分')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        data = data + " " + bz;
                    }
                    else
                    {
                        data = data + " 0" + bz;
                    }

                    s = "";
                    i++;
                    while (ss[i] != '秒')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        data = data + " " + bz;
                    }
                    else
                    {
                        data = data + " 0" + bz;
                    }
                    s = "";
                    i++;
                    while (ss[i] != '毫')
                    {
                        s = s + ss[i];
                        i++;
                    }
                        int a1 = int.Parse(s);
                        a1 = a1 / 100;
                        s = a1.ToString();
                        bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        data = data + " " + bz;
                    }
                    else
                    {
                        data = data + " 0" + bz;
                    }

                }
                ss = rd7["shangchuan"].ToString();
                if (ss == "")
                {
                    data = data + " 00 00 00";

                }
                else
                {
                    string s = "";
                    string temp = "";
                    int i = 0;
                    while (ss[i] != '分')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        data = data + " " + bz;
                        temp = temp + " 0" + bz;
                    }
                    else
                    {
                        data = data + " 0" + bz;
                        temp = temp + " 0" + bz;
                    }

                    s = "";
                    i++;
                    while (ss[i] != '秒')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        data = data + " " + bz;
                        temp = temp + " 0" + bz;
                    }
                    else
                    {
                        data = data + " 0" + bz;
                        temp = temp + " 0" + bz;
                    }
                    s = "";
                    i++;
                    while (ss[i] != '毫')
                    {
                        s = s + ss[i];
                        i++;
                    }
                       int a1 = int.Parse(s);
                        a1 = a1 / 100;
                        s = a1.ToString();
                        bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        data = data + " " + bz;
                        temp = temp + bz;
                    }
                    else
                    {
                        data = data + " 0" + bz;
                        temp = temp + bz;
                    }

                }
            }
            rd6.Close();
            msc1.Close();


            if (qqq.Length == 1)
            {
                qqq = "0" + qqq;
            }
                //else if (qqq.Length == 2)
                //{
                //    qqq = "00 " + qqq;
                //}
                //else if (qqq.Length == 3)
                //{
                //    qqq = "0"+qqq[0]+" "+qqq[1]+qqq[2];
                //}
                //else if (qqq.Length == 3)
                //{
                //    qqq =  qqq[0]+qqq[1] + " " + qqq[2] + qqq[3];
                //}
                
                data = data + " " + qqq + shebei;
                Log1("data  " + data);

                MessageBox.Show("发送成功");
            this.addOne.Invoke(data.ToString());
        }
        catch(Exception eee)
            {
                Log1("错误" + eee);
            }


    }
        private Mutex file_mutex = new Mutex();//文件互斥锁
        void Log1(string str)    // 记录服务启动  
        {
            file_mutex.WaitOne();
            {
                string info = string.Format("{0}-{1}", DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"), str);
                string path = "C://Mqtt//jieshpou.txt";//"C://Mqtt//MyTestLog.txt"
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(info);
                    //关闭
                    sw.Close();
                }
            }
            file_mutex.ReleaseMutex();

        }
        public static string Ten2Hex(string ten)//十进制转化十六进制函数
        {
            ulong tenValue = Convert.ToUInt64(ten);
            ulong divValue, resValue;
            string hex = "";
            do
            {
                //divValue = (ulong)Math.Floor(tenValue / 16);

                divValue = (ulong)Math.Floor((decimal)(tenValue / 16));

                resValue = tenValue % 16;
                hex = tenValue2Char(resValue) + hex;
                tenValue = divValue;
            }
            while (tenValue >= 16);
            if (tenValue != 0)
                hex = tenValue2Char(tenValue) + hex;
            return hex;
        }
        string shuju(string s,string sse,string ss)
        {

            string sql66 = "SELECT * from equiptype where Etype = "+ ss+"";
            string hh="";
            MySqlConn msc1 = new MySqlConn();
            MySqlDataReader rd31 = msc1.getDataFromTable(sql66);

            while (rd31.Read())
            {
                hh = rd31["Ename"].ToString();

            }
            rd31.Close();
            msc1.Close();
            string sql19 = "SELECT count(*) FROM dsensor where workid =" +s + " and dsensor.Ename ='" + hh + "' and dsensor.Eid =" + sse + "";
            string aaa = " 01";
            MySqlConn msc2 = new MySqlConn();
            MySqlDataReader rd3 = msc2.getDataFromTable(sql19);

            while (rd3.Read())
            {
                aaa = rd3["count(*)"].ToString();

            }
            rd3.Close();
            msc2.Close();
            return aaa;
        }
         static string tenValue2Char(ulong ten)
        {
            switch (ten)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return ten.ToString();
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
                default:
                    return "";
            }
        }
    }
}
