#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Windows.Forms;

namespace HeuristicLab.Core.Views {
  partial class MovieView<T> {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
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
      this.itemsGroupBox = new System.Windows.Forms.GroupBox();
      this.indexLabel = new System.Windows.Forms.Label();
      this.delayComboBox = new System.Windows.Forms.ComboBox();
      this.stopButton = new System.Windows.Forms.Button();
      this.nextButton = new System.Windows.Forms.Button();
      this.lastButton = new System.Windows.Forms.Button();
      this.playButton = new System.Windows.Forms.Button();
      this.previousButton = new System.Windows.Forms.Button();
      this.firstButton = new System.Windows.Forms.Button();
      this.maximumLabel = new System.Windows.Forms.Label();
      this.minimumLabel = new System.Windows.Forms.Label();
      this.trackBar = new System.Windows.Forms.TrackBar();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
      this.itemsGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
      this.SuspendLayout();
      // 
      // itemsGroupBox
      // 
      this.itemsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.itemsGroupBox.Controls.Add(this.indexLabel);
      this.itemsGroupBox.Controls.Add(this.delayComboBox);
      this.itemsGroupBox.Controls.Add(this.stopButton);
      this.itemsGroupBox.Controls.Add(this.nextButton);
      this.itemsGroupBox.Controls.Add(this.lastButton);
      this.itemsGroupBox.Controls.Add(this.playButton);
      this.itemsGroupBox.Controls.Add(this.previousButton);
      this.itemsGroupBox.Controls.Add(this.firstButton);
      this.itemsGroupBox.Controls.Add(this.maximumLabel);
      this.itemsGroupBox.Controls.Add(this.minimumLabel);
      this.itemsGroupBox.Controls.Add(this.trackBar);
      this.itemsGroupBox.Controls.Add(this.viewHost);
      this.itemsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.itemsGroupBox.Name = "itemsGroupBox";
      this.itemsGroupBox.Size = new System.Drawing.Size(532, 383);
      this.itemsGroupBox.TabIndex = 0;
      this.itemsGroupBox.TabStop = false;
      this.itemsGroupBox.Text = "Items";
      // 
      // indexLabel
      // 
      this.indexLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.indexLabel.Location = new System.Drawing.Point(173, 364);
      this.indexLabel.Name = "indexLabel";
      this.indexLabel.Size = new System.Drawing.Size(186, 13);
      this.indexLabel.TabIndex = 9;
      this.indexLabel.Text = "0";
      this.indexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // delayComboBox
      // 
      this.delayComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.delayComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.delayComboBox.FormattingEnabled = true;
      this.delayComboBox.Location = new System.Drawing.Point(472, 355);
      this.delayComboBox.Name = "delayComboBox";
      this.delayComboBox.Size = new System.Drawing.Size(54, 21);
      this.delayComboBox.TabIndex = 11;
      this.toolTip.SetToolTip(this.delayComboBox, "Visualization Delay");
      this.delayComboBox.SelectedIndexChanged += new System.EventHandler(this.delayComboBox_SelectedIndexChanged);
      // 
      // stopButton
      // 
      this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stopButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Stop;
      this.stopButton.Location = new System.Drawing.Point(36, 355);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(24, 24);
      this.stopButton.TabIndex = 7;
      this.toolTip.SetToolTip(this.stopButton, "Stop");
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // nextButton
      // 
      this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.nextButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.MoveNext;
      this.nextButton.Location = new System.Drawing.Point(472, 325);
      this.nextButton.Name = "nextButton";
      this.nextButton.Size = new System.Drawing.Size(24, 24);
      this.nextButton.TabIndex = 4;
      this.nextButton.UseVisualStyleBackColor = true;
      this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
      // 
      // lastButton
      // 
      this.lastButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lastButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.MoveLast;
      this.lastButton.Location = new System.Drawing.Point(502, 325);
      this.lastButton.Name = "lastButton";
      this.lastButton.Size = new System.Drawing.Size(24, 24);
      this.lastButton.TabIndex = 5;
      this.toolTip.SetToolTip(this.lastButton, "Move to Last");
      this.lastButton.UseVisualStyleBackColor = true;
      this.lastButton.Click += new System.EventHandler(this.lastButton_Click);
      // 
      // playButton
      // 
      this.playButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.playButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Play;
      this.playButton.Location = new System.Drawing.Point(6, 355);
      this.playButton.Name = "playButton";
      this.playButton.Size = new System.Drawing.Size(24, 24);
      this.playButton.TabIndex = 6;
      this.toolTip.SetToolTip(this.playButton, "Play");
      this.playButton.UseVisualStyleBackColor = true;
      this.playButton.Click += new System.EventHandler(this.playButton_Click);
      // 
      // previousButton
      // 
      this.previousButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.previousButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.MovePrevious;
      this.previousButton.Location = new System.Drawing.Point(36, 325);
      this.previousButton.Name = "previousButton";
      this.previousButton.Size = new System.Drawing.Size(24, 24);
      this.previousButton.TabIndex = 2;
      this.previousButton.UseVisualStyleBackColor = true;
      this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
      // 
      // firstButton
      // 
      this.firstButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.firstButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.MoveFirst;
      this.firstButton.Location = new System.Drawing.Point(6, 325);
      this.firstButton.Name = "firstButton";
      this.firstButton.Size = new System.Drawing.Size(24, 24);
      this.firstButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.firstButton, "Move to First");
      this.firstButton.UseVisualStyleBackColor = true;
      this.firstButton.Click += new System.EventHandler(this.firstButton_Click);
      // 
      // maximumLabel
      // 
      this.maximumLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.maximumLabel.Location = new System.Drawing.Point(351, 364);
      this.maximumLabel.Name = "maximumLabel";
      this.maximumLabel.Size = new System.Drawing.Size(108, 13);
      this.maximumLabel.TabIndex = 10;
      this.maximumLabel.Text = "10";
      this.maximumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // minimumLabel
      // 
      this.minimumLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.minimumLabel.Location = new System.Drawing.Point(73, 364);
      this.minimumLabel.Name = "minimumLabel";
      this.minimumLabel.Size = new System.Drawing.Size(38, 13);
      this.minimumLabel.TabIndex = 8;
      this.minimumLabel.Text = "0";
      this.minimumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // trackBar
      // 
      this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.trackBar.LargeChange = 1;
      this.trackBar.Location = new System.Drawing.Point(66, 316);
      this.trackBar.Name = "trackBar";
      this.trackBar.Size = new System.Drawing.Size(400, 45);
      this.trackBar.TabIndex = 3;
      this.trackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
      this.trackBar.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(6, 19);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(520, 291);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewType = null;
      // 
      // backgroundWorker
      // 
      this.backgroundWorker.WorkerSupportsCancellation = true;
      this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
      this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
      // 
      // MovieView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.itemsGroupBox);
      this.Name = "MovieView";
      this.Size = new System.Drawing.Size(532, 383);
      this.itemsGroupBox.ResumeLayout(false);
      this.itemsGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    protected GroupBox itemsGroupBox;
    protected ToolTip toolTip;
    protected MainForm.WindowsForms.ViewHost viewHost;
    protected TrackBar trackBar;
    protected Button stopButton;
    protected Button lastButton;
    protected Button playButton;
    protected Button firstButton;
    protected Label maximumLabel;
    protected Label indexLabel;
    protected Label minimumLabel;
    protected System.ComponentModel.BackgroundWorker backgroundWorker;
    protected Button nextButton;
    protected Button previousButton;
    protected ComboBox delayComboBox;
  }
}
