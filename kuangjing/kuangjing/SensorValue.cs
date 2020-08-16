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
    public partial class SensorValue : CCSkinMain
    {
        public SensorValue(AddOneDelegate addone)
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.addOne = addone;
        }
        private AddOneDelegate addOne;
        MySqlConn msc = new MySqlConn();
        private Mutex file_mutex = new Mutex();//文件互斥锁
        string data = "";


        private void getFactory(object obj)
        {
            try
            {
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                comboBox4.Items.Clear();
                string sql1 = "select * from work";

                MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                while (rd1.Read())
                {
                    comboBox3.Items.Add(rd1["workname"].ToString());
                }
                rd1.Close();
                if (comboBox3.Items.Count > 0)
                {
                    comboBox3.Text = comboBox3.Items[0].ToString();
                }



            }
            catch (SqlException se)
            {
                MessageBox.Show("数据库异常", "提示");
            }

        }
        void Log(string str)    // 记录服务启动  
        {
            file_mutex.WaitOne();
            {
                string info = string.Format("{0}-{1}", DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"), str);
                string path = "C://Mqtt//kuangjingLog.txt";//"C://Mqtt//MyTestLog.txt"
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(info);
                    //关闭
                    sw.Close();
                }
            }
            file_mutex.ReleaseMutex();

        }
        private void SensorValue_Load(object sender, EventArgs e)
        {

            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            skinTextBox1.Text = "";
            skinTextBox2.Text = "";
            //skinTextBox3.Text = "";


            //comboBox2属性设置
            Thread fac_thread = new Thread(getFactory);
            fac_thread.Start();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                comboBox4.Items.Clear();
                string sql4 = "select workid from work where workname =" + "'" + comboBox3.Text + "'";
                int workbzid = 5;
                MySqlDataReader rd4 = msc.getDataFromTable(sql4);
                while (rd4.Read())
                {
                    workbzid = int.Parse(rd4["workid"].ToString());
                    //MessageBox.Show("===" + workbzid);

                }

                rd4.Close();
                comboBox2.Items.Clear();
                comboBox2.Items.Add("所有");

                string sql5 = "select * from equiptype";

                MySqlDataReader rd5 = msc.getDataFromTable(sql5);
                while (rd5.Read())
                {
                    comboBox2.Items.Add(rd5["Ename"].ToString());
                }
                rd5.Close();
                if (comboBox2.Items.Count > 0)
                {
                    comboBox2.Text = comboBox2.Items[0].ToString();
                }
                comboBox4.Items.Clear();
                comboBox4.Items.Add("所有");

                string sql16 = "select equipment.Eid from equipment where workid = " + "'" + workbzid + "'";

                MySqlDataReader rd16 = msc.getDataFromTable(sql16);

                while (rd16.Read())
                {
                    comboBox4.Items.Add(rd16["Eid"].ToString());
                }
                rd16.Close();
                if (comboBox4.Items.Count > 0)
                {
                    comboBox4.Text = comboBox4.Items[0].ToString();
                }
                comboBox1.Items.Clear();
                comboBox1.Items.Add("所有");
                string sql13 = "select Sid from dsensor where workid = " + "'" + workbzid + "'";

                MySqlDataReader rd13 = msc.getDataFromTable(sql13);

                while (rd13.Read())
                {
                    comboBox1.Items.Add(rd13["Sid"].ToString());
                }
                rd13.Close();
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.Text = comboBox1.Items[0].ToString();
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show("数据库异常", "提示");
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string sql4 = "select workid from work where workname =" + "'" + comboBox3.Text + "'";
                int workbzid = 5;
                MySqlDataReader rd4 = msc.getDataFromTable(sql4);
                while (rd4.Read())
                {
                    workbzid = int.Parse(rd4["workid"].ToString());


                }

                rd4.Close();
                if (comboBox2.Text.Equals("所有"))
                {
                    comboBox1.Items.Clear();


                    comboBox4.Items.Clear();


                    comboBox4.Items.Add("所有");
                    string sql2 = "select equipment.Eid from equipment where workid = " + "'" + workbzid + "'";

                    MySqlDataReader rd2 = msc.getDataFromTable(sql2);

                    while (rd2.Read())
                    {
                        comboBox4.Items.Add(rd2["Eid"].ToString());
                    }
                    rd2.Close();
                    if (comboBox4.Items.Count > 0)
                    {
                        comboBox4.Text = comboBox4.Items[0].ToString();
                    }
                    comboBox1.Items.Clear();
                    comboBox1.Items.Add("所有");
                    string sql3 = "select Sid from dsensor where workid = " + "'" + workbzid + "'";

                    MySqlDataReader rd3 = msc.getDataFromTable(sql3);

                    while (rd3.Read())
                    {
                        comboBox1.Items.Add(rd3["Sid"].ToString());
                    }
                    rd3.Close();
                    if (comboBox1.Items.Count > 0)
                    {
                        comboBox1.Text = comboBox1.Items[0].ToString();
                    }

                }
                else
                {
                    string sql6 = "select Etype from equiptype where Ename =" + "'" + comboBox2.Text + "'";
                    int ebzid = 3;
                    MySqlDataReader rd6 = msc.getDataFromTable(sql6);
                    while (rd6.Read())
                    {
                        ebzid = int.Parse(rd6["Etype"].ToString());


                    }

                    rd6.Close();
                    comboBox4.Items.Clear();
                    comboBox4.Items.Add("所有");
                    string sql5 = "select equipment.Eid from equipment where equipment.Etype = " + "'" + ebzid + "' AND workid = " + "'" + workbzid + "'";

                    MySqlDataReader rd5 = msc.getDataFromTable(sql5);
                    while (rd5.Read())
                    {
                        comboBox4.Items.Add(rd5["Eid"].ToString());
                    }
                    rd5.Close();
                    if (comboBox4.Items.Count > 0)
                    {
                        comboBox4.Text = comboBox4.Items[0].ToString();
                    }
                    comboBox1.Items.Clear();
                    comboBox1.Items.Add("所有");

                    if (comboBox1.Items.Count > 0)
                    {
                        comboBox1.Text = comboBox1.Items[0].ToString();
                    }

                }

            }
            catch (Exception ex)
            {

            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.Text.Equals("所有"))
            {
                comboBox1.Items.Clear();
                comboBox1.Items.Add("所有");
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.Text = comboBox1.Items[0].ToString();
                }

            }
            else
            {
                comboBox1.Items.Clear();
                comboBox1.Items.Add("所有");
                string sql4 = "select workid from work where workname =" + "'" + comboBox3.Text + "'";
                int workbzid = 5;
                MySqlDataReader rd4 = msc.getDataFromTable(sql4);
                while (rd4.Read())
                {
                    workbzid = int.Parse(rd4["workid"].ToString());


                }

                rd4.Close();
                string sql5 = "select dsensor.Sid from dsensor where Eid = '" + comboBox4.Text + "' AND workid = '" + workbzid + "'";

                MySqlDataReader rd5 = msc.getDataFromTable(sql5);
                while (rd5.Read())
                {
                    comboBox1.Items.Add(rd5["Sid"].ToString());
                }
                rd5.Close();
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.Text = comboBox1.Items[0].ToString();
                }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

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

        private void button1_Click(object sender, EventArgs e)
        {
            if (skinTextBox1.Text == "")
            {
                MessageBox.Show("请输入设置的高阈值");
            }
            else if (skinTextBox2.Text == "")   
            {
                MessageBox.Show("请输入设置的低阈值");
            }

            else
            {
                string sql4 = "select workid from work where workname =" + "'" + comboBox3.Text + "'";
                string workbzid = "";
                MySqlDataReader rd4 = msc.getDataFromTable(sql4);
                while (rd4.Read())
                {
                    workbzid = rd4["workid"].ToString();


                }

                rd4.Close();
                msc.Close();
                if (comboBox2.Text == "所有" && comboBox4.Text == "所有")
                {
                   string zuida = skinTextBox1.Text;
                   string zuixiao = skinTextBox2.Text;

                    //    //MessageBox.Show("skinTextBox2.Text是" + bz, "提示");
                     // string sql = "update equipment set caiji = '" + ss + "' where  workid = " + workbzid + "";
                        MySqlConn ms2 = new MySqlConn();
                    //   int n = ms2.nonSelect(sql);
                    //    ms2.Close();
                    //    string sql1 = "update dsensor set caiji = '" + ss + "' where  workid = " + workbzid + "";

                    //    int m = ms2.nonSelect(sql1);
                    //    ms2.Close();
                }
                //else if (comboBox1.Text == "所有" && comboBox4.Text != "所有")
                //{
                //    string ss = skinTextBox1.Text + "分" + skinTextBox2.Text + "秒" + skinTextBox3.Text + "毫秒";
                //    MySqlConn ms2 = new MySqlConn();
                //    //MessageBox.Show("skinTextBox2.Text是" + bz, "提示");
                //    string sql = "update equipment set caiji = '" + ss + "' where Eid =" + comboBox4.Text + " AND workid = " + workbzid + "";

                //    int n = ms2.nonSelect(sql);
                //    ms2.Close();
                //    string sql1 = "update dsensor set caiji = '" + ss + "' where Eid =" + comboBox4.Text + " AND workid = " + workbzid + "";

                //    int m = ms2.nonSelect(sql1);
                //    ms2.Close();
                //}
                //else if (comboBox1.Text != "所有" && comboBox4.Text != "所有")
                //{
                //    string ss = skinTextBox1.Text + "分" + skinTextBox2.Text + "秒" + skinTextBox3.Text + "毫秒";

                //    //MessageBox.Show("skinTextBox2.Text是" + bz, "提示");

                //    MySqlConn ms2 = new MySqlConn();


                //    string sql1 = "update dsensor set caiji = '" + ss + "' where Eid =" + comboBox4.Text + " AND Sid =" + comboBox1.Text + " AND workid = " + workbzid + "";

                //    int m = ms2.nonSelect(sql1);
                //    ms2.Close();
                //}

                string bz2 = Ten2Hex(workbzid);
                if (bz2.Length == 2)
                {
                    data = Ten2Hex(workbzid);
                }
                else
                {
                    data = "0" + Ten2Hex(workbzid);
                }
                data = data + " 11 00 09";
                //设备类型
                if (comboBox2.Text.Equals("所有"))
                {
                    data = data + " " + "FF";

                }
                else if (comboBox2.Text.Equals("压力计"))
                {
                    data = data + " " + "01";
                }
                else if (comboBox2.Text.Equals("减速机"))
                {
                    data = data + " " + "02";
                }
                else if (comboBox2.Text.Equals("中继"))
                {
                    data = " " + "03";
                }

                //设备编号
                if (comboBox4.Text.Equals("所有"))
                {
                    data = data + " FF";

                }
                else
                {
                    string bz = Ten2Hex(comboBox4.Text);
                    if (bz.Length == 2)
                    {
                        data = data + " " + Ten2Hex(comboBox4.Text);
                    }
                    else
                    {
                        data = data + " 0" + Ten2Hex(comboBox4.Text);
                    }


                }
                
                //传感器编号
                if (comboBox1.Text.Equals("所有"))
                {
                    data = data + " FF";

                }
                else
                {
                    string bz1 = Ten2Hex(comboBox1.Text);
                    if (bz1.Length == 2)
                    {
                        data = data + " " + Ten2Hex(comboBox1.Text);
                    }
                    else
                    {
                        data = data + " 0" + Ten2Hex(comboBox1.Text);
                    }

                }
                int gYvalue = 0;
                int dYvalue = 0;
                //MessageBox.Show("高阈值前：" + skinTextBox1.Text);
                if (int.Parse(skinTextBox1.Text) < 0)
                {
                    data += " 00";
                    gYvalue = Math.Abs(int.Parse(skinTextBox1.Text));
                }
                else
                {
                    data += " 80";
                    gYvalue = int.Parse(skinTextBox1.Text);
                }
                string bz3 = Ten2Hex(gYvalue.ToString());

        
                if (bz3.Length == 1)
                {
                    data = data + " 00 0" + bz3;
                }else if(bz3.Length == 2)
                {
                    data = data + " 00 " + bz3;
                }else if(bz3.Length == 3)
                {
                    data = data + " 0" + bz3[0] + " " + bz3.Substring(1, 2);
                }

                if (int.Parse(skinTextBox2.Text) < 0)
                {
                    data += " 00";
                    dYvalue = Math.Abs(int.Parse(skinTextBox2.Text));
                }
                else
                {
                    data += " 80";
                    dYvalue = int.Parse(skinTextBox2.Text);
                }
                string bz4 = Ten2Hex(dYvalue.ToString());


                if (bz4.Length == 1)
                {
                    data = data + " 00 0" + bz4;
                }
                else if (bz4.Length == 2)
                {
                    data = data + " 00 " + bz4;
                }
                else if (bz4.Length == 3)
                {
                    data = data + " 0" + bz4[0] + " " + bz4.Substring(1, 2);
                }


                Log("设置阈值：" + data);

                this.addOne.Invoke(data.ToString());
                MessageBox.Show("设置成功");
            }
        }
    }
}
