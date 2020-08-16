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
using Coldairarrow.Util.Sockets;
using MyTest;
using System.Net.Sockets;
using System.Net;



using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;


using System.Globalization;

using System.Configuration;
using MySql.Data.MySqlClient;
using CCWin;

namespace kuangjing
{
    public partial class ip : CCSkinMain
    {
        DataTable dtInfo = new DataTable();
        MySqlConn msc = new MySqlConn();
        public int change = 0;
        public ip(AddOneDelegate addone)
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.addOne = addone;

        }

        public ip()
        {
        }

        private string counter = "0";
        string[] sendip1 = new string[12];
        string sendip = "";

        private AddOneDelegate addOne;
        private int msgBoxNum = 0;//MessageBox编号，每弹出一个MessageBox自增1，大于2000变为0


        private void ShowBox(object obj)
        {
            string message = (string)obj;
            if (msgBoxNum >= 2000)
            {
                msgBoxNum = 0;
            }
            MyMessageBox MymsgBox = new MyMessageBox(message, (msgBoxNum++).ToString());
            MymsgBox.ShowBox();
        }

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
                string sql = "select workname from work where workid =" + "" + comboBox1.Text + "";

                MySqlDataReader rd = msc.getDataFromTable(sql);
                while (rd.Read())
                {
                    skinLabel4.Text = rd["workname"].ToString();
                }
                rd.Close();
                msc.Close();
                //string sql2 = "select * from ip where workid =" + "" + comboBox1.Text + "";

