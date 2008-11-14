using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Settings {
  public partial class MainForm : Form {
    private Properties.Settings settings;

    public MainForm() {
      InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e) {
      settings = Properties.Settings.Default;
      textBox1.Text = settings.AnotherSetting;
      textBox2.Text = settings.MySetting;
    }

    private void button1_Click(object sender, EventArgs e) {
      settings.Save();
    }

    private void textBox1_TextChanged(object sender, EventArgs e) {
      settings.AnotherSetting = textBox1.Text;
    }
  }
}
