using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using CCWin;
using Coldairarrow.Util.Sockets;
using MyTest;
using System.Net.Sockets;
using System.Net;

namespace kuangjing
{
    public partial class MainForm : CCSkinMain
    {
        static private Form_Login form_login = null;
        private System.IO.Ports.SerialPort serialPort1=new System.IO.Ports.SerialPort();
        private CircularQueue<string> cirQueue = new CircularQueue<string>(3000);//定义接收机器数据的串口队列
        DataTable dtInfo = new DataTable();
        MySqlConn msc = new MySqlConn();
        private string data_buffer = "";
        private string data_take = "";
        private int data_flag = 0;
        private string workId = "";
        private System.Timers.Timer t_status;//用于遍历状态链表的定时器
        private Mutex file_mutex = new Mutex();//文件互斥锁
        Users curr_user = new Users();
        user1 f1 = new user1();
        user2 f2 = new user2();
        user3 f3 = new user3();
        user4 f4 = new user4();
        company company1 = new company();
        quxian q1 = new quxian();
        shishixianshi s1 = new shishixianshi();
        work w1 = new work();
        //ip ip1 = new ip();
        equipment e1 = new equipment();
        //创建服务器对象，默认监听本机0.0.0.0，端口12345
        Socket serversocket;
        Socket udpserver;
        //SocketServer server = new SocketServer(PORT);//服务端

        string IP1 = "192.168.1.30";
        string port1 = "5001";

        public static int[] yaliji = new int[100];//压力计在线离线标志数组
        public static int[] jiansuji1 = new int[100];//减速机在线离线标志数组
        public static int[] zhongji = new int[1000];//中继在线离线标志数组
        public static int[,] j2 = new int[100,10];
        int[] y1 = new int[100];
        int[] j1 = new int[100];
        int[] z1 = new int[1000];
        int[,] j3 = new int[100, 10];


        uint polynomial = 07;
        uint init = 0;
        List<byte> CRC8List = new List<byte>();
        //创建服务器对象，默认监听本机0.0.0.0，端口12345


        private int flag_threadout = 1;//退出登录时，将这个标识改为0，所有的线程将阻塞
        public MainForm(string name, string admin, Form_Login frm)
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            




        }
        private void SetValueToUI(string counter)//发送ip的函数
        {
            try
            {
                
                string s1 = counter.Replace(" ", "");
                string crc_check = Crc_Check(s1).ToUpper();
                
                string s2 = counter + " " + crc_check;
                Log("发送ip:"+s2);

         
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(IP1);
        
                try
                {
                  
                    clientSocket.Connect(ip, int.Parse(port1));
                }
                catch (Exception e)
                {
                    Log("客户端连接出错" + e.ToString());
                }

                int flag = clientSocket.Send(send_data(s2));

                clientSocket.Close();

                //更新IP地址和端口号
                string sql = "select ip,duankou from ip where workid = 1";
                MySqlDataReader rd = msc.getDataFromTable(sql);
                while (rd.Read())
                {
                    IP1 = rd["ip"].ToString();
                    port1 = rd["duankou"].ToString();
                }
                rd.Close();
                msc.Close();

             



            }
            catch(Exception e)
            {
                Log("ip发送出差：" + e.ToString());
            }
            
            

        }
        private void set(string data)//请求发送实时数据
        {

            try
            {

                string s1 = data.Replace(" ", "");
                Log("原始字符串：" + s1);
                string crc_1 = Crc_Check(s1).ToUpper();
                Log("校验位：" + crc_1);
                string s2 = data + " " + crc_1;
                Log("校验字符串：" + s2);


                Thread t1 = new Thread(Tcp_send);
                t1.Start(s2);


            }
            catch (Exception e)
            {
                Log("请求发送实时数据发送出差：" + e.ToString());
            }
        }
        private void set1(string data)//设置节点采集间隔
        {
            try
            {

                string s1 = data.Replace(" ", "");
                string crc_1 = Crc_Check(s1).ToUpper();

                string s2 = data + " " + crc_1;

                Log("采集间隔指令:" + s2);

                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(IP1);
                try
                {
                    clientSocket.Connect(ip, int.Parse(port1));
                }
                catch (Exception e)
                {
                    Log("客户端连接出错" + e.ToString());
                }

                int flag = clientSocket.Send(send_data(s2));

            }
            catch (Exception e)
            {
                Log("设置节点采集间隔发送出差：" + e.ToString());
            }
        }
        private void set2(string data)//设置节点上传间隔
        {
            try
            {

                string s1 = data.Replace(" ", "");
                string crc_1 = Crc_Check(s1).ToUpper();

                string s2 = data + " " + crc_1;

                Log("上传间隔指令:" + s2);

                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(IP1);
                try
                {
                    clientSocket.Connect(ip, int.Parse(port1));
                }
                catch (Exception e)
                {
                    Log("客户端连接出错" + e.ToString());
                }

                int flag = clientSocket.Send(send_data(s2));
 

            }
            catch (Exception e)
            {
                Log("设置节点上传间隔发送出差：" + e.ToString());
            }
        }
        private void set3(string data)//请求分站保存的历史数据
        {
            try
            {

                string s1 = data.Replace(" ", "");
                string crc_1 = Crc_Check(s1).ToUpper();

                string s2 = data + " " + crc_1;

                Log("s2  是  " + s2);
                Thread t1 = new Thread(Tcp_send);
                t1.Start(s2);


            }
            catch (Exception e)
            {
                Log("请求分站保存的历史数据发送出差：" + e.ToString());
            }
        }
        private void set4(string data)//发送设备基本信息给工作面
        {
            //label1.Text = data;
            //Log1("shuju" + data);

            try
            {

                string s1 = data.Replace(" ", "");
                string crc_1 = Crc_Check(s1).ToUpper();

                string s2 = data + " " + crc_1;

                Log("设备基本信息:" + s2);


                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(IP1);
                try
                {
                    clientSocket.Connect(ip, int.Parse(port1));
                }
                catch (Exception e)
                {
                    Log("客户端连接出错" + e.ToString());
                }

                int flag = clientSocket.Send(send_data(s2));

                clientSocket.Close();

            }
            catch (Exception e)
            {
                Log("发送设备基本信息给工作面发送出差：" + e.ToString());
            }
        }

        private void set5(string data)//发送传感器阈值
        {
            try
            {

                string s1 = data.Replace(" ", "");
                string crc_1 = Crc_Check(s1).ToUpper();

                string s2 = data + " " + crc_1;

                Log("设置传感器阈值:" + s2);


                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(IP1);
                try
                {
                    clientSocket.Connect(ip, int.Parse(port1));
                }
                catch (Exception e)
                {
                    Log("客户端连接出错" + e.ToString());
                }

                int flag = clientSocket.Send(send_data(s2));

                clientSocket.Close();

            }
            catch (Exception e)
            {
                Log("设置传感器阈值出错：" + e.ToString());
            }
        }

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

        /// <summary>
        /// 启动主函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            //Thread t2 = new Thread();//发送指令线程
            //t2.Start();
            //basicdata_xiangying("1");

            //TCP Server
            //serversocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            //IPAddress ip = IPAddress.Parse("192.168.1.31");
            //EndPoint point = new IPEndPoint(ip, PORT);
            //serversocket.Bind(point);
            //serversocket.Listen(10);
            //Thread mtthread = new Thread(ListenClientSocket);
            //mtthread.Start();




            //UDP线程    
           
                udpserver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPAddress ip = IPAddress.Parse("192.168.1.31");
                //IPAddress ip = IPAddress.Parse("192.168.136.213");
                EndPoint point1 = new IPEndPoint(ip, 8001);
                udpserver.Bind(point1);//绑定端口号和IP 


            string sql = "select ip,duankou from ip where workid = 1";
            MySqlDataReader rd = msc.getDataFromTable(sql);
            while (rd.Read())
            {
                IP1 = rd["ip"].ToString();
                port1 = rd["duankou"].ToString();
            }
            rd.Close();
            msc.Close();


            Thread t = new Thread(ReciveMsg);//开启接收消息线程
            t.Start();


