#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.QuadraticAssignment.Views {
  partial class QuadraticAssignmentProblemView {
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
      this.importInstanceButton = new System.Windows.Forms.Button();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.QAPLIBInstancesLabel = new System.Windows.Forms.Label();
      this.instancesComboBox = new System.Windows.Forms.ComboBox();
      this.loadInstanceButton = new System.Windows.Forms.Button();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.problemTabPage = new System.Windows.Forms.TabPage();
      this.visualizationTabPage = new System.Windows.Forms.TabPage();
      this.qapView = new HeuristicLab.Problems.QuadraticAssignment.Views.QAPVisualizationControl();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Location = new System.Drawing.Point(8, 107);
      this.parameterCollectionView.Size = new System.Drawing.Size(629, 375);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(105, 29);
      this.nameTextBox.Size = new System.Drawing.Size(517, 20);
      // 
      // nameLabel
      // 
      this.nameLabel.Location = new System.Drawing.Point(3, 32);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(628, 32);
      // 
      // importInstanceButton
      // 
      this.importInstanceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.importInstanceButton.Location = new System.Drawing.Point(548, 0);
      this.importInstanceButton.Name = "importInstanceButton";
      this.importInstanceButton.Size = new System.Drawing.Size(99, 23);
      this.importInstanceButton.TabIndex = 5;
      this.importInstanceButton.Text = "Import...";
      this.importInstanceButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.importInstanceButton, "Import files in QAPLIB format.");
      this.importInstanceButton.UseVisualStyleBackColor = true;
      this.importInstanceButton.Click += new System.EventHandler(this.importInstanceButton_Click);
      // 
      // openFileDialog
      // 
      this.openFileDialog.FileName = "instance.dat";
      this.openFileDialog.Filter = "Dat files|*.dat|All files|*.*";
      // 
      // QAPLIBInstancesLabel
      // 
      this.QAPLIBInstancesLabel.AutoSize = true;
      this.QAPLIBInstancesLabel.Cursor = System.Windows.Forms.Cursors.Hand;
      this.QAPLIBInstancesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.QAPLIBInstancesLabel.ForeColor = System.Drawing.Color.Blue;
      this.QAPLIBInstancesLabel.Location = new System.Drawing.Point(3, 5);
      this.QAPLIBInstancesLabel.Name = "QAPLIBInstancesLabel";
      this.QAPLIBInstancesLabel.Size = new System.Drawing.Size(96, 13);
      this.QAPLIBInstancesLabel.TabIndex = 6;
      this.QAPLIBInstancesLabel.Text = "QAPLIB instances:";
      this.toolTip.SetToolTip(this.QAPLIBInstancesLabel, "These instances were taken from the QAPLIB homepage at http://www.seas.upenn.edu/" +
              "qaplib/");
      this.QAPLIBInstancesLabel.Click += new System.EventHandler(this.QAPLIBInstancesLabel_Click);
      this.QAPLIBInstancesLabel.MouseEnter += new System.EventHandler(this.QAPLIBInstancesLabel_MouseEnter);
      this.QAPLIBInstancesLabel.MouseLeave += new System.EventHandler(this.QAPLIBInstancesLabel_MouseLeave);
      // 
      // instancesComboBox
      // 
      this.instancesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.instancesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.instancesComboBox.FormattingEnabled = true;
      this.instancesComboBox.Location = new System.Drawing.Point(105, 2);
      this.instancesComboBox.Name = "instancesComboBox";
      this.instancesComboBox.Size = new System.Drawing.Size(358, 21);
      this.instancesComboBox.TabIndex = 7;
      this.instancesComboBox.SelectedValueChanged += new System.EventHandler(this.instancesComboBox_SelectedValueChanged);
      // 
      // loadInstanceButton
      // 
      this.loadInstanceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.loadInstanceButton.Location = new System.Drawing.Point(469, 0);
      this.loadInstanceButton.Name = "loadInstanceButton";
      this.loadInstanceButton.Size = new System.Drawing.Size(73, 23);
      this.loadInstanceButton.TabIndex = 5;
      this.loadInstanceButton.Text = "Load";
      this.loadInstanceButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.loadInstanceButton, "Load the selected QAPLIB instance.");
      this.loadInstanceButton.UseVisualStyleBackColor = true;
      this.loadInstanceButton.Click += new System.EventHandler(this.loadInstanceButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.problemTabPage);
      this.tabControl.Controls.Add(this.visualizationTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 55);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(647, 437);
      this.tabControl.TabIndex = 8;
      // 
      // problemTabPage
      // 
      this.problemTabPage.Location = new System.Drawing.Point(4, 22);
      this.problemTabPage.Name = "problemTabPage";
      this.problemTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.problemTabPage.Size = new System.Drawing.Size(639, 411);
      this.problemTabPage.TabIndex = 0;
      this.problemTabPage.Text = "Problem";
      this.problemTabPage.UseVisualStyleBackColor = true;
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.Controls.Add(this.qapView);
      this.visualizationTabPage.Location = new System.Drawing.Point(4, 22);
      this.visualizationTabPage.Name = "visualizationTabPage";
      this.visualizationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.visualizationTabPage.Size = new System.Drawing.Size(639, 411);
      this.visualizationTabPage.TabIndex = 1;
      this.visualizationTabPage.Text = "Visualization";
      this.visualizationTabPage.UseVisualStyleBackColor = true;
      // 
      // qapView
      // 
      this.qapView.Assignment = null;
      this.qapView.Distances = null;
      this.qapView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.qapView.Location = new System.Drawing.Point(3, 3);
      this.qapView.Name = "qapView";
      this.qapView.Size = new System.Drawing.Size(633, 405);
      this.qapView.TabIndex = 0;
      this.qapView.Weights = null;
      // 
      // QuadraticAssignmentProblemView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.instancesComboBox);
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.importInstanceButton);
      this.Controls.Add(this.loadInstanceButton);
      this.Controls.Add(this.QAPLIBInstancesLabel);
      this.Name = "QuadraticAssignmentProblemView";
      this.Size = new System.Drawing.Size(647, 492);
      this.Controls.SetChildIndex(this.QAPLIBInstancesLabel, 0);
      this.Controls.SetChildIndex(this.loadInstanceButton, 0);
      this.Controls.SetChildIndex(this.importInstanceButton, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.instancesComboBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.parameterCollectionView, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.visualizationTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button importInstanceButton;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.Label QAPLIBInstancesLabel;
    private System.Windows.Forms.ComboBox instancesComboBox;
    private System.Windows.Forms.Button loadInstanceButton;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage problemTabPage;
    private System.Windows.Forms.TabPage visualizationTabPage;
    private QAPVisualizationControl qapView;
  }
}
