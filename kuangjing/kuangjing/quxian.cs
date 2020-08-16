using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;
using CCWin;
using CCWin.SkinClass;

namespace kuangjing
{
    public partial class quxian : CCSkinMain
    {
        public quxian()
        {
            InitializeComponent();
        }
        MySqlConn msc = new MySqlConn();
        MySqlConn msc1 = new MySqlConn();
        private Mutex file_mutex = new Mutex();//文件互斥锁
        string ret = "";
        string sql = "";


        /// <summary>
        /// 根据BinName找BinID
        /// </summary>
        /// <returns></returns>
        private string selectID(string str)
        {
            sql = "select workid from  work where workname  = '" + str + "'";
            try
            {
                //DataBase db = new DataBase();
                //db.command.CommandText = sql;
                //db.command.Connection = db.connection;
                
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

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {



                chart1.Series["变量"].Points.Clear();
                if (comboBox2.Text.Equals(""))
                {
                    MessageBox.Show("请选择要查询的设备", "提示");
                }
                else if (comboBox1.Text.Equals(""))
                {
                    MessageBox.Show("请选择要查询的数据", "提示");
                }
                else if (comboBox3.Text.Equals(""))
                {
                    MessageBox.Show("请选择要查询的工作面", "提示");
                }
                else if (comboBox4.Text.Equals(""))
                {
                    MessageBox.Show("请选择要查询的设备号", "提示");
                }
                else
                {

                    string variable = "";//
                    int bz = 0;
                    if (comboBox1.Text.Equals("压力"))
                    {
                        variable = "yali";
                        bz = 0;
                    }

                    else if (comboBox1.Text.Equals("油温"))
                    {
                        variable = "youwen";
                        bz = 1;
                    }
                    else if (comboBox1.Text.Equals("油位"))
                    {
                        variable = "youya";
                        bz = 2;

                    }

                    else if (comboBox1.Text.Equals("低速轴承温度"))
                    {
                        variable = "youwei";
                        bz = 3;

                    }

                    else if (comboBox1.Text.Equals("高速轴承温度"))
                    {
                        variable = "zhoucheng";
                        bz = 4;

                    }

                    else if (comboBox1.Text.Equals("水压"))
                    {
                        variable = "shuiya";
                        bz = 5;

                    }

                    else if (comboBox1.Text.Equals("冷却水温度"))
                    {
                        variable = "lenque";
                        bz = 6;

                    }

                    else if (comboBox1.Text.Equals("冷却水流量"))
                    {
                        variable = "zaihe";
                        bz = 7;

                    }

                    else if (comboBox1.Text.Equals("振动频率"))
                    {
                        variable = "zhengdong";
                        bz = 8;

                    }
                    else if (comboBox1.Text.Equals("电量"))
                    {
                        variable = "";
                        bz = 9;

                    }

                    string id = selectID(comboBox3.Text);
                    DateTime time_start = new DateTime();
                    DateTime time_end = new DateTime();
                    string start = "";
                    string end = "";

                    try
                    {
                        time_start = Convert.ToDateTime(dateTimePicker1.Text);
                        time_end = (Convert.ToDateTime(dateTimePicker2.Text)).AddDays(1);





                        if (DateTime.Compare(time_start, time_end) > 0)
                            MessageBox.Show("请输入正确的时间范围", "提示");

                        if (id.Equals("") == false)
                        {
                            if (id.Equals("error"))
                            {
                                MessageBox.Show("在查询工作面编号时数据库连接失败", "提示");
                            }
                            else
                            {
                                chart1.Series["变量"].LegendText = comboBox1.Text;

                                //                //设置坐标轴名称
                                chart1.ChartAreas["ChartArea1"].AxisX.Title = "时间";
                                switch (comboBox1.Text)
                                {
                                    case "压力":
                                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "Mpa";
                                        break;
                                    case "油位":
                                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "mm（毫米）";
                                        break;
                                    case "轴承温度":
                                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "摄氏度";
                                        break;
                                    case "冷却水温度":
                                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "摄氏度";
                                        break;
                                    case "油温":
                                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "摄氏度";
                                        break;
                                    case "电量":
                                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "%";
                                        break;
                                }
                                //验证时间格式
                                //string[] arr = time_start.ToString().Split('-');
                                string[] arr = time_start.ToString().Split('-');
                                string[] arr1 = time_end.ToString().Split('-');
                                //MessageBox.Show("长度：" + arr.Length);
                                if (arr.Length == 3)//说明格式不正确
                                {


                                    start = arr[0] + '/' + arr[1] + '/' + arr[2];
                                    end = arr1[0] + '/' + arr1[1] + '/' + arr1[2];
                                }
                                else
                                {
                                    start = time_start.ToString("yyyy/MM/dd HH:mm:ss");
                                    end = time_end.ToString("yyyy/MM/dd HH:mm:ss");

                                }
                                if (bz == 0)
                                {
                                    string youya = "压力";
                                    string sql1 = "select * from dsensor where workid = " + id + " and Eid = " + comboBox4.Text + " and Sname = '" + youya + "'";
                                    string sid = "0";
                                    MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                                    while (rd1.Read())
                                    {
                                        sid = rd1["Sid"].ToString();
                                    }
                                    msc.Close();
                                    rd1.Close();
                                    try
                                    {
                                        string sql = "select * from sensordata where workid = " + id + " and Eid = " + comboBox4.Text + " and Sid = " + sid + " and time between '" + start + "' and '" + end + "' order by time asc";

                                        //MessageBox.Show("sql" + sql);
                                        string value = "value";
                                        MySqlDataReader rd = msc.getDataFromTable(sql);
                                        while (rd.Read())
                                        {
                                            if (rd["value"].ToString().Equals("") == false)
                                            {
                                                if (rd["value"].ToString().Equals("-1"))
                                                    continue;
                                                string d = rd["time"].ToString();
                                                String date_str = d.ToString();
                                                String[] date_list = date_str.Split(' ');

                                                if (date_list.Length == 2)
                                                {

                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                else
                                                {
                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                //MessageBox.Show(d.ToString("G"));
                                            }
                                        }
                                        rd.Close();
                                        msc.Close();
                                    }
                                    catch (FormatException ex)
                                    {
                                        Log("错误" + ex);
                                    }


                                }

                                else if (bz == 1)
                                {
                                    string youya = "油温";
                                    string sql1 = "select * from dsensor where workid = " + id + " and Eid = " + comboBox4.Text + " and Sname = '" + youya + "'";
                                    string sid = "0";
                                    MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                                    while (rd1.Read())
                                    {
                                        sid = rd1["Sid"].ToString();
                                    }
                                    msc.Close();
                                    rd1.Close();
                                    try
                                    {
                                        string sql = "select * from sensordata where workid = " + id + " and Eid = " + comboBox4.Text + " and Sid = " + sid + " and time between '" + start + "' and '" + end + "' order by time asc";

                                        //MessageBox.Show("sql" + sql);
                                        string value = "value";
                                        MySqlDataReader rd = msc.getDataFromTable(sql);
                                        while (rd.Read())
                                        {
                                            if (rd["value"].ToString().Equals("") == false)
                                            {
                                                if (rd["value"].ToString().Equals("-1"))
                                                    continue;
                                                string d = rd["time"].ToString();
                                                String date_str = d.ToString();
                                                String[] date_list = date_str.Split(' ');

                                                if (date_list.Length == 2)
                                                {

                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                else
                                                {
                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                //MessageBox.Show(d.ToString("G"));
                                            }
                                        }
                                        rd.Close();
                                        msc.Close();
                                    }
                                    catch (FormatException ex)
                                    {
                                        Log("错误" + ex);
                                    }


                                }
                                else if (bz == 2)
                                {
                                    string youya = "低速轴承温度";
                                    string sql1 = "select * from dsensor where workid = " + id + " and Eid = " + comboBox4.Text + " and Sname = '" + youya + "'";
                                    string sid = "0";
                                    MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                                    while (rd1.Read())
                                    {
                                        sid = rd1["Sid"].ToString();
                                    }
                                    msc.Close();
                                    rd1.Close();
                                    try
                                    {
                                        string sql = "select * from sensordata where workid = " + id + " and Eid = " + comboBox4.Text + " and Sid = " + sid + " and time between '" + start + "' and '" + end + "' order by time asc";

                                        //MessageBox.Show("sql" + sql);
                                        string value = "value";
                                        MySqlDataReader rd = msc.getDataFromTable(sql);
                                        while (rd.Read())
                                        {
                                            if (rd["value"].ToString().Equals("") == false)
                                            {
                                                if (rd["value"].ToString().Equals("-1"))
                                                    continue;
                                                string d = rd["time"].ToString();
                                                String date_str = d.ToString();
                                                String[] date_list = date_str.Split(' ');


                                                if (date_list.Length == 2)
                                                {

                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), Math.Round(f, 2));
                                                }
                                                else
                                                {
                                                    double f = double.Parse(rd[value].ToString());
                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), Math.Round(f, 2));
                                                }
                                                //MessageBox.Show(d.ToSt
                                                //MessageBox.Show(d.ToString("G"));
                                            }
                                        }
                                        rd.Close();
                                        msc.Close();
                                    }
                                    catch (FormatException ex)
                                    {
                                        Log("错误" + ex);
                                    }


                                }
                                else if (bz == 3)
                                {
                                    string youya = "高速轴承温度";
                                    string sql1 = "select * from dsensor where workid = " + id + " and Eid = " + comboBox4.Text + " and Sname = '" + youya + "'";
                                    string sid = "0";
                                    MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                                    while (rd1.Read())
                                    {
                                        sid = rd1["Sid"].ToString();
                                    }
                                    msc.Close();
                                    rd1.Close();
                                    try
                                    {
                                        string sql = "select * from sensordata where workid = " + id + " and Eid = " + comboBox4.Text + " and Sid = " + sid + " and time between '" + start + "' and '" + end + "' order by time asc";

                                        //MessageBox.Show("sql" + sql);
                                        string value = "value";
                                        MySqlDataReader rd = msc.getDataFromTable(sql);
                                        while (rd.Read())
                                        {
                                            if (rd["value"].ToString().Equals("") == false)
                                            {
                                                if (rd["value"].ToString().Equals("-1"))
                                                    continue;
                                                string d = rd["time"].ToString();
                                                String date_str = d.ToString();
                                                String[] date_list = date_str.Split(' ');

                                                if (date_list.Length == 2)
                                                {

                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                else
                                                {
                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                //MessageBox.Show(d.ToSt
                                                //MessageBox.Show(d.ToString("G"));
                                            }
                                        }
                                        rd.Close();
                                        msc.Close();
                                    }
                                    catch (FormatException ex)
                                    {
                                        Log("错误" + ex);
                                    }


                                }
                                else if (bz == 4)
                                {
                                    string youya = "水压";
                                    string sql1 = "select * from dsensor where workid = " + id + " and Eid = " + comboBox4.Text + " and Sname = '" + youya + "'";
                                    string sid = "0";
                                    MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                                    while (rd1.Read())
                                    {
                                        sid = rd1["Sid"].ToString();
                                    }
                                    msc.Close();
                                    rd1.Close();
                                    try
                                    {
                                        string sql = "select * from sensordata where workid = " + id + " and Eid = " + comboBox4.Text + " and Sid = " + sid + " and time between '" + start + "' and '" + end + "' order by time asc";

                                        //MessageBox.Show("sql" + sql);
                                        string value = "value";
                                        MySqlDataReader rd = msc1.getDataFromTable(sql);
                                        while (rd.Read())
                                        {
                                            if (rd["value"].ToString().Equals("") == false)
                                            {
                                                if (rd["value"].ToString().Equals("-1"))
                                                    continue;
                                                string d = rd["time"].ToString();
                                                String date_str = d.ToString();
                                                String[] date_list = date_str.Split(' ');

                                                if (date_list.Length == 2)
                                                {

                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                else
                                                {
                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                //MessageBox.Show(d.ToSt
                                                //MessageBox.Show(d.ToString("G"));
                                            }
                                        }
                                        rd.Close();
                                        msc1.Close();
                                    }
                                    catch (FormatException ex)
                                    {
                                        Log("错误" + ex);
                                    }



                                }
                                else if (bz == 5)
                                {
                                    string youya = "冷却水温度";
                                    string sql1 = "select * from dsensor where workid = " + id + " and Eid = " + comboBox4.Text + " and Sname = '" + youya + "'";
                                    string sid = "0";
                                    MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                                    while (rd1.Read())
                                    {
                                        sid = rd1["Sid"].ToString();
                                    }
                                    msc.Close();
                                    rd1.Close();
                                    try
                                    {
                                        string sql = "select * from sensordata where workid = " + id + " and Eid = " + comboBox4.Text + " and Sid = " + sid + " and time between '" + start + "' and '" + end + "' order by time asc";

                                        //MessageBox.Show("sql" + sql);
                                        string value = "value";
                                        MySqlDataReader rd = msc.getDataFromTable(sql);
                                        while (rd.Read())
                                        {
                                            if (rd["value"].ToString().Equals("") == false)
                                            {
                                                if (rd["value"].ToString().Equals("-1"))
                                                    continue;
                                                string d = rd["time"].ToString();
                                                String date_str = d.ToString();
                                                String[] date_list = date_str.Split(' ');

                                                if (date_list.Length == 2)
                                                {

                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                else
                                                {
                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                //MessageBox.Show(d.ToSt
                                                //MessageBox.Show(d.ToString("G"));
                                            }
                                        }
                                        rd.Close();
                                        msc.Close();
                                    }
                                    catch (FormatException ex)
                                    {
                                        Log("错误" + ex);
                                    }


                                }
                                else if (bz == 6)
                                {
                                    string youya = "冷却水流量";
                                    string sql1 = "select * from dsensor where workid = " + id + " and Eid = " + comboBox4.Text + " and Sname = '" + youya + "'";
                                    string sid = "0";
                                    MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                                    while (rd1.Read())
                                    {
                                        sid = rd1["Sid"].ToString();
                                    }
                                    msc.Close();
                                    rd1.Close();
                                    try
                                    {
                                        string sql = "select * from sensordata where workid = " + id + " and Eid = " + comboBox4.Text + " and Sid = " + sid + " and time between '" + start + "' and '" + end + "' order by time asc";

                                        //MessageBox.Show("sql" + sql);
                                        string value = "value";
                                        MySqlDataReader rd = msc.getDataFromTable(sql);
                                        while (rd.Read())
                                        {
                                            if (rd["value"].ToString().Equals("") == false)
                                            {
                                                if (rd["value"].ToString().Equals("-1"))
                                                    continue;
                                                string d = rd["time"].ToString();
                                                String date_str = d.ToString();
                                                String[] date_list = date_str.Split(' ');

                                                if (date_list.Length == 2)
                                                {

                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                else
                                                {
                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                //MessageBox.Show(d.ToSt
                                                //MessageBox.Show(d.ToString("G"));
                                            }
                                        }
                                        rd.Close();
                                        msc.Close();
                                    }
                                    catch (FormatException ex)
                                    {
                                        Log("错误" + ex);
                                    }


                                }
                                else if (bz == 7)
                                {
                                    string youya = "振动频率";
                                    string sql1 = "select * from dsensor where workid = " + id + " and Eid = " + comboBox4.Text + " and Sname = '" + youya + "'";
                                    string sid = "0";
                                    MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                                    while (rd1.Read())
                                    {
                                        sid = rd1["Sid"].ToString();
                                    }
                                    msc.Close();
                                    rd1.Close();
                                    try
                                    {
                                        string sql = "select * from sensordata where workid = " + id + " and Eid = " + comboBox4.Text + " and Sid = " + sid + " and time between '" + start + "' and '" + end + "' order by time asc";

                                        //MessageBox.Show("sql" + sql);
                                        string value = "value";
                                        MySqlDataReader rd = msc.getDataFromTable(sql);
                                        while (rd.Read())
                                        {
                                            if (rd["value"].ToString().Equals("") == false)
                                            {
                                                if (rd["value"].ToString().Equals("-1"))
                                                    continue;
                                                string d = rd["time"].ToString();
                                                String date_str = d.ToString();
                                                String[] date_list = date_str.Split(' ');

                                                if (date_list.Length == 2)
                                                {

                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                else
                                                {
                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                                }
                                                //MessageBox.Show(d.ToSt
                                                //MessageBox.Show(d.ToString("G"));
                                            }
                                        }
                                        rd.Close();
                                        msc.Close();
                                    }
                                    catch (FormatException ex)
                                    {
                                        Log("错误" + ex);
                                    }


                                }
                                else if (bz == 8)
                                {
                                    string youya = "油位";
                                    string sql1 = "select * from dsensor where workid = " + id + " and Eid = " + comboBox4.Text + " and Sname = '" + youya + "'";
                                    string sid = "0";
                                    MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                                    while (rd1.Read())
                                    {
                                        sid = rd1["Sid"].ToString();
                                    }
                                    msc.Close();
                                    rd1.Close();
                                    try
                                    {
                                        string sql = "select * from sensordata where workid = " + id + " and Eid = " + comboBox4.Text + " and Sid = " + sid + " and time between '" + start + "' and '" + end + "' order by time asc";

                                        //MessageBox.Show("sql" + sql);
                                        string value = "value";
                                        MySqlDataReader rd = msc1.getDataFromTable(sql);
                                        while (rd.Read())
                                        {
                                            if (rd["value"].ToString().Equals("") == false)
                                            {
                                                if (rd["value"].ToString().Equals("-1"))
                                                    continue;
                                                string d = rd["time"].ToString();
                                                String date_str = d.ToString();
                                                String[] date_list = date_str.Split(' ');

                                                if (date_list.Length == 2)
                                                {

                                                    double f = double.Parse(rd[value].ToString());

                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), Math.Round(f, 2));
                                                }
                                                else
                                                {
                                                    double f = double.Parse(rd[value].ToString());
                                                    chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), Math.Round(f, 2));
                                                }
                                                //MessageBox.Show(d.ToString("G"));
                                            }
                                        }
                                        rd.Close();
                                        msc1.Close();
                                    }
                                    catch (FormatException ex)
                                    {
                                        Log("错误" + ex);
                                    }



                                }
                                else if (bz == 9)
                                {
                                    string sql = "select * from sensordata where workid = " + id + " and Eid = " + comboBox4.Text + "  and time between '" + start + "' and '" + end + "' order by time asc";

                                    //MessageBox.Show("sql" + sql);
                                    string value = "value";
                                    MySqlDataReader rd = msc.getDataFromTable(sql);
                                    while (rd.Read())
                                    {
                                       
                                            DateTime d = DateTime.Parse(rd["time"].ToString());
                                            String date_str = d.ToString();
                                            String[] date_list = date_str.Split(' ');

                                            if (date_list.Length == 2)
                                            {
                                                double f;

                                                f = double.Parse(rd["Electric"].ToString());




                                                chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (int)f);
                                            }
                                            else
                                            {
                                                double f;
                                                if (comboBox1.Text.Equals("电量"))
                                                {
                                                    f = double.Parse(rd["Electric"].ToString());
                                                }
                                                else
                                                {
                                                    f = double.Parse(rd[value].ToString());

                                                }

                                                chart1.Series["变量"].Points.AddXY(d.ToString("yyyy/MM/dd HH:mm:ss"), (int)f);
                                            }
                                            //MessageBox.Show(d.ToString("G"));
                                       
                                    }
                                    rd.Close();
                                    msc.Close();

                                }
                                if (chart1.Series["变量"].Points.Count == 0)
                                {
                                    if (DateTime.Compare(time_start, time_end) < 0)
                                        MessageBox.Show("没有数据可供显示", "提示");
                                }
                            }


                        }
                        else
                        {
                            MessageBox.Show("没有此工作面，请重新输入", "提示");
                        }

                    }
                    catch (FormatException exc)
                    {
                        MessageBox.Show(exc.ToString());
                        MessageBox.Show("时间格式输入有误", "提示");
                    }


                }
            }
            catch(Exception ced)
            {
                Log("原因   " + ced);
            }
        }

        private void Chart_Load(object sender, EventArgs e)
        {
            //comboBox2属性设置
            Thread fac_thread = new Thread(getFactory);
            fac_thread.Start();

            //ChartArea1属性设置
            //设置网格颜色
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineColor = Color.LightGray;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.LightGray;
            //设置坐标轴名称
            chart1.ChartAreas["ChartArea1"].AxisX.Title = "变量";
            chart1.ChartAreas["ChartArea1"].AxisY.Title = "数值";
            //启用变量显示
            chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;

            //series属性设置
            //设置显示类型--线性
            chart1.Series["变量"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            //设置坐标轴Value显示类型
            chart1.Series["变量"].XValueType = ChartValueType.Time;
            //是否显示标签的数值
            chart1.Series["变量"].IsValueShownAsLabel = true;

            //设置标记图案
            chart1.Series["变量"].MarkerStyle = MarkerStyle.Circle;
            //设置图案颜色
            chart1.Series["变量"].Color = Color.Blue;
            //设置图案的宽度
            chart1.Series["变量"].BorderWidth = 3;
            chart1.ChartAreas[0].AxisX.Interval = 1;   //设置X轴坐标的间隔为1
            chart1.ChartAreas[0].AxisX.IntervalOffset = 1;  //设置X轴坐标偏移为1
            chart1.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;   //设置是否交错显示,比如数据多的时间分成两行来显示

            


            chart1.ChartAreas["ChartArea1"].AxisX.ScrollBar.IsPositionedInside = false;//设置滚动条是在外部显示

            chart1.ChartAreas["ChartArea1"].AxisX.ScrollBar.Size = 20;//设置滚动条的宽度

            chart1.ChartAreas["ChartArea1"].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;//滚动条只显示向前的按钮，主要是为了不显示取消显示的按钮

            chart1.ChartAreas["ChartArea1"].AxisX.ScaleView.Size = 10;//设置图表可视区域数据点数，说白了一次可以看到多少个X轴区域

            chart1.ChartAreas["ChartArea1"].AxisX.ScaleView.MinSize = 1;//设置滚动一次，移动几格区域

            chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;//设置X轴的间隔，设置它是为了看起来方便点，也就是要每个X轴的记录都显示出来

            //chart1.ChartAreas["ChartArea1"].AxisX.Minimum = 1;//X轴起始点
            //chart1.ChartAreas["ChartArea1"].AxisX.Maximum = 100;//X轴结束点，一般这个是应该在后台设置的，
            //对于我而言，是用的第一列作为X轴，那么有多少行，就有多少个X轴的刻度，所以最大值应该就等于行数；

            //该值设置大了，会在后边出现一推空白，设置小了，会出后边多出来的数据在图表中不显示，所以最好是在后台根据你的数据列来设置.

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
                string sql = "select * from equiptype";
                msc.Close();
                rd1.Close();
                //DataBase db = new DataBase();
                //db.command.CommandText = sql;
                //db.command.Connection = db.connection;
                //db.Dr = db.command.ExecuteReader();
                MySqlDataReader rd = msc.getDataFromTable(sql);
                while (rd.Read())
                {
                    comboBox2.Items.Add(rd["Ename"].ToString());
                }
                rd.Close();
                msc.Close();

                if (comboBox2.Items.Count > 0)
                {
                    comboBox2.Text = comboBox2.Items[0].ToString();
                }
              
                //string sql2 = "select equipment.Eid from equipment where equipment.Etype in(1) AND workid in(1)";

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


        private void Chart_FormClosed(object sender, FormClosedEventArgs e)
        {

            this.Dispose();
        }

        private void Chart_FormClosing(object sender, FormClosingEventArgs e)
        {
            chart1.Series[0].Points.Clear();
            this.Visible = false;
            e.Cancel = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {



        }

        
      

        void Log(string str)    // 记录服务启动  
        {
            file_mutex.WaitOne();
            {
                string info = string.Format("{0}-{1}", DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"), str);
                string path = "C://Mqtt//Maaaaa.txt";//"C://Mqtt//Maaaa.txt"
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(info);
                    //关闭
                    sw.Close();
                }
            }
            file_mutex.ReleaseMutex();

        }

        private void N2(object sender, FormatNumberEventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string sql4 = "select workid from work where workname =" + "'" + comboBox3.Text + "'";

            try
            {
                int workbzid = 5;
                MySqlDataReader rd4 = msc.getDataFromTable(sql4);
                while (rd4.Read())
                {
                    workbzid = int.Parse(rd4["workid"].ToString());
                    //MessageBox.Show("===" + workbzid);

                }

                rd4.Close();
                msc.Close();
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
                string sql5 = "select equipment.Eid from equipment where equipment.Etype = " + "'" + ebzid + "' AND workid = " + "'" + workbzid + "'";

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

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            if (comboBox2.Text.Equals("压力计"))
            {
                comboBox1.Items.Clear();
                comboBox1.Items.Add("压力");
                comboBox1.Items.Add("电量");
                comboBox1.Text = comboBox1.Items[0].ToString();

            }
            else if (comboBox2.Text.Equals("减速机"))
            {
                comboBox1.Items.Clear();
                comboBox1.Text = "";
                string sql1 = "select * from type where bzid != 0";

                MySqlDataReader rd1 = msc.getDataFromTable(sql1);
                while (rd1.Read())
                {
                    comboBox1.Items.Add(rd1["type"].ToString());
                }
                rd1.Close();
                msc.Close();
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.Text = comboBox1.Items[0].ToString();
                }
            }
            else
            {
                comboBox1.Items.Clear();
            }
            string sql4 = "select workid from work where workname =" + "'" + comboBox3.Text + "'";

            try
            {
                int workbzid = 5;
                MySqlDataReader rd4 = msc.getDataFromTable(sql4);
                while (rd4.Read())
                {
                    workbzid = int.Parse(rd4["workid"].ToString());
                    //MessageBox.Show("===" + workbzid);

                }

                rd4.Close();
                msc.Close();
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
                string sql5 = "select equipment.Eid from equipment where equipment.Etype = " + "'" + ebzid + "' AND workid = " + "'" + workbzid + "'order by Eid asc";

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

        private void skinComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
