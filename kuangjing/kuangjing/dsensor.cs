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
    public partial class dsensor : CCSkinMain
    {
        public dsensor()
        {
            InitializeComponent();
        }
        MySqlConn msc = new MySqlConn();
        string bz = "";
        string bz1 = "";
        string bz2 = "";
        string t1 = "";
        private void label6_Click(object sender, EventArgs e)
        {

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
                string sql3 = "select * from equiptype";

                MySqlDataReader rd3 = msc.getDataFromTable(sql3);
                while (rd3.Read())
                {
                    comboBox2.Items.Add(rd3["Ename"].ToString());
                }
                rd3.Close();
                msc.Close();
                if (comboBox2.Items.Count > 0)
                {
                    comboBox2.Text = comboBox2.Items[0].ToString();
                }
                //string sql2 = "select * from equipment WHERE Etype = 1 and workid =" + "" + comboBox1.Text + "";

                //MySqlDataReader rd2 = msc.getDataFromTable(sql2);
                //while (rd2.Read())
                //{
                //    comboBox4.Items.Add(rd2["Eid"].ToString());
                //}
                //rd2.Close();
                //msc.Close();
                //if (comboBox4.Items.Count > 0)
                //{
                //    comboBox4.Text = comboBox4.Items[0].ToString();
                //}




            }
            catch (SqlException se)
            {
                MessageBox.Show("数据库异常", "提示");
            }

        }
        private void dsensor_Load(object sender, EventArgs e)
        {
            skinLabel4.Text = "";
            skinTextBox4.Text = "";
            skinTextBox5.Text = "";
            skinTextBox6.Text = "";
            skinTextBox7.Text = "";
            skinTextBox8.Text = "";
            skinTextBox9.Text = "";

            Thread fac_thread = new Thread(getFactory);
            fac_thread.Start();
            try
            {
                string sql = "select dsensor.workid,workname,Ename,Eid,Sid,Sname,Smax,Smin,dsensor.caiji,dsensor.shangchuan from work,dsensor where `work`.workid = dsensor.workid ORDER BY workid asc";



                MySqlConnection conn = msc.GetConn();
                MySqlDataAdapter sda = new MySqlDataAdapter(sql, conn);//获取数据表
                                                                       //DataTable table = new DataTable();
                DataSet ds = new DataSet();
                sda.Fill(ds, "ds");//填充数据库
                this.dataGridView1.DataSource = ds.Tables[0];
            }
            catch (Exception ee)
            {
                MessageBox.Show("查询数据库错误：" + ee.ToString());
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void skinTextBox6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            if (comboBox2.Text.Equals("压力计"))
            {
                comboBox3.Items.Clear();
                comboBox3.Items.Add("压力");

                comboBox3.Text = comboBox3.Items[0].ToString();

            }
            else if (comboBox2.Text.Equals("减速机"))
            {
                comboBox3.Items.Clear();

                string sql1 = "select * from type where bzid!=0";

                MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                while (rd1.Read())
                {
                    comboBox3.Items.Add(rd1["type"].ToString());
                }
                rd1.Close();
                msc.Close();
                if (comboBox3.Items.Count > 0)
                {
                    comboBox3.Text = comboBox3.Items[0].ToString();
                }
            }
            else
            {
                comboBox3.Items.Clear();
                comboBox3.Text = "";
            }
            //string sql4 = "select workid from work where workname =" + "" + comboBox1.Text + "";

            try
            {
                //    int workbzid = 5;
                //    MySqlDataReader rd4 = msc.getDataFromTable(sql4);
                //    while (rd4.Read())
                //    {
                //        workbzid = int.Parse(rd4["workid"].ToString());
                //        //MessageBox.Show("===" + workbzid);

                //    }

                //    rd4.Close();
                //    msc.Close();
                string sql6 = "select Etype from equiptype where Ename =" + "'" + comboBox2.Text + "'";
                int ebzid = 3;
                MySqlDataReader rd6 = msc.getDataFromTable(sql6);
                while (rd6.Read())
                {
                    ebzid = int.Parse(rd6["Etype"].ToString());
                    //MessageBox.Show("===" + ebzid);

                }

                rd6.Close();
                msc.Close();
                comboBox4.Items.Clear();
                string sql5 = "select equipment.Eid from equipment where equipment.Etype = " + ebzid + " AND workid = " + comboBox1.Text + "";

                MySqlDataReader rd5 = msc.getDataFromTable(sql5);
                while (rd5.Read())
                {
                    comboBox4.Items.Add(rd5["Eid"].ToString());
                }
                rd5.Close();
                msc.Close();
                if (comboBox4.Items.Count > 0)
                {
                    comboBox4.Text = comboBox4.Items[0].ToString();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (skinTextBox1.Text.Equals(""))
            {
                MessageBox.Show("请输入传感器编号", "提示");
            }
            else if (comboBox3.Text.Equals(""))
            {
                MessageBox.Show("请选择传感器的类型", "提示");
            }
            else if (skinTextBox2.Text.Equals(""))
            {
                MessageBox.Show("请输入高阈值", "提示");
            }
            else if (skinTextBox3.Text.Equals(""))
            {
                MessageBox.Show("请输入低阈值", "提示");
            }

            else
            {
                try
                {


                    int p1 = 1;
                    //int A= int.Parse(skinTextBox1.Text);
                    //MessageBox.Show(""+skinTextBox1.Text, "提示");
                    string sql11 = "SELECT Sid FROM dsensor WHERE workid = " + comboBox1.Text + " and Eid = " + comboBox4.Text + " ";
                    MySqlConn ms = new MySqlConn();
                    MySqlDataReader rd = ms.getDataFromTable(sql11);
                    while (rd.Read())
                    {
                        t1 = rd["Sid"].ToString();
                        //MessageBox.Show("t1是" +t1, "提示");
                        if (skinTextBox1.Text.Equals(t1))
                        {
                            p1 = 0;
                            break;

                        }
                    }
                    rd.Close();
                    ms.Close();

                    int t2;
                    if (p1 != 0)
                    {

                        if (skinTextBox4.Text == "")
                            skinTextBox4.Text = "0";
                        if (skinTextBox5.Text == "")
                            skinTextBox5.Text = "0";
                        if (skinTextBox6.Text == "")
                            skinTextBox6.Text = "0";
                        if (skinTextBox7.Text == "")
                            skinTextBox7.Text = "0";
                        if (skinTextBox8.Text == "")
                            skinTextBox8.Text = "0";
                        if (skinTextBox9.Text == "")
                            skinTextBox9.Text = "0";

                        string ss = skinTextBox4.Text + "分" + skinTextBox5.Text + "秒" + skinTextBox6.Text + "毫秒";
                        string sss = skinTextBox7.Text + "分" + skinTextBox8.Text + "秒" + skinTextBox9.Text + "毫秒";
                        string sql = "insert into dsensor values(" + comboBox1.Text + "," + comboBox4.Text + ", '" + comboBox2.Text + "'," + skinTextBox1.Text + ", '" + comboBox3.Text + "', " + skinTextBox2.Text + ", " + skinTextBox3.Text + ",'" + ss + "', '" + sss + "')";
                        MySqlConn ms2 = new MySqlConn();
                        int n = ms2.nonSelect(sql);
                        ms2.Close();
                        MessageBox.Show("添加成功", "提示");
                        skinTextBox1.Text = "";
                        skinTextBox2.Text = "";
                        try
                        {
                            sql = "select dsensor.workid,workname,Ename,Eid,Sid,Sname,Smax,Smin,dsensor.caiji,dsensor.shangchuan from work,dsensor where `work`.workid = dsensor.workid ORDER BY workid asc";



                            MySqlConnection conn = msc.GetConn();
                            MySqlDataAdapter sda = new MySqlDataAdapter(sql, conn);//获取数据表
                                                                                   //DataTable table = new DataTable();
                            DataSet ds = new DataSet();
                            sda.Fill(ds, "ds");//填充数据库
                            this.dataGridView1.DataSource = ds.Tables[0];
                        }
                        catch (Exception ee)
                        {
                            MessageBox.Show("查询数据库错误：" + ee.ToString());
                        }

                    }
                    else
                    {
                        MessageBox.Show("已存在该传感器", "提示");
                    }
                }
                catch (SqlException se)
                {
                    MessageBox.Show("数据库异常", "提示");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (skinTextBox1.Text.Equals(""))
            {
                MessageBox.Show("请输入传感器编号", "提示");
            }
            else if (comboBox3.Text.Equals(""))
            {
                MessageBox.Show("请选择传感器的类型", "提示");
            }
            else if (skinTextBox2.Text.Equals(""))
            {
                MessageBox.Show("请输入高阈值", "提示");
            }
            else if (skinTextBox3.Text.Equals(""))
            {
                MessageBox.Show("请输入低阈值", "提示");
            }

            else
            {
                if (comboBox1.Text != bz)
                {
                    MessageBox.Show("请勿修改工作面", "提示");
                }
                else if(comboBox4.Text!=bz1)
                {
                    MessageBox.Show("请勿修改设备", "提示");
                }
                else
                {
                    int p1 = 1;
                    //int A= int.Parse(skinTextBox1.Text);
                    //MessageBox.Show(""+skinTextBox1.Text, "提示");
                    string sql11 = "SELECT Sid FROM dsensor WHERE workid = " + comboBox1.Text + " and Eid = " + comboBox4.Text + " ";
                    MySqlConn ms = new MySqlConn();
                    MySqlDataReader rd = ms.getDataFromTable(sql11);
                    while (rd.Read())
                    {
                        t1 = rd["Sid"].ToString();
                        //MessageBox.Show("t1是" +t1, "提示");
                        if (skinTextBox1.Text.Equals(t1)&&skinTextBox1.Text!=bz2)
                        {
                            p1 = 0;
                            break;

                        }
                    }
                    rd.Close();
                    ms.Close();

                    int t2;
                    if (p1 != 0)
                    {
                        try
                        {
                            if (skinTextBox4.Text == "")
                            skinTextBox4.Text = "0";
                        if (skinTextBox5.Text == "")
                            skinTextBox5.Text = "0";
                        if (skinTextBox6.Text == "")
                            skinTextBox6.Text = "0";
                        if (skinTextBox7.Text == "")
                            skinTextBox7.Text = "0";
                        if (skinTextBox8.Text == "")
                            skinTextBox8.Text = "0";
                        if (skinTextBox9.Text == "")
                            skinTextBox9.Text = "0";

                        string ss = skinTextBox4.Text + "分" + skinTextBox5.Text + "秒" + skinTextBox6.Text + "毫秒";
                        string sss = skinTextBox7.Text + "分" + skinTextBox8.Text + "秒" + skinTextBox9.Text + "毫秒";
                        string sql = "update dsensor set Sid = " + skinTextBox1.Text + ",Sname = '" + comboBox3.Text + "',Smax=" + skinTextBox2.Text + ",Smin=" + skinTextBox3.Text + ",caiji = '" + ss + "',shangchuan = '" + sss + "' where Eid =" + bz1 + " AND workid = " + bz + " and Sid = " + bz2 + "";
                        MySqlConn ms2 = new MySqlConn();
                        int n = ms2.nonSelect(sql);
                        ms2.Close();
                        MessageBox.Show("修改成功", "提示");
                        skinTextBox1.Text = "";
                        skinTextBox2.Text = "";
                        
                            sql = "select dsensor.workid,workname,Ename,Eid,Sid,Sname,Smax,Smin,dsensor.caiji,dsensor.shangchuan from work,dsensor where `work`.workid = dsensor.workid ORDER BY workid asc";



                            MySqlConnection conn = msc.GetConn();
                            MySqlDataAdapter sda = new MySqlDataAdapter(sql, conn);//获取数据表
                                                                                   //DataTable table = new DataTable();
                            DataSet ds = new DataSet();
                            sda.Fill(ds, "ds");//填充数据库
                            this.dataGridView1.DataSource = ds.Tables[0];
                        }
                        catch (Exception ee)
                        {
                            MessageBox.Show("查询数据库错误：" + ee.ToString());
                        }

                    }
                    else
                    {
                        MessageBox.Show("已存在该传感器编号", "提示");
                    }
                }

                
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentRow.Index; //获取选中行的行号

            comboBox1.Text = dataGridView1.Rows[index].Cells[0].Value.ToString();
            skinLabel4.Text = dataGridView1.Rows[index].Cells[1].Value.ToString();
            comboBox2.Text = dataGridView1.Rows[index].Cells[2].Value.ToString();
            comboBox4.Text = dataGridView1.Rows[index].Cells[3].Value.ToString();
            skinTextBox1.Text = dataGridView1.Rows[index].Cells[4].Value.ToString();
            comboBox3.Text = dataGridView1.Rows[index].Cells[5].Value.ToString();
            skinTextBox2.Text = dataGridView1.Rows[index].Cells[6].Value.ToString();
            skinTextBox3.Text = dataGridView1.Rows[index].Cells[7].Value.ToString();

            bz = comboBox1.Text;
            bz1 = comboBox4.Text;
            bz2 = skinTextBox1.Text;
            string ss = dataGridView1.Rows[index].Cells[8].Value.ToString();
            if (ss == "")
            {
                skinTextBox4.Text = "0";
                skinTextBox5.Text = "0";
                skinTextBox6.Text = "0";

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
                skinTextBox4.Text = s;

                s = "";
                i++;
                while (ss[i] != '秒')
                {
                    s = s + ss[i];
                    i++;
                }
                skinTextBox5.Text = s;
                s = "";
                i++;
                while (ss[i] != '毫')
                {
                    s = s + ss[i];
                    i++;
                }
                skinTextBox6.Text = s;
            }
            ss = dataGridView1.Rows[index].Cells[9].Value.ToString();
            if (ss == "")
            {
                skinTextBox7.Text = "0";
                skinTextBox8.Text = "0";
                skinTextBox9.Text = "0";

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
                skinTextBox7.Text = s;

                s = "";
                i++;
                while (ss[i] != '秒')
                {
                    s = s + ss[i];
                    i++;
                }
                skinTextBox8.Text = s;
                s = "";
                i++;
                while (ss[i] != '毫')
                {
                    s = s + ss[i];
                    i++;
                }
                skinTextBox9.Text = s;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认删除", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                //点确定的代码


                string sql = "DELETE FROM dsensor WHERE workid =" + bz + " AND Eid =" + bz1 + " AND Sid =" + bz2 + "";
                MySqlConn ms2 = new MySqlConn();
                int n = ms2.nonSelect(sql);
                ms2.Close();
                MessageBox.Show(" 删除设备成功", "提示");
                skinTextBox1.Text = "";
                skinTextBox2.Text = "";
                skinTextBox3.Text = "";
                skinTextBox4.Text = "";
                skinTextBox5.Text = "";
                skinTextBox6.Text = "";
                skinTextBox7.Text = "";
                skinTextBox8.Text = "";
                skinTextBox9.Text = "";

                try
                {
                    sql = "select dsensor.workid,workname,Ename,Eid,Sid,Sname,Smax,Smin,dsensor.caiji,dsensor.shangchuan from work,dsensor where `work`.workid = dsensor.workid ORDER BY workid asc";



                    MySqlConnection conn = msc.GetConn();
                    MySqlDataAdapter sda = new MySqlDataAdapter(sql, conn);//获取数据表
                                                                           //DataTable table = new DataTable();
                    DataSet ds = new DataSet();
                    sda.Fill(ds, "ds");//填充数据库
                    this.dataGridView1.DataSource = ds.Tables[0];
                }
                catch (Exception ee)
                {
                    MessageBox.Show("查询数据库错误：" + ee.ToString());
                }
            }
        }
    }
}