                //MySqlDataReader rd2 = msc.getDataFromTable(sql2);
                //while (rd2.Read())
                //{
                //    Label2.Text = rd2["ip"].ToString();
                //    Label3.Text = rd2["duankou"].ToString();
                //}
                //rd2.Close();
                //msc.Close();




            }
            catch (SqlException se)
            {
                MessageBox.Show("数据库异常", "提示");
            }

        }
        private void company_Load(object sender, EventArgs e)
        {
            Label2.Text = "";
            try
            {

                Thread fac_thread = new Thread(getFactory);
                fac_thread.Start();

            }
            catch (SqlException se)
            {

            }
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (skinTextBox1.Text.Equals(""))
            {
                //MessageBox.Show("请输入原密码", "提示");
                new Thread(new ParameterizedThreadStart(ShowBox)).Start("请输入新ip");
            }
            else if (skinTextBox2.Text.Equals(""))
            {
                //MessageBox.Show("请输入原密码", "提示");
                new Thread(new ParameterizedThreadStart(ShowBox)).Start("请输入新端口");
            }
            else
            {
                try
                {
                    //MessageBox.Show("====33333=" + old_passwd);

                    string sql="";

                    if(Label2.Text!="")
                    {
                        sql = "update ip set ip='" + skinTextBox1.Text + "',duankou='" + skinTextBox2.Text + "' where workid = " + comboBox1.Text + "";

                    }
                    else
                    {
                        sql = "insert into ip (workid,ip,duankou) values(" + comboBox1.Text + ", '" + skinTextBox1.Text + "', " + skinTextBox2.Text + ")";
                      
                    }

                    //string sql = "update user set PassWord='" + textBox6.Text + "' where UserName ='" + curr_user.name + "';";
                   
                    MySqlConn ms1 = new MySqlConn();
                    int res = ms1.nonSelect(sql);
                    ms1.Close();
                    Label2.Text = "";
                    try
                    {

                        string sql2 = "select * from ip where workid =" + "" + comboBox1.Text + "";

                        MySqlDataReader rd2 = msc.getDataFromTable(sql2);
                        while (rd2.Read())
                        {
                            string bbb = "";
                            string ccc = "";

                            bbb = rd2["ip"].ToString();
                            if (bbb == "")
                            {
                                Label2.Text = "";
                            }
                            else
                            {
                                Label2.Text = bbb;
                            }
                            ccc = rd2["duankou"].ToString();
                            if (ccc == "")
                            {
                                Label3.Text = "";
                            }
                            else
                            {
                                Label3.Text = ccc;
                            }

                        }
                        rd2.Close();
                        msc.Close();
                        try//传ip的操作
                        {
                            string bz2 = Ten2Hex(comboBox1.Text);
                            if (bz2.Length == 2)
                            {
                                sendip = bz2;
                            }
                            else
                            {
                                sendip = "0" + bz2;
                            }
                            sendip = sendip + " 05 00 0C";
                            sql = "select * from ip where workid =" + "" + comboBox1.Text + "";
                            string localip = GetLocalIP();
                            string ip = "";
                            string duankou = "";
                            MySqlDataReader rd = msc.getDataFromTable(sql);
                            while (rd.Read())
                            {
                                ip = rd["ip"].ToString();
                                duankou = rd["duankou"].ToString();
                            }
                            rd.Close();
                            int i = 0, j = 0, geshu = 0;
                            while (!ip[i].Equals('.'))
                            {
                                i++;
                                geshu++;

                            }
                            string bz1 = Ten2Hex(ip.Substring(j, geshu));
                            if (bz1.Length == 2)
                            {
                                sendip1[0] = bz1;
                            }
                            else
                            {
                                sendip1[0] = "0" + bz1;
                            }
                            geshu = 0;
                            i++;
                            j = i;
                            while (!ip[i].Equals('.'))
                            {
                                i++;
                                geshu++;

                            }
                            bz1 = Ten2Hex(ip.Substring(j, geshu));
                            if (bz1.Length == 2)
                            {
                                sendip1[1] = bz1;
                            }
                            else
                            {
                                sendip1[1] = "0" + bz1;
                            }
                            geshu = 0;
                            i++;
                            j = i;
                            while (!ip[i].Equals('.'))
                            {
                                i++;
                                geshu++;

                            }
                            bz1 = Ten2Hex(ip.Substring(j, geshu));
                            if (bz1.Length == 2)
                            {
                                sendip1[2] = bz1;
                            }
                            else
                            {
                                sendip1[2] = "0" + bz1;
                            }

                            i++;
                            j = i;
                            geshu = ip.Length - i;

                            bz1 = Ten2Hex(ip.Substring(j, geshu));
                            if (bz1.Length == 2)
                            {
                                sendip1[3] = bz1;
                            }
                            else
                            {
                                sendip1[3] = "0" + bz1;
                            }
                            while (duankou.Length < 4)
                            {
                                duankou = "0" + duankou;
                            }
                            string tt2=Ten2Hex(duankou.Substring(0, 4));

                            sendip1[4] = tt2[0] + "" + tt2[1];
                            sendip1[5] = tt2[2] + "" + tt2[3];
                            //bz1 = Ten2Hex(duankou.Substring(0, 2));
                            //if (bz1.Length == 2)
                            //{
                            //    sendip1[4] = bz1;
                            //}
                            //else
                            //{
                            //    sendip1[4] = "0" + bz1;
                            //}
                            //bz1 = Ten2Hex(duankou.Substring(2, 2));
                            //if (bz1.Length == 2)
                            //{
                            //    sendip1[5] = bz1;
                            //}
                            //else
                            //{
                            //    sendip1[5] = "0" + bz1;
                            //}
                            i = 0; j = 0; geshu = 0;
                            while (!localip[i].Equals('.'))
                            {
                                i++;
                                geshu++;

                            }
                            bz1 = Ten2Hex(localip.Substring(j, geshu));
                            if (bz1.Length == 2)
                            {
                                sendip1[6] = bz1;
                            }
                            else
                            {
                                sendip1[6] = "0" + bz1;
                            }
                            geshu = 0;
                            i++;
                            j = i;
                            while (!localip[i].Equals('.'))
                            {
                                i++;
                                geshu++;

                            }
                            bz1 = Ten2Hex(localip.Substring(j, geshu));
                            if (bz1.Length == 2)
                            {
                                sendip1[7] = bz1;
                            }
                            else
                            {
                                sendip1[7] = "0" + bz1;
                            }
                            geshu = 0;
                            i++;
                            j = i;
                            while (!localip[i].Equals('.'))
                            {
                                i++;
                                geshu++;

                            }
                            bz1 = Ten2Hex(localip.Substring(j, geshu));
                            if (bz1.Length == 2)
                            {
                                sendip1[8] = bz1;
                            }
                            else
                            {
                                sendip1[8] = "0" + bz1;
                            }

                            i++;
                            j = i;
                            geshu = localip.Length - i;

                            bz1 = Ten2Hex(localip.Substring(j, geshu));
                            if (bz1.Length == 2)
                            {
                                sendip1[9] = bz1;
                            }
                            else
                            {
                                sendip1[9] = "0" + bz1;
                            }
                            string duankou1 = "8001";
                            string temp= Ten2Hex(duankou1.Substring(0, 4));
                            sendip1[10] = temp[0] + "" + temp[1];
                            sendip1[11] = temp[2] + "" + temp[3];

                            for (i = 0; i < 12; i++)
                            {
                                sendip = sendip + " " + sendip1[i];
                            }

                        }
                        catch (SqlException se1)
                    {

                        }
                    }
                    catch (SqlException se)
                    {

                    }
                    if (res > 0)
                    {
                        MessageBox.Show("修改成功", "提示");
                        counter="abcd";
                        //【4】调用
                        this.addOne.Invoke(sendip.ToString());
                    }
                    else
                    {
                        MessageBox.Show("修改失败", "提示");
                    }


                }
                catch (Exception exc)
                {
                    //MessageBox.Show("请检查数据库连接设置", "提示");

                }
            }
        }
        public static string GetLocalIP()//获取本机ip地址
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "获取本机IP出错:" + ex.Message.ToString();
            }
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

        public static string tenValue2Char(ulong ten)
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
        private void skinButton2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void skinTextBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label2.Text = "";
            Label3.Text = "";
            string sql = "select workname from work where workid =" + "" + comboBox1.Text + "";

            MySqlDataReader rd = msc.getDataFromTable(sql);
            while (rd.Read())
            {
                skinLabel4.Text = rd["workname"].ToString();
            }
            rd.Close();
            msc.Close();
            string sql2 = "select * from ip where workid =" + "" + comboBox1.Text + "";

            MySqlDataReader rd2 = msc.getDataFromTable(sql2);
            while (rd2.Read())
            {
                string bbb="";
                string ccc = "";
                
                bbb= rd2["ip"].ToString();
                if(bbb=="")
                {
                    Label2.Text="";
                }
                else
                {
                    Label2.Text=bbb;
                }
                ccc = rd2["duankou"].ToString();
                if (ccc == "")
                {
                    Label3.Text = "";
                }
                else
                {
                    Label3.Text = ccc;
                }

            }
            rd2.Close();
            msc.Close();

        }
    }
}
