namespace WeiBoGrab
{
    partial class LoginForm
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
            this.btExport = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mainListView = new System.Windows.Forms.ListView();
            this.seqCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.telCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pwdCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.leCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btExport
            // 
            this.btExport.Location = new System.Drawing.Point(332, 6);
            this.btExport.Name = "btExport";
            this.btExport.Size = new System.Drawing.Size(75, 23);
            this.btExport.TabIndex = 6;
            this.btExport.Text = "导入数据";
            this.btExport.UseVisualStyleBackColor = true;
            this.btExport.Click += new System.EventHandler(this.btExport_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.mainListView);
            this.splitContainer1.Size = new System.Drawing.Size(966, 817);
            this.splitContainer1.SplitterDistance = 213;
            this.splitContainer1.TabIndex = 9;
            // 
            // mainListView
            // 
            this.mainListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.seqCol,
            this.telCol,
            this.pwdCol,
            this.leCol});
            this.mainListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainListView.FullRowSelect = true;
            this.mainListView.Location = new System.Drawing.Point(0, 0);
            this.mainListView.MultiSelect = false;
            this.mainListView.Name = "mainListView";
            this.mainListView.Size = new System.Drawing.Size(749, 817);
            this.mainListView.TabIndex = 0;
            this.mainListView.UseCompatibleStateImageBehavior = false;
            this.mainListView.View = System.Windows.Forms.View.Details;
            // 
            // seqCol
            // 
            this.seqCol.Text = "序号";
            this.seqCol.Width = 100;
            // 
            // telCol
            // 
            this.telCol.Text = "手机号";
            this.telCol.Width = 120;
            // 
            // pwdCol
            // 
            this.pwdCol.Text = "密码";
            // 
            // leCol
            // 
            this.leCol.Text = "乐豆";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(787, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 12);
            this.label2.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btExport);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 817);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(966, 39);
            this.panel1.TabIndex = 8;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(500, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "查看乐豆";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 856);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btExport;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListView mainListView;
        private System.Windows.Forms.ColumnHeader seqCol;
        private System.Windows.Forms.ColumnHeader telCol;
        private System.Windows.Forms.ColumnHeader pwdCol;
        private System.Windows.Forms.ColumnHeader leCol;
    }
}