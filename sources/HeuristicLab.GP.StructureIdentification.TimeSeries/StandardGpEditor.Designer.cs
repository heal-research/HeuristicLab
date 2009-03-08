#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.GP.StructureIdentification.TimeSeries {
  partial class StandardGpEditor {
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
      this.label1 = new System.Windows.Forms.Label();
      this.autoregressionCheckbox = new System.Windows.Forms.CheckBox();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.label1);
      this.parametersTabPage.Controls.Add(this.autoregressionCheckbox);
      this.parametersTabPage.Controls.SetChildIndex(this.autoregressionCheckbox, 0);
      this.parametersTabPage.Controls.SetChildIndex(this.label1, 0);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(65, 235);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(127, 13);
      this.label1.TabIndex = 21;
      this.label1.Text = "Autoregressive modelling:";
      // 
      // autoregressionCheckbox
      // 
      this.autoregressionCheckbox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.autoregressionCheckbox.AutoSize = true;
      this.autoregressionCheckbox.Location = new System.Drawing.Point(218, 234);
      this.autoregressionCheckbox.Name = "autoregressionCheckbox";
      this.autoregressionCheckbox.Size = new System.Drawing.Size(135, 17);
      this.autoregressionCheckbox.TabIndex = 20;
      this.autoregressionCheckbox.Text = "Autoregression allowed";
      this.autoregressionCheckbox.UseVisualStyleBackColor = true;
      // 
      // StandardGpEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "StandardGpEditor";
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.parametersTabPage.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckBox autoregressionCheckbox;
  }
}
