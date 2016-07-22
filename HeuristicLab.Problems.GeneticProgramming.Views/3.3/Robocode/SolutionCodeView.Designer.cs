#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion
namespace HeuristicLab.Problems.GeneticProgramming.Views.Robocode {
  partial class SolutionCodeView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.programCode = new System.Windows.Forms.TextBox();
      this.btnRunInRobocode = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.SuspendLayout();
      // 
      // programCode
      // 
      this.programCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.programCode.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.programCode.Location = new System.Drawing.Point(3, 3);
      this.programCode.Multiline = true;
      this.programCode.Name = "programCode";
      this.programCode.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.programCode.Size = new System.Drawing.Size(345, 203);
      this.programCode.TabIndex = 0;
      // 
      // btnRunInRobocode
      // 
      this.btnRunInRobocode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRunInRobocode.AutoSize = true;
      this.btnRunInRobocode.Location = new System.Drawing.Point(247, 212);
      this.btnRunInRobocode.Name = "btnRunInRobocode";
      this.btnRunInRobocode.Size = new System.Drawing.Size(101, 23);
      this.btnRunInRobocode.TabIndex = 2;
      this.btnRunInRobocode.Text = "Run in Robocode";
      this.btnRunInRobocode.UseVisualStyleBackColor = true;
      this.btnRunInRobocode.Click += new System.EventHandler(this.btnRunInRobocode_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.AutoSize = true;
      this.btnSave.Location = new System.Drawing.Point(166, 212);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 1;
      this.btnSave.Text = "Save...";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // saveFileDialog
      // 
      this.saveFileDialog.DefaultExt = "java";
      this.saveFileDialog.Filter = "Java |*java";
      // 
      // SolutionCodeView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.btnRunInRobocode);
      this.Controls.Add(this.programCode);
      this.Name = "SolutionCodeView";
      this.Size = new System.Drawing.Size(351, 238);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox programCode;
    private System.Windows.Forms.Button btnRunInRobocode;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.SaveFileDialog saveFileDialog;
  }
}
