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
          this.canvasUI1 = new HeuristicLab.Visualization.CanvasUI();
          this.SuspendLayout();
          // 
          // canvasUI1
          // 
          this.canvasUI1.Location = new System.Drawing.Point(3, 3);
          this.canvasUI1.MouseEventListener = null;
          this.canvasUI1.Name = "canvasUI1";
          this.canvasUI1.Size = new System.Drawing.Size(546, 384);
          this.canvasUI1.TabIndex = 0;
          this.canvasUI1.Text = "canvasUI1";
          // 
          // LineChart
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.Controls.Add(this.canvasUI1);
          this.Name = "LineChart";
          this.Size = new System.Drawing.Size(552, 390);
          this.ResumeLayout(false);

        }

        #endregion

        private CanvasUI canvasUI1;
    }
}
