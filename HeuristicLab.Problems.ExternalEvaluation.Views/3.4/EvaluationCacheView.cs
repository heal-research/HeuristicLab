#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.ExternalEvaluation.Views {

  [View("EvaluationCacheView")]
  [Content(typeof(EvaluationCache), IsDefaultView = true)]
  public sealed partial class EvaluationCacheView : ParameterizedNamedItemView {

    public new EvaluationCache Content {
      get { return (EvaluationCache)base.Content; }
      set { base.Content = value; }
    }

    public EvaluationCacheView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.Changed -= new System.EventHandler(Content_StatusChanged);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += new System.EventHandler(Content_StatusChanged);
    }

    #region Event Handlers (Content)
    void Content_StatusChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_StatusChanged), sender, e);
      else
        hits_sizeTextBox.Text = string.Format("{0}/{1} ({2} active)", Content.Hits, Content.Size, Content.ActiveEvaluations);
    }

    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        hits_sizeTextBox.Text = "#/#";
      } else {
        Content_StatusChanged(this, EventArgs.Empty);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      clearButton.Enabled = !ReadOnly && Content != null;
      saveButton.Enabled = !ReadOnly && Content != null;
    }

    #region Event Handlers (child controls)
    private void clearButton_Click(object sender, EventArgs e) {
      Content.Reset();
    }
    #endregion

    private void saveButton_Click(object sender, EventArgs e) {
      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        saveButton.Enabled = false;
        BackgroundWorker worker = new BackgroundWorker();
        worker.DoWork += (s, a) => {
          Content.Save((string)a.Argument);
        };
        worker.RunWorkerCompleted += (s, a) => {
          SetEnabledStateOfControls();
        };
        worker.RunWorkerAsync(saveFileDialog.FileName);
      }
    }
  }
}
