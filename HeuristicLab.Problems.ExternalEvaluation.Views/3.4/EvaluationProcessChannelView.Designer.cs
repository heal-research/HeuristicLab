#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.ExternalEvaluation.Views {
  partial class EvaluationProcessChannelView {
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
      this.executableTextBox = new System.Windows.Forms.TextBox();
      this.browseExecutableButton = new System.Windows.Forms.Button();
      this.argumentsTextBox = new System.Windows.Forms.TextBox();
      this.executableLabel = new System.Windows.Forms.Label();
      this.argumentsLabel = new System.Windows.Forms.Label();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.startButton = new System.Windows.Forms.Button();
      this.terminateButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(72, 0);
      this.nameTextBox.Size = new System.Drawing.Size(287, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(370, 3);
      // 
      // executableTextBox
      // 
      this.executableTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.executableTextBox.Location = new System.Drawing.Point(72, 26);
      this.executableTextBox.Name = "executableTextBox";
      this.executableTextBox.ReadOnly = true;
      this.executableTextBox.Size = new System.Drawing.Size(287, 20);
      this.executableTextBox.TabIndex = 4;
      // 
      // browseExecutableButton
      // 
      this.browseExecutableButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.browseExecutableButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.browseExecutableButton.Location = new System.Drawing.Point(365, 24);
      this.browseExecutableButton.Name = "browseExecutableButton";
      this.browseExecutableButton.Size = new System.Drawing.Size(26, 23);
      this.browseExecutableButton.TabIndex = 5;
      this.browseExecutableButton.UseVisualStyleBackColor = true;
      this.browseExecutableButton.Click += new System.EventHandler(this.browseExecutableButton_Click);
      // 
      // argumentsTextBox
      // 
      this.argumentsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.argumentsTextBox.Location = new System.Drawing.Point(72, 52);
      this.argumentsTextBox.Name = "argumentsTextBox";
      this.argumentsTextBox.Size = new System.Drawing.Size(287, 20);
      this.argumentsTextBox.TabIndex = 6;
      this.argumentsTextBox.Validated += new System.EventHandler(this.argumentsTextBox_Validated);
      // 
      // executableLabel
      // 
      this.executableLabel.AutoSize = true;
      this.executableLabel.Location = new System.Drawing.Point(3, 29);
      this.executableLabel.Name = "executableLabel";
      this.executableLabel.Size = new System.Drawing.Size(63, 13);
      this.executableLabel.TabIndex = 7;
      this.executableLabel.Text = "Executable:";
      // 
      // argumentsLabel
      // 
      this.argumentsLabel.AutoSize = true;
      this.argumentsLabel.Location = new System.Drawing.Point(3, 55);
      this.argumentsLabel.Name = "argumentsLabel";
      this.argumentsLabel.Size = new System.Drawing.Size(60, 13);
      this.argumentsLabel.TabIndex = 7;
      this.argumentsLabel.Text = "Arguments:";
      // 
      // openFileDialog
      // 
      this.openFileDialog.Filter = "Executables|*.exe|All files|*.*";
      this.openFileDialog.Title = "Select the executable of the external process";
      // 
      // startButton
      // 
      this.startButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Play;
      this.startButton.Location = new System.Drawing.Point(72, 78);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(26, 23);
      this.startButton.TabIndex = 8;
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButton_Click);
      // 
      // terminateButton
      // 
      this.terminateButton.Enabled = false;
      this.terminateButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Stop;
      this.terminateButton.Location = new System.Drawing.Point(104, 78);
      this.terminateButton.Name = "terminateButton";
      this.terminateButton.Size = new System.Drawing.Size(26, 23);
      this.terminateButton.TabIndex = 8;
      this.terminateButton.UseVisualStyleBackColor = true;
      this.terminateButton.Click += new System.EventHandler(this.terminateButton_Click);
      // 
      // EvaluationProcessChannelView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.executableTextBox);
      this.Controls.Add(this.argumentsLabel);
      this.Controls.Add(this.argumentsTextBox);
      this.Controls.Add(this.startButton);
      this.Controls.Add(this.executableLabel);
      this.Controls.Add(this.browseExecutableButton);
      this.Controls.Add(this.terminateButton);
      this.Name = "EvaluationProcessChannelView";
      this.Size = new System.Drawing.Size(391, 106);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.terminateButton, 0);
      this.Controls.SetChildIndex(this.browseExecutableButton, 0);
      this.Controls.SetChildIndex(this.executableLabel, 0);
      this.Controls.SetChildIndex(this.startButton, 0);
      this.Controls.SetChildIndex(this.argumentsTextBox, 0);
      this.Controls.SetChildIndex(this.argumentsLabel, 0);
      this.Controls.SetChildIndex(this.executableTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox executableTextBox;
    private System.Windows.Forms.Button browseExecutableButton;
    private System.Windows.Forms.TextBox argumentsTextBox;
    private System.Windows.Forms.Label executableLabel;
    private System.Windows.Forms.Label argumentsLabel;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.Button terminateButton;
  }
}
