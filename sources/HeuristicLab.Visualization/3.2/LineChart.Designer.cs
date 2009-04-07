using System.Windows.Forms;

namespace HeuristicLab.Visualization
{
    partial class LineChart
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
          this.components = new System.ComponentModel.Container();
          this.canvasUI = new HeuristicLab.Visualization.CanvasUI();
          this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
          this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.contextMenuStrip1.SuspendLayout();
          this.SuspendLayout();
          // 
          // canvasUI
          // 
          this.canvasUI.Dock = System.Windows.Forms.DockStyle.Fill;
          this.canvasUI.Location = new System.Drawing.Point(0, 0);
          this.canvasUI.Name = "canvasUI";
          this.canvasUI.Size = new System.Drawing.Size(551, 373);
          this.canvasUI.TabIndex = 0;
          this.canvasUI.Text = "canvas";
          this.canvasUI.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.canvasUI1_MouseWheel);
          this.canvasUI.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvasUI_MouseMove);
          this.canvasUI.ContextMenuStripChanged += new System.EventHandler(this.optionsToolStripMenuItem_Click);
          this.canvasUI.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvasUI1_MouseDown);
          this.canvasUI.MouseUp += new System.Windows.Forms.MouseEventHandler(this.canvasUI_MouseUp);
          this.canvasUI.KeyDown += new System.Windows.Forms.KeyEventHandler(this.canvasUI1_KeyDown);
          // 
          // contextMenuStrip1
          // 
          this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
          this.contextMenuStrip1.Name = "contextMenuStrip1";
          this.contextMenuStrip1.Size = new System.Drawing.Size(123, 26);
          // 
          // optionsToolStripMenuItem
          // 
          this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
          this.optionsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
          this.optionsToolStripMenuItem.Text = "Options";
          this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
          // 
          // LineChart
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.Controls.Add(this.canvasUI);
          this.Name = "LineChart";
          this.Size = new System.Drawing.Size(551, 373);
          this.contextMenuStrip1.ResumeLayout(false);
          this.ResumeLayout(false);

        }

        #endregion

        private CanvasUI canvasUI;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem optionsToolStripMenuItem;
    }
}
