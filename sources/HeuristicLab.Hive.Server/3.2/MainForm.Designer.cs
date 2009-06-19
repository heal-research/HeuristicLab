namespace HeuristicLab.Hive.Server
{
    partial class MainForm
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
          this.components = new System.ComponentModel.Container();
          this.label1 = new System.Windows.Forms.Label();
          this.rtfServices = new System.Windows.Forms.RichTextBox();
          this.ni = new System.Windows.Forms.NotifyIcon(this.components);
          this.ctxNotifyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
          this.tsShowInfo = new System.Windows.Forms.ToolStripMenuItem();
          this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
          this.tsExit = new System.Windows.Forms.ToolStripMenuItem();
          this.btnClose = new System.Windows.Forms.Button();
          this.ctxNotifyMenu.SuspendLayout();
          this.SuspendLayout();
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Location = new System.Drawing.Point(12, 9);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(97, 13);
          this.label1.TabIndex = 0;
          this.label1.Text = "Available Services:";
          // 
          // rtfServices
          // 
          this.rtfServices.Location = new System.Drawing.Point(12, 25);
          this.rtfServices.Name = "rtfServices";
          this.rtfServices.ReadOnly = true;
          this.rtfServices.Size = new System.Drawing.Size(280, 121);
          this.rtfServices.TabIndex = 1;
          this.rtfServices.Text = "";
          // 
          // ni
          // 
          this.ni.ContextMenuStrip = this.ctxNotifyMenu;
          this.ni.Text = "notifyIcon1";
          this.ni.Visible = true;
          // 
          // ctxNotifyMenu
          // 
          this.ctxNotifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsShowInfo,
            this.toolStripSeparator1,
            this.tsExit});
          this.ctxNotifyMenu.Name = "ctxNotifyMenu";
          this.ctxNotifyMenu.Size = new System.Drawing.Size(106, 54);
          // 
          // tsShowInfo
          // 
          this.tsShowInfo.Name = "tsShowInfo";
          this.tsShowInfo.Size = new System.Drawing.Size(105, 22);
          this.tsShowInfo.Text = "&Info";
          this.tsShowInfo.Click += new System.EventHandler(this.ShowInfo);
          // 
          // toolStripSeparator1
          // 
          this.toolStripSeparator1.Name = "toolStripSeparator1";
          this.toolStripSeparator1.Size = new System.Drawing.Size(102, 6);
          // 
          // tsExit
          // 
          this.tsExit.Name = "tsExit";
          this.tsExit.Size = new System.Drawing.Size(105, 22);
          this.tsExit.Text = "E&xit";
          this.tsExit.Click += new System.EventHandler(this.CloseApp);
          // 
          // btnClose
          // 
          this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
          this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.btnClose.Location = new System.Drawing.Point(279, 1);
          this.btnClose.Name = "btnClose";
          this.btnClose.Size = new System.Drawing.Size(22, 21);
          this.btnClose.TabIndex = 2;
          this.btnClose.Text = "X";
          this.btnClose.UseVisualStyleBackColor = true;
          this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
          // 
          // MainForm
          // 
          this.AcceptButton = this.btnClose;
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.CancelButton = this.btnClose;
          this.ClientSize = new System.Drawing.Size(304, 166);
          this.ControlBox = false;
          this.Controls.Add(this.btnClose);
          this.Controls.Add(this.rtfServices);
          this.Controls.Add(this.label1);
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
          this.Name = "MainForm";
          this.ShowInTaskbar = false;
          this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
          this.Text = "Hive Server";
          this.TopMost = true;
          this.ctxNotifyMenu.ResumeLayout(false);
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtfServices;
        private System.Windows.Forms.NotifyIcon ni;
        private System.Windows.Forms.ContextMenuStrip ctxNotifyMenu;
        private System.Windows.Forms.ToolStripMenuItem tsShowInfo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsExit;
        private System.Windows.Forms.Button btnClose;
    }
}