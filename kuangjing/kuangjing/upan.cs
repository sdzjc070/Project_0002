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
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;
using CCWin;
using CCWin.SkinClass;
using System.Configuration;

namespace kuangjing
{
    public partial class upan : CCSkinMain
    {
        public upan()
        {
            InitializeComponent();
        }
        private Mutex file_mutex = new Mutex();//文件互斥锁
        
        private void upan_Load(object sender, EventArgs e)
        {

        }
        public static string byteToHexStr(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach(byte b in data)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            }
            return sb.ToString().ToUpper();
            //string returnStr = "";
            //if (bytes != null)
            //{
            //    for (int i = 0; i < Length; i++)
            //    {
            //        returnStr += bytes[i].ToString("X2");
            //    }
            //}
            //return returnStr;
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
        private void trans_sensor(char[] r)//数据存数据库
        {

            string workId = r[0] + "" + r[1  ];//工作面id
            string device_type = r[2  ] + "" + r[3  ];//

            string device_id = r[4  ] + "" + r[5  ];//
            string sensorid = r[6  ] + "" + r[7  ];
            int sensor_id_int = Int32.Parse(sensorid, System.Globalization.NumberStyles.HexNumber);
            float year = Int32.Parse(r[8  ] + "" + r[9  ], System.Globalization.NumberStyles.HexNumber);//年


            float month = Int32.Parse(r[10  ] + "" + r[11  ], System.Globalization.NumberStyles.HexNumber);//月


            float day = Int32.Parse(r[12  ] + "" + r[13  ], System.Globalization.NumberStyles.HexNumber);//日


            float shi = Int32.Parse(r[14  ] + "" + r[15  ], System.Globalization.NumberStyles.HexNumber);//时


            float fen = Int32.Parse(r[16  ] + "" + r[17  ], System.Globalization.NumberStyles.HexNumber);//分


            float miao = Int32.Parse(r[18  ] + "" + r[19  ], System.Globalization.NumberStyles.HexNumber);//秒


            float hmiao = Int32.Parse(r[20  ] + "" + r[21  ], System.Globalization.NumberStyles.HexNumber);//毫秒

            int device_type_int = Int32.Parse(device_type, System.Globalization.NumberStyles.HexNumber);//整型设备类型


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

            int device_id_int = Int32.Parse(device_id, System.Globalization.NumberStyles.HexNumber);

            string time = "20" + year.ToString() + "/" + month.ToString() + "/" + day.ToString() + " " + shi_g + ":" + fen_g + ":" + miao_g.ToString();
            string flag = "" + r[23  ];

            string Zh = r[24  ] + "" + r[25  ];



            float zh_int = Int32.Parse(Zh, System.Globalization.NumberStyles.HexNumber);

            string Zl = r[26  ] + "" + r[27  ];



            float zi_int = Int32.Parse(Zl, System.Globalization.NumberStyles.HexNumber);


            string xS = r[28  ] + "" + r[29  ];
            float i_xs = Int32.Parse(xS, System.Globalization.NumberStyles.HexNumber);

            float and = zh_int * 100 + zi_int + i_xs / 100;
            string dataq = and.ToString();
            if (flag.Equals("0"))
            {
                dataq = "-" + dataq;
            }
            if (flag.Equals("F") == true)
            {

            }
            else
            {
                try
                {

                    string sql = "insert into sensordata (workid,Eid,Sid,time,value,biaozhi,Etype) values(" + workId + ", " + device_id_int + ", " + sensor_id_int + ", '" + time + "', " + dataq + ", " + device_type_int + "," + device_type + ")";
                    MySqlConn ms2 = new MySqlConn();

                    int n = ms2.nonSelect(sql);
                    ms2.Close();
                }
                catch (Exception se)
                {

                }
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            int sum = 0;//记录读取的行数
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文档|*.dat";

            if (ofd.ShowDialog() != DialogResult.OK)
            {//如果没有选择文件直接返回../temp/
                return;
            }
            string fileName = ofd.FileName;
            string s = Path.GetDirectoryName(fileName);

            
            
            string[] data = File.ReadAllLines(fileName, Encoding.Default);
            foreach (string line1 in data)
            {
                string ab = line1;
                string c= @"\";
                string b = s +c+ab;

                using (FileStream fs = new FileStream(b, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        byte[] buffur = new byte[fs.Length];
                        fs.Read(buffur, 0, (int)fs.Length);
                        string str = System.Text.Encoding.UTF8.GetString(buffur);
                        string readstr = byteToHexStr(buffur);
                        string abc = byteToHexStr(buffur); 

                        for (int i = 0; i < readstr.Length; i = i + 32)
                        {
                            
                            for (int j=0;j<32;j++)
                            {
                               //abc[j]= readstr[i + j];
                            }
                            //Thread t0 = new Thread(trans_sensor(abc));//开启循环缓冲区
                            //t0.Start();
                        }
                        string str1 = readstr.Substring(0, 1000);

                        //MessageBox.Show(str1.ToString());
                    }
                    catch (Exception ex)
                    {

                    }
                }
                // StreamReader sr1 = new StreamReader(b, Encoding.Default);

                //string line = null;
                // while ((line = sr1.ReadLine()) != null)
                // {
                //     MessageBox.Show(line.ToString());
                // }




            }


        }
    }
}