            Thread t0 = new Thread(take_data);//开启循环缓冲区
            t0.Start();
            for(int j=0;j<100;j++)
            {
                yaliji[j] = 0;
                jiansuji1[j] = 0;
                y1[j] = 0;
                j1[j] = 0;
                zhongji[j] = 0;
                z1[0] = 0;

            }
            for (int j = 100; j < 1000; j++)
            {
                
                zhongji[j] = 0;
                z1[0] = 0;

            }
            for (int i=0;i<100;i++)
            {
                for(int j=0;j<10;j++)
                {
                    j2[i,j] = 0;
                    j3[i,j] = 0;
                }
            }
            timer1.Enabled = true;
            timer1.Interval = 300000;
        }

        /// <summary>
        /// UDP接收函数
        /// </summary>
        private void ReciveMsg()
        {
            while (true)
            {
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
                byte[] buffer = new byte[1024];
                int length = udpserver.ReceiveFrom(buffer, ref point);//接收数据报
                string message = Encoding.UTF8.GetString(buffer, 0, length);

                string readstr = byteToHexStr(buffer, length);
                //Log("UDP接收到字符串缓冲区" + readstr);

                Thread t2 = new Thread(udp_trans);
                t2.Start(readstr);
               

            }
        }

        private void ListenClientSocket()
        {
            while (true)
              {
                  Socket clientSocket = serversocket.Accept();//接受客户端的连接
                  Log("客户端连接成功");
                      
             
                  Thread receive = new Thread(receiveSocket);//receiveSocket 被传递的方法
                  receive.Start(clientSocket); //clientSocket 被传递的参数
              }
        }

        private void receiveSocket(object clientSocket)  
         {
             Socket myClientSocket = (Socket)clientSocket;

      
            while (myClientSocket!=null && myClientSocket.Connected)
            {
                Log("连接状态：" + myClientSocket.Connected);
                byte[] data = new byte[1024];
                int length = myClientSocket.Receive(data);
                if (length == 0)
                {
                    myClientSocket.Close();
                    break;
                }
           

                string readstr = byteToHexStr(data, length);
                Log("接收到字符串缓冲区" + readstr);


                Thread t1 = new Thread(trans);
                t1.Start(readstr);
            }
   
           
                
        }

        /// <summary>
        ///  利用TCPClient接收并回复
        /// </summary>
        /// <param name="obj"></param>
        private void Tcp_send(Object obj)
        {
            string str = obj.ToString();

            string IP = "";
            string port = "";

            string sql = "select ip,duankou from ip where workid = 1";
            MySqlDataReader rd = msc.getDataFromTable(sql);
            while (rd.Read())
            {
                IP = rd["ip"].ToString();
                port = rd["duankou"].ToString();
            }
            rd.Close();
            msc.Close();
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(IP);
            try
            {
                clientSocket.Connect(ip, int.Parse(port));

            }
            catch (Exception e)
            {
                Log("客户端连接出错" + e.ToString());
            }

            int flag = clientSocket.Send(send_data(str));

            byte[] data1 = new byte[1024];

            while (true)
            {
                int length = clientSocket.Receive(data1);

                if (length == 0)
                {
                    clientSocket.Close();
                    break;
                }
                else
                {
                    Log("长度:" + length);

                    string readstr = byteToHexStr(data1, length);
                    Log("接收到字符串缓冲区" + readstr);

                    trans(readstr);
                }
            }
        }

    
        /// <summary>
        /// 实时数据读取缓冲区
        /// </summary>
        /// <param name="obj"></param>
        private void take_data(object obj)
        {
            try
            {

                while (data_buffer.Length.Equals("") != true)
                {
                    Thread.Sleep(50);
                    int i = 0;//i记录出现":"的位置
                    for (i = 0; i < data_buffer.Length; i++)
                    {

                        if (data_buffer[i] == '5' && data_buffer[i + 1] == '5')//指令的开头
                        {
                            data_take = "";
                            break;
                        }
                    }
                    int j = 0;//j记录出现"\n"的位置
                    for (j = i; j < data_buffer.Length; j++)
                    {
                        if (data_buffer[j] == 'A' && data_buffer[j - 1] == 'A')
                        {
                            data_take += data_buffer.Substring(i, j - i + 1);
                            new Thread(new ParameterizedThreadStart(trans_sensor)).Start(data_take);
                            data_buffer = data_buffer.Remove(i, data_take.Length);
                            //Log("接收到Byte数组位" + data_buffer);
                            data_buffer = "";
                            data_flag = 1;
                            //进行代指令的解析！！！
                            break;
                        }
                    }


                }
            }
            catch (Exception e)
            {
                Log("线程循环" + e.ToString());
            }

        }

     
        /// <summary>
        /// UDP解析数据
        /// </summary>
        /// <param name="obj"></param>
        private void udp_trans(Object obj)
        {
            string ins=obj.ToString();
            //Log("UDP解析" + ins);

            int index_str = ins.Length;//指令长度

           // Log("UDP指令长度：" + index_str);

            string workid = ins[0] + "" + ins[1];//工作面ID

            workId = workid;

            string instruct = ins[2] + "" + ins[3];//十六进制指令

           // Log("UDP  instruct:" + instruct);

            string data_long = ins[4] + "" + ins[5] + ins[6] + "" + ins[7];//数据长度

            //Log("UDP数据长度字符串：" + data_long);

            int data_len = int.Parse(data_long, System.Globalization.NumberStyles.HexNumber);//数据长度十进制

           // Log("UDP数据长度:" + data_len);

            try
            {
                string crc_str = ins.Substring(0, index_str - 2);

                string crc_check = Crc_Check(crc_str).ToUpper();

                string crc_flag = ins[index_str - 2] + "" + ins[index_str - 1];//校验位

               // Log("CRC校验位" + crc_check);
               // Log("指令校验位" + crc_flag);

                if (crc_flag.Equals(crc_check))
                {
                    //Log("UDP校验成功!");
                    try
                    {
                        if (instruct.Equals("00") == true)
                        {
                            //节点实时上传数据
                            Log("UDP实时上传数据");
                            string sensor_data = obj.ToString();
                            cirQueue.In(sensor_data);
                            data_buffer += cirQueue.Out();//获取数据
                            cirQueue.Clear();

                        }
                        else if (instruct.Equals("02") == true)
                        {
                            //请求上位机发送时间
                           // Log("UDP收到数据包");

                            string device_id = ins[8] + "" + ins[9];//工作面编号

                            string sss = send_time(device_id);
                            try
                            {

                                string s1 = sss.Replace(" ", "");
                                string crc_1 = Crc_Check(s1).ToUpper();

                                string s2 = sss + " " + crc_1;

                                //Log("发送时间字符串:" + s2);

                                string IP = "";
                                string port = "";

                                string sql = "select ip,duankou from ip where workid = 1";
                                MySqlDataReader rd = msc.getDataFromTable(sql);
                                while (rd.Read())
                                {
                                    IP = rd["ip"].ToString();
                                    port = rd["duankou"].ToString();
                                }
                                rd.Close();
                                msc.Close();

                                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                IPAddress ip = IPAddress.Parse(IP);
                                try
                                {
                                    clientSocket.Connect(ip, int.Parse(port));
                                }
                                catch (Exception e)
                                {
                                    Log("UDP返回连接出错" + e.ToString());
                                }

                                int flag = clientSocket.Send(send_data(s2));                     

                                //clientSocket.Close();

                            }
                            catch (Exception e)
                            {
                                //Log("UDP时间发送出差：" + e.ToString());
                            }



                        }
                        else if (instruct.Equals("04") == true)
                        {
                            //节点在线心跳包

                            string device_id = ins[8] + "" + ins[9];//工作面编号

                            Log("UDP心跳包" + device_id);
                            string sss = huifuxintiao(device_id);
                            try
                            {

                                string s1 = sss.Replace(" ", "");
                                string crc_1 = Crc_Check(s1).ToUpper();

                                string s2 = sss + " " + crc_1;
                                Log("回复指令" + s2);

                                string IP = "";
                                string port = "";

                                string sql = "select ip,duankou from ip where workid = 1";
                                MySqlDataReader rd = msc.getDataFromTable(sql);
                                while (rd.Read())
                                {
                                    IP = rd["ip"].ToString();
                                    port = rd["duankou"].ToString();
                                }
                                rd.Close();
                                msc.Close();
                                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                IPAddress ip = IPAddress.Parse(IP);
                                try
                                {
                                    clientSocket.Connect(ip, int.Parse(port));
                                }
                                catch (Exception e)
                                {
                                    Log("客户端连接出错" + e.ToString());
                                }

                                int flag = clientSocket.Send(send_data(s2));

                                //Log("UDP心跳发送字节长度" + flag);

                                clientSocket.Close();

                            }
                            catch (Exception e)
                            {
                                Log("心跳出错：" + e.ToString());
                            }



                        }                       
                        else if (instruct.Equals("08") == true)
                        {
                            //请求设备信息

                            string device_id = ins[8] + "" + ins[9];//工作面编号
                            Log("工作面ID:" + device_id);
                            string sss = basicdata_xiangying(device_id);
                            try
                            {

                                string s1 = sss.Replace(" ", "");
                                string crc_1 = Crc_Check(s1).ToUpper();

                                string s2 = sss + " " + crc_1;
                                Log("UDP发送的基础信息：" + s2);

                                //string IP = "";
                                //string port = "";

                                //string sql = "select ip,duankou from ip where workid = 1";
                                //MySqlDataReader rd = msc.getDataFromTable(sql);
                                //while (rd.Read())
                                //{
                                //    IP = rd["ip"].ToString();
                                //    port = rd["duankou"].ToString();
                                //}
                                //rd.Close();
                                //msc.Close();

                                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                IPAddress ip = IPAddress.Parse(IP1);
                                try
                                {
                                    clientSocket.Connect(ip, int.Parse(port1));
                                }
                                catch (Exception e)
                                {
                                    Log("客户端连接出错" + e.ToString());
                                }

                                int flag = clientSocket.Send(send_data(s2));

                                clientSocket.Close();
                               

                            }
                            catch (Exception e)
                            {
                                Log("时间发送出差：" + e.ToString());
                            }


                        }
                        else if (instruct.Equals("0A") == true)
                        {
                            //发送剩余电量
                            
                            string device_type = "" + ins[9];//ins[8] + "" + ins[9];//设备类型

                            string device_id = ins[10] + "" + ins[11];//设备编号

                            string q_electricity = ins[12] + "" + ins[13];//电量百分比


                            float work_flo = Int32.Parse(workId, System.Globalization.NumberStyles.HexNumber);//工作面ID

                            float dev_flo = Int32.Parse(device_id, System.Globalization.NumberStyles.HexNumber);//设备ID

                            float e_flo = Int32.Parse(q_electricity, System.Globalization.NumberStyles.HexNumber);//电量
                            string sql1 = "select count(*) from sensordata where Eid = '" + dev_flo + "' and Etype = 3";
                            MySqlDataReader rd = msc.getDataFromTable(sql1);
                            string zj1="";
                            int a1= (int)dev_flo;
                            while (rd.Read())
                            {
                                zj1 = rd["count(*)"].ToString();
                            }
                            rd.Close();
                            msc.Close();
                            string time= DateTime.Now.ToString(); 
                            Log1("电量" + e_flo + "工作面" + work_flo + "设备号" + dev_flo+"设备类型"+device_type);

                            try
                            {
                                if(zj1=="0")
                                {
                                    string sql = "insert into sensordata (workid,Eid,time,Electric,Etype) values('" + work_flo + "', '" + dev_flo + "','" + time + "','" + e_flo + "',3)";
                                    MySqlConn ms2 = new MySqlConn();
                                    int n = ms2.nonSelect(sql);
                                    zhongji[a1] = 1;
                                    z1[a1] = 1;
                                    ms2.Close();
                                }
                                else
                                {
                                    string sql = "update sensordata set Electric='" + e_flo + "',time ='" + time + "' where workid='" + work_flo + "' and Eid='" + dev_flo + "'and Etype=" + device_type + " order by time DESC LIMIT 1";
                                    MySqlConn ms2 = new MySqlConn();
                                    int n = ms2.nonSelect(sql);
                                    zhongji[a1] = 1;
                                    z1[a1] = 1;
                                    ms2.Close();
                                }
                                //string sql = "insert into sensordata (workid,Eid,Sid,time,value,biaozhi) values(" + workId + ", " + device_id + ", " + sensor_id_int + ", '" + time + "', " + data + ", " + device_type_int + ")";
                                
                            }
                            catch (Exception e)
                            {
                                Log("电量报错数据库" + e.ToString());
                            }
                        }                       
                        else if (instruct.Equals("1F") == true)
                        {
                            Log("测试******8");

                            string IP = "";
                            string port = "";

                            string sql = "select ip,duankou from ip where workid = 1";
                            MySqlDataReader rd = msc.getDataFromTable(sql);
                            while (rd.Read())
                            {
                                IP = rd["ip"].ToString();
                                port = rd["duankou"].ToString();
                            }
                            rd.Close();
                            msc.Close();
                            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            IPAddress ip = IPAddress.Parse(IP);
                            try
                            {
                                clientSocket.Connect(ip, int.Parse(port));
                            }
                            catch (Exception e)
                            {
                                Log("客户端连接出错" + e.ToString());
                            }

        
                            string message2 = "08 09 0a 02";

                            int flag = clientSocket.Send(send_data(message2));
                            Log("发送长度" + flag);


                        }

                    }
                    catch (Exception e)
                    {
                        Log("解析分站指令:" + e.ToString());
                    }
                }

            }
            catch (Exception e)
            {
                Log("CRC校验错误:" + e.ToString());
            }

        }

   

        private void trans(Object obj)
        {

            Log("Trans解析" + obj.ToString());
            string ins = obj.ToString();

            int index_str = ins.Length;//指令长度

            Log("指令长度：" + index_str);

            string workid = ins[0] + "" + ins[1];//工作面ID

            workId = workid;

            string instruct = ins[2] + "" + ins[3];//十六进制指令

            Log("instruct:" + instruct);

            string data_long = ins[4] + "" + ins[5]+ ins[6] + "" + ins[7];//数据长度

            Log("数据长度字符串：" + data_long);

            int data_len = int.Parse(data_long, System.Globalization.NumberStyles.HexNumber);//数据长度十进制

            Log("数据长度:" + data_len);


            try
            {
                string crc_str = ins.Substring(0, index_str - 2);

                string crc_check = Crc_Check(crc_str).ToUpper();

                string crc_flag = ins[index_str - 2] + "" + ins[index_str - 1];//校验位

                Log("CRC校验位" + crc_check);
                Log("指令校验位" + crc_flag);

                if (crc_flag.Equals(crc_check))
                {
                    Log("校验成功!");
                    try
                    {
                        if (instruct.Equals("00") == true)
                        {
                            //节点实时上传数据
                            Log("实时上传数据");
                            string sensor_data = obj.ToString();
                            cirQueue.In(sensor_data);
                            data_buffer += cirQueue.Out();//获取数据
                            cirQueue.Clear();

                        }
                        else if (instruct.Equals("02") == true)
                        {
                            //请求上位机发送时间
                            Log("收到数据包");

                            string device_id = ins[8] + "" + ins[9];//工作面编号

                            string sss = send_time(device_id);
                            try
                            {

                                string s1 = sss.Replace(" ", "");
                                string crc_1 = Crc_Check(s1).ToUpper();

                                string s2 = sss + " " + crc_1;
                                string IP = "";
                                string port = "";

                                string sql = "select ip,duankou from ip where workid = 1";
                                MySqlDataReader rd = msc.getDataFromTable(sql);
                                while (rd.Read())
                                {
                                    IP = rd["ip"].ToString();
                                    port = rd["duankou"].ToString();
                                }
                                rd.Close();
                                msc.Close();
                                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                IPAddress ip = IPAddress.Parse(IP);
                                try
                                {
                                    clientSocket.Connect(ip, int.Parse(port));
                                }
                                catch (Exception e)
                                {
                                    Log("客户端连接出错" + e.ToString());
                                }

                                int flag = clientSocket.Send(send_data(s2));

                            }
                            catch (Exception e)
                            {
                                Log("时间发送出差：" + e.ToString());
                            }



                        }
                        else if (instruct.Equals("04") == true)
                        {
                            //节点在线心跳包

                            string device_id = ins[8] + "" + ins[9];//工作面编号

                            Log("心跳包" +device_id);
                            string sss = huifuxintiao(device_id);
                            try
                            {

                                string s1 = sss.Replace(" ", "");
                                string crc_1 = Crc_Check(s1).ToUpper();

                                string s2 = sss + " " + crc_1;
                                Log("回复指令" + s2);

                                string IP = "";
                                string port = "";

                                string sql = "select ip,duankou from ip where workid = 1";
                                MySqlDataReader rd = msc.getDataFromTable(sql);
                                while (rd.Read())
                                {
                                    IP = rd["ip"].ToString();
                                    port = rd["duankou"].ToString();
                                }
                                rd.Close();
                                msc.Close();
                                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                IPAddress ip = IPAddress.Parse(IP);
                                try
                                {
                                    clientSocket.Connect(ip, int.Parse(port));
                                }
                                catch (Exception e)
                                {
                                    Log("客户端连接出错" + e.ToString());
                                }

                                int flag = clientSocket.Send(send_data(s2));

                                clientSocket.Close();

                            }
                            catch (Exception e)
                            {
                                Log("心跳出错：" + e.ToString());
                            }



                        }
                        else if (instruct.Equals("06") == true)
                        {
                            //分站保存的历史数据
                            string message_type = ins[8] + "" + ins[9];//信息种类
                            int num = 0;

                            if (message_type.Equals("00") == true)
                            {
                                //数据的数量

                                //四个字节表示
                                string message_num = ins[10] + "" + ins[11] + "" + ins[12] + "" + ins[13] + "" + ins[14] + "" + ins[15] + "" + ins[16] + "" + ins[17];

                                num = Int32.Parse(message_num, System.Globalization.NumberStyles.HexNumber);//数据数量

                                Log("传输数量完成：" + num);
                            }
                            else if (message_type.Equals("01") == true)
                            {
                                //数据

                                Log("数据接收");
                                string device_type = ins[10] + "" + ins[11];//设备类型

                                Log("设备类型" + device_type);

                                string device_id = ins[12] + "" + ins[13];//设备编号

                                Log("设备编号" + device_id);

                                string sensor_id = ins[14] + "" + ins[15];//传感器编号

                                Log("传感器编号" + sensor_id);


                                float year = Int32.Parse(ins[16] + "" + ins[17], System.Globalization.NumberStyles.HexNumber);//年

                                float month = Int32.Parse(ins[18] + "" + ins[19], System.Globalization.NumberStyles.HexNumber);//月

                                float day = Int32.Parse(ins[20] + "" + ins[21], System.Globalization.NumberStyles.HexNumber);//日

                                float shi = Int32.Parse(ins[22] + "" + ins[23], System.Globalization.NumberStyles.HexNumber);//时

                                float fen = Int32.Parse(ins[24] + "" + ins[25], System.Globalization.NumberStyles.HexNumber);//分

                                float miao = Int32.Parse(ins[26] + "" + ins[27], System.Globalization.NumberStyles.HexNumber);//秒

                                float hmiao = Int32.Parse(ins[28] + "" + ins[29], System.Globalization.NumberStyles.HexNumber);//毫秒

                                string time = year.ToString() + "/" + month.ToString() + "/" + day.ToString() + "/" + shi.ToString() + "/" + fen.ToString() + "/" + miao.ToString();

                                Log("时间：" + time);

                                string flag = ins[30] + "" + ins[31];//正负号

                                Log("正负" + sensor_id);

                                string zS = ins[32] + "" + ins[33] + "" + ins[34] + "" + ins[35];//整数部分

                                float i_zs = Int32.Parse(zS, System.Globalization.NumberStyles.HexNumber);//整数部分

                                string xS = ins[36] + "" + ins[37];//小数部分

                                float i_xs = Int32.Parse(xS, System.Globalization.NumberStyles.HexNumber);//小数部分

                                float data_f = i_zs + i_xs / 100;//数据

                                Log("历史数据：" + data_f);

                            }
                            else if (message_type.Equals("02") == true)
                            {
                                //结束
                                Log("历史数据传输结束");
                            }
                        }
                        else if (instruct.Equals("08") == true)
                        {
                            //请求设备信息

                            string device_id = ins[8] + "" + ins[9];//工作面编号
                            string sss = basicdata_xiangying(device_id);
                            try
                            {

                                string s1 = sss.Replace(" ", "");
                                string crc_1 = Crc_Check(s1).ToUpper();

                                string s2 = sss + " " + crc_1;

                                string IP = "";
                                string port = "";

                                string sql = "select ip,duankou from ip where workid = 1";
                                MySqlDataReader rd = msc.getDataFromTable(sql);
                                while (rd.Read())
                                {
                                    IP = rd["ip"].ToString();
                                    port = rd["duankou"].ToString();
                                }
                                rd.Close();
                                msc.Close();
                                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                IPAddress ip = IPAddress.Parse(IP);
                                try
                                {
                                    clientSocket.Connect(ip, int.Parse(port));
                                }
                                catch (Exception e)
                                {
                                    Log("客户端连接出错" + e.ToString());
                                }

                                int flag = clientSocket.Send(send_data(s2));
                                //client.Send(s2);

                            }
                            catch (Exception e)
                            {
                                Log("时间发送出差：" + e.ToString());
                            }


                        }
                        else if (instruct.Equals("0A") == true)
                        {
                            //发送剩余电量

                            string device_type = ins[8] + "" + ins[9];//设备类型

                            string device_id = ins[10] + "" + ins[11];//设备编号

                            string q_electricity = ins[12] + "" + ins[13];//电量百分比


                            float work_flo = Int32.Parse(workId, System.Globalization.NumberStyles.HexNumber);//工作面ID

                            float dev_flo = Int32.Parse(device_id, System.Globalization.NumberStyles.HexNumber);//设备ID

                            float e_flo = Int32.Parse(q_electricity, System.Globalization.NumberStyles.HexNumber);//电量

                            Log("工作面ID:" + work_flo + "设备ID:" + dev_flo + "e_flo:" + e_flo);


                            try
                            {

                                //string sql = "insert into sensordata (workid,Eid,Sid,time,value,biaozhi) values(" + workId + ", " + device_id + ", " + sensor_id_int + ", '" + time + "', " + data + ", " + device_type_int + ")";
                                //string sql = "update sensordata set Electric='" + e_flo + "' where workid='" + work_flo + "' and Eid='" + dev_flo + "' order by time DESC LIMIT 1";
                                //MySqlConn ms2 = new MySqlConn();
                                //int n = ms2.nonSelect(sql);
                                //ms2.Close();
                            }
                            catch (Exception e)
                            {
                                Log("电量报错数据库" + e.ToString());
                            }
                        }
                        else if (instruct.Equals("0C") == true)
                        {
                            //分站传送保存实时数据
                            string message_type = ins[8] + "" + ins[9];//信息种类
                            int num = 0;

                            if (message_type.Equals("00") == true)
                            {
                                //数据的数量

                                //四个字节表示
                                string message_num = ins[10] + "" + ins[11] + "" + ins[12] + "" + ins[13] + "" + ins[14] + "" + ins[15] + "" + ins[16] + "" + ins[17];

                                num = Int32.Parse(message_num, System.Globalization.NumberStyles.HexNumber);//数据数量

                                Log("传输数量完成：" + num);
                            }
                            else if (message_type.Equals("01") == true)
                            {
                                //数据
                                
                                    Log("数据接收");
                                    string device_type = ins[10] + "" + ins[11];//设备类型

                                    Log("设备类型" + device_type);

                                    string device_id = ins[12] + "" + ins[13];//设备编号

                                    Log("设备编号" + device_id);

                                    string sensor_id = ins[14] + "" + ins[15];//传感器编号

                                    Log("传感器编号" + sensor_id);

  

                                    float year = Int32.Parse(ins[16] + "" + ins[17], System.Globalization.NumberStyles.HexNumber);//年

                                    float month = Int32.Parse(ins[18] + "" + ins[19], System.Globalization.NumberStyles.HexNumber);//月

                                    float day = Int32.Parse(ins[20] + "" + ins[21], System.Globalization.NumberStyles.HexNumber);//日

                                    float shi = Int32.Parse(ins[22] + "" + ins[23], System.Globalization.NumberStyles.HexNumber);//时

                                    float fen = Int32.Parse(ins[24] + "" + ins[25], System.Globalization.NumberStyles.HexNumber);//分

                                    float miao = Int32.Parse(ins[26] + "" + ins[27], System.Globalization.NumberStyles.HexNumber);//秒

                                    float hmiao = Int32.Parse(ins[28] + "" + ins[29], System.Globalization.NumberStyles.HexNumber);//毫秒

                                    string time = year.ToString() + "/" + month.ToString() + "/" + day.ToString() + "/" + shi.ToString() + "/" + fen.ToString() + "/" + miao.ToString();

                                    Log("时间：" + time);

                                    string flag = ins[30] + "" + ins[31];//正负号
                                  
                                    Log("正负" + sensor_id);

                                    string zS = ins[32] + "" + ins[33] + "" + ins[34] + "" + ins[35];//整数部分

                                    float i_zs = Int32.Parse(zS, System.Globalization.NumberStyles.HexNumber);//整数部分

                                    string xS = ins[36] + "" + ins[37];//小数部分

                                    float i_xs = Int32.Parse(xS, System.Globalization.NumberStyles.HexNumber);//小数部分

                                    float data_f = i_zs + i_xs / 100;//数据

                            
                                    Log("数据：" + data_f);
                                                               
                           
                            }
                            else if (message_type.Equals("02") == true)
                            {
                                //结束
                                Log("历史数据传输结束");
                            }

                        }
                        else if (instruct.Equals("1F") == true)
                        {
                            Log("测试******8");

                            string IP = "";
                            string port = "";

                            string sql = "select ip,duankou from ip where workid = 1";
                            MySqlDataReader rd = msc.getDataFromTable(sql);
                            while (rd.Read())
                            {
                                IP = rd["ip"].ToString();
                                port = rd["duankou"].ToString();
                            }
                            rd.Close();
                            msc.Close();
                            Socket clientSocket= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            IPAddress ip = IPAddress.Parse(IP);
                            try
                            {
                                clientSocket.Connect(ip, int.Parse(port));
                            }catch(Exception e)
                            {
                                Log("客户端连接出错" + e.ToString());
                            }

                            string str = "";
                            string message2 = "08 09 0a 02";                   
                           
                            int flag = clientSocket.Send(send_data(message2));
                            Log("发送长度" + flag);


                        }

                    }
                    catch (Exception e)
                    {
                        Log("解析分站指令:" + e.ToString());
                    }
                }






            }
            catch (Exception e)
            {
                Log("CRC校验错误:" + e.ToString());
            }

        }

        /// <summary>
        /// 十六进制字符串转为byte数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private byte[] send_data(string str)
        {
            string[] ssArray = str.Split(' ');
            List<byte> bytList = new List<byte>();
            foreach (var s in ssArray)
            {                //将十六进制的字符串转换成数值  
                bytList.Add(Convert.ToByte(s, 16));
            }
            return bytList.ToArray();
        }

        /// <summary>
        /// 生成CRC函数
        /// </summary>
        /// <param name="crc_str"></param>
        /// <returns></returns>
        private string Crc_Check(string crc_str)
        {
            CRC8List.Clear();//清空CRC表
            creatCrc8Table();

            List<byte> datalist = new List<byte>();
            string sb = "";

            sb = crc_str;
            for (int i = 0; i < sb.Length; i++)
            {
                try
                {
                    byte b = Convert.ToByte(sb[i].ToString(), 16);
                }
                catch
                {
                    Log("输入数据有误！");

                }
            }

            while (sb.Length != 0)
            {
                if (sb.Length >= 2)
                {
                    datalist.Add(Convert.ToByte(sb.Substring(0, 2), 16));

                    sb = sb.Substring(2);
                }
                else
                {
                    datalist.Add(Convert.ToByte(sb.Substring(0, 1), 16));

                    sb = sb.Substring(1);
                }
            }

            uint mycrc = 0;
            mycrc = calCrc8Table(datalist);
            string mydata = mycrc.ToString("x");

            if (mydata.Length == 1)
            {
                mydata = "0" + mydata;
            }

            return mydata;

        }

        private uint calCrc8Table(List<byte> datalist)
        {
            uint crc = init;

            int i = 0;
            while (i != datalist.Count)
            {
                crc = CRC8List[(int)(crc ^ datalist[i++])];//8
            }


            return (byte)crc;

        }

        /// <summary>
        /// CRC调用函数
        /// </summary>
        private void creatCrc8Table()
        {
            uint i = 0;

            for (i = 0; i <= 0xff; i++)
            {
                byte d = (byte)calTableHightFirst(i);

                CRC8List.Add(d);
            }


        }

        /// <summary>
        /// CRC调用函数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private uint calTableHightFirst(uint value)//MSB First
        {
            uint i = 8, crc = 0;
            uint bigbit = 0;

            crc ^= value;
            bigbit = 0x80;

            for (; i > 0; i--)
            {
                if ((crc & bigbit) > 0)
                {
                    crc = (crc << 1) ^ polynomial;
                }
                else
                {
                    crc = (crc << 1);
                }
            }

            return crc;
        }
        /// <summary>
        /// 解析传感器发来的数据
        /// </summary>
        /// <param name="obj"></param>
        private void trans_sensor(object obj)//数据存数据库
        {
            string ins = obj.ToString();
           // Log1("收到数据" + ins);


            try
            {
                string time1 = DateTime.Now.ToString();
                if (ins.Equals("") != true)
                {
                    if (data_flag == 1)
                    {

                        string length = ins[2] + "" + ins[3];//载体长度

                        string device_type = ins[10] + "" + ins[11];//设备类型

                        

                        if (ins[11] == '0'&& ins[12] == '2')//减速机
                        {
                            string device_id = ins[13] + "" + ins[14];//设备编号

                            string sensor_num = ins[17] + "" + ins[18];//传感器数量
                            float year = Int32.Parse(ins[19] + "" + ins[20], System.Globalization.NumberStyles.HexNumber);//年
                            string year_str = "20" + ins[19] + "" + ins[20];
                            //Log1("年" + year_str);
                            float month = Int32.Parse(ins[21] + "" + ins[22], System.Globalization.NumberStyles.HexNumber);//月
                            string month_str = ins[21] + "" + ins[22];

                            float day = Int32.Parse(ins[23] + "" + ins[24], System.Globalization.NumberStyles.HexNumber);//日
                            string day_str = ins[23] + "" + ins[24];

                            float shi = Int32.Parse(ins[25] + "" + ins[26], System.Globalization.NumberStyles.HexNumber);//时
                            string shi_str = ins[25] + "" + ins[26];

                            float fen = Int32.Parse(ins[27] + "" + ins[28], System.Globalization.NumberStyles.HexNumber);//分
                            string fen_str = ins[27] + "" + ins[28];

                            float miao = Int32.Parse(ins[29] + "" + ins[30], System.Globalization.NumberStyles.HexNumber);//秒
                            string miao_str = ins[29] + "" + ins[30];

                            float hmiao = Int32.Parse(ins[31] + "" + ins[32], System.Globalization.NumberStyles.HexNumber);//毫秒
                            string hmiao_str = ins[31] + "" + ins[32];
                            int sensor_num_int = Int32.Parse(sensor_num, System.Globalization.NumberStyles.HexNumber);//整型传感器数量

                            int device_type_int = Int32.Parse(device_type, System.Globalization.NumberStyles.HexNumber);//整型设备类型
                            int device_id_int = Int32.Parse(device_id, System.Globalization.NumberStyles.HexNumber);
                            string miao_g = miao.ToString();
                            if (miao_g.Length == 1)
                            {
                                miao_g = "0" + miao_g;
                            }
                            string fen_g = fen.ToString();
                            if (fen_g.Length == 1)
                            {
                                fen_g = "0" + fen_g;
                            }
                            string shi_g = shi.ToString();
                            if (shi_g.Length == 1)
                            {
                                shi_g = "0" + shi_g;
                            }


                            string ee = "2";
                            string time = "20" + year.ToString() + "/" + month.ToString() + "/" + day.ToString() + " " + shi_g + ":" + fen_g + ":" + miao_g.ToString();

                           // Log1("时间：" + time);
                           // Log1("传感器数量" + sensor_num_int);
                            for (int i = 0; i < sensor_num_int; i++)
                            {

                                string sensorid = ins[33 + i * 10] + "" + ins[34 + i * 10];
                                string flag = ins[35 + i * 10] + "";

                               // Log1("正负标志:" + flag);


                                string Zh = ins[37 + i * 10] + "" + ins[38 + i * 10];

                                //Log1("整数高位" + Zh);

                                float zh_int = Int32.Parse(Zh, System.Globalization.NumberStyles.HexNumber);

                                string Zl = ins[39 + i * 10] + "" + ins[40 + i * 10];

                                //Log1("整数低位" + Zl);

                                float zi_int = Int32.Parse(Zl, System.Globalization.NumberStyles.HexNumber);


                                string xS = ins[41 + i * 10] + "" + ins[42 + i * 10];
                                float i_xs = Int32.Parse(xS, System.Globalization.NumberStyles.HexNumber);

                                float and = zh_int * 100 + zi_int + i_xs / 100;

                                int sensor_id_int = Int32.Parse(sensorid, System.Globalization.NumberStyles.HexNumber);


                                string data = and.ToString();
                                int a1 = device_id_int;
                                if (flag.Equals("0"))
                                {
                                    data = "-" + data;
                                }

                               // Log1("数据" + data);
                               // Log1("传感器ID" + sensor_id_int);
                                if (flag.Equals("F") == true)
                                {
                                    //j2[a1,sensor_id_int] = 0;
                                }
                               else{
                                    try
                                    {

                                        string sql = "insert into sensordata (workid,Eid,Sid,time,value,biaozhi,Etype) values(" + workId + ", " + device_id_int + ", " + sensor_id_int + ", '" + time1 + "', " + data + ", " + device_type_int + "," + ee + ")";
                                        MySqlConn ms2 = new MySqlConn();
                                       // Log1("成功");
                                        
                                        jiansuji1[a1] = 1;
                                        j1[a1] = 1;
                                        j2[a1,sensor_id_int] = 1;
                                        int n = ms2.nonSelect(sql);
                                        ms2.Close();
                                    }
                                    catch (Exception e)
                                    {
                                        Log("报错数据库" + e.ToString());
                                    }
                                }


                            }

                        }
                        else if (ins[11] == '1')//压力计
                        {
                            string device_id = ins[12] + "" + ins[13];//设备编号
                            
                            string sensor_num = ins[14] + "" + ins[15];//电量

                            float year = Int32.Parse(ins[18] + "" + ins[19], System.Globalization.NumberStyles.HexNumber);//年
                            string year_str = "20" + ins[18] + "" + ins[19];
                            
                            float month = Int32.Parse(ins[20] + "" + ins[21], System.Globalization.NumberStyles.HexNumber);//月
                            string month_str = ins[20] + "" + ins[21];

                            float day = Int32.Parse(ins[22] + "" + ins[23], System.Globalization.NumberStyles.HexNumber);//日
                            string day_str = ins[22] + "" + ins[23];

                            float shi = Int32.Parse(ins[24] + "" + ins[25], System.Globalization.NumberStyles.HexNumber);//时
                            string shi_str = ins[24] + "" + ins[25];

                            float fen = Int32.Parse(ins[26] + "" + ins[27], System.Globalization.NumberStyles.HexNumber);//分
                            string fen_str = ins[26] + "" + ins[27];

                            float miao = Int32.Parse(ins[28] + "" + ins[29], System.Globalization.NumberStyles.HexNumber);//秒
                            string miao_str = ins[28] + "" + ins[29];

                            float hmiao = Int32.Parse(ins[30] + "" + ins[31], System.Globalization.NumberStyles.HexNumber);//毫秒
                            string hmiao_str = ins[30] + "" + ins[31];

                            int sensor_num_int = Int32.Parse(sensor_num, System.Globalization.NumberStyles.HexNumber);//电量

                            int device_type_int = Int32.Parse(device_type, System.Globalization.NumberStyles.HexNumber);//整型设备类型
                            int device_id_int = Int32.Parse(device_id, System.Globalization.NumberStyles.HexNumber);//设备号

                            string miao_g = miao.ToString();
                            if (miao_g.Length == 1)
                            {
                                miao_g = "0" + miao_g;
                            }
                            string fen_g = fen.ToString();
                            if (fen_g.Length == 1)
                            {
                                fen_g = "0" + fen_g;
                            }
                            string shi_g = shi.ToString();
                            if (shi_g.Length == 1)
                            {
                                shi_g = "0" + shi_g;
                            }



                            string time = "20" + year.ToString() + "/" + month.ToString() + "/" + day.ToString() + " " + shi_g + ":" + fen_g + ":" + miao_g.ToString();

                           Log("时间：" + time);
                           // Log1("设备类型" + device_type_int+"设备号"+ device_id);
                            int qq = 1;
                            string ee = "1";
                            string flag =  "" + ins[33];//正负
                            if(flag=="2")
                            {   
                                qq = -1;
                            }

                            string zS = ins[36] + "" + ins[37] + "" + ins[38] + "" + ins[39];
                            float i_zs = Int32.Parse(zS, System.Globalization.NumberStyles.HexNumber);//整数部分

                            string xS = ins[40] + "" + ins[41];
                            float i_xs = Int32.Parse(xS, System.Globalization.NumberStyles.HexNumber);//小数部分
                            float and = qq*(i_zs + i_xs / 100);
                            try
                            {
                                string sql = "";
                                if (sensor_num_int >= 100)
                                {
                                     sql = "insert into sensordata (workid,Eid,Sid,time,value,biaozhi,Etype) values(" + workId + ", " + device_id_int + ", 1, '" + time1 + "', " + and + ", " + device_type_int + "," + ee + ")";
                                }
                                else
                                {
                                     sql = "insert into sensordata (workid,Eid,Sid,time,value,Electric,biaozhi,Etype) values(" + workId + ", " + device_id_int + ", 1, '" + time1 + "', " + and + "," + sensor_num_int + ", " + device_type_int + "," + ee + ")";
                                }
                                
                                MySqlConn ms2 = new MySqlConn();
                                
                                int a1 = device_id_int;
                                yaliji[a1] = 1;
                                y1[a1] = 1;
                                int n = ms2.nonSelect(sql);
                                ms2.Close();
                            }
                            catch (Exception e)
                            {
                                Log("报错数据库" + e.ToString());
                            }


                        }

                    }
                }


            }
            catch (Exception e)
            {

                Log1("数据库错误:" + e.ToString());
            }

        }

        private string  send_time(string workid)//发送时间的拼接数组函数
        {
            string[] sendtime1 = new string[8];
            string sendtime = "";
            string time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string time1 = Ten2Hex(time.Substring(0, 4));
            int week=(int)DateTime.Now.DayOfWeek;
            string temp = "";
            try
            {
                
                if (time1.Length == 3)
                {
                    //Log("time" + time1);
                    temp = "0" + time1;
                    //Log("temp" + temp);
                    sendtime1[0] = temp[0]+""+temp[1];
                    sendtime1[1] = temp[2] + "" + temp[3];
                }
                else
                {
                    sendtime1[0] = time1;
                    sendtime1[0] = temp.Substring(0, 1);
                    sendtime1[1] = temp.Substring(2, 3);
                }
                //Log("sendtime1[0]" + sendtime1[0]);
                //Log("sendtime1[1]" + sendtime1[1]);
                //time1 = Ten2Hex(time.Substring(2, 2));
                //if (time1.Length == 2)
                //    sendtime1[1] = time1;
                //else
                //    sendtime1[1] = "0" + time1;
            }catch(Exception e)
            {
                Log("字符串时间" + e.ToString());
            }
            time1 = Ten2Hex(time.Substring(5, 2));
            if (time1.Length == 2)
                sendtime1[2] = time1;
            else
                sendtime1[2] = "0" + time1;
            time1 = Ten2Hex(time.Substring(8, 2));
            if (time1.Length == 2)
                sendtime1[3] = time1;
            else
                sendtime1[3] = "0" + time1;
            time1 = Ten2Hex(time.Substring(11, 2));
            if (time1.Length == 2)
                sendtime1[4] = time1;
            else
                sendtime1[4] = "0" + time1;
            time1 = Ten2Hex(time.Substring(14, 2));
            if (time1.Length == 2)
                sendtime1[5] = time1;
            else
                sendtime1[5] = "0" + time1;
            time1 = Ten2Hex(time.Substring(17, 2));
            if (time1.Length == 2)
                sendtime1[6] = time1;
            else
                sendtime1[6] = "0" + time1;

            //Log("week" + week);
            if (week == 0)
            {
                week = 7;
            }
            //Log("修改week" + week);

            sendtime1[7] = "0" + week;

            int i;
            for (i = 0; i < 8; i++)
            {
                //Log("sendtime" + sendtime1[i]);
                sendtime = sendtime + " " + sendtime1[i];
                
            }
            //Log("sadadas11" + sendtime);
            string bz2 = Ten2Hex(workid);
            if (bz2.Length == 2)
            {
                
            }
            else
            {
                bz2 = "0" + bz2;
            }
            //Log("sadadas22" + sendtime);
            sendtime = bz2 + " " + "03" + " 00 08" + sendtime;
            //Log("sadadas发送" + sendtime);
           
  

            if(workid=="")
            {
                return "0";
            }
            else
            {
                return sendtime;

            }
        }
        
        //private  int send_ip(object obj)//发送ip的拼接数组函数
        //{
        //    string[] sendip1 = new string[12];
        //    string sendip="";
        //    MySqlConn msc = new MySqlConn();
        //    string sql = "SELECT * from ip";
        //    string localip = GetLocalIP();
        //    string ip="";
        //    string duankou="";
        //    MySqlDataReader rd = msc.getDataFromTable(sql);
        //    while (rd.Read())
        //    {
        //        ip = rd["ip"].ToString();
        //        duankou= rd["duankou"].ToString();
        //    }
        //    rd.Close();
        //    int i = 0, j = 0, geshu = 0;
        //    while (!ip[i].Equals('.'))
        //    {
        //        i++;
        //        geshu++;

        //    }
        //    string bz1 = Ten2Hex(ip.Substring(j, geshu));
        //    if (bz1.Length == 2)
        //    {
        //        sendip1[0] = bz1;
        //    }
        //    else
        //    {
        //        sendip1[0] = "0" + bz1;
        //    }
        //    geshu = 0;
        //    i++;
        //    j = i;
        //    while (!ip[i].Equals('.'))
        //    {
        //        i++;
        //        geshu++;

        //    }
        //    bz1 = Ten2Hex(ip.Substring(j, geshu));
        //    if (bz1.Length == 2)
        //    {
        //        sendip1[1] = bz1;
        //    }
        //    else
        //    {
        //        sendip1[1] = "0" + bz1;
        //    }
        //    geshu = 0;
        //    i++;
        //    j = i;
        //    while (!ip[i].Equals('.'))
        //    {
        //        i++;
        //        geshu++;

        //    }
        //    bz1 = Ten2Hex(ip.Substring(j, geshu));
        //    if (bz1.Length == 2)
        //    {
        //        sendip1[2] = bz1;
        //    }
        //    else
        //    {
        //        sendip1[2] = "0" + bz1;
        //    }

        //    i++;
        //    j = i;
        //    geshu = ip.Length - i;

        //    bz1 = Ten2Hex(ip.Substring(j, geshu));
        //    if (bz1.Length == 2)
        //    {
        //        sendip1[3] = bz1;
        //    }
        //    else
        //    {
        //        sendip1[3] = "0" + bz1;
        //    }
        //    while(duankou.Length<4)
        //    {
        //        duankou = "0" + duankou;
        //    }
        //    bz1 = Ten2Hex(duankou.Substring(0, 2));
        //    if (bz1.Length == 2)
        //    {
        //        sendip1[4] = bz1;
        //    }
        //    else
        //    {
        //        sendip1[4] = "0" + bz1;
        //    }
        //    bz1 = Ten2Hex(duankou.Substring(2, 2));
        //    if (bz1.Length == 2)
        //    {
        //        sendip1[5] = bz1;
        //    }
        //    else
        //    {
        //        sendip1[5] = "0" + bz1;
        //    }
        //    i = 0; j = 0; geshu = 0;
        //    while (!localip[i].Equals('.'))
        //    {
        //        i++;
        //        geshu++;

        //    }
        //     bz1 = Ten2Hex(localip.Substring(j, geshu));
        //    if (bz1.Length == 2)
        //    {
        //        sendip1[6] = bz1;
        //    }
        //    else
        //    {
        //        sendip1[6] = "0" + bz1;
        //    }
        //    geshu = 0;
        //    i++;
        //    j = i;
        //    while (!localip[i].Equals('.'))
        //    {
        //        i++;
        //        geshu++;

        //    }
        //    bz1 = Ten2Hex(localip.Substring(j, geshu));
        //    if (bz1.Length == 2)
        //    {
        //        sendip1[7] = bz1;
        //    }
        //    else
        //    {
        //        sendip1[7] = "0" + bz1;
        //    }
        //    geshu = 0;
        //    i++;
        //    j = i;
        //    while (!localip[i].Equals('.'))
        //    {
        //        i++;
        //        geshu++;

        //    }
        //    bz1 = Ten2Hex(localip.Substring(j, geshu));
        //    if (bz1.Length == 2)
        //    {
        //        sendip1[8] = bz1;
        //    }
        //    else
        //    {
        //        sendip1[8] = "0" + bz1;
        //    }

        //    i++;
        //    j = i;
        //    geshu = localip.Length - i;

        //    bz1 = Ten2Hex(localip.Substring(j, geshu));
        //    if (bz1.Length == 2)
        //    {
        //        sendip1[9] = bz1;
        //    }
        //    else
        //    {
        //        sendip1[9] = "0" + bz1;
        //    }
        //    string duankou1 = "8080";
        //    sendip1[10] = Ten2Hex(duankou1.Substring(0, 2));
        //    sendip1[11] = Ten2Hex(duankou1.Substring(2, 2));

        //    for (i = 0; i < 10; i++)
        //    {
        //        sendip = sendip + " " + sendip1[i];
        //    }
        //    if(sendip=="")
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        return 1;
        //    }

        //}
        public static string GetLocalIP()//获取本机ip地址
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "获取本机IP出错:" + ex.Message.ToString();
            }
        }
        
        private string basicdata_xiangying(string workid)//响应08发送基础信息
        {
            string shebei = " 01";
            MySqlConn msc = new MySqlConn();
            MySqlConn msc1 = new MySqlConn();
            string data = "";
            int length = 12;
            string ss = "";
            string bz = "";
            int elength = 0;
            string sss = "";
            int a1=0;
          
                string sql1 = "SELECT count(*) FROM equipment where workid =" + workid + "";

                MySqlDataReader rd2 = msc.getDataFromTable(sql1);

                while (rd2.Read())
                {
                    ss = rd2["count(*)"].ToString();

                }
                rd2.Close();
                msc.Close();

       
                bz = Ten2Hex(ss);
                 int o = 0;
                if (bz.Length == 2)
                {
                    shebei = shebei + " " + bz;
                }
                else
                {
                    shebei = shebei + " 0" + bz;
                }
                elength = int.Parse(ss);
                string qqq = bz;
                length = length + elength * 9;
                sss = "";




                string sql2 = "SELECT * FROM equipment where workid =" + workid + "";

                MySqlDataReader rd = msc1.getDataFromTable(sql2);

                while (rd.Read())
                {
                    ss = rd["Etype"].ToString();
                    shebei = shebei + " 0" + ss;
                    sss = rd["Eid"].ToString();
                    bz = Ten2Hex(sss);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }
                string ss1 = shuju(workid, sss, ss);
                bz = Ten2Hex(ss1);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }

                ss = rd["caiji"].ToString();
                if (ss == "")
                {
                    shebei = shebei + " 00 00 00";

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
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }

                    s = "";
                    i++;
                    while (ss[i] != '秒')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }
                    s = "";
                    i++;
                    while (ss[i] != '毫')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    a1 = int.Parse(s);
                    a1 = a1 / 100;
                    s = a1.ToString();
                    
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }
                    ss = rd["shangchuan"].ToString();
                    if (ss == "")
                    {
                        shebei = shebei + " 00 00 00";

                    }
                    else
                    {
                        s = "";
                        i = 0;
                        while (ss[i] != '分')
                        {
                            s = s + ss[i];
                            i++;
                        }
                        bz = Ten2Hex(s);
                        if (bz.Length == 2)
                        {
                            shebei = shebei + " " + bz;
                        }
                        else
                        {
                            shebei = shebei + " 0" + bz;
                        }

                        s = "";
                        i++;
                        while (ss[i] != '秒')
                        {
                            s = s + ss[i];
                            i++;
                        }
                        bz = Ten2Hex(s);
                        if (bz.Length == 2)
                        {
                            shebei = shebei + " " + bz;
                        }
                        else
                        {
                            shebei = shebei + " 0" + bz;
                        }
                        s = "";
                        i++;
                        while (ss[i] != '毫')
                        {
                            s = s + ss[i];
                            i++;
                        }
                        a1 = int.Parse(s);
                        a1 = a1 / 100;
                        s = a1.ToString();
                        bz = Ten2Hex(s);
                        if (bz.Length == 2)
                        {
                            shebei = shebei + " " + bz;
                        }
                        else
                        {
                            shebei = shebei + " 0" + bz;
                        }
                    }

                }

            }
                rd.Close();
                msc1.Close();
                shebei = shebei + " 02";

                sql1 = "SELECT count(*) FROM dsensor where workid =" + workid + "";

                MySqlDataReader rd5 = msc.getDataFromTable(sql1);

                while (rd5.Read())
                {
                    ss = rd5["count(*)"].ToString();

                }
                rd5.Close();
                msc.Close();
                bz = Ten2Hex(ss);
                 
                if (bz.Length == 2)
                {
                    shebei = shebei + " " + bz;
                }
                else
                {
                    shebei = shebei + " 0" + bz;
                }
                //length = length + 2;

                elength = int.Parse(ss);
                length = length + elength * 16;
                sss = "";



            sql2 = "SELECT equiptype.Etype,Eid,type.bzid,Sid,Smax,Smin,caiji,shangchuan FROM equiptype,dsensor,type where equiptype.Ename=dsensor.Ename and type.type= dsensor.Sname and workid = " + workid + " ORDER BY Eid asc";

            MySqlDataReader rd6 = msc1.getDataFromTable(sql2);

            while (rd6.Read())
            {
                ss = rd6["Etype"].ToString();

                shebei = shebei + " 0" + ss;
                sss = rd6["Eid"].ToString();
                bz = Ten2Hex(sss);
                if (bz.Length == 2)
                {
                    shebei = shebei + " " + bz;
                }
                else
                {
                    shebei = shebei + " 0" + bz;
                }
                ss = rd6["bzid"].ToString();
                shebei = shebei + " 0" + ss;
                sss = rd6["Sid"].ToString();
                bz = Ten2Hex(sss);
                if (bz.Length == 2)
                {
                    shebei = shebei + " " + bz;
                }
                else
                {
                    shebei = shebei + " 0" + bz;
                }
                sss = rd6["Smax"].ToString();
                if (sss[0] == '-')
                {
                    shebei = shebei + " 00";

                }
                else
                {
                    shebei = shebei + " 08";
                }
                bz = Ten2Hex(sss);
                if (bz.Length == 4)
                {
                    shebei = shebei + " " + bz[0] + bz[1] + " " + bz[2] + bz[3];
                }
                else if (bz.Length == 3)
                {
                    shebei = shebei + " 0" + bz[0] + " " + bz[1] + bz[2];
                }
                else if (bz.Length == 2)
                {
                    shebei = shebei + " 00 " + bz;
                }
                else if (bz.Length == 1)
                {
                    shebei = shebei + " 00 0" + bz;
                }
                sss = rd6["Smin"].ToString();
                if (sss[0] == '-')
                {
                    shebei = shebei + " 00";

                }
                else
                {
                    shebei = shebei + " 08";
                }
                bz = Ten2Hex(sss);
                if (bz.Length == 4)
                {
                    shebei = shebei + " " + bz[0] + bz[1] + " " + bz[2] + bz[3];
                }
                else if (bz.Length == 3)
                {
                    shebei = shebei + " 0" + bz[0] + " " + bz[1] + bz[2];
                }
                else if (bz.Length == 2)
                {
                    shebei = shebei + " 00 " + bz;
                }
                else if (bz.Length == 1)
                {
                    shebei = shebei + " 00 0" + bz;
                }

                ss = rd6["caiji"].ToString();
                if (ss == "")
                {
                    shebei = shebei + " 00 00 00";

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
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }

                    s = "";
                    i++;
                    while (ss[i] != '秒')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }
                    s = "";
                    i++;
                    while (ss[i] != '毫')
                    {
                        s = s + ss[i];
                        i++;
                    }
                     a1 = int.Parse(s);
                    a1 = a1 / 100;
                    s = a1.ToString();
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        shebei = shebei + " " + bz;
                    }
                    else
                    {
                        shebei = shebei + " 0" + bz;
                    }
                    ss = rd6["shangchuan"].ToString();
                    if (ss == "")
                    {
                        shebei = shebei + " 00 00 00";

                    }
                    else
                    {
                        //Log1("ss   " + ss+"    "+ss[1]);
                        s = "";
                        i = 0;
                        while (ss[i] != '分')
                        {
                            s = s + ss[i];
                            i++;

                        }
                        bz = Ten2Hex(s);
                        if (bz.Length == 2)
                        {
                            shebei = shebei + " " + bz;
                        }
                        else
                        {
                            shebei = shebei + " 0" + bz;
                        }

                        s = "";
                        i++;
                        while (ss[i] != '秒')
                        {
                            s = s + ss[i];
                            i++;
                        }
                        bz = Ten2Hex(s);
                        if (bz.Length == 2)
                        {
                            shebei = shebei + " " + bz;
                        }
                        else
                        {
                            shebei = shebei + " 0" + bz;
                        }
                        s = "";
                        i++;
                        while (ss[i] != '毫')
                        {
                            s = s + ss[i];
                            i++;
                        }
                        a1 = int.Parse(s);
                        a1 = a1 / 100;
                        s = a1.ToString();
                        bz = Ten2Hex(s);
                        if (bz.Length == 2)
                        {
                            shebei = shebei + " " + bz;
                        }
                        else
                        {
                            shebei = shebei + " 0" + bz;
                        }
                    }

                }

                //Log1("shebei    " + shebei);
            }
            rd6.Close();
            msc1.Close();
            if (Ten2Hex(workid).Length == 1)
            {
                data = "0" + Ten2Hex(workid);
            }
            else
            {
                data = Ten2Hex(workid);
            }
            data = data + " 0F ";
            string qq = Ten2Hex(length.ToString());
            if (qq.Length == 4)
            {
                data = data + qq[0] + qq[1] + " " + qq[3] + qq[4];
            }
            if (qq.Length == 3)
            {
                data = data + "0" + qq[0] + " " + qq[1] + qq[2];
            }
            if (qq.Length == 2)
            {
                data = data + "00" + " " + qq[0] + qq[1];
            }
            if (qq.Length == 1)
            {
                data = data + "00 0" + qq[0];
            }



            data = data + " " + "00";
            sql2 = "SELECT * FROM `work` where workid = " + workid + " ";
            

            MySqlDataReader rd7 = msc1.getDataFromTable(sql2);

            while (rd7.Read())
            {
                ss = rd7["caiji"].ToString();
                if (ss == "")
                {
                    data = data + " 00 00 00";

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
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        data = data + " " + bz;
                    }
                    else
                    {
                        data = data + " 0" + bz;
                    }

                    s = "";
                    i++;
                    while (ss[i] != '秒')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        data = data + " " + bz;
                    }
                    else
                    {
                        data = data + " 0" + bz;
                    }
                    s = "";
                    i++;
                    while (ss[i] != '毫')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    a1 = int.Parse(s);
                    a1 = a1 / 100;
                    s = a1.ToString();
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        data = data + " " + bz;
                    }
                    else
                    {
                        data = data + " 0" + bz;
                    }

                }
                ss = rd7["shangchuan"].ToString();
                if (ss == "")
                {
                    data = data + " 00 00 00";

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
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        data = data + " " + bz;
                    }
                    else
                    {
                        data = data + " 0" + bz;
                    }

                    s = "";
                    i++;
                    while (ss[i] != '秒')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        data = data + " " + bz;
                    }
                    else
                    {
                        data = data + " 0" + bz;
                    }
                    s = "";
                    i++;
                    while (ss[i] != '毫')
                    {
                        s = s + ss[i];
                        i++;
                    }
                    a1 = int.Parse(s);
                    a1 = a1 / 100;
                    s = a1.ToString();
                    bz = Ten2Hex(s);
                    if (bz.Length == 2)
                    {
                        data = data + " " + bz;
                    }
                    else
                    {
                        data = data + " 0" + bz;
                    }
                }
            }
            rd6.Close();
            msc1.Close();
            if (qqq.Length == 1)
            {
                qqq = "0" + qqq;
            }
            //else if (qqq.Length == 2)
            //{
            //    qqq = "00 " + qqq;
            //}
            //else if (qqq.Length == 3)
            //{
            //    qqq = "0" + qqq[0] + " " + qqq[1] + qqq[2];
            //}
            //else if (qqq.Length == 3)
            //{
            //    qqq = qqq[0] + qqq[1] + " " + qqq[2] + qqq[3];
            //}
            Log1("data  " + data);
            Log1("qqq  " + qqq);
            Log1("shebei  " + shebei);
            data = data + " " + qqq + shebei;
            
            return data;
            
        }

        private void basicData(string workid)
        {
      

        }
        string shuju(string s, string sse,string ss)
        {
            string sql66 = "SELECT * from equiptype where Etype = " + ss + "";
            string hh = "";
            MySqlConn msc1 = new MySqlConn();
            MySqlDataReader rd31 = msc1.getDataFromTable(sql66);

            while (rd31.Read())
            {
                hh = rd31["Ename"].ToString();

            }
            rd31.Close();
            msc1.Close();
            string sql19 = "SELECT count(*) FROM dsensor where workid =" + s + " and dsensor.Ename ='" + hh + "' and dsensor.Eid =" + sse + "";
            string aaa = " 01";
            MySqlConn msc2 = new MySqlConn();
            MySqlDataReader rd3 = msc2.getDataFromTable(sql19);

            while (rd3.Read())
            {
                aaa = rd3["count(*)"].ToString();

            }
            rd3.Close();
            msc2.Close();
            return aaa;
        }
        string huifuxintiao(string ss)
        {
            string data = "01 01 00 01 "+ss;
            return data;

        }

        public static string Ten2Hex(string ten)//十进制转化十六进制函数
        {
            ulong tenValue = Convert.ToUInt64(ten);
            ulong divValue, resValue;
            string hex = "";
            do
            {
                //divValue = (ulong)Math.Floor(tenValue / 16);

                divValue = (ulong)Math.Floor((decimal)(tenValue / 16));

                resValue = tenValue % 16;
                hex = tenValue2Char(resValue) + hex;
                tenValue = divValue;
            }
            while (tenValue >= 16);
            if (tenValue != 0)
                hex = tenValue2Char(tenValue) + hex;
            return hex;
        }

        public static string tenValue2Char(ulong ten)
        {
            switch (ten)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return ten.ToString();
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
                default:
                    return "";
            }
        }
        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void 密码修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            Thread user1 = new Thread(method1);
            user1.Start();
        }
        private void method1(object obj)
        {
            MethodInvoker MethInvo = new MethodInvoker(showuser);
            BeginInvoke(MethInvo);

        }

        public static string byteToHexStr(byte[] bytes,int Length)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }


        /// <summary>
        /// 显示计时窗体
        /// </summary>
        private void showuser()
        {


            if (f1 != null)
            {
                if (f1.IsDisposed)
                    f1 = new user1();//如果已经销毁，则重新创建子窗口对象
                f1.Show();
                f1.Focus();
            }
            else
            {
                f1 = new user1();
                f1.Show();
                f1.Focus();
            }
        }

        private void 用户添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Thread user2 = new Thread(method2);
            user2.Start();
        }
        private void method2(object obj)
        {
            MethodInvoker MethInvo = new MethodInvoker(adduser);
            BeginInvoke(MethInvo);

        }

        /// <summary>
        /// 显示计时窗体
        /// </summary>
        private void adduser()
        {
            if (f2 != null)
            {
                if (f2.IsDisposed)
                    f2 = new user2();//如果已经销毁，则重新创建子窗口对象
                f2.Show();
                f2.Focus();
            }
            else
            {
                f2 = new user2();
                f2.Show();
                f2.Focus();
            }
        }

        private void 用户删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            Thread user3 = new Thread(method3);
            user3.Start();
        }
        private void method3(object obj)
        {
            MethodInvoker MethInvo = new MethodInvoker(usersc);
            BeginInvoke(MethInvo);

        }

        /// <summary>
        /// 显示计时窗体
        /// </summary>
        private void usersc()
        {
            if (f3 != null)
            {
                if (f3.IsDisposed)
                    f3 = new user3();//如果已经销毁，则重新创建子窗口对象
                f3.Show();
                f3.Focus();
            }
            else
            {
                f3 = new user3();
                f3.Show();
                f3.Focus();
            }
        }

        private void 用户初始化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread user4 = new Thread(method5);
            user4.Start();
        }
        private void method5(object obj)
        {
            MethodInvoker MethInvo = new MethodInvoker(usercsh);
            BeginInvoke(MethInvo);

        }

        /// <summary>
        /// 显示计时窗体
        /// </summary>
        private void usercsh()
        {
            if (f4 != null)
            {
                if (f4.IsDisposed)
                    f4 = new user4();//如果已经销毁，则重新创建子窗口对象
                f4.Show();
                f4.Focus();
            }
            else
            {
                f4 = new user4();
                f4.Show();
                f4.Focus();
            }
        }

        private void 退出登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("确认要退出?", "提示", messButton);
            if (dr == DialogResult.OK)
            {
                curr_user.Logout();
                if (form_login == null)
                    form_login = new Form_Login();
                form_login.Show();
                //Form_Login form_Login = new Form_Login();
                //form_Login.Show();


                //form_login.Show();
                flag_threadout = 0;

                Thread.Sleep(300);
                //while (sendIns_queue.Count != 0 || list_status.Count != 0) ;
                this.Dispose();
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void 公司名称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread company = new Thread(method4);
            company.Start();
        }
        private void method4(object obj)
        {
            MethodInvoker MethInvo = new MethodInvoker(companya);
            BeginInvoke(MethInvo);

        }

        /// <summary>
        /// 显示计时窗体
        /// </summary>
        private void companya()
        {
            if (company1 != null)
            {
                if (company1.IsDisposed)
                    company1 = new company();//如果已经销毁，则重新创建子窗口对象
                company1.Show();
                company1.Focus();
            }
            else
            {
                company1 = new company();
                company1.Show();
                company1.Focus();
            }
        }
        private quxian chart = new quxian();


        private void 数据曲线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Thread quxian = new Thread(method6);
            quxian.Start();

            }
            catch (FormatException exc)
            {
                Log("" + exc);

            }
        
        
        }
        private void method6(object obj)
        {
            
            try
            {
                MethodInvoker MethInvo = new MethodInvoker(qu1);
                BeginInvoke(MethInvo);

            }
            catch (FormatException exc)
            {
                Log("" + exc);

            }

        }

        /// <summary>
        /// 显示计时窗体
        /// </summary>
        private void qu1()
        {
            try
            {
                if (q1 != null)
                {
                    if (q1.IsDisposed)
                        q1 = new quxian();//如果已经销毁，则重新创建子窗口对象
                    q1.Show();
                    q1.Focus();
                }
                else
                {
                    q1 = new quxian();
                    q1.Show();
                    q1.Focus();
                }

            }
            catch (FormatException exc)
            {
                Log("" + exc);

            }
            
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            System.Environment.Exit(0);
        }

        private void 分站IP和端口设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Thread ip = new Thread(method7);
            //ip.Start();
          
        }
        //private void method7(object obj)
        //{
        //    MethodInvoker MethInvo = new MethodInvoker(ipduank);
        //    BeginInvoke(MethInvo);

        //}
        //private void ipduank()
        //{
        //    if (ip1 != null)
        //    {
        //        if (ip1.IsDisposed)
        //            ip1 = new ip();//如果已经销毁，则重新创建子窗口对象
        //        ip1.Show();
        //        ip1.Focus();
        //    }
        //    else
        //    {
        //        ip1 = new ip();
        //        ip1.Show();
        //        ip1.Focus();
        //    }
        //}

        private void 实时数据监视ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        private void method8(object obj)
        {
            MethodInvoker MethInvo = new MethodInvoker(shishi1);
            BeginInvoke(MethInvo);

        }
        private void shishi1()
        {
            if (s1 != null)
            {
                if (s1.IsDisposed)
                    s1 = new shishixianshi();//如果已经销毁，则重新创建子窗口对象
                s1.Show();
                s1.Focus();
            }
            else
            {
                s1 = new shishixianshi();
                s1.Show();
                s1.Focus();
            }
        }

        private void 工作面信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            
        }
        private void method9(object obj)
        {
            MethodInvoker MethInvo = new MethodInvoker(work1);
            BeginInvoke(MethInvo);

        }
        private void work1()
        {
            if (w1 != null)
            {
                if (w1.IsDisposed)
                    w1 = new work();//如果已经销毁，则重新创建子窗口对象
                w1.Show();
                w1.Focus();
            }
            else
            {
                w1 = new work();
                w1.Show();
                w1.Focus();
            }
        }

        private void 监测分机设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread equipment = new Thread(method10);
            equipment.Start();
        }
        private void method10(object obj)
        {
            MethodInvoker MethInvo = new MethodInvoker(eq1);
            BeginInvoke(MethInvo);

        }
        private void eq1()
        {
            if (e1 != null)
            {
                if (e1.IsDisposed)
                    e1 = new equipment();//如果已经销毁，则重新创建子窗口对象
                e1.Show();
                e1.Focus();
            }
            else
            {
                e1 = new equipment();
                e1.Show();
                e1.Focus();
            }
        }

        private void lblCounter_Click(object sender, EventArgs e)
        {

        }

        private void 请求发送实时数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            realdata real1 = new realdata(set);
            real1.Show();
        }

        private void 设置节点采集间隔ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        private void 设置节点阈值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
     
        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void 设置节点数据上传间隔ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void 请求分站上保存的历史数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lishishuju jied3 = new lishishuju(set3);
            jied3.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            
        }

        private void 请求发送实时数据ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            realdata real1 = new realdata(set);
            real1.Show();
        }

        private void 请求分站上保存的历史数据ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            lishishuju jied3 = new lishishuju(set3);
            jied3.Show();
        }

        private void 发送设备基本信息数据给工作面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void 压力日报表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 abc = new Form1();
            abc.Show();
        }

        private void 时间分布曲线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dianchi abc = new dianchi();
            abc.Show();
        }

        private void 单个减速机实时数据显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jiansuji jied3 = new jiansuji();
            jied3.Show();
        }

        private void 实时数据显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread shishixianshi = new Thread(method8);
            shishixianshi.Start();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void 工作面设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread work = new Thread(method9);
            work.Start();
        }

        private void 设备设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread equipment = new Thread(method10);
            equipment.Start();
        }

        private void 传感器设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dsensor jied4 = new dsensor();
            jied4.Show();
        }

        private void 分站IP和端口设置ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ip objFrm = new ip(SetValueToUI);
            objFrm.Show();
        }

        private void 设置节点数据采集间隔ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shujucaiji jied1 = new shujucaiji(set1);
            jied1.Show();
        }

        private void 设置节点数据上传间隔ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            shujushangchuan jied2 = new shujushangchuan(set2);
            jied2.Show();
        }

        private void 设置节点阈值ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SensorValue jied1 = new SensorValue(set5);
            jied1.Show();
        }

        private void 发送设备基本信息数据给工作面ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            senddata jied4 = new senddata(set4);
            jied4.Show();
        }

        private void 数据分布图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fenbu jied4 = new fenbu();
            jied4.Show();
        }

        private void 报表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            baobiao jied4 = new baobiao();
            jied4.Show();
        }

        private void 查看设备树ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shebeishu jied4 = new shebeishu();
            jied4.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)//定时器在标志设备是否离线
        {
            for (int j = 0; j < 100; j++)
            {
                if(y1[j]==0)
                {
                    yaliji[j] = 0;

                }
                else
                {
                    y1[j] = 0;
                }
                if (j1[j] == 0)
                    jiansuji1[j] = 0;
                else
                    j1[j] = 0;
            }
            for (int j = 0; j < 1000; j++)
            {
                if (z1[j] == 0)
                {
                    zhongji[j] = 0;

                }
                else
                {
                    z1[j] = 0;
                }
                
            }
            for (int i=0;i<100;i++)
            {
                for(int j=0;j<10;j++)
                {
                    if (j3[i,j] == 0)
                        j2[i,j] = 0;
                    else
                        j3[i,j] = 0;
                }
            }
            string abc= DateTime.Now.ToLocalTime().ToString();
            //MessageBox.Show("时间"+abc);

        }

        private void u盘导入数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            upan jied4 = new upan();
            jied4.Show();
        }
    }

    public delegate void AddOneDelegate(string counter);

}
