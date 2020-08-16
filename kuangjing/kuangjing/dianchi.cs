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
    public partial class dianchi : CCSkinMain
    {
        public dianchi()
        {
            InitializeComponent();
        }
        MySqlConn msc = new MySqlConn();
        private Mutex file_mutex = new Mutex();//文件互斥锁
        string variable = "";//
        string bz = "";
        string w1 = "";
        string shebei1 = "";
        string bz1 = "";



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
        private void dianchi_Load(object sender, EventArgs e)
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
                comboBox2.Items.Add("显示全部设备");


                comboBox2.Items.Add("只显示压力计");
                comboBox2.Items.Add("只显示中继");


                if (comboBox2.Items.Count > 0)
                {
                    comboBox2.Text = comboBox2.Items[0].ToString();
                }

                

            }
            catch (SqlException se)
            {
                MessageBox.Show("数据库异常", "提示");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            w1= comboBox3.Text;
            bz1 = comboBox2.Text;


            if (comboBox2.Text=="显示全部设备")
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
                        chart1.Series["变量"].LegendText = "剩余电量百分比";

                        //                //设置坐标轴名称
                        chart1.ChartAreas["ChartArea1"].AxisX.Title =  "设备";
                        //switch (comboBox1.Text)
                        //{
                        //    case "压力":
                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "%";
                      
                        string[] arr = time_start.ToString().Split('-');
                        string[] arr1 = time_end.ToString().Split('-');
                        //MessageBox.Show("长度：" + arr.Length);

                        string sql = "select * from (select sensordata.Eid,equiptype.Ename,sensordata.Electric,sensordata.time from sensordata,equipment,equiptype where sensordata.workid=equipment.workid and sensordata.workid=" + id + " and sensordata.Etype !=2 and sensordata.Eid = equipment.Eid and equipment.Etype = equiptype.Etype and sensordata.Etype=equipment.Etype order by time desc)a group by Eid order by Ename desc";
                        
                        //MessageBox.Show("sql" + sql);
                        string value = "value";
                        
                        MySqlDataReader rd = msc.getDataFromTable(sql);
                        while (rd.Read())
                        {
                            if (rd["Electric"].ToString().Equals("") == false)
                            {
                                
                                string d = rd["Eid"].ToString();
                                int a1= int.Parse(d);
                                int qqq = 0;
                                string name = rd["Ename"].ToString();
                                if(name=="压力计")
                                {
                                    qqq = yaliji[a1];
                                }
                                else
                                {
                                    qqq = zhongji[a1];
                                }
                                string time = rd["time"].ToString();
                                String date_str = d+ "号" + name+""+time;
                                String[] date_list = date_str.Split(' ');

                                if (qqq == 1)
                                {
                                    
                                    float f = float.Parse(rd["Electric"].ToString());
                                    chart1.Series["变量"].Points.AddXY(date_str.ToString("yyyy/MM/dd HH:mm:ss"), (float)f);
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
            else if(comboBox2.Text=="只显示压力计")
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
                        chart1.Series["变量"].LegendText = "剩余电量百分比";

                        //                //设置坐标轴名称
                        chart1.ChartAreas["ChartArea1"].AxisX.Title = "设备";
                        //switch (comboBox1.Text)
                        //{
                        //    case "压力":
                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "%";
                        
                        string[] arr = time_start.ToString().Split('-');
                        string[] arr1 = time_end.ToString().Split('-');
                        //MessageBox.Show("长度：" + arr.Length);
                        string cd = "压力计";
                        string sql = "select * from (select sensordata.Eid,equiptype.Ename,sensordata.Electric,sensordata.time from sensordata,equipment,equiptype where sensordata.workid=equipment.workid and sensordata.workid=" + id + " and sensordata.Eid = equipment.Eid and equipment.Etype = equiptype.Etype and sensordata.Etype=equipment.Etype and Ename ='" + cd+"' order by time desc)a group by Eid order by Ename desc";

                        //MessageBox.Show("sql" + sql);
                        string value = "value";
                        MySqlDataReader rd = msc.getDataFromTable(sql);
                        while (rd.Read())
                        {
                            if (rd["Electric"].ToString().Equals("") == false)
                            {
                                
                                string d = rd["Eid"].ToString();
                                int a1 = int.Parse(d);
                                string name = rd["Ename"].ToString();
                                string time = rd["time"].ToString();
                                String date_str = d + "号" + name + "  " + time;
                                String[] date_list = date_str.Split(' ');

                                if (yaliji[a1] == 1)
                                {
                                    
                                    float f = float.Parse(rd["Electric"].ToString());
                                    chart1.Series["变量"].Points.AddXY(date_str.ToString("yyyy/MM/dd HH:mm:ss"), (float)f);
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
            else
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
                        chart1.Series["变量"].LegendText = "剩余电量百分比";

                        //                //设置坐标轴名称
                        chart1.ChartAreas["ChartArea1"].AxisX.Title = "设备";
                        //switch (comboBox1.Text)
                        //{
                        //    case "压力":
                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "%";
                        //        break;
                        //        //                    case "体积":
                        //        //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "立方米";
                        //        //                        break;
                        //        //                    case "湿度":
                        //        //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "百分比";
                        //        //                        break;
                        //        //                    case "重量":
                        //        //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "吨";
                        //        //                        break;
                        //}
                        //验证时间格式
                        //string[] arr = time_start.ToString().Split('-');
                        string[] arr = time_start.ToString().Split('-');
                        string[] arr1 = time_end.ToString().Split('-');
                        //MessageBox.Show("长度：" + arr.Length);
                        string cd = "中继";
                        string sql = "select * from (select sensordata.Eid,equiptype.Ename,sensordata.Electric,sensordata.time from sensordata,equipment,equiptype where sensordata.workid=equipment.workid and sensordata.Etype=equipment.Etype and sensordata.workid=" + id + " and sensordata.Eid = equipment.Eid and equipment.Etype = equiptype.Etype and Ename ='" + cd + "' order by time desc)a group by Eid order by Ename desc";

                        //MessageBox.Show("sql" + sql);
                        string value = "value";
                        MySqlDataReader rd = msc.getDataFromTable(sql);
                        while (rd.Read())
                        {
                            if (rd["Electric"].ToString().Equals("") == false)
                            {
                                string d = rd["Eid"].ToString();
                                int a1 = int.Parse(d);
                                string name = rd["Ename"].ToString();
                                string time = rd["time"].ToString();
                                String date_str = d + "号" + name + "  " + time;
                                String[] date_list = date_str.Split(' ');

                                if (zhongji[a1] == 1)
                                {
                                    
                                    float f = float.Parse(rd["Electric"].ToString());
                                    chart1.Series["变量"].Points.AddXY(date_str.ToString("yyyy/MM/dd HH:mm:ss"), (float)f);
                                }

                                
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

        private void dianchi_FormClosing(object sender, FormClosingEventArgs e)
        {
            chart1.Series[0].Points.Clear();
            this.Dispose();
            this.Visible = false;
            e.Cancel = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            this.Dispose();
            this.Visible = false;
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bz1 == "显示全部设备")
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
                        chart1.Series["变量"].LegendText = "剩余电量百分比";

                        //                //设置坐标轴名称
                        chart1.ChartAreas["ChartArea1"].AxisX.Title = "设备";
                        //switch (comboBox1.Text)
                        //{
                        //    case "压力":
                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "%";
                        //        break;
                        //        //                    case "体积":
                        //        //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "立方米";
                        //        //                        break;
                        //        //                    case "湿度":
                        //        //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "百分比";
                        //        //                        break;
                        //        //                    case "重量":
                        //        //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "吨";
                        //        //                        break;
                        //}
                        //验证时间格式
                        //string[] arr = time_start.ToString().Split('-');
                        string[] arr = time_start.ToString().Split('-');
                        string[] arr1 = time_end.ToString().Split('-');
                        //MessageBox.Show("长度：" + arr.Length);

                        string sql = "select * from (select sensordata.Eid,equiptype.Ename,sensordata.Electric,sensordata.time from sensordata,equipment,equiptype where sensordata.workid=equipment.workid and sensordata.workid=" + id + " and sensordata.Etype !=2 and sensordata.Eid = equipment.Eid and equipment.Etype = equiptype.Etype and sensordata.Etype=equipment.Etype order by time desc)a group by Eid order by Ename desc";

                        //MessageBox.Show("sql" + sql);
                        string value = "value";
                        MySqlDataReader rd = msc.getDataFromTable(sql);
                        while (rd.Read())
                        {
                            if (rd["Electric"].ToString().Equals("") == false)
                            {

                                string d = rd["Eid"].ToString();
                                int a1 = int.Parse(d);
                                string name = rd["Ename"].ToString();
                                string time = rd["time"].ToString();
                                String date_str = d + "号" + name + "" + time;
                                String[] date_list = date_str.Split(' ');
                                int qqq = 0;
                                if(name=="压力计")
                                {
                                    qqq = yaliji[a1];
                                }
                                else
                                {
                                    qqq = zhongji[a1];
                                }
                                if (qqq == 1)
                                {

                                    float f = float.Parse(rd["Electric"].ToString());
                                    chart1.Series["变量"].Points.AddXY(date_str.ToString("yyyy/MM/dd HH:mm:ss"), (float)f);
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
            else if (bz1 == "只显示压力计")
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
                        chart1.Series["变量"].LegendText = "剩余电量百分比";

                        //                //设置坐标轴名称
                        chart1.ChartAreas["ChartArea1"].AxisX.Title = "设备";
                        //switch (comboBox1.Text)
                        //{
                        //    case "压力":
                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "%";
                        //        break;
                        //        //                    case "体积":
                        //        //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "立方米";
                        //        //                        break;
                        //        //                    case "湿度":
                        //        //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "百分比";
                        //        //                        break;
                        //        //                    case "重量":
                        //        //                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "吨";
                        //        //                        break;
                        //}
                        //验证时间格式
                        //string[] arr = time_start.ToString().Split('-');
                        string[] arr = time_start.ToString().Split('-');
                        string[] arr1 = time_end.ToString().Split('-');
                        //MessageBox.Show("长度：" + arr.Length);
                        string cd = "压力计";
                        string sql = "select * from (select sensordata.Eid,equiptype.Ename,sensordata.Electric,sensordata.time from sensordata,equipment,equiptype where sensordata.workid=equipment.workid and sensordata.workid=" + id + " and sensordata.Eid = equipment.Eid and equipment.Etype = equiptype.Etype and sensordata.Etype=equipment.Etype and Ename ='" + cd + "' order by time desc)a group by Eid order by Ename desc";

                        //MessageBox.Show("sql" + sql);
                        string value = "value";
                        MySqlDataReader rd = msc.getDataFromTable(sql);
                        while (rd.Read())
                        {
                            if (rd["Electric"].ToString().Equals("") == false)
                            {

                                string d = rd["Eid"].ToString();
                                int a1 = int.Parse(d);
                                string name = rd["Ename"].ToString();
                                string time = rd["time"].ToString();
                                String date_str = d + "号" + name + "  " + time;
                                String[] date_list = date_str.Split(' ');

                                if (yaliji[a1] == 1)
                                {
                                    
                                    float f = float.Parse(rd["Electric"].ToString());
                                    chart1.Series["变量"].Points.AddXY(date_str.ToString("yyyy/MM/dd HH:mm:ss"), (float)f);
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
            else
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
                        chart1.Series["变量"].LegendText = "剩余电量百分比";

                        //                //设置坐标轴名称
                        chart1.ChartAreas["ChartArea1"].AxisX.Title = "设备";
                        //switch (comboBox1.Text)
                        //{
                        //    case "压力":
                        chart1.ChartAreas["ChartArea1"].AxisY.Title = "%";
                        
                        string[] arr = time_start.ToString().Split('-');
                        string[] arr1 = time_end.ToString().Split('-');
                        //MessageBox.Show("长度：" + arr.Length);
                        string cd = "中继";
                        string sql = "select * from (select sensordata.Eid,equiptype.Ename,sensordata.Electric,sensordata.time from sensordata,equipment,equiptype where sensordata.workid=equipment.workid and sensordata.Etype=equipment.Etype and sensordata.workid=" + id + " and sensordata.Eid = equipment.Eid and equipment.Etype = equiptype.Etype and Ename ='" + cd + "' order by time desc)a group by Eid order by Ename desc";

                       
                        string value = "value";
                        MySqlDataReader rd = msc.getDataFromTable(sql);
                        while (rd.Read())
                        {
                            if (rd["Electric"].ToString().Equals("") == false)
                            {
                                string d = rd["Eid"].ToString();
                                int a1 = int.Parse(d);
                                string name = rd["Ename"].ToString();
                                string time = rd["time"].ToString();
                                String date_str = d + "号" + name + "  " + time;
                                String[] date_list = date_str.Split(' ');

                                if (zhongji[a1] == 2)
                                {
                                    
                                    float f = float.Parse(rd["Electric"].ToString());
                                    chart1.Series["变量"].Points.AddXY(date_str.ToString("yyyy/MM/dd HH:mm:ss"), (float)f);
                                }

                                
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
        }
    }
}
