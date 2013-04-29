#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [View("OKBProblem View")]
  [Content(typeof(SingleObjectiveOKBProblem), true)]
  [Content(typeof(MultiObjectiveOKBProblem), true)]
  public sealed partial class OKBProblemView : NamedItemView {
    public new OKBProblem Content {
      get { return (OKBProblem)base.Content; }
      set { base.Content = value; }
    }

    public OKBProblemView() {
      InitializeComponent();
    }

    protected override void OnInitialized(System.EventArgs e) {
      base.OnInitialized(e);
      RunCreationClient.Instance.Refreshing += new EventHandler(RunCreationClient_Refreshing);
      RunCreationClient.Instance.Refreshed += new EventHandler(RunCreationClient_Refreshed);
      PopulateComboBox();
    }

    protected override void DeregisterContentEvents() {
      Content.ProblemChanged -= new EventHandler(Content_ProblemChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ProblemChanged += new EventHandler(Content_ProblemChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        problemComboBox.SelectedIndex = -1;
        parameterCollectionView.Content = null;
      } else {
        problemComboBox.SelectedItem = RunCreationClient.Instance.Problems.FirstOrDefault(x => x.Id == Content.ProblemId);
        parameterCollectionView.Content = Content.Parameters;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      problemComboBox.Enabled = (Content != null) && !ReadOnly && !Locked && (problemComboBox.Items.Count > 0);
      cloneProblemButton.Enabled = (Content != null) && (Content.ProblemId != -1) && !ReadOnly && !Locked;
      refreshButton.Enabled = (Content != null) && !ReadOnly && !Locked;
      parameterCollectionView.Enabled = Content != null;
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      RunCreationClient.Instance.Refreshing -= new EventHandler(RunCreationClient_Refreshing);
      RunCreationClient.Instance.Refreshed -= new EventHandler(RunCreationClient_Refreshed);
      base.OnClosed(e);
    }

    private void RunCreationClient_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(RunCreationClient_Refreshing), sender, e);
      } else {
        Cursor = Cursors.AppStarting;
        problemComboBox.Enabled = cloneProblemButton.Enabled = refreshButton.Enabled = parameterCollectionView.Enabled = false;
      }
    }
    private void RunCreationClient_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(RunCreationClient_Refreshed), sender, e);
      } else {
        PopulateComboBox();
        SetEnabledStateOfControls();
        Cursor = Cursors.Default;
      }
    }

    #region Content Events
    private void Content_ProblemChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ProblemChanged), sender, e);
      else
        OnContentChanged();
    }
    #endregion

    #region Control Events
    private void cloneProblemButton_Click(object sender, EventArgs e) {
      MainFormManager.MainForm.ShowContent(Content.CloneProblem());
    }
    private void refreshButton_Click(object sender, System.EventArgs e) {
      RunCreationClient.Instance.Refresh();
    }
    private void problemComboBox_SelectedValueChanged(object sender, System.EventArgs e) {
      Problem problem = problemComboBox.SelectedValue as Problem;
      if ((problem != null) && (Content != null)) {
        Content.Load(problem.Id);
        if (Content.ProblemId != problem.Id)  // reset selected item if load was not successful
          problemComboBox.SelectedItem = RunCreationClient.Instance.Problems.FirstOrDefault(x => x.Id == Content.ProblemId);
      }
    }
    #endregion

    #region Helpers
    private void PopulateComboBox() {
      problemComboBox.DataSource = null;
      problemComboBox.DataSource = RunCreationClient.Instance.Problems.ToList();
      problemComboBox.DisplayMember = "Name";
    }
    #endregion
  }
}
