using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kuangjing
{
    class MySqlConn
    {
        public string connString = "";
        public MySqlConnection conn;
        public MySqlConn()
        {
            String line = "";
            string name = "";
            string pwd = "";
            string port = "";
            try
            {
                //读取sql地址
                string ippath = "C://Mqtt" + @"\SqlConfig.txt";
                if (File.Exists(ippath) == false)//创建保存串口信息的文件
                    File.Create(ippath).Close();
                StreamReader sr = new StreamReader(ippath, Encoding.Default);
                line = sr.ReadLine();

                if (line == null)
                {
                    //Log("文件无内容");
                    name = "localhost";
                    pwd = "123456";
                    port = "3306";
                }
                else
                {
                    string[] arr = line.Split('+');
                    name = arr[0];
                    pwd = arr[1];
                    port = arr[2];
                }
                //关闭文件输入流
                sr.Close();
            }
            catch (Exception ee)
            {
                name = "localhost";
                pwd = "123456";
                port = "3306";
                //Log("读取sql连接文件出错：" + ee.ToString());

            }

            if (connString.Equals("") || conn == null)
            {
                connString = "SslMode=None;port=" + port + ";database=kj;Password=" + pwd + ";User ID=root;server=" + name;
                //Log("客户端数据库连接" + connString);
                conn = new MySqlConnection(connString);

            }
        }
        public MySqlConnection GetConn()
        {
            if (this.conn.State != ConnectionState.Open)//判断数据库连接状态
            {
                conn.Open(); //连接MySQL服务器数据库。
            }

            return conn;
        }
        public void Close()
        {
            conn.Close();//关闭数据库
        }

        //查询数据库
        public MySqlDataReader getDataFromTable(string sql)
        {
            MySqlConnection conn = this.GetConn();
            MySqlCommand cmmd = new MySqlCommand(sql, conn);//执行一条sql语句。
            MySqlDataReader rdr = cmmd.ExecuteReader();//包含sql语句执行的结果，并提供一个方法从结果中阅读一行。
            return rdr;
        }
        //修改数据库
        public int nonSelect(string sql)
        {
            MySqlConnection conn = this.GetConn();
            MySqlCommand cmmd = new MySqlCommand(sql, conn);//执行一条sql语句。
            return cmmd.ExecuteNonQuery();
        }
        void Log(string str)    // 记录服务启动  
        {
            {
                string info = string.Format("{0}-{1}", DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"), str);
                string path = "C://Mqtt//kj.txt";//"C://Mqtt//MyTestLog.txt"
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(info);
                    //关闭
                    sw.Close();
                }
            }

        }
    }
}
