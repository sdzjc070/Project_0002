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

using System.Configuration;
using MySql.Data.MySqlClient;
using CCWin;

namespace kuangjing
{
    public partial class user4 : CCSkinMain
    {
        DataTable dtInfo = new DataTable();
        MySqlConn msc = new MySqlConn();
        public user4()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }


        private int msgBoxNum = 0;//MessageBox编号，每弹出一个MessageBox自增1，大于2000变为0
        private void showBox(object obj)
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
                string sql = "select * from user";
                //DataBase db = new DataBase();
                //db.command.CommandText = sql;
                //db.command.Connection = db.connection;
                //db.Dr = db.command.ExecuteReader();
                MySqlConn msq = new MySqlConn();
                MySqlDataReader rd = msq.getDataFromTable(sql);
                while (rd.Read())
                {
                    if (rd["UserName"].ToString().Equals("root"))
                        continue;
                    comboBox8.Items.Add(rd["UserName"].ToString());
                }
                rd.Close();
                msq.Close();
            }
            catch (Exception exc)
            {
                //MessageBox.Show("请检查数据库设置", "提示");
                new Thread(new ParameterizedThreadStart(showBox)).Start("请检查数据库设置");
            }

        }

        private void user4_Load(object sender, EventArgs e)
        {
            Thread fac_thread = new Thread(getFactory);
            fac_thread.Start();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (comboBox8.Text.Equals(""))
            {
                MessageBox.Show("请输入用户名", "提示");
            }
            else if (textBox14.Text.Equals(""))
            {
                MessageBox.Show("请输入密码", "提示");
            }
            else
            {
                try
                {

                    string sql = "update user set PassWord='" + textBox14.Text + "' where UserName='" + comboBox8.Text.Trim() + "';";
                    MySqlConn ms = new MySqlConn();
                    int res = ms.nonSelect(sql);
                    ms.Close();
                    if (res > 0)
                    {
                        MessageBox.Show("用户初始化成功", "提示");
                        //让文本框获取焦点，不过注释这行也能达到效果
                        // richTextBox1.Focus();
                        //设置光标的位置到文本尾   
                        // richTextBox1.Select(richTextBox1.TextLength, 0);
                        //滚动到控件光标处   
                        //richTextBox1.ScrollToCaret();
                        //richTextBox1.AppendText(DateTime.Now.ToString("G") + "\r\n" + "用户初始化成功\r\n\r\n");
                    }
                    else
                    {
                        MessageBox.Show("用户初始化失败", "提示");
                    }
                }
                catch (Exception exc)
                {
                    //MessageBox.Show("请检查数据库设置", "提示");
                    new Thread(new ParameterizedThreadStart(showBox)).Start("请检查数据库设置");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
