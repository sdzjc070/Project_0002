namespace kuangjing
{
    partial class shebeishu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.设备名称 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.当前数据 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.设备类型 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.设备地址 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.当前电量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.高阈值 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.低阈值 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.是否在线 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.设备名称,
            this.当前数据,
            this.设备类型,
            this.设备地址,
            this.当前电量,
            this.高阈值,
            this.低阈值,
            this.是否在线});
            this.dataGridView1.Location = new System.Drawing.Point(251, 64);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(779, 334);
            this.dataGridView1.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 19F);
            this.label1.Location = new System.Drawing.Point(25, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 33);
            this.label1.TabIndex = 6;
            this.label1.Text = "设备树";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(690, 462);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(187, 55);
            this.button2.TabIndex = 9;
            this.button2.Text = "数据分析";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(357, 462);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(168, 55);
            this.button1.TabIndex = 8;
            this.button1.Text = "历史数据";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(13, 55);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(219, 473);
            this.treeView1.TabIndex = 5;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // 设备名称
            // 
            this.设备名称.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.设备名称.Frozen = true;
            this.设备名称.HeaderText = "设备名称";
            this.设备名称.Name = "设备名称";
            this.设备名称.Width = 96;
            // 
            // 当前数据
            // 
            this.当前数据.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.当前数据.HeaderText = "当前数据";
            this.当前数据.Name = "当前数据";
            this.当前数据.Width = 96;
            // 
            // 设备类型
            // 
            this.设备类型.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.设备类型.HeaderText = "设备类型";
            this.设备类型.Name = "设备类型";
            this.设备类型.Width = 96;
            // 
            // 设备地址
            // 
            this.设备地址.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.设备地址.HeaderText = "设备地址";
            this.设备地址.Name = "设备地址";
            this.设备地址.Width = 96;
            // 
            // 当前电量
            // 
            this.当前电量.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.当前电量.HeaderText = "当前电量";
            this.当前电量.Name = "当前电量";
            this.当前电量.Width = 96;
            // 
            // 高阈值
            // 
            this.高阈值.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.高阈值.HeaderText = "高阈值";
            this.高阈值.Name = "高阈值";
            this.高阈值.Width = 81;
            // 
            // 低阈值
            // 
            this.低阈值.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.低阈值.HeaderText = "低阈值";
            this.低阈值.Name = "低阈值";
            this.低阈值.Width = 81;
            // 
            // 是否在线
            // 
            this.是否在线.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.是否在线.DataPropertyName = "是";
            this.是否在线.HeaderText = "是否在线";
            this.是否在线.Name = "是否在线";
            this.是否在线.Width = 96;
            // 
            // shebeishu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 598);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "shebeishu";
            this.Text = "工作面设备";
            this.Load += new System.EventHandler(this.shebeishu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn 设备名称;
        private System.Windows.Forms.DataGridViewTextBoxColumn 当前数据;
        private System.Windows.Forms.DataGridViewTextBoxColumn 设备类型;
        private System.Windows.Forms.DataGridViewTextBoxColumn 设备地址;
        private System.Windows.Forms.DataGridViewTextBoxColumn 当前电量;
        private System.Windows.Forms.DataGridViewTextBoxColumn 高阈值;
        private System.Windows.Forms.DataGridViewTextBoxColumn 低阈值;
        private System.Windows.Forms.DataGridViewTextBoxColumn 是否在线;
    }
}