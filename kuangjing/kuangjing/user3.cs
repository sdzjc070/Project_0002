using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using CCWin;


namespace kuangjing

{
    public partial class user3 : CCSkinMain
    {
        DataTable dtInfo = new DataTable();
        MySqlConn msc = new MySqlConn();
        public user3()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Control.CheckForIllegalCrossThreadCalls = false;
        }


        private int msgBoxNum = 0;//MessageBox编号，每弹出一个MessageBox自增1，大于2000变为0
        private void showBox(object obj)
        {
            string message = (string)obj;
            if (msgBoxNum >= 2000)
            {
                msgBoxNum = 0;
            }
            MyMessageBox MymsgBox = new MyMessageBox(message, (msgBoxNum++).ToString());
            MymsgBox.ShowBox();
        }


        private void getFactory(object obj)
        {
            try
            {
                string sql = "select * from user";
                //DataBase db = new DataBase();

                //db.command.CommandText = sql;
                //db.command.Connection = db.connection;
                //db.Dr = db.command.ExecuteReader();
                MySqlConn ms = new MySqlConn();
                MySqlDataReader rd = ms.getDataFromTable(sql);
                while (rd.Read())
                {
                    if (rd["UserName"].ToString().Equals("root"))
                        continue;
                    else
                    {
                        comboBox7.Items.Add(rd["UserName"].ToString());
                        
                    }
                }
                rd.Close();
                ms.Close();
            }
            catch (Exception exc)
            {
                //MessageBox.Show("数据库连接失败", "提示");
                new Thread(new ParameterizedThreadStart(showBox)).Start("数据库连接失败");
            }

        }
        private void button14_Click(object sender, EventArgs e)
        {
            if (comboBox7.Text.Equals(""))
            {
                MessageBox.Show("请输入用户名", "提示");
            }
            else if (textBox12.Text.Equals(""))
            {
                MessageBox.Show("请输入管理员密码", "提示");
            }
            else
            {
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("确认要删除用户 " + comboBox7.Text + " 吗？", "提示", messButton);
                if (dr == DialogResult.OK)
                {
                    try
                    {
                        string sql1 = "select * from user where UserName = 'root';";
                        MySqlConn ms = new MySqlConn();
                        MySqlDataReader rd = ms.getDataFromTable(sql1);
                        string str = "";
                        while (rd.Read())
                        {
                            str = rd["PassWord"].ToString();
                        }
                        rd.Close();
                        ms.Close();

                        if (str.Equals(textBox12.Text))
                        {
                            MySqlConn ms1 = new MySqlConn();
                            string sql = "delete from user where UserName='" + comboBox7.Text + "';";

                            int res = ms1.nonSelect(sql);
                            //MessageBox.Show("====33333=" + res);
                            ms1.Close();
                            if (res > 0)
                            {
                                MessageBox.Show("删除成功", "提示");
                                //让文本框获取焦点，不过注释这行也能达到效果
                                //richTextBox1.Focus();
                                //设置光标的位置到文本尾   
                                //richTextBox1.Select(richTextBox1.TextLength, 0);
                                //滚动到控件光标处   
                                //richTextBox1.ScrollToCaret();
                                //richTextBox1.AppendText(DateTime.Now.ToString("G") + "\r\n" + "删除用户成功\r\n\r\n");


                            }
                            else
                            {
                                MessageBox.Show("用户不存在", "提示");
                            }
                        }
                        else
                        {
                            MessageBox.Show("管理员密码错误", "提示");
                        }
                    }
                    catch (Exception exc)
                    {
                        //MessageBox.Show("请检查数据库是否创建好", "提示");
                        new Thread(new ParameterizedThreadStart(showBox)).Start("请检查数据库是否创建好");
                    }
                }


            }
        }
        private System.Windows.Forms.RichTextBox richTextBox1;

        private void user3_Load_1(object sender, EventArgs e)
        {
            Thread fac_thread = new Thread(getFactory);
            fac_thread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
