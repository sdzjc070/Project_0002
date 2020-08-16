using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
//using Update;
using System.Diagnostics;
using System.IO;
using MySql.Data.MySqlClient;
using CCWin;



namespace kuangjing
{
    public partial class Form_Login : CCSkinMain
    {
   //输入框水印  Win32Utility.SetCueText(textBox2, "请输入密码。。。");
        Users user_login = new Users();
        private MainForm mainform = null;
        private int SaveUsrInfo = 1;//是否需要保存密码
        public Form_Login()
        {
            InitializeComponent();
            //this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            //this.skinEngine1.SkinFile = Application.StartupPath + "Eighteen.ssk";

            //this.skinEngine1.SkinFile = "Eighteen.ssk";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            string path = System.Windows.Forms.Application.StartupPath;
            if (File.Exists(path + "\\userinfo.txt") == false)//创建保存串口信息的文件
                File.Create(path + "\\userinfo.txt").Close();

            StreamReader sr = new StreamReader(path + "\\userinfo.txt");//去除 Encoding.Default解决了中文乱码
            String line = sr.ReadLine();
            if (line != null)
            {
                String[] UserInfo = line.Split('+');
                if (UserInfo.Length == 2)
                {
                    textBox1.Text = UserInfo[0];
                    textBox2.Text = UserInfo[1];

                }
            }
            sr.Close();
        }
        private Mutex file_mutex = new Mutex();//文件互斥锁
        void Log(string str)    // 记录服务启动  
        {
            file_mutex.WaitOne();
            {
                string info = string.Format("{0}-{1}", DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"), str);
                string path = "C://Mqtt//MyTestLog.txt";//"C://Mqtt//MyTestLog.txt"
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(info);
                    //关闭
                    sw.Close();
                }
            }
            file_mutex.ReleaseMutex();

        }
        private void Button1_Click(object sender, EventArgs e)// 点击登录函数
        {
            if (textBox1.Text.Equals(""))
                MessageBox.Show("请输入用户名", "提示");
            else if (textBox2.Text.Equals(""))
                MessageBox.Show("请输入密码", "提示");
            else
            {
                try
                {
                    //DataBase db = new DataBase();
                    //db.command.CommandText = "select * from [user]";
                    //db.command.Connection = db.connection;

                    //db.Dr = db.command.ExecuteReader();
                    MySqlConn msc = new MySqlConn();
                    string sql = "select * from user";
                    MySqlDataReader rdr = msc.getDataFromTable(sql);
                    user_login.Logout();
                    while (rdr.Read())
                    {
                        if (rdr["UserName"].ToString().Equals(textBox1.Text.Trim()))
                        {//当存在用户时，首先将用户名提取出来
                            user_login.name = textBox1.Text;
                            if (rdr["PassWord"].ToString().Equals(textBox2.Text))
                            {//如果密码也相同，则提取权限
                                user_login.admin = rdr["Admin"].ToString();
                                label3.Visible = true;
                                button1.Enabled = false;
                                textBox2.Text = "";
                                if (SaveUsrInfo == 1)
                                {//点击了保存用户名密码按钮
                                    string path = System.Windows.Forms.Application.StartupPath;
                                    FileStream fs = new FileStream(path + "\\userinfo.txt", FileMode.Create, FileAccess.Write);
                                    StreamWriter sw = new StreamWriter(fs);
                                    sw.WriteLine(user_login.name + "+" + rdr["PassWord"].ToString());
                                    sw.Flush();
                                    sw.Close();
                                    fs.Close();

                                }


                                Thread.Sleep(50);

                                new Thread((ParameterizedThreadStart)this.conTosql).Start();
                                break;
                                //Thread thread = new Thread(show_mainform);
                                //thread.IsBackground = false;
                                //thread.Start();
                            }
                            else
                            {
                                MessageBox.Show("密码错误", "提示");
                            }
                        }
                    }
                    if (user_login.name.Equals(""))
                        MessageBox.Show("用户不存在", "提示");
                    rdr.Close();

                }
                catch (Exception exc)
                {
                    //DataBase database = new DataBase();
                    //string sql_createT = "create table [user] (UserName nvarchar(MAX), PassWord nvarchar(MAX), Admin int);";
                    //database.command.CommandText = sql_createT;
                    //database.command.Connection = database.connection;
                    //database.command.ExecuteNonQuery();

                    //string sql_init = "insert into [user] values ('root', '123', 1);";
                    //database.command.CommandText = sql_init;
                    //database.command.ExecuteNonQuery();
                    MessageBox.Show("数据库连接出错，请检查数据库连接\r\n错误代码：" + exc.ToString(), "提示");
                    return;
                }


            }
        }
        private void conTosql(object obj)
        {

            MethodInvoker meth = new MethodInvoker(Show_mainform);
            BeginInvoke(meth);
            //Application.Run(new MainForm(user_login.name, user_login.admin));
        }

