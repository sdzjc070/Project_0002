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
    public partial class equipment : CCSkinMain
    {
        public equipment()
        {
            InitializeComponent();
        }
        MySqlConn msc = new MySqlConn();
        private Mutex file_mutex = new Mutex();//文件互斥锁
        string bz;
        string bz1;
        string t1;
        void Log(string str)    // 记录服务启动  
        {
            file_mutex.WaitOne();
            {
                string info = string.Format("{0}-{1}", DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"), str);
                string path = "C://Mqtt//kuangjingLog.txt";//"C://Mqtt//MyTestLog.txt"
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(info);
                    //关闭
                    sw.Close();
                }
            }
            file_mutex.ReleaseMutex();

        }
        private void getFactory(object obj)
        {
            try
            {
                comboBox1.Items.Clear();
                
                string sql = "select * from equiptype";
                //DataBase db = new DataBase();
                //db.command.CommandText = sql;
                //db.command.Connection = db.connection;
                //db.Dr = db.command.ExecuteReader();
                MySqlDataReader rd = msc.getDataFromTable(sql);
                while (rd.Read())
                {
                    comboBox1.Items.Add(rd["Ename"].ToString());
                }
                rd.Close();
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.Text = comboBox1.Items[0].ToString();
                }



            }
            catch (SqlException se)
            {
              Log("sadfas"+se);
            }

        }

        private void equipment_Load(object sender, EventArgs e)
        {
            
            Thread fac_thread = new Thread(getFactory);
            fac_thread.Start();
            try
            {
                string sql = "select equipment.workid,workname,equipment.Eid,equiptype.Ename,equipment.caiji,equipment.shangchuan from work,equipment,equiptype where `work`.workid = equipment.workid and equipment.Etype = equiptype.Etype ORDER BY workid asc";



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

        private void skinButton1_Click(object sender, EventArgs e)
        {

        }

        private void skinButton1_Click_1(object sender, EventArgs e)
        {
            if (skinTextBox1.Text.Equals(""))
            {
                MessageBox.Show("请输入所属工作面的编号", "提示");
            }
            else if (skinTextBox2.Text.Equals(""))
            {
                MessageBox.Show("请输入设备编号", "提示");
            }
            else if (comboBox1.Text.Equals(""))
            {
                MessageBox.Show("设备类型", "提示");
            }

            else
            {
                try
                {


                    int p1 = 1;
                    string t2;
                    if (comboBox1.Text.Equals("压力计"))
                    {
                        t2 = "1";
                    }
                    else if (comboBox1.Text.Equals("减速机"))
                    {
                        t2 = "2";
                    }
                    else
                    {
                        t2 = "3";
                    }
                    //int A= int.Parse(skinTextBox1.Text);
                    //MessageBox.Show(""+skinTextBox1.Text, "提示");
                    string sql11 = "SELECT equipment.Eid FROM equipment WHERE workid = "+skinTextBox1.Text+ " and Etype =" + t2 + " ";
                    MySqlConn ms = new MySqlConn();
                    MySqlDataReader rd = ms.getDataFromTable(sql11);
                    while (rd.Read())
                    {
                        t1 = rd["Eid"].ToString();
                        //MessageBox.Show("t1是" +t1, "提示");
                        if (skinTextBox2.Text.Equals(t1))
                        {
                            p1 = 0;
                            break;
                           
                        }
                    }
                    rd.Close();
                    ms.Close();

                    
                    if (p1 != 0)
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
                        string sql = "insert into equipment  values(" + skinTextBox1.Text + ", '" + skinTextBox2.Text + "', " + t2 + ", '" + ss + "', '" + sss + "')";
                        MySqlConn ms2 = new MySqlConn();
                        int n = ms2.nonSelect(sql);
                        ms2.Close();
                        MessageBox.Show("添加设备成功", "提示");
                        skinTextBox1.Text = "";
                        skinTextBox2.Text = "";
                        try
                        {
                            string sql1 = "select equipment.workid,workname,equipment.Eid,equiptype.Ename,equipment.caiji,equipment.shangchuan from work,equipment,equiptype where `work`.workid = equipment.workid and equipment.Etype = equiptype.Etype ORDER BY workid asc";



                            MySqlConnection conn = msc.GetConn();
                            MySqlDataAdapter sda = new MySqlDataAdapter(sql1, conn);//获取数据表
                                                                                   //DataTable table = new DataTable();
                            DataSet ds = new DataSet();
                            sda.Fill(ds, "ds");//填充数据库
                            this.dataGridView1.DataSource = ds.Tables[0];
                        }
                        catch (Exception ee)
                        {
                            MessageBox.Show("查询数据库错误：" + ee.ToString());
                        }

                    }
                    else
                    {
                        MessageBox.Show("添加设备重复", "提示");
                    }
                }
                catch (SqlException se)
                {
                    MessageBox.Show("数据库异常", "提示");
                }
            }
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {

        }

        private void skinButton2_Click_1(object sender, EventArgs e)
        {
            if (skinTextBox1.Text.Equals(""))
            {
                MessageBox.Show("请输入所属工作面的编号", "提示");
            }
            else if (skinTextBox2.Text.Equals(""))
            {
                MessageBox.Show("请输入设备号", "提示");
            }
            else if (comboBox1.Text.Equals(""))
            {
                MessageBox.Show("请输入", "提示");
            }
            
            else
            {
                if (skinTextBox1.Text.Equals(bz))
                {

                    int p1 = 1;
                    
                    string t2;
                    if (comboBox1.Text.Equals("压力计"))
                    {
                        t2 = "1";
                    }
                    else if (comboBox1.Text.Equals("减速机"))
                    {
                        t2 = "2";
                    }
                    else
                    {
                        t2 = "3";
                    }
                    //int A= int.Parse(skinTextBox1.Text);
                    //MessageBox.Show(""+skinTextBox1.Text, "提示");
                    string sql11 = "SELECT equipment.Eid FROM equipment WHERE workid = " + skinTextBox1.Text + " and Etype =" + t2 + " "; ;
                    MySqlConn ms = new MySqlConn();
                    MySqlDataReader rd = ms.getDataFromTable(sql11);
                    while (rd.Read())
                    {
                        t1 = rd["Eid"].ToString();
                        //MessageBox.Show("t1是" +t1, "提示");
                        if (skinTextBox2.Text.Equals(t1)&& skinTextBox2.Text!=bz1)
                        {
                            p1 = 0;
                            break;

                        }
                    }
                    rd.Close();
                    ms.Close();
                    

                    
                    if (p1 != 0)
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
                        string sql = "update equipment set Eid = " + skinTextBox2.Text + ",Etype = " + t2 + ",caiji = '" + ss + "',shangchuan = '" + sss + "' where Eid =" + bz1 + " AND workid = " + skinTextBox1.Text + "";
                    MySqlConn ms2 = new MySqlConn();
                    int n = ms2.nonSelect(sql);
                    ms2.Close();
                    MessageBox.Show("修改设备成功", "提示");
                    skinTextBox1.Text = "";
                    skinTextBox2.Text = "";
                  
                    try
                    {
                        string sql2 = "select equipment.workid,workname,equipment.Eid,equiptype.Ename,equipment.caiji,equipment.shangchuan from work,equipment,equiptype where `work`.workid = equipment.workid and equipment.Etype = equiptype.Etype ORDER BY workid asc";



                            MySqlConnection conn = msc.GetConn();
                        MySqlDataAdapter sda = new MySqlDataAdapter(sql2, conn);//获取数据表
                                                                                //DataTable table = new DataTable();
                        DataSet ds = new DataSet();
                        sda.Fill(ds, "ds");//填充数据库
                        this.dataGridView1.DataSource = ds.Tables[0];
                    }
                    catch (Exception ee)
                    {

                    }
                        }
                }
                else
                {
                    MessageBox.Show("请勿修改工作面编号", "提示");
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentRow.Index; //获取选中行的行号
            
            skinTextBox1.Text = dataGridView1.Rows[index].Cells[0].Value.ToString();
            skinTextBox2.Text = dataGridView1.Rows[index].Cells[1].Value.ToString();
            comboBox1.Text = dataGridView1.Rows[index].Cells[2].Value.ToString();
            
            string ss= dataGridView1.Rows[index].Cells[3].Value.ToString();
            string s = "";
            int i = 0;
            while(ss[i]!='分')
            {
                s = s + ss[i];
                i++;
            }
            skinTextBox4.Text = s;
            MessageBox.Show("" + ss);
            //s = "";
            //while (ss[i] != '秒')
            //{
            //    s = s + ss[i];
            //    i++;
            //}
            //skinTextBox5.Text = s;
            //s = "";
            //while (ss[i] != '毫')
            //{
            //    s = s + ss[i];
            //    i++;
            //}
            //skinTextBox6.Text = s;



            //MessageBox.Show(""+bz, "提示");

        }

        private void dataGridView1_SelectionChanged_1(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentRow.Index; //获取选中行的行号

            skinTextBox1.Text = dataGridView1.Rows[index].Cells[0].Value.ToString();
            skinTextBox2.Text = dataGridView1.Rows[index].Cells[2].Value.ToString();
            comboBox1.Text = dataGridView1.Rows[index].Cells[3].Value.ToString();
            bz = skinTextBox1.Text;
            bz1 = skinTextBox2.Text;
            string ss = dataGridView1.Rows[index].Cells[4].Value.ToString();
            if(ss=="")
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
            ss = dataGridView1.Rows[index].Cells[5].Value.ToString();
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


            //MessageBox.Show(""+bz, "提示");
        }

        private void skinButton3_Click(object sender, EventArgs e)
        {

        }

        private void skinButton3_Click_1(object sender, EventArgs e)
        {

            DialogResult dr = MessageBox.Show("确认删除", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                //点确定的代码

                int p1 = 1;
                string t2;
                if (comboBox1.Text.Equals("压力计"))
                {
                    t2 = "1";
                }
                else if (comboBox1.Text.Equals("减速机"))
                {
                    t2 = "2";
                }
                else
                {
                    t2 = "3";
                }
                string sql = "DELETE FROM equipment WHERE workid =" + bz + " AND Eid =" + bz1 + " and Etype =" + t2 + "  ";
                MySqlConn ms2 = new MySqlConn();
                int n = ms2.nonSelect(sql);
                ms2.Close();
                MessageBox.Show(" 删除设备成功", "提示");
                skinTextBox1.Text = "";
                skinTextBox2.Text = "";

                try
                {
                    string sql2 = "select equipment.workid,workname,equipment.Eid,equiptype.Ename,equipment.caiji,equipment.shangchuan from work,equipment,equiptype where `work`.workid = equipment.workid and equipment.Etype = equiptype.Etype ORDER BY workid asc";



                    MySqlConnection conn = msc.GetConn();
                    MySqlDataAdapter sda = new MySqlDataAdapter(sql2, conn);//获取数据表
                                                                            //DataTable table = new DataTable();
                    DataSet ds = new DataSet();
                    sda.Fill(ds, "ds");//填充数据库
                    this.dataGridView1.DataSource = ds.Tables[0];
                }
                catch (Exception ee)
                {

                }
            }


        }

        private void skinTextBox3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void skinTextBox4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void skinTextBox5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void skinTextBox6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void skinTextBox7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void skinTextBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    }
