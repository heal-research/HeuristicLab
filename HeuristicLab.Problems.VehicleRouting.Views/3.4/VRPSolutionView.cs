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
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.VehicleRouting.Views {
  [View("VRPSolution View")]
  [Content(typeof(VRPSolution), true)]
  public partial class VRPSolutionView : HeuristicLab.Core.Views.ItemView {
    public new VRPSolution Content {
      get { return (VRPSolution)base.Content; }
      set { base.Content = value; }
    }

    public VRPSolutionView() {
      InitializeComponent();
      problemInstanceView.ViewsLabelVisible = false;
    }

    protected override void DeregisterContentEvents() {
      Content.SolutionChanged -= new EventHandler(Content_SolutionChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.SolutionChanged += new EventHandler(Content_SolutionChanged);
    }

    private void UpdateContent() {
      var view = problemInstanceView.ActiveView as VRPProblemInstanceView;
      if (view != null) view.Solution = null;

      problemInstanceView.Content = Content.ProblemInstance;

      view = problemInstanceView.ActiveView as VRPProblemInstanceView;
      if (view != null) view.Solution = Content.Solution;

      UpdateTourView();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        problemInstanceView.Content = null;
      } else {
        UpdateContent();
      }
    }

    private void Content_SolutionChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_SolutionChanged), sender, e);
      else {
        UpdateContent();
      }
    }

    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      SetEnabledStateOfControls();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      tabControl1.Enabled = Content != null;
      problemInstanceView.Enabled = Content != null;
    }

    private void UpdateTourView() {
      StringBuilder sb = new StringBuilder();

      if (Content != null && Content.Solution != null) {
        foreach (Tour tour in Content.Solution.GetTours()) {
          foreach (int city in tour.Stops) {
            sb.Append(city);
            sb.Append(" ");
          }
          sb.AppendLine();
        }
      }

      valueTextBox.Text = sb.ToString();
    }
  }
}
