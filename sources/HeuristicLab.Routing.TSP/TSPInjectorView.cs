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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;

namespace HeuristicLab.Routing.TSP {
  public partial class TSPInjectorView : ViewBase {
    public TSPInjector TSPInjector {
      get { return (TSPInjector)Item; }
      set { base.Item = value; }
    }

    public TSPInjectorView() {
      InitializeComponent();
    }
    public TSPInjectorView(TSPInjector tspInjector)
      : this() {
      TSPInjector = tspInjector;
    }

    protected override void RemoveItemEvents() {
      operatorBaseVariableInfosView.Operator = null;
      operatorBaseDescriptionView.Operator = null;
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      operatorBaseVariableInfosView.Operator = TSPInjector;
      operatorBaseDescriptionView.Operator = TSPInjector;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (TSPInjector == null) {
        citiesTextBox.Text = "-";
        bestKnownQualityAvailableCheckBox.Checked = false;
        bestKnownQualityAvailableCheckBox.Enabled = false;
        bestKnownQualityTextBox.Text = "-";
        bestKnownQualityTextBox.Enabled = false;
        importInstanceButton.Enabled = false;
      } else {
        citiesTextBox.Text = TSPInjector.GetVariable("Cities").Value.ToString();
        bestKnownQualityAvailableCheckBox.Checked = TSPInjector.GetVariable("InjectBestKnownQuality").GetValue<BoolData>().Data;
        bestKnownQualityAvailableCheckBox.Enabled = true;
        if (bestKnownQualityAvailableCheckBox.Checked) {
          bestKnownQualityTextBox.Text = TSPInjector.GetVariable("BestKnownQuality").Value.ToString();
          bestKnownQualityTextBox.Enabled = true;
        } else {
          bestKnownQualityTextBox.Text = "-";
          bestKnownQualityTextBox.Enabled = false;
        }
        importInstanceButton.Enabled = true;
      }
    }

    private void bestKnownQualityAvailableCheckBox_CheckedChanged(object sender, EventArgs e) {
      TSPInjector.GetVariable("InjectBestKnownQuality").GetValue<BoolData>().Data = bestKnownQualityAvailableCheckBox.Checked;
      if (bestKnownQualityAvailableCheckBox.Checked) {
        bestKnownQualityTextBox.Text = TSPInjector.GetVariable("BestKnownQuality").Value.ToString();
        bestKnownQualityTextBox.Enabled = true;
      } else {
        bestKnownQualityTextBox.Text = "-";
        bestKnownQualityTextBox.Enabled = false;
      }
    }

    private void bestKnownQualityTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      try {
        TSPInjector.GetVariable("BestKnownQuality").GetValue<DoubleData>().Data = double.Parse(bestKnownQualityTextBox.Text);
      }
      catch (Exception) {
        bestKnownQualityTextBox.SelectAll();
        e.Cancel = true;
      }
    }

    private void importInstanceButton_Click(object sender, EventArgs e) {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        TSPParser parser = null;
        bool success = false;
        try {
          parser = new TSPParser(openFileDialog.FileName);
          parser.Parse();
          success = true;
        }
        catch (Exception ex) {
          MessageBox.Show(this, ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        if (success) {
          TSPInjector.GetVariable("Cities").Value = new IntData(parser.Vertices.GetLength(0));
          TSPInjector.GetVariable("Coordinates").Value = new DoubleMatrixData(parser.Vertices);
          Refresh();
        }
      }
    }
  }
}
