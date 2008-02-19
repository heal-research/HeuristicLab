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

namespace HeuristicLab.Scheduling.JSSP {
  public partial class OperationView : ViewBase {
    public Operation Operation {
      get { return (Operation)Item; }
      set { base.Item = value; }
    }

    public OperationView() {
      InitializeComponent();
    }
    public OperationView(Operation operation)
      : this() {
      Operation = operation;
    }

    protected override void RemoveItemEvents() {
      Operation.Changed -= new EventHandler(Operation_Changed);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      Operation.Changed += new EventHandler(Operation_Changed);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      predTextBox.Enabled = false;
      if (Operation == null) {
        jobTextBox.Enabled = false;
        opTextBox.Enabled = false;
        startTextBox.Enabled = false;
        durTextBox.Enabled = false;
      } else {
        jobTextBox.Enabled = true;
        opTextBox.Enabled = true;
        startTextBox.Enabled = true;
        durTextBox.Enabled = true;
        this.jobTextBox.Text = Operation.Job.ToString();
        this.opTextBox.Text = Operation.OperationIndex.ToString();
        this.startTextBox.Text = Operation.Start.ToString();
        this.durTextBox.Text = Operation.Duration.ToString();
        if (Operation.Predecessors != null) {
          this.predTextBox.Text = Operation.Predecessors.ToString();
        }
      }
    }

    private void Operation_Changed(object sender, EventArgs e) {
      Refresh();
    }

    private void jobTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      try {
        Operation.Job = int.Parse(jobTextBox.Text);
      }
      catch (Exception) {
        jobTextBox.SelectAll();
        e.Cancel = true;
      }
    }

    private void startTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      try {
        Operation.Start = int.Parse(startTextBox.Text);
      }
      catch (Exception) {
        startTextBox.SelectAll();
        e.Cancel = true;
      }
    }

    private void opTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      try {
        Operation.OperationIndex = int.Parse(opTextBox.Text);
      }
      catch (Exception) {
        opTextBox.SelectAll();
        e.Cancel = true;
      }
    }

    private void durTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      try {
        Operation.Duration = int.Parse(durTextBox.Text);
      }
      catch (Exception) {
        durTextBox.SelectAll();
        e.Cancel = true;
      }
    }

  }
}
