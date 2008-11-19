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
      textBox1.Text = settings.MyApplicationSetting;
      textBox2.Text = settings.MyUserSetting;
    }

    private void button1_Click(object sender, EventArgs e) {
      settings.Save();
    }

    private void textBox1_TextChanged(object sender, EventArgs e) {
      settings.MyApplicationSetting = textBox1.Text;
    }

    private void textBox2_TextChanged(object sender, EventArgs e) {
      settings.MyUserSetting = textBox2.Text;
    }

    private void button2_Click(object sender, EventArgs e) {
      settings.Reload();
      textBox1.Text = settings.MyApplicationSetting;
      textBox2.Text = settings.MyUserSetting;
    }
  }
}
