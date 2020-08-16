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
    public partial class company : CCSkinMain
    {
        DataTable dtInfo = new DataTable();
        MySqlConn msc = new MySqlConn();
        public int change = 0;
        public company()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }
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


        private void company_Load(object sender, EventArgs e)
        {
            Label2.Text = "";
            try
            {

                string sql = "select company from company where company is not NULL";
                MySqlConn ms = new MySqlConn();
                MySqlDataReader rd = ms.getDataFromTable(sql);
                while (rd.Read())
                {
                    Label2.Text = rd["company"].ToString();
                }
                rd.Close();
                ms.Close();
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
                new Thread(new ParameterizedThreadStart(ShowBox)).Start("请输入新名称");
            }
            else
            {
                try
                {
                    //MessageBox.Show("====33333=" + old_passwd);
                    
                   
                    

                        //string sql = "update user set PassWord='" + textBox6.Text + "' where UserName ='" + curr_user.name + "';";
                        string sql = "update company set company = '" + skinTextBox1.Text + "' where company is not NULL";
                        MySqlConn ms1 = new MySqlConn();
                        int res = ms1.nonSelect(sql);
                        ms1.Close();
                    Label2.Text = "";
                    try
                    {

                        string sql1 = "select company from company where company is not NULL";
                        MySqlConn ms = new MySqlConn();
                        MySqlDataReader rd = ms.getDataFromTable(sql1);
                        while (rd.Read())
                        {
                            Label2.Text = rd["company"].ToString();
                        }
                        rd.Close();
                        ms.Close();
                    }
                    catch (SqlException se)
                    {

                    }
                    if (res > 0)
                        {
                            MessageBox.Show("修改公司名称成功", "提示");
                        }
                        else
                        {
                            MessageBox.Show("修改公司名称失败", "提示");
                        }
                   

                }
                catch (Exception exc)
                {
                    //MessageBox.Show("请检查数据库连接设置", "提示");
                    
                }
            }
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
