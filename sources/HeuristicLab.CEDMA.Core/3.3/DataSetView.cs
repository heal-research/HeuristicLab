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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.CEDMA.Core {
  public partial class DataSetView : ViewBase {
    private DataSet dataSet;
    public DataSet DataSet {
      get { return dataSet; }
      set { dataSet = value; }
    }
    private Results results;
    public DataSetView() {
      InitializeComponent();
      Caption = "Data Set";
    }
    public DataSetView(DataSet dataSet)
      : this() {
      DataSet = dataSet;
      UpdateControls();
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      editorGroupBox.Controls.Clear();
      Control problemControl = (Control)DataSet.Problem.CreateView();
      problemControl.Dock = DockStyle.Fill;
      editorGroupBox.Controls.Add(problemControl);
      PopulateViewComboBox();
      resultsButton.Enabled = viewComboBox.SelectedItem != null;
      if (dataSet.Activated) {
        activateButton.Enabled = false;
        editorGroupBox.Enabled = true;
        viewComboBox.Enabled = true;
        resultsButton.Enabled = true;
        progressBar.Enabled = true;
      } else {
        activateButton.Enabled = true;
        editorGroupBox.Enabled = true;
        viewComboBox.Enabled = false;
        resultsButton.Enabled = false;
        progressBar.Enabled = false;
      }
    }


    private void activateButton_Click(object sender, EventArgs e) {
      DataSet.Activate();
      activateButton.Enabled = false;
    }


    private void ReloadResults() {
      results = dataSet.GetResults();
    }
  }
}

