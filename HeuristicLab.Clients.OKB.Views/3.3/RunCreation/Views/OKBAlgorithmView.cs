#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [View("OKBAlgorithm View")]
  [Content(typeof(OKBAlgorithm), true)]
  public sealed partial class OKBAlgorithmView : NamedItemView {
    private TypeSelectorDialog problemTypeSelectorDialog;

    public new OKBAlgorithm Content {
      get { return (OKBAlgorithm)base.Content; }
      set { base.Content = value; }
    }

    public OKBAlgorithmView() {
      InitializeComponent();
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (problemTypeSelectorDialog != null) problemTypeSelectorDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void OnInitialized(EventArgs e) {
      // Set order of tab pages according to z order.
      // NOTE: This is required due to a bug in the VS designer.
      List<Control> tabPages = new List<Control>();
      for (int i = 0; i < tabControl.Controls.Count; i++) {
        tabPages.Add(tabControl.Controls[i]);
      }
      tabControl.Controls.Clear();
      foreach (Control control in tabPages)
        tabControl.Controls.Add(control);

      base.OnInitialized(e);
      //TODO: deregistration of eventhandlers is missing?
      RunCreationClient.Instance.Refreshing += new EventHandler(RunCreationClient_Refreshing);
      RunCreationClient.Instance.Refreshed += new EventHandler(RunCreationClient_Refreshed);
      PopulateComboBox();
    }

    protected override void DeregisterContentEvents() {
      Content.AlgorithmChanged -= new EventHandler(Content_AlgorithmChanged);
      Content.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionStateChanged -= new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged -= new EventHandler(Content_ExecutionTimeChanged);
      Content.Prepared -= new EventHandler(Content_Prepared);
      Content.Started -= new EventHandler(Content_Started);
      Content.Paused -= new EventHandler(Content_Paused);
      Content.Stopped -= new EventHandler(Content_Stopped);
      Content.ProblemChanged -= new EventHandler(Content_ProblemChanged);
      Content.StoreRunsAutomaticallyChanged -= new EventHandler(Content_StoreRunsAutomaticallyChanged);
      Content.StoreAlgorithmInEachRunChanged -= new EventHandler(Content_StoreAlgorithmInEachRunChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.AlgorithmChanged += new EventHandler(Content_AlgorithmChanged);
      Content.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionStateChanged += new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
      Content.Prepared += new EventHandler(Content_Prepared);
      Content.Started += new EventHandler(Content_Started);
      Content.Paused += new EventHandler(Content_Paused);
      Content.Stopped += new EventHandler(Content_Stopped);
      Content.ProblemChanged += new EventHandler(Content_ProblemChanged);
      Content.StoreRunsAutomaticallyChanged += new EventHandler(Content_StoreRunsAutomaticallyChanged);
      Content.StoreAlgorithmInEachRunChanged += new EventHandler(Content_StoreAlgorithmInEachRunChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        algorithmComboBox.SelectedIndex = -1;
        parameterCollectionView.Content = null;
        problemViewHost.Content = null;
        resultsView.Content = null;
        runsView.Content = null;
        storeRunsAutomaticallyCheckBox.Checked = true;
        storeAlgorithmInEachRunCheckBox.Checked = true;
        executionTimeTextBox.Text = "-";
      } else {
        algorithmComboBox.SelectedItem = RunCreationClient.Instance.Algorithms.FirstOrDefault(x => x.Id == Content.AlgorithmId);
        Locked = ReadOnly = Content.ExecutionState == ExecutionState.Started;
        parameterCollectionView.Content = Content.Parameters;
        problemViewHost.ViewType = null;
        problemViewHost.Content = Content.Problem;
        resultsView.Content = Content.Results.AsReadOnly();
        runsView.Content = Content.Runs;
        storeRunsAutomaticallyCheckBox.Checked = Content.StoreRunsAutomatically;
        storeAlgorithmInEachRunCheckBox.Checked = Content.StoreAlgorithmInEachRun;
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      algorithmComboBox.Enabled = (Content != null) && !ReadOnly && !Locked && (algorithmComboBox.Items.Count > 0);
      cloneAlgorithmButton.Enabled = (Content != null) && (Content.AlgorithmId != -1) && !ReadOnly && !Locked;
      refreshButton.Enabled = (Content != null) && !ReadOnly && !Locked;
      parameterCollectionView.Enabled = Content != null;
      newProblemButton.Enabled = Content != null && !ReadOnly;
      openProblemButton.Enabled = Content != null && !ReadOnly;
      problemViewHost.Enabled = Content != null;
      resultsView.Enabled = Content != null;
      runsView.Enabled = Content != null;
      storeRunsAutomaticallyCheckBox.Enabled = Content != null && !ReadOnly;
      storeAlgorithmInEachRunCheckBox.Enabled = Content != null && !ReadOnly;
      executionTimeTextBox.Enabled = Content != null;
      SetEnabledStateOfExecutableButtons();
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      RunCreationClient.Instance.Refreshing -= new EventHandler(RunCreationClient_Refreshing);
      RunCreationClient.Instance.Refreshed -= new EventHandler(RunCreationClient_Refreshed);
      if ((Content != null) && (Content.ExecutionState == ExecutionState.Started)) {
        //The content must be stopped if no other view showing the content is available
        var optimizers = MainFormManager.MainForm.Views.OfType<IContentView>().Where(v => v != this).Select(v => v.Content).OfType<IOptimizer>();
        if (!optimizers.Contains(Content)) {
          var nestedOptimizers = optimizers.SelectMany(opt => opt.NestedOptimizers);
          if (!nestedOptimizers.Contains(Content)) Content.Stop();
        }
      }
      base.OnClosed(e);
    }

    private void RunCreationClient_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(RunCreationClient_Refreshing), sender, e);
      } else {
        Cursor = Cursors.AppStarting;
        Enabled = false;
      }
    }
    private void RunCreationClient_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(RunCreationClient_Refreshed), sender, e);
      } else {
        PopulateComboBox();
        Enabled = true;
        SetEnabledStateOfControls();
        Cursor = Cursors.Default;
      }
    }

    #region Content Events
    private void Content_AlgorithmChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_AlgorithmChanged), sender, e);
      else
        OnContentChanged();
    }
    private void Content_ProblemChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ProblemChanged), sender, e);
      else {
        problemViewHost.ViewType = null;
        problemViewHost.Content = Content.Problem;
      }
    }
    private void Content_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionStateChanged), sender, e);
      else
        startButton.Enabled = pauseButton.Enabled = stopButton.Enabled = resetButton.Enabled = false;
    }
    private void Content_ExecutionTimeChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionTimeChanged), sender, e);
      else
        executionTimeTextBox.Text = Content == null ? "-" : Content.ExecutionTime.ToString();
    }
    private void Content_StoreRunsAutomaticallyChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_StoreRunsAutomaticallyChanged), sender, e);
      else
        storeRunsAutomaticallyCheckBox.Checked = Content.StoreRunsAutomatically;
    }
    private void Content_StoreAlgorithmInEachRunChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_StoreAlgorithmInEachRunChanged), sender, e);
      else
        storeAlgorithmInEachRunCheckBox.Checked = Content.StoreAlgorithmInEachRun;
    }
    private void Content_Prepared(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Prepared), sender, e);
      else {
        resultsView.Content = Content.Results.AsReadOnly();
        ReadOnly = Locked = false;
        SetEnabledStateOfExecutableButtons();
      }
    }
    private void Content_Started(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Started), sender, e);
      else {
        ReadOnly = Locked = true;
        SetEnabledStateOfExecutableButtons();
      }
    }
    private void Content_Paused(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Paused), sender, e);
      else {
        ReadOnly = Locked = false;
        SetEnabledStateOfExecutableButtons();
      }
    }
    private void Content_Stopped(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Stopped), sender, e);
      else {
        ReadOnly = Locked = false;
        SetEnabledStateOfExecutableButtons();
      }
    }
    private void Content_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred), sender, e);
      else
        ErrorHandling.ShowErrorDialog(this, e.Value);
    }
    #endregion

    #region Control Events
    private void cloneAlgorithmButton_Click(object sender, EventArgs e) {
      MainFormManager.MainForm.ShowContent(Content.CloneAlgorithm());
    }
    private void refreshButton_Click(object sender, System.EventArgs e) {
      RunCreationClient.Instance.Refresh();
    }
    private void algorithmComboBox_SelectedValueChanged(object sender, System.EventArgs e) {
      Algorithm algorithm = algorithmComboBox.SelectedValue as Algorithm;
      if ((algorithm != null) && (Content != null)) {
        Content.Load(algorithm.Id);
        if (Content.AlgorithmId != algorithm.Id)  // reset selected item if load was not successful
          algorithmComboBox.SelectedItem = RunCreationClient.Instance.Algorithms.FirstOrDefault(x => x.Id == Content.AlgorithmId);
      }
    }
    private void newProblemButton_Click(object sender, EventArgs e) {
      if (problemTypeSelectorDialog == null) {
        problemTypeSelectorDialog = new TypeSelectorDialog();
        problemTypeSelectorDialog.Caption = "Select Problem";
        problemTypeSelectorDialog.TypeSelector.Caption = "Available Problems";
        problemTypeSelectorDialog.TypeSelector.Configure(Content.ProblemType, false, true);
      }
      if (problemTypeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.Problem = (IProblem)problemTypeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }
    private void openProblemButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Problem";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        newProblemButton.Enabled = openProblemButton.Enabled = false;
        problemViewHost.Enabled = false;

        ContentManager.LoadAsync(openFileDialog.FileName, delegate(IStorableContent content, Exception error) {
          try {
            if (error != null) throw error;
            IProblem problem = content as IProblem;
            if (problem == null)
              Invoke(new Action(() =>
                MessageBox.Show(this, "The selected file does not contain a problem.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            else if (!Content.ProblemType.IsInstanceOfType(problem))
              Invoke(new Action(() =>
                MessageBox.Show(this, "The selected file contains a problem type which is not supported by this algorithm.", "Invalid Problem Type", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            else
              Content.Problem = problem;
          }
          catch (Exception ex) {
            Invoke(new Action(() => ErrorHandling.ShowErrorDialog(this, ex)));
          }
          finally {
            Invoke(new Action(delegate() {
              problemViewHost.Enabled = true;
              newProblemButton.Enabled = openProblemButton.Enabled = true;
            }));
          }
        });
      }
    }
    private void storeRunsAutomaticallyCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content != null) Content.StoreRunsAutomatically = storeRunsAutomaticallyCheckBox.Checked;
    }
    private void storeAlgorithmInEachRunCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content != null) Content.StoreAlgorithmInEachRun = storeAlgorithmInEachRunCheckBox.Checked;
    }
    private void startButton_Click(object sender, EventArgs e) {
      Content.Start();
    }
    private void pauseButton_Click(object sender, EventArgs e) {
      Content.Pause();
    }
    private void stopButton_Click(object sender, EventArgs e) {
      Content.Stop();
    }
    private void resetButton_Click(object sender, EventArgs e) {
      Content.Prepare(false);
    }
    private void problemTabPage_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      Type type = e.Data.GetData("Type") as Type;
      if ((type != null) && (Content.ProblemType.IsAssignableFrom(type))) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) e.Effect = DragDropEffects.Copy;
        else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) e.Effect = DragDropEffects.Move;
        else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) e.Effect = DragDropEffects.Link;
      }
    }
    private void problemTabPage_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IProblem problem = e.Data.GetData("Value") as IProblem;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) problem = (IProblem)problem.Clone();
        Content.Problem = problem;
      }
    }
    #endregion

    #region Helpers
    private void PopulateComboBox() {
      algorithmComboBox.DataSource = null;
      algorithmComboBox.DataSource = RunCreationClient.Instance.Algorithms.ToList();
      algorithmComboBox.DisplayMember = "Name";
    }
    private void SetEnabledStateOfExecutableButtons() {
      if (Content == null) {
        startButton.Enabled = pauseButton.Enabled = stopButton.Enabled = resetButton.Enabled = false;
      } else {
        startButton.Enabled = (Content.ExecutionState == ExecutionState.Prepared) || (Content.ExecutionState == ExecutionState.Paused);
        pauseButton.Enabled = Content.ExecutionState == ExecutionState.Started;
        stopButton.Enabled = (Content.ExecutionState == ExecutionState.Started) || (Content.ExecutionState == ExecutionState.Paused);
        resetButton.Enabled = Content.ExecutionState != ExecutionState.Started;
      }
    }
    #endregion
  }
}
