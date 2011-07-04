#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.ExternalEvaluation.Views {
  [View("Process-Channel View")]
  [Content(typeof(EvaluationProcessChannel), IsDefaultView = true)]
  public sealed partial class EvaluationProcessChannelView : NamedItemView {
    public new EvaluationProcessChannel Content {
      get { return (EvaluationProcessChannel)base.Content; }
      set { base.Content = value; }
    }

    public EvaluationProcessChannelView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.ExecutableChanged -= new EventHandler(Content_ExecutableChanged);
      Content.ArgumentsChanged -= new EventHandler(Content_ArgumentsChanged);
      Content.ProcessStarted -= new EventHandler(Content_ProcessStarted);
      Content.ProcessExited -= new EventHandler(Content_ProcessExited);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ExecutableChanged += new EventHandler(Content_ExecutableChanged);
      Content.ArgumentsChanged += new EventHandler(Content_ArgumentsChanged);
      Content.ProcessStarted += new EventHandler(Content_ProcessStarted);
      Content.ProcessExited += new EventHandler(Content_ProcessExited);
    }

    #region Event Handlers (Content)
    private void Content_ExecutableChanged(object sender, EventArgs e) {
      executableTextBox.Text = Content.Executable;
    }
    private void Content_ArgumentsChanged(object sender, EventArgs e) {
      argumentsTextBox.Text = Content.Arguments;
    }
    private void Content_ProcessStarted(object sender, EventArgs e) {
      if (InvokeRequired) Invoke(new Action<object, EventArgs>(Content_ProcessStarted), sender, e);
      else SetEnabledStateOfControls();
    }
    private void Content_ProcessExited(object sender, EventArgs e) {
      if (InvokeRequired) Invoke(new Action<object, EventArgs>(Content_ProcessExited), sender, e);
      else SetEnabledStateOfControls();
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        executableTextBox.Text = String.Empty;
        argumentsTextBox.Text = String.Empty;
      } else {
        executableTextBox.Text = Content.Executable;
        argumentsTextBox.Text = Content.Arguments;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      bool readOnlyDriverNullOrStarted = ReadOnly || Content == null || Content.IsInitialized;
      browseExecutableButton.Enabled = !readOnlyDriverNullOrStarted;
      startButton.Enabled = !readOnlyDriverNullOrStarted;
      terminateButton.Enabled = !ReadOnly && Content != null && Content.IsInitialized;
      executableTextBox.Enabled = !readOnlyDriverNullOrStarted;
      argumentsTextBox.Enabled = !readOnlyDriverNullOrStarted;
    }

    #region Event Handlers (child controls)
    private void browseExecutableButton_Click(object sender, EventArgs e) {
      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        try {
          Content.Executable = openFileDialog.FileName;
        } catch (InvalidOperationException ex) {
          MessageBox.Show(ex.Message);
        }
      }
    }
    private void argumentsTextBox_Validated(object sender, EventArgs e) {
      if (Content != null) {
        Content.Arguments = argumentsTextBox.Text;
      }
    }
    #endregion

    private void startButton_Click(object sender, EventArgs e) {
      try {
        Content.Open();
      } catch (InvalidOperationException ex) {
        MessageBox.Show(ex.Message);
      }
    }

    private void terminateButton_Click(object sender, EventArgs e) {
      try {
        Content.Close();
      } catch (InvalidOperationException ex) {
        MessageBox.Show(ex.Message);
      }
    }
  }
}
