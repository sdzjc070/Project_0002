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
    public partial class Form1 : Form
    {
        string workid="";
        string time = "";
        string end = "";
        MySqlConn msc = new MySqlConn();
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            
        }

        private void Form1_Load(object sender, EventArgs e)
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
            dt.Columns.Add("gmax", typeof(string));
            dt.Columns.Add("gmin", typeof(string));
            dt.Columns.Add("chu", typeof(string));
            dt.Columns.Add("momax", typeof(string));
            dt.Columns.Add("momin", typeof(string));
            dt.Columns.Add("time", typeof(string));
            dt.Columns.Add("workid", typeof(string));
            string sql = "select count(Eid) from  equipment where Etype=1 and workid  = " + workid + "";
            MySqlDataReader rd = msc.getDataFromTable(sql);
            string cout = "";
            int i=0, j=0;
            string[] eid= { };
           
            while (rd.Read())
            {
                 cout= rd["count(Eid)"].ToString();
            }
            rd.Close();
            msc.Close();
           
            j = int.Parse(cout);
            string a="";
            string sql1 = "select Eid from  equipment where Etype=1 and workid  = " + workid + "";
            MySqlDataReader rd1= msc.getDataFromTable(sql1);
            List<string> _list = new List<string>(eid);
            while (rd1.Read())
            {
                _list.Add(rd1["Eid"].ToString());
                
                
            }
            rd1.Close();
            msc.Close();
            eid = _list.ToArray();
            //for(i=0;i<3;i++)
            //{
            //    MessageBox.Show("shebei" + eid[i]);
            //}
            //动态生成一些测试用数据  
            for (i = 0; i < j; i++)
            {
                DataRow row = dt.NewRow();
                row[0] = eid[i];
                int[] shuju = { };
                sql = "select * from sensordata where workid = " + workid + " and Eid = " + eid[i] + "  and time between '" + time + "' and '" + end + "' order by time asc";

                //MessageBox.Show("sql" + sql);

                MySqlDataReader rd2 = msc.getDataFromTable(sql);
                List<int> list3 = new List<int>(shuju);
                int sum = 0;
                while (rd2.Read())
                {
                    try
                    {
                        list3.Add(int.Parse(rd2["value"].ToString()));
                        sum++;
                        
                    }
                    catch
                    {

                    }
                    
                }
                rd2.Close();
                msc.Close();
                
                shuju = list3.ToArray();
                if (sum>5)
                {
                    int[] chucheng = { };
                    int[] mozu = { };
                    List<int> list1 = new List<int>(chucheng);
                    List<int> list2 = new List<int>(mozu);
                    int c1 = 0;
                    int m1 = 0;
                    for (int b = 1; b < sum - 1; b++)
                    {

                        if ((shuju[b] - shuju[b - 1] > 2) && ((shuju[b + 1] - shuju[b]) < 2) && ((shuju[b + 1] - shuju[b-1]) > 2)&&(shuju[b]>18))
                        {
                            list1.Add(b);
                            //MessageBox.Show("chu" + b);
                            c1++;
                        }
                        else if ((shuju[b] - shuju[b - 1] > -2) && (shuju[b + 1] - shuju[b] <-2) && (shuju[b+1] - shuju[b - 1] < -2) && (shuju[b] > 18))
                        {
                            list2.Add(b);
                            //MessageBox.Show("mo" + b);
                            m1++;
                        }

                    }


                    chucheng = list1.ToArray();
                    mozu = list2.ToArray();

                    //MessageBox.Show(c1+"="+m1);
                    //MessageBox.Show(chucheng[1].ToString());



                    if (chucheng[0] < mozu[0])
                    {
                        double max = shuju[chucheng[0]];
                        int q = 0;
                        double y = 0;
                        if (c1 > m1)
                            c1 = c1 - 1;


                        for (int ac = 0; ac < c1; ac++)
                        {
                            int ab = chucheng[ac] + 1;
                            while (ab < mozu[ac])
                            {
                                y = y + shuju[ab];
                                q++;
                                if (shuju[ab] > max)
                                    max = shuju[ab];
                                ab++;
                            }
                        }
                        row[1] = (max).ToString();
                        row[2] = (y/q*1.00).ToString();
                        double y1 = 0;
                        for(int i1=0;i1<c1;i1++)
                        {
                            y1 = y1 + shuju[chucheng[i1]];
                        }
                        row[3] = (y1/c1).ToString();
                        double y2 = 0;
                        double max1 = shuju[mozu[0]];
                        for(int i2=0;i2<m1;i2++)
                        {
                            y2 = y2 + shuju[mozu[i2]];
                            if(shuju[mozu[i2]]>max1)
                            {
                                max1 = shuju[mozu[i2]];
                            }
                        }
                        row[4] = (max1).ToString();
                        row[5] = (y2/m1).ToString();
                    }
                }
                else
                {
                    row[1] = (0).ToString();
                    row[2] = (0).ToString();
                    row[3] = (0).ToString();
                    row[4] = (0).ToString();
                    row[5] = (0).ToString();
                }

                string[] ids1 = time.Split(' ');
                string abc= ids1[0];
                row[6] = abc;
                row[7] = workid+"号工作面";
                dt.Rows.Add(row);
            }

            //设置本地报表，使程序与之前所建的testReport.rdlc报表文件进行绑定。  
            this.reportViewer1.LocalReport.ReportPath = "testReport.rdlc";
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetName", dt));
        }
    }
}
