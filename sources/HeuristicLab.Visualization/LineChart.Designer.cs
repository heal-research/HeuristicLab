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
          this.canvas = new HeuristicLab.Visualization.CanvasUI();
          this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
          this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.contextMenuStrip1.SuspendLayout();
          this.SuspendLayout();
          // 
          // canvas
          // 
          this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
          this.canvas.Location = new System.Drawing.Point(0, 0);
          this.canvas.MouseEventListener = null;
          this.canvas.Name = "canvas";
          this.canvas.Size = new System.Drawing.Size(552, 390);
          this.canvas.TabIndex = 0;
          this.canvas.Text = "canvas";
          this.canvas.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.canvasUI1_MouseWheel);
          this.canvas.ContextMenuStripChanged += new System.EventHandler(this.optionsToolStripMenuItem_Click);
          this.canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvasUI1_MouseDown);
          this.canvas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.canvasUI1_KeyDown);
          // 
          // contextMenuStrip1
          // 
          this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
          this.contextMenuStrip1.Name = "contextMenuStrip1";
          this.contextMenuStrip1.Size = new System.Drawing.Size(112, 26);

          // 
          // optionsToolStripMenuItem
          // 
          this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
          this.optionsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
          this.optionsToolStripMenuItem.Text = "Options";
          this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
          // 
          // LineChart
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.Controls.Add(this.canvas);
          this.Name = "LineChart";
          this.Size = new System.Drawing.Size(552, 390);
          this.contextMenuStrip1.ResumeLayout(false);
          this.ResumeLayout(false);

        }

        #endregion

        private CanvasUI canvas;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem optionsToolStripMenuItem;
    }
}
