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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Routing.TSP {
  /// <summary>
  /// Class to represent a <see cref="TSPInjector"/> visually.
  /// </summary>
  [Content(typeof(TSPInjector), true)]
  public partial class TSPInjectorView : ItemViewBase {
    /// <summary>
    /// Gets or set the <see cref="TSPInjector"/> to represent visually.
    /// </summary>        
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public TSPInjector TSPInjector {
      get { return (TSPInjector)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TSPInjectorView"/>.
    /// </summary>
    public TSPInjectorView() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="TSPInjectorView"/> with the given 
    /// <paramref name="tspInjector"/>.
    /// </summary>
    /// <param name="tspInjector">The <see cref="TSPInjector"/> to display.</param>
    public TSPInjectorView(TSPInjector tspInjector)
      : this() {
      TSPInjector = tspInjector;
    }

    /// <summary>
    /// Removes the event handlers in all children.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RemoveItemEvents() {
      operatorBaseVariableInfosView.Operator = null;
      operatorBaseDescriptionView.Operator = null;
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds event handlers in all children.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      operatorBaseVariableInfosView.Operator = TSPInjector;
      operatorBaseDescriptionView.Operator = TSPInjector;
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
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
