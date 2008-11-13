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
          this.label1 = new System.Windows.Forms.Label();
          this.lblAddress = new System.Windows.Forms.Label();
          this.SuspendLayout();
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Location = new System.Drawing.Point(18, 18);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(113, 13);
          this.label1.TabIndex = 0;
          this.label1.Text = "Hive server running @";
          // 
          // lblAddress
          // 
          this.lblAddress.AutoSize = true;
          this.lblAddress.Location = new System.Drawing.Point(18, 43);
          this.lblAddress.Name = "lblAddress";
          this.lblAddress.Size = new System.Drawing.Size(44, 13);
          this.lblAddress.TabIndex = 1;
          this.lblAddress.Text = "address";
          // 
          // MainForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(304, 78);
          this.Controls.Add(this.lblAddress);
          this.Controls.Add(this.label1);
          this.Name = "MainForm";
          this.Text = "Hive Server Console";
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblAddress;
    }
}