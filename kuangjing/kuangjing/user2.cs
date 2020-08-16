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
    public partial class user2 : CCSkinMain
    {
        DataTable dtInfo = new DataTable();
        MySqlConn msc = new MySqlConn();
        public user2()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }

        private void label2_Click(object sender, EventArgs e)
        {

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
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox8.Text.Equals(""))
            {
                MessageBox.Show("请输入用户名", "提示");
            }
            else if (textBox9.Text.Equals(""))
            {
                MessageBox.Show("请输入密码", "提示");
            }
            else if (textBox10.Text.Equals(""))
            {
                MessageBox.Show("请确认密码", "提示");
            }
            else if (textBox10.Text.Equals(textBox9.Text) != true)
            {
                MessageBox.Show("两次输入的密码不一致", "提示");
            }
            else
            {
                try
                {
                    string sql = "select * from user";
                    //DataBase db = new DataBase();
                    //db.command.CommandText = sql;
                    //db.command.Connection = db.connection;
                    //db.Dr = db.command.ExecuteReader();
                    MySqlConn ms = new MySqlConn();
                    MySqlDataReader rd = ms.getDataFromTable(sql);
                    bool isExit = false;
                    while (rd.Read())
                    {
                        //richTextBox1.AppendText(db.Dr["UserName"].ToString() + "   " + textBox8.Text + "\r\n");
                        if (rd["UserName"].ToString().Equals(textBox8.Text.Trim()))
                        {
                            isExit = true;
                            break;
                        }
                    }
                    rd.Close();
                    ms.Close();
                    if (isExit)
                    {
                        MessageBox.Show("用户已存在", "提示");
                    }
                    else
                    {
                        sql = "insert into user values('" + textBox8.Text.Trim() + "','" + textBox9.Text + "',0);";
                        MySqlConn ms2 = new MySqlConn();
                        int res = ms2.nonSelect(sql);
                        ms2.Close();
                        if (res > 0)
                        {
                            MessageBox.Show("添加成功", "提示");
                            //让文本框获取焦点，不过注释这行也能达到效果
                            //richTextBox1.Focus();
                            //设置光标的位置到文本尾   
                            //richTextBox1.Select(richTextBox1.TextLength, 0);
                            //滚动到控件光标处   
                            //richTextBox1.ScrollToCaret();
                            //richTextBox1.AppendText(DateTime.Now.ToString("G") + "\r\n" + "添加用户成功\r\n\r\n");
                        }
                        else
                        {
                            MessageBox.Show("添加失败", "提示");
                        }
                    }
                }
                catch (Exception exc)
                {
                    //MessageBox.Show("请检查数据库是否创建好", "提示");
                    new Thread(new ParameterizedThreadStart(showBox)).Start("请检查数据库是否创建好");
                }


            }
        }
        private System.Windows.Forms.RichTextBox richTextBox1;

        private void button13_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
