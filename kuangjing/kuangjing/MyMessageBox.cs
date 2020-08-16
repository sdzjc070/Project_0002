using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CCWin;

namespace kuangjing

{
    /// <summary>
    /// 弹出MessageBox，并定时关闭
    /// </summary>
    class MyMessageBox : CCSkinMain
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        public const int WM_CLOSE = 0x10;
        public string message;
        public string title;
        public MyMessageBox(string msg, string title)
        {
            this.message = msg;
            this.title = title;

        }

        public void ShowBox()
        {
            StartKiller(10);
            MessageBox.Show(message, title);
        }

        private void StartKiller(int getTime)
        {
            Timer SetTimer = new Timer();          // 宣告一個Timer物件，名稱為 SetTimer 
            SetTimer.Interval = (getTime * 1000);           //x秒后启动
            SetTimer.Tick += new EventHandler(SetTimer_Tick);   // 建立一个触发Tick 事件 
            SetTimer.Start();                      // 启动Timer (即 SetTimer 开始启动 ) 
        }

        void SetTimer_Tick(object sender, EventArgs e)
        {
            KillMessageBox();          // 執行把 MessageBox 刪除 (即让MessageBox消失) 
            ((Timer)sender).Stop();    // 停止時間。記得要转型为 Timer 型別 
        }

        private void KillMessageBox()
        {
            // 按照MessageBox的标题，找出Messagebox这个视窗   
            IntPtr ptr = FindWindow(null, title);
            if (ptr != IntPtr.Zero)     // 此表示只要 ptr 不为空值的内容时 
            {
                //找到则关闭 MessageBox 視窗   
                PostMessage(ptr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }
    }
}
