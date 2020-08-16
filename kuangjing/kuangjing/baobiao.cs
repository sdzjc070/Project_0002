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
    public partial class baobiao : CCSkinMain
    {
        public baobiao()
        {
            InitializeComponent();
        }
        public static class Config
        {
            public static string value = null;
        }
        MySqlConn msc = new MySqlConn();
        private string selectID(string str)
        {
            string sql = "select workid from  work where workname  = '" + str + "'";
            try
            {
                //DataBase db = new DataBase();
                //db.command.CommandText = sql;
                //db.command.Connection = db.connection;
                string ret = "";
                //db.Dr = db.command.ExecuteReader();
                MySqlDataReader rd = msc.getDataFromTable(sql);
                while (rd.Read())
                {
                    ret = rd["workid"].ToString();
                }
                rd.Close();
                msc.Close();
                return ret;
            }
            catch (SqlException se)
            {
                string message_error = se.ToString();
                string path = System.Windows.Forms.Application.StartupPath;
                FileStream fs = new FileStream(path + "\\log.txt", FileMode.Create | FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("错误信息是：" + message_error + " 时间是：" + DateTime.Now.ToString());
                sw.Flush();
                sw.Close();
                fs.Close();
                return "error";
            }
        }
            private void baobiao_Load(object sender, EventArgs e)
        {
            Thread fac_thread = new Thread(getFactory);
            fac_thread.Start();
        }
        private void getFactory(object obj)
        {
            try
            {
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
                msc.Close();
                rd1.Close();
              

            }
            catch (SqlException se)
            {
                MessageBox.Show("数据库异常", "提示");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime time_start = new DateTime();
            DateTime time_end = new DateTime();
            string start = "";
            string end = "";
            time_start = Convert.ToDateTime(dateTimePicker1.Text);
            time_end = (Convert.ToDateTime(dateTimePicker1.Text)).AddDays(1);
            start = time_start.ToString("yyyy/MM/dd HH:mm:ss");
            end = time_end.ToString("yyyy/MM/dd HH:mm:ss");
            string abc = selectID(comboBox3.Text);
            //MessageBox.Show(abc+"*"+start);
            Config.value = abc + "*" + start+"*"+end;
            Form1 jied4 = new Form1();
            jied4.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime time_start = new DateTime();
            DateTime time_end = new DateTime();
            string start = "";
            string end = "";
            time_start = Convert.ToDateTime(dateTimePicker1.Text);
            time_end = (Convert.ToDateTime(dateTimePicker1.Text)).AddDays(1);
            start = time_start.ToString("yyyy/MM/dd HH:mm:ss");
            end = time_end.ToString("yyyy/MM/dd HH:mm:ss");
            string abc = selectID(comboBox3.Text);
            //MessageBox.Show(abc+"*"+start);
            Config.value = abc + "*" + start + "*" + end;
            Form2 jied4 = new Form2();
            jied4.Show();
        }
    }
}
