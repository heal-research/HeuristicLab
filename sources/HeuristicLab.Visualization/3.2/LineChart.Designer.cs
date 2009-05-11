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
          this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
          this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.contextMenu.SuspendLayout();
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
          // contextMenu
          // 
          this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.exportToolStripMenuItem});
          this.contextMenu.Name = "contextMenuStrip1";
          this.contextMenu.Size = new System.Drawing.Size(153, 70);
          // 
          // optionsToolStripMenuItem
          // 
          this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
          this.optionsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
          this.optionsToolStripMenuItem.Text = "Opti&ons...";
          this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
          // 
          // exportToolStripMenuItem
          // 
          this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
          this.exportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
          this.exportToolStripMenuItem.Text = "E&xport...";
          this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
          // 
          // LineChart
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.Controls.Add(this.canvasUI);
          this.Name = "LineChart";
          this.Size = new System.Drawing.Size(551, 373);
          this.contextMenu.ResumeLayout(false);
          this.ResumeLayout(false);

        }

        #endregion

        private CanvasUI canvasUI;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem;
    }
}
