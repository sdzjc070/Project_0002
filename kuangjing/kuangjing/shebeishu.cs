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

namespace kuangjing
{
    public partial class shebeishu : Form
    {
        public shebeishu()
        {
            InitializeComponent();
        }
        MySqlConn msc = new MySqlConn();
        int w1 = 1;
        private void shebeishu_Load(object sender, EventArgs e)
        {
            TreeNode rootTreeNode = treeView1.Nodes.Add("Root", "XX煤矿");
            string sql = "select * from  work";
            MySqlDataReader rd = msc.getDataFromTable(sql);
            string[] workid = { };
            string[] workname = { };
            int i = 0, j = 0;
            List<string> _list = new List<string>(workid);
            List<string> _list1 = new List<string>(workname);
            while (rd.Read())
            {
                _list.Add(rd["workid"].ToString());
                _list1.Add(rd["workname"].ToString());
                i++;
            }
            rd.Close();
            msc.Close();
            workid = _list.ToArray();
            workname = _list1.ToArray();
            if (i > 0)
            {
                for (j = 0; j < i; j++)
                {
                    TreeNode mTreeNode = rootTreeNode.Nodes.Add(workname[j]);
                    TreeNode mTreeNode2 = mTreeNode.Nodes.Add("减速机");
                    sql = "select * from equipment where Etype =2 and workid = " + workid[j] + "";
                    MySqlDataReader rd1 = msc.getDataFromTable(sql);
                    while (rd1.Read())
                    {
                        TreeNode mTreeNode3 = mTreeNode2.Nodes.Add(rd1["Eid"].ToString() + "号设备       " + workid[j]);


                    }
                    rd1.Close();
                    msc.Close();
                    TreeNode mTreeNode4 = mTreeNode.Nodes.Add("压力计");
                    sql = "select * from equipment where Etype =1 and workid = " + workid[j] + "";
                    MySqlDataReader rd2 = msc.getDataFromTable(sql);
                    while (rd2.Read())
                    {
                        TreeNode mTreeNode3 = mTreeNode4.Nodes.Add(rd2["Eid"].ToString() + "号设备       "+workid[j]);


                    }
                    rd2.Close();
                    msc.Close();
                    
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode a = e.Node;
            string nodename = a.Text;
            
                if (nodename[2]=='设')
            {
                string[] ids = nodename.Split(' ');
                string[] ids1 = nodename.Split('号');
                string sql = "select sensordata.Electric,sensordata.`value` from sensordata where sensordata.workid =" + ids[7] + " and Eid =" + ids1[0] + " order by time desc limit 1";
                MySqlDataReader rd2 = msc.getDataFromTable(sql);
                while (rd2.Read())
                {

                    this.dataGridView1.Rows[0].Cells[1].Value = rd2["value"].ToString();
                    this.dataGridView1.Rows[0].Cells[4].Value = rd2["Electric"].ToString();

                }
                rd2.Close();
                msc.Close();
                sql = "select workname,`work`.maxpressure,`work`.minpressure from `work` where workid =" + ids[7] + " ";
                MySqlDataReader rd7 = msc.getDataFromTable(sql);
                while (rd7.Read())
                {

                    this.dataGridView1.Rows[0].Cells[3].Value = rd7["workname"].ToString();
                    this.dataGridView1.Rows[0].Cells[5].Value = rd7["maxpressure"].ToString();
                    this.dataGridView1.Rows[0].Cells[6].Value = rd7["minpressure"].ToString();

                }
                rd7.Close();
                msc.Close();
                sql = "select equiptype.Ename from equipment,equiptype where equipment.workid =" + ids[7] + " and equipment.Eid =" + ids1[0] + " and equipment.Etype=equiptype.Etype";
                MySqlDataReader rd1 = msc.getDataFromTable(sql);
                while (rd1.Read())
                {

                    this.dataGridView1.Rows[0].Cells[2].Value = rd1["Ename"].ToString();
                    

                }
                rd1.Close();
                msc.Close();
                //string sql = "select equipment.workid,workname,equipment.Eid,equiptype.Ename,equipment.caiji,equipment.shangchuan from work,equipment,equiptype where `work`.workid = equipment.workid and equipment.Etype = equiptype.Etype ORDER BY workid asc";
                this.dataGridView1.Rows[0].Cells[0].Value = ids[0];
                this.dataGridView1.Rows[0].Cells[7].Value = "是";

                //MySqlConnection conn = msc.GetConn();
                //MySqlDataAdapter sda = new MySqlDataAdapter(sql, conn);//获取数据表
                //                                                       //DataTable table = new DataTable();
                //DataSet ds = new DataSet();
                //sda.Fill(ds, "ds");//填充数据库
                //this.dataGridView1.DataSource = ds.Tables[0];
                //msc.Close();
            }
            
        }
        
    }
}