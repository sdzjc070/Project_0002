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
using static kuangjing.MainForm;

namespace kuangjing
{
    public partial class shishixianshi : CCSkinMain
    {
        public shishixianshi()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }
        MySqlConn msc = new MySqlConn();
        private Mutex file_mutex = new Mutex();//文件互斥锁
        string variable = "";//
        string  bz = "";
        string w1 = "";
        string shebei1 = "";



        /// <summary>
        /// 根据BinName找BinID
        /// </summary>
        /// <returns></returns>
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
        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }
        void Log1(string str)    // 记录服务启动  
        {
            file_mutex.WaitOne();
            {
                string info = string.Format("{0}-{1}", DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"), str);
                string path = "C://Mqtt//jieshpou.txt";//"C://Mqtt//MyTestLog.txt"
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(info);
                    //关闭
                    sw.Close();
                }
            }
            file_mutex.ReleaseMutex();

        }
        string abc ="";
        private void button1_Click(object sender, EventArgs e)
        {
            
            abc = comboBox2.Text;
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
            
            else
            {
                
                if (comboBox1.Text.Equals("压力"))
                {
                    variable = "yali";
                    bz = "压力";
                }

                else if (comboBox1.Text.Equals("油温"))
                {
                    variable = "youwen";
                    bz = "油温";
                }
                else if (comboBox1.Text.Equals("油压"))
                {
                    variable = "youya";
                    bz = "油压";

                }

                else if (comboBox1.Text.Equals("油位"))
                {
                    variable = "youwei";
                    bz = "油位";

                }

                else if (comboBox1.Text.Equals("轴承温度"))
                {
                    variable = "zhoucheng";
                    bz = "轴承温度";

                }

                else if (comboBox1.Text.Equals("水压"))
                {
                    variable = "shuiya";
                    bz = "水压";

                }

                else if (comboBox1.Text.Equals("冷却水温度"))
                {
                    variable = "lenque";
                    bz = "冷却水温度";

                }

                else if (comboBox1.Text.Equals("载荷"))
                {
                    variable = "zaihe";
                    bz = "载荷";

                }

                else if (comboBox1.Text.Equals("振动"))
                {
                    variable = "zhengdong";
                    bz = "振动";

                }
                w1 = comboBox3.Text;
                shebei1 = comboBox2.Text;
                

                try
                {


                    chart1.Series[0].Points.Clear();
                    string id = selectID(w1);
                    DateTime time_start = new DateTime();
                    DateTime time_end = new DateTime();
                    string start = "";
                    string end = "";
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
                            chart1.ChartAreas["ChartArea1"].AxisX.Title = "设备";
                            switch (comboBox1.Text)
                            {
                                case "压力":
                                    chart1.ChartAreas["ChartArea1"].AxisY.Title = "Mpa";
                                    break;
                                    //                    case "体积":
                                    //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "立方米";
                                    //                        break;
                                    //                    case "湿度":
                                    //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "百分比";
                                    //                        break;
                                    //                    case "重量":
                                    //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "吨";
                                    //                        break;
                            }
                            //验证时间格式
                            //string[] arr = time_start.ToString().Split('-');
                            string[] arr = time_start.ToString().Split('-');
                            string[] arr1 = time_end.ToString().Split('-');
                            //MessageBox.Show("长度：" + arr.Length);

                            string sql = "select * from  ( select sensordata.Eid,`value`,sensordata.time from sensordata,dsensor where sensordata.workid =" + id + " and sensordata.Eid =dsensor.Eid and dsensor.Sname ='" + comboBox1.Text + "' order by time desc) a group by Eid";

                            //MessageBox.Show("sql" + sql);
                            string value = "value";
                            MySqlDataReader rd = msc.getDataFromTable(sql);
                            while (rd.Read())
                            {
                                if (rd["value"].ToString().Equals("") == false)
                                {
                                    if (rd["value"].ToString().Equals("-1"))
                                        continue;
                                    int d = int.Parse(rd["Eid"].ToString());
                                    string time = rd["time"].ToString();
                                    String date_str = d.ToString() + "号设备   " + time;
                                    String[] date_list = date_str.Split(' ');
                                    int bz = 0;
                                    if(abc=="减速机")
                                    {
                                        if(jiansuji1[d]==1)
                                        {
                                            bz = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (yaliji[d] == 1)
                                        {
                                            bz = 1;
                                        }
                                    }
                                    if(bz==1)
                                    {
                                        if (date_list.Length == 2)
                                        {
                                            double f = double.Parse(rd[value].ToString());

                                            chart1.Series["变量"].Points.AddXY(date_str.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                        }
                                        else
                                        {
                                            double f = double.Parse(rd[value].ToString());

                                            chart1.Series["变量"].Points.AddXY(date_str.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                        }
                                    }
                                    
                                    //MessageBox.Show(d.ToString("G"));
                                }
                            }
                            rd.Close();
                            msc.Close();
                            if (chart1.Series["变量"].Points.Count == 0)
                            {
                                if (DateTime.Compare(time_start, time_end) < 0)
                                    MessageBox.Show("没有数据可供显示", "提示");
                            }
                        }

                    }

                    if (button1.Text == "停止")
                    {
                        button1.Text = "开始";
                        timer1.Enabled = false;
                        chart1.Series[0].Points.Clear();

                    }
                    else
                    {
                        button1.Text = "停止";
                        timer1.Enabled = true;
                        timer1.Interval = 4000;
                    }




                }
                catch (FormatException exc)
                {
                    MessageBox.Show(exc.ToString());
                    MessageBox.Show("时间格式输入有误", "提示");
                }


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
            chart1.Series["变量"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
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
            //chart1.ChartAreas[0].AxisX.TitleFont
            chart1.ChartAreas[0].AxisX.LabelStyle.IsStaggered = false;   //设置是否交错显示,比如数据多的时间分成两行来显示




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
                msc.Close();
                if (comboBox3.Items.Count > 0)
                {
                    comboBox3.Text = comboBox3.Items[0].ToString();
                }
                string sql = "select * from equiptype";
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
                //comboBox4.Items.Clear();
                //while (rd2.Read())
                //{
                //    comboBox4.Items.Add(rd2["Eid"].ToString());
                //}
                //rd2.Close();
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
            chart1.Series.Clear();
            this.Dispose();
            
        }

        private void Chart_FormClosing(object sender, FormClosingEventArgs e)
        {
            chart1.Series[0].Points.Clear();
            this.Dispose();
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

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            if (comboBox2.Text.Equals("压力计"))
            {
                comboBox1.Items.Clear();
                comboBox1.Items.Add("压力");
                comboBox1.Text = comboBox1.Items[0].ToString();

            }
            else if (comboBox2.Text.Equals("减速机"))
            {
                comboBox1.Items.Clear();
                comboBox1.Text = "";
                string sql1 = "select * from type where bzid!=0";

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
                comboBox1.Text = "";
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

                string sql5 = "select equipment.Eid from equipment where equipment.Etype = " + "'" + ebzid + "' AND workid = " + "'" + workbzid + "'";


            }
            catch (Exception ex)
            {

            }
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

                string sql5 = "select equipment.Eid from equipment where equipment.Etype = " + "'" + ebzid + "' AND workid = " + "'" + workbzid + "'";


            }
            catch (Exception ex)
            {

            }

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void chart1_Paint(object sender, PaintEventArgs e)
        {

        }
        int t = 0;
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            
            try
            {


                chart1.Series[0].Points.Clear();
                string id = selectID(w1);
                DateTime time_start = new DateTime();
                DateTime time_end = new DateTime();
                string start = "";
                string end = "";
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
                        chart1.ChartAreas["ChartArea1"].AxisX.Title = "设备";
                        switch (comboBox1.Text)
                        {
                            case "压力":
                                chart1.ChartAreas["ChartArea1"].AxisY.Title = "Mpa";
                                break;
                                //                    case "体积":
                                //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "立方米";
                                //                        break;
                                //                    case "湿度":
                                //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "百分比";
                                //                        break;
                                //                    case "重量":
                                //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "吨";
                                //                        break;
                        }
                        //验证时间格式
                        //string[] arr = time_start.ToString().Split('-');
                        string[] arr = time_start.ToString().Split('-');
                        string[] arr1 = time_end.ToString().Split('-');
                        //MessageBox.Show("长度：" + arr.Length);

                        string sql = "select * from  ( select sensordata.Eid,`value`,sensordata.time from sensordata,dsensor where sensordata.workid =" + id + " and sensordata.Eid =dsensor.Eid and dsensor.Sname ='" + comboBox1.Text + "' order by time desc) a group by Eid";

                        //MessageBox.Show("sql" + sql);
                        string value = "value";
                        MySqlDataReader rd = msc.getDataFromTable(sql);
                        while (rd.Read())
                        {
                            if (rd["value"].ToString().Equals("") == false)
                            {
                                if (rd["value"].ToString().Equals("-1"))
                                    continue;
                                int d = int.Parse(rd["Eid"].ToString());
                                string time = rd["time"].ToString();
                                String date_str = d.ToString() + "号设备   " + time;
                                String[] date_list = date_str.Split(' ');
                                int bz = 0;
                                if (abc == "减速机")
                                {
                                    if (jiansuji1[d] == 1)
                                    {
                                        bz = 1;
                                    }
                                }
                                else
                                {
                                    if (yaliji[d] == 1)
                                    {
                                        bz = 1;
                                    }
                                }
                                if (bz == 1)
                                {
                                    if (date_list.Length == 2)
                                    {
                                        double f = double.Parse(rd[value].ToString());

                                        chart1.Series["变量"].Points.AddXY(date_str.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                    }
                                    else
                                    {
                                        double f = double.Parse(rd[value].ToString());

                                        chart1.Series["变量"].Points.AddXY(date_str.ToString("yyyy/MM/dd HH:mm:ss"), (double)f);
                                    }
                                }

                                //MessageBox.Show(d.ToString("G"));
                            }
                        }
                        rd.Close();
                        msc.Close();
                        if (chart1.Series["变量"].Points.Count == 0)
                        {
                            if (DateTime.Compare(time_start, time_end) < 0)
                                MessageBox.Show("没有数据可供显示", "提示");
                        }
                    }

                }

               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
              
                        msc.Close();
            }
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
    }
}
