using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Assignment.QAP {
  public partial class QAPInjectorView : ViewBase {

    public QAPInjectorView() {
      InitializeComponent();
    }

    public QAPInjector injector {
      get { return (QAPInjector)Item; }
      set { base.Item = value; }
    }

    public QAPInjectorView(QAPInjector injector) : this() {
      this.injector = injector; 
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (injector == null) {
        facilitiesTextBox.Text = "-";
        facilitiesTextBox.Enabled = false;
        knownQualCheckBox.Checked = false;
        knownQualCheckBox.Enabled = false; 
        qualTextBox.Visible = false; 
        qualTextBox.Enabled = false;
      } else {
        facilitiesTextBox.Text = injector.GetVariable("Facilities").Value.ToString();
        knownQualCheckBox.Checked = injector.GetVariable("InjectBestKnownQuality").GetValue<BoolData>().Data;
        knownQualCheckBox.Enabled = true;
        if (knownQualCheckBox.Checked) {
          qualTextBox.Text = injector.GetVariable("BestKnownQuality").Value.ToString();
          qualTextBox.Enabled = true;
        } else {
          qualTextBox.Text = "-";
          qualTextBox.Enabled = false;
        }
      }
    }

    private void loadButton_Click(object sender, EventArgs e) {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        QAPParser parser = null;
        bool success = false;
        try {
          parser = new QAPParser();
          parser.Parse(openFileDialog.FileName);
          success = true;
        } catch (Exception ex) {
          MessageBox.Show(this, ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        if (success) {
          injector.GetVariable("Facilities").Value = new IntData(parser.facilities);
          injector.GetVariable("Distances").Value = new DoubleMatrixData(parser.distances);
          injector.GetVariable("Weights").Value = new DoubleMatrixData(parser.weights);
          Refresh();
        }
      }
    }

    protected override void RemoveItemEvents() {
      operatorBaseVariableInfosView.Operator = null;
      operatorBaseDescriptionView.Operator = null;
      base.RemoveItemEvents();
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      operatorBaseVariableInfosView.Operator = injector;
      operatorBaseDescriptionView.Operator = injector;
    }

    private void knownQualCheckBox_CheckedChanged(object sender, EventArgs e) {
      injector.GetVariable("InjectBestKnownQuality").GetValue<BoolData>().Data = knownQualCheckBox.Checked;
      if (knownQualCheckBox.Checked) {
        qualTextBox.Text = injector.GetVariable("BestKnownQuality").Value.ToString();
        qualTextBox.Enabled = true;
      } else {
        qualTextBox.Text = "-";
        qualTextBox.Enabled = false;
      }
    }

    private void qualTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      try {
        injector.GetVariable("BestKnownQuality").GetValue<DoubleData>().Data = double.Parse(qualTextBox.Text);
      } catch (Exception) {
        qualTextBox.SelectAll();
        e.Cancel = true;
      }
    }

    private void facilitiesTextBox_TextChanged(object sender, EventArgs e) {

    }
  }
}
