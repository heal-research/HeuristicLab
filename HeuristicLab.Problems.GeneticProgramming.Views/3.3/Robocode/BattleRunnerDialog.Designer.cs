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
  partial class BattleRunnerDialog {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.label1 = new System.Windows.Forms.Label();
      this.robocodePathTextBox = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.nrOfRoundsNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.searchButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.runInRobocodeButton = new System.Windows.Forms.Button();
      this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.enemyCollectionView = new EnemyCollectionView();
      ((System.ComponentModel.ISupportInitialize)(this.nrOfRoundsNumericUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(85, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Robocode Path:";
      // 
      // robocodePathTextBox
      // 
      this.robocodePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.robocodePathTextBox.Location = new System.Drawing.Point(117, 12);
      this.robocodePathTextBox.Name = "robocodePathTextBox";
      this.robocodePathTextBox.Size = new System.Drawing.Size(715, 20);
      this.robocodePathTextBox.TabIndex = 1;
      this.robocodePathTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.robocodePathTextBox_Validating);
      this.robocodePathTextBox.Validated += new System.EventHandler(this.robocodePathTextBox_Validated);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 40);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(99, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Number of Rounds:";
      // 
      // nrOfRoundsNumericUpDown
      // 
      this.nrOfRoundsNumericUpDown.Location = new System.Drawing.Point(117, 38);
      this.nrOfRoundsNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.nrOfRoundsNumericUpDown.Name = "nrOfRoundsNumericUpDown";
      this.nrOfRoundsNumericUpDown.Size = new System.Drawing.Size(73, 20);
      this.nrOfRoundsNumericUpDown.TabIndex = 3;
      this.nrOfRoundsNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // searchButton
      // 
      this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.searchButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Zoom;
      this.searchButton.Location = new System.Drawing.Point(838, 9);
      this.searchButton.Name = "searchButton";
      this.searchButton.Size = new System.Drawing.Size(24, 24);
      this.searchButton.TabIndex = 4;
      this.searchButton.UseVisualStyleBackColor = true;
      this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(787, 567);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 5;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // runInRobocodeButton
      // 
      this.runInRobocodeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.runInRobocodeButton.Location = new System.Drawing.Point(656, 567);
      this.runInRobocodeButton.Name = "runInRobocodeButton";
      this.runInRobocodeButton.Size = new System.Drawing.Size(125, 23);
      this.runInRobocodeButton.TabIndex = 6;
      this.runInRobocodeButton.Text = "Run in Robocode";
      this.runInRobocodeButton.UseVisualStyleBackColor = true;
      this.runInRobocodeButton.Click += new System.EventHandler(this.runInRobocodeButton_Click);
      // 
      // folderBrowserDialog
      // 
      this.folderBrowserDialog.ShowNewFolderButton = false;
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // enemyCollectionView
      // 
      this.enemyCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.enemyCollectionView.Caption = "EnemyCollection View";
      this.enemyCollectionView.Content = null;
      this.enemyCollectionView.Location = new System.Drawing.Point(12, 64);
      this.enemyCollectionView.Name = "enemyCollectionView";
      this.enemyCollectionView.ReadOnly = false;
      this.enemyCollectionView.Size = new System.Drawing.Size(850, 497);
      this.enemyCollectionView.TabIndex = 0;
      // 
      // BattleRunnerDialog
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(874, 602);
      this.Controls.Add(this.runInRobocodeButton);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.searchButton);
      this.Controls.Add(this.enemyCollectionView);
      this.Controls.Add(this.nrOfRoundsNumericUpDown);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.robocodePathTextBox);
      this.Controls.Add(this.label1);
      this.Name = "BattleRunnerDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Run Battle";
      ((System.ComponentModel.ISupportInitialize)(this.nrOfRoundsNumericUpDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox robocodePathTextBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown nrOfRoundsNumericUpDown;
    private EnemyCollectionView enemyCollectionView;
    private System.Windows.Forms.Button searchButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button runInRobocodeButton;
    private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    private System.Windows.Forms.ErrorProvider errorProvider;
  }
}