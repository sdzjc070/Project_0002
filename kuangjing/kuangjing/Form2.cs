using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using static kuangjing.baobiao;

namespace kuangjing
{
    public partial class Form2 : Form
    {
        string workid = "";
        string time = "";
        string end = "";
        string sid = "0";
        double max = 0;
        MySqlConn msc = new MySqlConn();
        public Form2()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            string ab = Config.value;
            string[] ids = ab.Split('*');
            workid = ids[0];
            time = ids[1];
            end = ids[2];
            //MessageBox.Show(workid + "==" + time+"=="+end);
            loadReport();
            this.reportViewer1.RefreshReport();
        }
        public void loadReport()
        {
            DataTable dt = new DataTable();
            //定义本地数据表的列，名称应跟之前所建的testDataTable表中列相同。  
            dt.Columns.Add("Eid", typeof(string));
            dt.Columns.Add("youwen", typeof(string));
            dt.Columns.Add("youwen1", typeof(string));
            dt.Columns.Add("youya", typeof(string));
            dt.Columns.Add("youya1", typeof(string));
            dt.Columns.Add("youwei", typeof(string));
            dt.Columns.Add("youwei1", typeof(string));
            dt.Columns.Add("zhoucheng", typeof(string));
            dt.Columns.Add("zhoucheng1", typeof(string));
            dt.Columns.Add("shuiya", typeof(string));
            dt.Columns.Add("shuiya1", typeof(string));
            dt.Columns.Add("lengque", typeof(string));
            dt.Columns.Add("lengque1", typeof(string));
            dt.Columns.Add("zaihe", typeof(string));
            dt.Columns.Add("zaihe1", typeof(string));
            dt.Columns.Add("zhendong", typeof(string));
            dt.Columns.Add("zhendong1", typeof(string));
            dt.Columns.Add("time", typeof(string));
            dt.Columns.Add("workid", typeof(string));
            string sql = "select count(Eid) from  equipment where Etype=2 and workid  = " + workid + "";
            MySqlDataReader rd = msc.getDataFromTable(sql);
            string cout = "";
            int i = 0, j = 0;
            string[] eid = { };

            while (rd.Read())
            {
                cout = rd["count(Eid)"].ToString();
            }
            rd.Close();
            msc.Close();

            j = int.Parse(cout);
            string a = "";
            string sql1 = "select Eid from  equipment where Etype=2 and workid  = " + workid + "";
            MySqlDataReader rd1 = msc.getDataFromTable(sql1);
            List<string> _list = new List<string>(eid);
            while (rd1.Read())
            {
                _list.Add(rd1["Eid"].ToString());


            }
            rd1.Close();
            msc.Close();
            eid = _list.ToArray();

            for (i = 0; i < j; i++)
            {
                DataRow row = dt.NewRow();
                row[0] = eid[i];
                
                dt.Rows.Add(row);
                string sql2 = "select * from dsensor where workid = " + workid + " and Eid = " + eid[i] + " and Sname = '油温'";
                
                MySqlDataReader rd2 = msc.getDataFromTable(sql2);
                while (rd2.Read())
                {
                    sid = rd2["Sid"].ToString();
                }
                msc.Close();
                rd2.Close();
                sql = "select * from sensordata where workid = " + workid + " and Eid = " + eid[i] + " and Sid = " + sid + "  and time between '" + time + "' and '" + end + "' order by time asc";

                //MessageBox.Show("sql" + sql);

                MySqlDataReader rd3 = msc.getDataFromTable(sql);
            
                int sum = 0;
                float abc = 0;
                while (rd3.Read())
                {
                    try
                    {
                        
                        sum++;
                        int ab = int.Parse(rd3["value"].ToString());
                        if(ab>max)
                        {
                            max = ab;
                        }
                        abc = abc + int.Parse(rd3["value"].ToString());

                    }
                    catch
                    {

                    }

                }
                rd3.Close();
                msc.Close();

                
                if(sum>0)
                {
                    row[1] = (max).ToString();
                    row[2] = (abc/sum).ToString();
                }
                else
                {
                    row[1] = (0).ToString();
                    row[2] = (0).ToString();
                }
                 sql2 = "select * from dsensor where workid = " + workid + " and Eid = " + eid[i] + " and Sname = '油压'";

                 rd2 = msc.getDataFromTable(sql2);
                while (rd2.Read())
                {
                    sid = rd2["Sid"].ToString();
                }
                msc.Close();
                rd2.Close();
                sql = "select * from sensordata where workid = " + workid + " and Eid = " + eid[i] + " and Sid = " + sid + "  and time between '" + time + "' and '" + end + "' order by time asc";

                //MessageBox.Show("sql" + sql);

                 rd3 = msc.getDataFromTable(sql);

                 sum = 0;
                 abc = 0;
                max = 0;
                while (rd3.Read())
                {
                    try
                    {

                        sum++;
                        int ab = int.Parse(rd3["value"].ToString());
                        if (ab > max)
                        {
                            max = ab;
                        }
                        abc = abc + int.Parse(rd3["value"].ToString());

                    }
                    catch
                    {

                    }

                }
                rd3.Close();
                msc.Close();


                if (sum > 0)
                {
                    row[3] = (max).ToString();
                    row[4] = (abc / sum).ToString();
                }
                else
                {
                    row[3] = (0).ToString();
                    row[4] = (0).ToString();
                }


                sql2 = "select * from dsensor where workid = " + workid + " and Eid = " + eid[i] + " and Sname = '油位'";

                rd2 = msc.getDataFromTable(sql2);
                while (rd2.Read())
                {
                    sid = rd2["Sid"].ToString();
                }
                msc.Close();
                rd2.Close();
                sql = "select * from sensordata where workid = " + workid + " and Eid = " + eid[i] + " and Sid = " + sid + "  and time between '" + time + "' and '" + end + "' order by time asc";

                //MessageBox.Show("sql" + sql);

                rd3 = msc.getDataFromTable(sql);

                sum = 0;
                abc = 0;
                max = 0;
                while (rd3.Read())
                {
                    try
                    {

                        sum++;
                        int ab = int.Parse(rd3["value"].ToString());
                        if (ab > max)
                        {
                            max = ab;
                        }
                        abc = abc + int.Parse(rd3["value"].ToString());

                    }
                    catch
                    {

                    }

                }
                rd3.Close();
                msc.Close();


                if (sum > 0)
                {
                    row[5] = (max).ToString();
                    row[6] = (abc / sum).ToString();
                }
                else
                {
                    row[5] = (0).ToString();
                    row[6] = (0).ToString();
                }


                sql2 = "select * from dsensor where workid = " + workid + " and Eid = " + eid[i] + " and Sname = '轴承温度'";

                rd2 = msc.getDataFromTable(sql2);
                while (rd2.Read())
                {
                    sid = rd2["Sid"].ToString();
                }
                msc.Close();
                rd2.Close();
                sql = "select * from sensordata where workid = " + workid + " and Eid = " + eid[i] + " and Sid = " + sid + "  and time between '" + time + "' and '" + end + "' order by time asc";

                //MessageBox.Show("sql" + sql);

                rd3 = msc.getDataFromTable(sql);

                sum = 0;
                abc = 0;
                max = 0;
                while (rd3.Read())
                {
                    try
                    {

                        sum++;
                        int ab = int.Parse(rd3["value"].ToString());
                        if (ab > max)
                        {
                            max = ab;
                        }
                        abc = abc + int.Parse(rd3["value"].ToString());

                    }
                    catch
                    {

                    }

                }
                rd3.Close();
                msc.Close();


                if (sum > 0)
                {
                    row[7] = (max).ToString();
                    row[8] = (abc / sum).ToString();
                }
                else
                {
                    row[7] = (0).ToString();
                    row[8] = (0).ToString();
                }


                sql2 = "select * from dsensor where workid = " + workid + " and Eid = " + eid[i] + " and Sname = '水压'";

                rd2 = msc.getDataFromTable(sql2);
                while (rd2.Read())
                {
                    sid = rd2["Sid"].ToString();
                }
                msc.Close();
                rd2.Close();
                sql = "select * from sensordata where workid = " + workid + " and Eid = " + eid[i] + " and Sid = " + sid + "  and time between '" + time + "' and '" + end + "' order by time asc";

                //MessageBox.Show("sql" + sql);

                rd3 = msc.getDataFromTable(sql);

                sum = 0;
                abc = 0;
                max = 0;
                while (rd3.Read())
                {
                    try
                    {

                        sum++;
                        int ab = int.Parse(rd3["value"].ToString());
                        if (ab > max)
                        {
                            max = ab;
                        }
                        abc = abc + int.Parse(rd3["value"].ToString());

                    }
                    catch
                    {

                    }

                }
                rd3.Close();
                msc.Close();


                if (sum > 0)
                {
                    row[9] = (max).ToString();
                    row[10] = (abc / sum).ToString();
                }
                else
                {
                    row[9] = (0).ToString();
                    row[10] = (0).ToString();
                }


                sql2 = "select * from dsensor where workid = " + workid + " and Eid = " + eid[i] + " and Sname = '冷却水温度'";

                rd2 = msc.getDataFromTable(sql2);
                while (rd2.Read())
                {
                    sid = rd2["Sid"].ToString();
                }
                msc.Close();
                rd2.Close();
                sql = "select * from sensordata where workid = " + workid + " and Eid = " + eid[i] + " and Sid = " + sid + "  and time between '" + time + "' and '" + end + "' order by time asc";

                //MessageBox.Show("sql" + sql);

                rd3 = msc.getDataFromTable(sql);

                sum = 0;
                abc = 0;
                max = 0;
                while (rd3.Read())
                {
                    try
                    {

                        sum++;
                        int ab = int.Parse(rd3["value"].ToString());
                        if (ab > max)
                        {
                            max = ab;
                        }
                        abc = abc + int.Parse(rd3["value"].ToString());

                    }
                    catch
                    {

                    }

                }
                rd3.Close();
                msc.Close();


                if (sum > 0)
                {
                    row[11] = (max).ToString();
                    row[12] = (abc / sum).ToString();
                }
                else
                {
                    row[11] = (0).ToString();
                    row[12] = (0).ToString();
                }


                sql2 = "select * from dsensor where workid = " + workid + " and Eid = " + eid[i] + " and Sname = '载荷'";

                rd2 = msc.getDataFromTable(sql2);
                while (rd2.Read())
                {
                    sid = rd2["Sid"].ToString();
                }
                msc.Close();
                rd2.Close();
                sql = "select * from sensordata where workid = " + workid + " and Eid = " + eid[i] + " and Sid = " + sid + "  and time between '" + time + "' and '" + end + "' order by time asc";

                //MessageBox.Show("sql" + sql);

                rd3 = msc.getDataFromTable(sql);

                sum = 0;
                abc = 0;
                max = 0;
                while (rd3.Read())
                {
                    try
                    {

                        sum++;
                        int ab = int.Parse(rd3["value"].ToString());
                        if (ab > max)
                        {
                            max = ab;
                        }
                        abc = abc + int.Parse(rd3["value"].ToString());

                    }
                    catch
                    {

                    }

                }
                rd3.Close();
                msc.Close();


                if (sum > 0)
                {
                    row[13] = (max).ToString();
                    row[14] = (abc / sum).ToString("F2");
                }
                else
                {
                    row[13] = (0).ToString();
                    row[14] = (0).ToString();
                }

                sql2 = "select * from dsensor where workid = " + workid + " and Eid = " + eid[i] + " and Sname = '振动'";

                rd2 = msc.getDataFromTable(sql2);
                while (rd2.Read())
                {
                    sid = rd2["Sid"].ToString();
                }
                msc.Close();
                rd2.Close();
                sql = "select * from sensordata where workid = " + workid + " and Eid = " + eid[i] + " and Sid = " + sid + "  and time between '" + time + "' and '" + end + "' order by time asc";

                //MessageBox.Show("sql" + sql);

                rd3 = msc.getDataFromTable(sql);

                sum = 0;
                abc = 0;
                max = 0;
                while (rd3.Read())
                {
                    try
                    {

                        sum++;
                        int ab = int.Parse(rd3["value"].ToString());
                        if (ab > max)
                        {
                            max = ab;
                        }
                        abc = abc + int.Parse(rd3["value"].ToString());

                    }
                    catch
                    {

                    }

                }
                rd3.Close();
                msc.Close();


                if (sum > 0)
                {
                    row[15] = (max).ToString();
                    row[16] = (abc / sum).ToString();
                }
                else
                {
                    row[15] = (0).ToString();
                    row[16] = (0).ToString();
                }

                string[] ids1 = time.Split(' ');
                string abcd = ids1[0];
                row[17] = abcd;
                row[18] = workid + "号工作面";
            }

                



            //设置本地报表，使程序与之前所建的testReport.rdlc报表文件进行绑定。  
            this.reportViewer1.LocalReport.ReportPath = "Report1.rdlc";
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetName", dt));
        }
    }

}
