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
namespace kuangjing
{
    public partial class work : CCSkinMain
    {
        public work()
        {
            InitializeComponent();
        }
        MySqlConn msc = new MySqlConn();
        MySqlConn msc1 = new MySqlConn();
        string bz;
        string t1;
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
        private void skinButton1_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text.Equals(""))
            {
                MessageBox.Show("请输入工作面的名称", "提示");
            }
            else if (TextBox2.Text.Equals(""))
            {
                MessageBox.Show("请输入支架型式", "提示");
            }
            else if (extBox3.Text.Equals(""))
            {
                MessageBox.Show("请输入缸径", "提示");
            }
            else if (extBox4.Text.Equals(""))
            {
                MessageBox.Show("请输入压力上限", "提示");
            }
            else if (extBox5.Text.Equals(""))
            {
                MessageBox.Show("请输入压力下限", "提示");
            }
            else if (TextBox6.Text.Equals(""))
            {
                MessageBox.Show("请输入工作面编号", "提示");
            }
            else
            {
                string sql11 = "SELECT max(workid) from work";
                MySqlConn ms = new MySqlConn();
                MySqlDataReader rd = ms.getDataFromTable(sql11);
                while (rd.Read())
                {
                    t1 = rd["max(workid)"].ToString();
                }
                rd.Close();
                ms.Close();
                //int A = int.Parse(skinTextBox6.Text);
                //int B = int.Parse(t1);
                int p1 = 1;
                //int A= int.Parse(skinTextBox1.Text);
                //MessageBox.Show(""+skinTextBox1.Text, "提示");
                string sql12 = "SELECT workid FROM work";
                MySqlConn ms1 = new MySqlConn();
                MySqlDataReader rd1 = ms1.getDataFromTable(sql12);
                while (rd1.Read())
                {
                    t1 = rd1["workid"].ToString();
                    //MessageBox.Show("t1是" +t1, "提示");
                    if (TextBox6.Text.Equals(t1))
                    {
                        p1 = 0;
                        break;

                    }
                }
                rd1.Close();
                ms1.Close();

                if (p1==1)
                {
                    if (skinTextBox4.Text == "")
                        skinTextBox4.Text = "0";
                    if (skinTextBox5.Text == "")
                        skinTextBox5.Text = "0";
                    if (skinTextBox6.Text == "")
                        skinTextBox6.Text = "0";
                    if (skinTextBox7.Text == "")
                        skinTextBox7.Text = "0";
                    if (skinTextBox8.Text == "")
                        skinTextBox8.Text = "0";
                    if (skinTextBox9.Text == "")
                        skinTextBox9.Text = "0";

                    string ss = skinTextBox4.Text + "分" + skinTextBox5.Text + "秒" + skinTextBox6.Text + "毫秒";
                    string sss = skinTextBox7.Text + "分" + skinTextBox8.Text + "秒" + skinTextBox9.Text + "毫秒";
                    string sql = "insert into work  values(" + TextBox6.Text + ", '" + TextBox1.Text + "',' " + TextBox2.Text + "', " + extBox3.Text + ", " + extBox4.Text + ", " + extBox5.Text + ", '" + ss + "', '" + sss + "')";
                    MySqlConn ms2 = new MySqlConn();
                    int n = ms2.nonSelect(sql);
                    ms2.Close();
                    MessageBox.Show("添加工作面成功", "提示");
                    TextBox1.Text = "";
                    TextBox2.Text = "";
                    extBox3.Text = "";
                    extBox4.Text = "";
                    extBox5.Text = "";
                    TextBox6.Text = "";
                    //if (dataGridView1.DataSource != null)
                    //{

                    //    DataTable dt = (DataTable)dataGridView1.DataSource;

                    //    dt.Rows.Clear();

                    //    dataGridView1.DataSource = dt;

                    //}
                    //else

                    //{

                    //    dataGridView1.Rows.Clear();

                    //}
                    try
                    {
                        string sql1 = "select * from work order by workid asc";



                        MySqlConnection conn = msc.GetConn();
                        MySqlDataAdapter sda = new MySqlDataAdapter(sql1, conn);//获取数据表
                                                                                //DataTable table = new DataTable();
                        DataSet ds = new DataSet();
                        sda.Fill(ds, "ds");//填充数据库
                        this.dataGridView1.DataSource = ds.Tables[0];
                        msc.Close();
                    }
                    catch (Exception ee)
                    {

                    }
                }
                else
                {
                    MessageBox.Show("请输入正确工作面编号", "提示");

                }


            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void work_Load(object sender, EventArgs e)
        {
            try
            {
                string sql = "select * from work order by workid asc";



                MySqlConnection conn = msc.GetConn();
                MySqlDataAdapter sda = new MySqlDataAdapter(sql, conn);//获取数据表
                                                                       //DataTable table = new DataTable();
                DataSet ds = new DataSet();
                sda.Fill(ds, "ds");//填充数据库
                this.dataGridView1.DataSource = ds.Tables[0];
                msc.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show("查询数据库错误：" + ee.ToString());
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            
            int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
            TextBox6.Text = dataGridView1.Rows[index].Cells[0].Value.ToString();
            TextBox1.Text = dataGridView1.Rows[index].Cells[1].Value.ToString();
            TextBox2.Text = dataGridView1.Rows[index].Cells[2].Value.ToString();
            extBox3.Text = dataGridView1.Rows[index].Cells[3].Value.ToString();
            extBox4.Text = dataGridView1.Rows[index].Cells[4].Value.ToString();
            extBox5.Text = dataGridView1.Rows[index].Cells[5].Value.ToString();
            bz = TextBox6.Text;
            //MessageBox.Show(""+bz, "提示");
            string ss = dataGridView1.Rows[index].Cells[6].Value.ToString();
            if (ss == "")
            {
                skinTextBox4.Text = "0";
                skinTextBox5.Text = "0";
                skinTextBox6.Text = "0";

            }
            else
            {
                string s = "";
                int i = 0;
                while (ss[i] != '分')
                {
                    s = s + ss[i];
                    i++;
                }
                skinTextBox4.Text = s;

                s = "";
                i++;
                while (ss[i] != '秒')
                {
                    s = s + ss[i];
                    i++;
                }
                skinTextBox5.Text = s;
                s = "";
                i++;
                while (ss[i] != '毫')
                {
                    s = s + ss[i];
                    i++;
                }
                skinTextBox6.Text = s;
            }
            ss = dataGridView1.Rows[index].Cells[6].Value.ToString();
            if (ss == "")
            {
                skinTextBox7.Text = "0";
                skinTextBox8.Text = "0";
                skinTextBox9.Text = "0";

            }
            else
            {
                string s = "";
                int i = 0;
                while (ss[i] != '分')
                {
                    s = s + ss[i];
                    i++;
                }
                skinTextBox7.Text = s;

                s = "";
                i++;
                while (ss[i] != '秒')
                {
                    s = s + ss[i];
                    i++;
                }
                skinTextBox8.Text = s;
                s = "";
                i++;
                while (ss[i] != '毫')
                {
                    s = s + ss[i];
                    i++;
                }
                skinTextBox9.Text = s;
            }
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text.Equals(""))
            {
                MessageBox.Show("请输入工作面的名称", "提示");
            }
            else if (TextBox2.Text.Equals(""))
            {
                MessageBox.Show("请输入支架型式", "提示");
            }
            else if (extBox3.Text.Equals(""))
            {
                MessageBox.Show("请输入缸径", "提示");
            }
            else if (extBox4.Text.Equals(""))
            {
                MessageBox.Show("请输入压力上限", "提示");
            }
            else if (extBox5.Text.Equals(""))
            {
                MessageBox.Show("请输入压力下限", "提示");
            }
            else if (TextBox6.Text.Equals(""))
            {
                MessageBox.Show("请输入工作面编号", "提示");
            }
            else
            {
                if (TextBox6.Text.Equals(bz))
                {
                    if (skinTextBox4.Text == "")
                        skinTextBox4.Text = "0";
                    if (skinTextBox5.Text == "")
                        skinTextBox5.Text = "0";
                    if (skinTextBox6.Text == "")
                        skinTextBox6.Text = "0";
                    if (skinTextBox7.Text == "")
                        skinTextBox7.Text = "0";
                    if (skinTextBox8.Text == "")
                        skinTextBox8.Text = "0";
                    if (skinTextBox9.Text == "")
                        skinTextBox9.Text = "0";


                    string ss = skinTextBox4.Text + "分" + skinTextBox5.Text + "秒" + skinTextBox6.Text + "毫秒";
                    string sss = skinTextBox7.Text + "分" + skinTextBox8.Text + "秒" + skinTextBox9.Text + "毫秒";
                    //MessageBox.Show("skinTextBox2.Text是" + bz, "提示");
                    string sql = "update work set workid = " + TextBox6.Text + ",workname ='" + TextBox1.Text + "',`Bracket type` = ' " + TextBox2.Text + "',`Bore diameter` = " + extBox3.Text + ", maxpressure =  " + extBox4.Text + ",minpressure= " + extBox5.Text + " ,caiji = '" + ss + "',shangchuan = '" + sss + "'where workid =" + bz + "";
                    MySqlConn ms2 = new MySqlConn();
                    int n = ms2.nonSelect(sql);
                    ms2.Close();
                    MessageBox.Show("修改工作面成功", "提示");
                    TextBox1.Text = "";
                    TextBox2.Text = "";
                    extBox3.Text = "";
                    extBox4.Text = "";
                    extBox5.Text = "";
                    TextBox6.Text = "";

                    try
                    {
                        string sql2 = "select * from work order by workid asc";



                        MySqlConnection conn = msc.GetConn();
                        MySqlDataAdapter sda = new MySqlDataAdapter(sql2, conn);//获取数据表
                                                                                //DataTable table = new DataTable();
                        DataSet ds = new DataSet();
                        sda.Fill(ds, "ds");//填充数据库
                        this.dataGridView1.DataSource = ds.Tables[0];
                        msc.Close();
                    }
                    catch (Exception ee)
                    {

                    }
                }
                else
                {
                    MessageBox.Show("请勿修改工作面编号", "提示");
                }
            }
        }

        private void skinButton3_Click(object sender, EventArgs e)
        {

            DialogResult dr = MessageBox.Show("确认删除", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                //点确定的代码


                string sql = "DELETE FROM work WHERE workid =" + bz + "";
                MySqlConn ms2 = new MySqlConn();
                int n = ms2.nonSelect(sql);
                ms2.Close();
                MessageBox.Show(" 删除工作面成功", "提示");
                TextBox1.Text = "";
                TextBox2.Text = "";
                extBox3.Text = "";
                extBox4.Text = "";
                extBox5.Text = "";
                TextBox6.Text = "";

                try
                {
                    string sql2 = "select * from work order by workid asc";



                    MySqlConnection conn = msc.GetConn();
                    MySqlDataAdapter sda = new MySqlDataAdapter(sql2, conn);//获取数据表
                                                                            //DataTable table = new DataTable();
                    DataSet ds = new DataSet();
                    sda.Fill(ds, "ds");//填充数据库
                    this.dataGridView1.DataSource = ds.Tables[0];
                        msc.Close();
                }
                catch (Exception ee)
                {

                }
            }
              
            }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void skinTextBox6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void skinTextBox1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
    }

