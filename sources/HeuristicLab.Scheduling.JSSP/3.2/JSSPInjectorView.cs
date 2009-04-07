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

namespace HeuristicLab.Scheduling.JSSP {
  public partial class JSSPInjectorView : ViewBase {

    public JSSPInjector JSSPInjector {
      get { return (JSSPInjector)Item; }
      set { base.Item = value; }
    }

    public JSSPInjectorView() {
      InitializeComponent();
    }
    public JSSPInjectorView(JSSPInjector jsspInjector)
      : this() {
      JSSPInjector = jsspInjector;
    }

    protected override void RemoveItemEvents() {
      JSSPInjector.Operations.Changed -= new EventHandler(JSSP_Changed);
      operatorBaseDescriptionView.Operator = null;
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      JSSPInjector.Operations.Changed += new EventHandler(JSSP_Changed);
      operatorBaseDescriptionView.Operator = JSSPInjector;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      OperatorBaseView view = new OperatorBaseView(JSSPInjector);
      tabvariables.Controls.Clear();
      tabvariables.Controls.Add(view);
      view.Dock = DockStyle.Fill;
      if (JSSPInjector != null) {
        tb_jobs.Text = JSSPInjector.NrOfJobs.ToString();
        tb_machines.Text = JSSPInjector.NrOfMachines.ToString();
      }
    }

    private void button1_Click(object sender, EventArgs e) {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        JSSPParser parser = null;
        bool success = false;
        try {
          parser = new JSSPParser(openFileDialog.FileName);
          parser.Parse();
          success = true;
        }
        catch (Exception ex) {
          MessageBox.Show(this, ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        if (success) {
          JSSPInjector.Operations = (ItemList)parser.Operations.Clone();
          JSSPInjector.NrOfJobs = parser.NrOfJobs;
          JSSPInjector.NrOfMachines = parser.NrOfMachines;
          Refresh();
        }
      }
    }

    private void JSSP_Changed(object sender, EventArgs e) {
      Refresh();
    }
  }
}
