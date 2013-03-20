#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.ParallelEngine.Views {
  [View("Parallel Engine View")]
  [Content(typeof(ParallelEngine), true)]
  public partial class ParallelEngineView : EngineView {
    public ParallelEngineView() {
      InitializeComponent();
    }

    public new ParallelEngine Content {
      get { return (ParallelEngine)base.Content; }
      set { base.Content = value; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      degreeOfParallelizationNumericUpDown.Enabled = Content != null && !Locked;
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.DegreeOfParallelismChanged += Content_DegreeOfParallelismChanged;
    }
    protected override void DeregisterContentEvents() {
      Content.DegreeOfParallelismChanged -= Content_DegreeOfParallelismChanged;
      base.DeregisterContentEvents();
    }

    private void Content_DegreeOfParallelismChanged(object sender, EventArgs e) {
      degreeOfParallelizationNumericUpDown.Value = Content.DegreeOfParallelism;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) degreeOfParallelizationNumericUpDown.Value = Content.DegreeOfParallelism;
      else degreeOfParallelizationNumericUpDown.Value = -1;
    }

    private void degreeOfParallelizationNumericUpDown_ValueChanged(object sender, EventArgs e) {
      if (Content == null) return;
      if (degreeOfParallelizationNumericUpDown.Value == 0) {
        degreeOfParallelizationNumericUpDown.Value = 1;
        return;
      }
      Content.DegreeOfParallelism = (int)degreeOfParallelizationNumericUpDown.Value;
    }

    protected virtual void infoLabel_DoubleClick(object sender, EventArgs e) {
      const string caption = "Degree of Parallelism Description";
      const string description = @"Specifies the maximum degree of parallelization (-1 no limit given) to balance the maximum processor load. For further information see http://msdn.microsoft.com/en-us/library/system.threading.tasks.paralleloptions.maxdegreeofparallelism.aspx";
      using (TextDialog dialog = new TextDialog(caption, description, true)) {
        dialog.ShowDialog(this);
      }
    }
  }
}
