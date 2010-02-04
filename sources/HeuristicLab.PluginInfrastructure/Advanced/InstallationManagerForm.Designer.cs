namespace HeuristicLab.PluginInfrastructure.Advanced {
  partial class InstallationManagerForm {
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
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.detailsTextBox = new System.Windows.Forms.TextBox();
      this.pluginsListView = new System.Windows.Forms.ListView();
      this.tableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // statusStrip
      // 
      this.statusStrip.Location = new System.Drawing.Point(0, 433);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new System.Drawing.Size(394, 22);
      this.statusStrip.TabIndex = 0;
      this.statusStrip.Text = "statusStrip1";
      // 
      // tableLayoutPanel
      // 
      this.tableLayoutPanel.ColumnCount = 1;
      this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel.Controls.Add(this.detailsTextBox, 0, 1);
      this.tableLayoutPanel.Controls.Add(this.pluginsListView, 0, 0);
      this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 2;
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 64F));
      this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 36F));
      this.tableLayoutPanel.Size = new System.Drawing.Size(394, 433);
      this.tableLayoutPanel.TabIndex = 1;
      // 
      // detailsTextBox
      // 
      this.detailsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.detailsTextBox.Location = new System.Drawing.Point(3, 280);
      this.detailsTextBox.Multiline = true;
      this.detailsTextBox.Name = "detailsTextBox";
      this.detailsTextBox.ReadOnly = true;
      this.detailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.detailsTextBox.Size = new System.Drawing.Size(388, 150);
      this.detailsTextBox.TabIndex = 0;
      // 
      // pluginsListView
      // 
      this.pluginsListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pluginsListView.Location = new System.Drawing.Point(3, 3);
      this.pluginsListView.MultiSelect = false;
      this.pluginsListView.Name = "pluginsListView";
      this.pluginsListView.Size = new System.Drawing.Size(388, 271);
      this.pluginsListView.TabIndex = 1;
      this.pluginsListView.UseCompatibleStateImageBehavior = false;
      this.pluginsListView.View = System.Windows.Forms.View.List;
      this.pluginsListView.SelectedIndexChanged += new System.EventHandler(this.pluginsListView_SelectedIndexChanged);
      // 
      // InstallationManagerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(394, 455);
      this.Controls.Add(this.tableLayoutPanel);
      this.Controls.Add(this.statusStrip);
      this.Name = "InstallationManagerForm";
      this.Text = "InstallationManager";
      this.tableLayoutPanel.ResumeLayout(false);
      this.tableLayoutPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    private System.Windows.Forms.TextBox detailsTextBox;
    private System.Windows.Forms.ListView pluginsListView;
  }
}