        private void Show_mainform()
        {

            MainForm mainform;//显示主页面
            mainform = new MainForm(user_login.name, user_login.admin, this);
            mainform.Show();

            this.Visible = false;//隐藏当前页面
            button1.Enabled = true;
            label3.Visible = false;
            //this.Close();

        }

        private string conTosql()//创建数据库
        {
            try
            {
                Log("检查数据库");
                int sum_table = 9;//一共需要9个表，存在一个就减减..再加上一个由于mqtt的code.20190429添加groupinfo表
                string[] tables = {"company", "work", "equipment", "Dsensor", "sensordata", "user","Equiptype","type" ,"ip"};
                //string[] tables = { "user" };
                string sql = "select table_name from information_schema.tables where table_schema='kj'";//获取数据库kaungjing中的和数据表

                MySqlConn ms = new MySqlConn();
                MySqlDataReader rd = ms.getDataFromTable(sql);

                while (rd.Read())
                {
                    for (int i = 0; i < tables.Length; i++)
                    {
                        if (rd["table_name"].ToString().Equals(tables[i]))//若有对应的表名，说明存在表
                        {
                            tables[i] = "";
                            sum_table--;
                        }
                    }
                }
                Log("没有表的数目：" + sum_table);
                rd.Close();
                ms.Close();
                if (sum_table == 0)
                {
                    return "1";
                }
                else
                {
                    Log("创建表");
                    string table = "";
                    for (int i = 0; i < 9; i++)
                    {
                        if (tables[i].Equals("") != true)
                        {
                            table += (tables[i] + "|");

                            string sql_createT = "";
                            string sql_initdata = "";
                            

                            if (tables[i].Equals("type"))
                            {
                                sql_createT = "create table `type` ( `bzid` int,`type` varchar(255));";
                                sql_initdata = "insert into type values( '0' ,'压力'),( '1' ,'油温'),('2' ,'油压'),('3', '油位'),('4', '轴承温度'),('5', '水压'),('6', '冷却水温度'),('7', '载荷'),('8', '振动')";
                                //sql_initdata2 = "insert into type values( '2', '油压')";
                                //sql_initdata3 = "insert into type values( '3', '油位')";
                                //sql_initdata4 = "insert into type values( '4', '轴承温度')";
                                //sql_initdata5 = "insert into type values( '5', '水压')";
                                //sql_initdata6 = "insert into type values( '6', '冷却水温度')";
                                //sql_initdata7 = "insert into type values( '7', '载荷')";
                                //sql_initdata8 = "insert into type values( '8', '振动')";
                                //'1', '油温','2', '油压','3', '油位','4', '轴承温度','5', '水压','6', '冷却水温度','7', '载荷','8', '振动'
                            }
                            else if (tables[i].Equals("company"))
                            {
                                sql_createT = "create table `company` ( `company` varchar(255));";
                                sql_initdata = "insert into company values( '物联智讯有限公司')";
                            }
                            else if (tables[i].Equals("work"))
                            {
                                sql_createT = "create table `work` (`workid` int null, `workname` varchar(255),`Bracket type` varchar(255), `Bore diameter` varchar(255), `maxpressure` int,`minpressure` int，`caiji` varchar(255), `shangchuan` varchar(255));";
                            }
                            else if (tables[i].Equals("equipment"))
                            {
                                sql_createT = "create table `equipment` (`workid` int,`Eid` int, `Etype` int, `caiji` varchar(255), `shangchuan` varchar(255));";
                            }
                            else if (tables[i].Equals("Dsensor"))
                            {
                                sql_createT = "create table `Dsensor` (`workid` int,`Eid` int,`Ename` varchar(255),`Sid` int,`Sname` varchar(255), `Smax` int,`Smin` int, `caiji` varchar(255), `shangchuan` varchar(255));";
                            }
                            else if (tables[i].Equals("sensordata"))
                            {
                                sql_createT = "create table `sensordata` (`workid` int,`Eid` int, `Sid` int,`time` varchar(255),`value` varchar(255),`Electric` varchar(255),`biaozhi` int );";
                            }
                          
                            else if (tables[i].Equals("user"))
                            {
                                sql_createT = "create table user (UserName varchar(255), PassWord varchar(255), Admin int);";
                                sql_initdata = "insert into user values('root', '123','1')";
                            }
                            else if (tables[i].Equals("Equiptype"))
                            {
                                sql_createT = "create table Equiptype ( Etype int , Ename varchar(255) );";
                                sql_initdata = "insert into Equiptype values( '1' ,'压力计'),('2' ,'减速机'),('3', '中继')";

                            }
                            else if (tables[i].Equals("ip"))
                            {
                                sql_createT = "create table ip ( workid int , ip varchar(255),duankou,int);";
                                

                            }






                            try
                            {
                                MySqlConn ms1 = new MySqlConn();
                                int iRet = ms1.nonSelect(sql_createT);
                                ms1.Close();
                                Log("iRet大小=" + iRet);
                                if (iRet == 1)
                                {
                                    Log("建表执行成功");
                                }

                                if (sql_initdata.Equals("") != true)
                                {
                                    MySqlConn ms2 = new MySqlConn();
                                    int iRet2 = ms2.nonSelect(sql_initdata);
                                    ms2.Close();
                                    if (iRet2 == 1) Log("插入数据成功");
                                    else Log("插入数据失败");
                                }


                            }
                            catch (Exception e)
                            {
                                Log("建表时出错" + e.ToString());
                                return "0";
                            }

                        }
                    }

                    Log("数据库表格已完善");//数据库表格已完善

                    return sum_table.ToString() + "|" + table;
                }
            }
#pragma warning disable CS0168 // 声明了变量“ee”，但从未使用过
            catch (Exception ee)
#pragma warning restore CS0168 // 声明了变量“ee”，但从未使用过
            {
                Log("数据库建表失败");
            }

            return "0";

        }
        private void TextBox2_KeyDowm(object sender, KeyEventArgs e)
        {//为了解决keyup的回调问题，将KeyUp改为KeyDown
            if (e.KeyCode == Keys.Return)
            {
                button1.PerformClick();
            }
        }


        private void Form_Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Application.Exit();
            System.Environment.Exit(0);
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                SaveUsrInfo = 1;
            }
            else
                SaveUsrInfo = 0;
        }

        private void Form_Login_Load(object sender, EventArgs e)
        {
            string contosql = conTosql();
           
            if (contosql.Equals("1"))
            {//检测到数据库中的表已经创建齐全
                Log("数据库中表已经存在成功");
                
            }
            else if (contosql.Equals("0"))
            {
                Log("数据库检验表是否 是全的时出错");
            }
            else
            {//创建未创建的数据表
                Log("新创建的数据表" + contosql.ToString());
                
            }

        }
    }
}
