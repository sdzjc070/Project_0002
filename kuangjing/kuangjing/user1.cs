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
    public partial class user1 : CCSkinMain
    {
        DataTable dtInfo = new DataTable();
        MySqlConn msc = new MySqlConn();
        public int change = 0;
        Users curr_user = new Users();
        public user1()
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

        /// <summary>
        /// 加载定时信息。。一开始加载窗体的时候就将所有的定时操作加载出来
        /// </summary>
        /// <param name="obj"></param>




        private void button10_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
            {
                //MessageBox.Show("请输入原密码", "提示");
                new Thread(new ParameterizedThreadStart(ShowBox)).Start("请输入原密码");
            }
            else if (textBox6.Text.Equals(""))
            {
                MessageBox.Show("请输入新密码", "提示");
            }
            else if (textBox7.Text.Equals(""))
            {
                MessageBox.Show("请输入确认密码", "提示");
            }
            else
            {
                if (textBox6.Text.Equals(textBox7.Text) == false)
                {
                    MessageBox.Show("两次输入的密码不一致", "提示");
                }
                else
                {
                    try
                    {
                        //MessageBox.Show("====33333=" + old_passwd);
                        //string sql_find = "select * from user where UserName = '" + curr_user.name + "'";
                        string sql_find = "select * from user where UserName = 'root'";
                        MySqlConn ms = new MySqlConn();
                        MySqlDataReader rd = ms.getDataFromTable(sql_find);
                        string old_passwd = "";
                        while (rd.Read())
                        {
                            old_passwd = rd["PassWord"].ToString();
                            break;
                        }
                        rd.Close();
                        ms.Close();

                        if (old_passwd.Equals(textBox1.Text))
                        {

                            //string sql = "update user set PassWord='" + textBox6.Text + "' where UserName ='" + curr_user.name + "';";
                            string sql = "update user set PassWord='" + textBox6.Text + "' where UserName ='root';";
                            MySqlConn ms1 = new MySqlConn();
                            int res = ms1.nonSelect(sql);
                            ms1.Close();
                            if (res > 0)
                            {
                                MessageBox.Show("修改密码成功", "提示");
                            }
                            else
                            {
                                MessageBox.Show("修改密码失败", "提示");
                            }
                        }
                        else
                        {
                            MessageBox.Show("原密码输入错误", "提示");
                        }

                    }
                    catch (Exception exc)
                    {
                        //MessageBox.Show("请检查数据库连接设置", "提示");
                        new Thread(new ParameterizedThreadStart(ShowBox)).Start("请检查数据库设置");
                    }
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void user1_Load(object sender, EventArgs e)
        {

        }
    }

}
