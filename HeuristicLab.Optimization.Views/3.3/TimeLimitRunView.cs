#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  [View("TimeLimit Run View")]
  [Content(typeof(TimeLimitRun), IsDefaultView = true)]
  public partial class TimeLimitRunView : IOptimizerView {
    protected TypeSelectorDialog algorithmTypeSelectorDialog;
    protected virtual bool SuppressEvents { get; set; }

    public new TimeLimitRun Content {
      get { return (TimeLimitRun)base.Content; }
      set { base.Content = value; }
    }

    public TimeLimitRunView() {
      InitializeComponent();
      snapshotButton.Text = String.Empty;
      snapshotButton.Image = VSImageLibrary.Camera;
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (algorithmTypeSelectorDialog != null) algorithmTypeSelectorDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void DeregisterContentEvents() {
      Content.PropertyChanged -= Content_PropertyChanged;
      Content.SnapshotTimes.ItemsAdded -= Content_SnapshotTimes_Changed;
      Content.SnapshotTimes.ItemsMoved -= Content_SnapshotTimes_Changed;
      Content.SnapshotTimes.ItemsRemoved -= Content_SnapshotTimes_Changed;
      Content.SnapshotTimes.ItemsReplaced -= Content_SnapshotTimes_Changed;
      Content.SnapshotTimes.CollectionReset -= Content_SnapshotTimes_Changed;
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += Content_PropertyChanged;
      Content.SnapshotTimes.ItemsAdded += Content_SnapshotTimes_Changed;
      Content.SnapshotTimes.ItemsMoved += Content_SnapshotTimes_Changed;
      Content.SnapshotTimes.ItemsRemoved += Content_SnapshotTimes_Changed;
      Content.SnapshotTimes.ItemsReplaced += Content_SnapshotTimes_Changed;
      Content.SnapshotTimes.CollectionReset += Content_SnapshotTimes_Changed;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      SuppressEvents = true;
      try {
        if (Content == null) {
          timeLimitTextBox.Text = TimeSpanHelper.FormatNatural(TimeSpan.FromSeconds(60));
          snapshotsTextBox.Text = String.Empty;
          storeAlgorithmInEachSnapshotCheckBox.Checked = false;
          algorithmViewHost.Content = null;
          snapshotsView.Content = null;
          runsView.Content = null;
        } else {
          timeLimitTextBox.Text = TimeSpanHelper.FormatNatural(Content.MaximumExecutionTime);
          snapshotsTextBox.Text = String.Join(" ; ", Content.SnapshotTimes.Select(x => TimeSpanHelper.FormatNatural(x, true)));
          storeAlgorithmInEachSnapshotCheckBox.Checked = Content.StoreAlgorithmInEachSnapshot;
          algorithmViewHost.Content = Content.Algorithm;
          snapshotsView.Content = Content.Snapshots;
          runsView.Content = Content.Runs;
        }
      } finally { SuppressEvents = false; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      timeLimitTextBox.Enabled = Content != null && !ReadOnly;
      snapshotsTextBox.Enabled = Content != null && !ReadOnly;
      storeAlgorithmInEachSnapshotCheckBox.Enabled = Content != null && !ReadOnly;
      newAlgorithmButton.Enabled = Content != null && !ReadOnly;
      openAlgorithmButton.Enabled = Content != null && !ReadOnly;
      algorithmViewHost.Enabled = Content != null;
      snapshotsView.Enabled = Content != null;
      runsView.Enabled = Content != null;
    }

    protected override void SetEnabledStateOfExecutableButtons() {
      base.SetEnabledStateOfExecutableButtons();
      snapshotButton.Enabled = Content != null && Content.Algorithm != null && Content.ExecutionState == ExecutionState.Paused;
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      if ((Content != null) && (Content.ExecutionState == ExecutionState.Started)) {
        //The content must be stopped if no other view showing the content is available
        var optimizers = MainFormManager.MainForm.Views.OfType<IContentView>().Where(v => v != this).Select(v => v.Content).OfType<IAlgorithm>();
        if (!optimizers.Contains(Content.Algorithm)) {
          var nestedOptimizers = optimizers.SelectMany(opt => opt.NestedOptimizers);
          if (!nestedOptimizers.Contains(Content)) Content.Stop();
        }
      }
      base.OnClosed(e);
    }

    #region Event Handlers
    #region Content events
    private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      switch (e.PropertyName) {
        case nameof(Content.MaximumExecutionTime):
          SuppressEvents = true;
          try {
            timeLimitTextBox.Text = TimeSpanHelper.FormatNatural(Content.MaximumExecutionTime);
          } finally { SuppressEvents = false; }
          break;
        case nameof(Content.SnapshotTimes):
          SuppressEvents = true;
          try {
            if (Content.SnapshotTimes.Any())
              snapshotsTextBox.Text = String.Join(" ; ", Content.SnapshotTimes.Select(x => TimeSpanHelper.FormatNatural(x, true)));
            else snapshotsTextBox.Text = String.Empty;
            Content.SnapshotTimes.ItemsAdded += Content_SnapshotTimes_Changed;
            Content.SnapshotTimes.ItemsMoved += Content_SnapshotTimes_Changed;
            Content.SnapshotTimes.ItemsRemoved += Content_SnapshotTimes_Changed;
            Content.SnapshotTimes.ItemsReplaced += Content_SnapshotTimes_Changed;
            Content.SnapshotTimes.CollectionReset += Content_SnapshotTimes_Changed;
          } finally { SuppressEvents = false; }
          break;
        case nameof(Content.StoreAlgorithmInEachSnapshot):
          SuppressEvents = true;
          try {
            storeAlgorithmInEachSnapshotCheckBox.Checked = Content.StoreAlgorithmInEachSnapshot;
          } finally { SuppressEvents = false; }
          break;
        case nameof(Content.Algorithm):
          SuppressEvents = true;
          try {
            algorithmViewHost.Content = Content.Algorithm;
          } finally { SuppressEvents = false; }
          break;
        case nameof(Content.Snapshots):
          SuppressEvents = true;
          try {
            snapshotsView.Content = Content.Snapshots;
          } finally { SuppressEvents = false; }
          break;
        case nameof(Content.Runs):
          SuppressEvents = true;
          try {
            runsView.Content = Content.Runs;
          } finally { SuppressEvents = false; }
          break;
      }
    }

    private void Content_SnapshotTimes_Changed(object sender, EventArgs e) {
      SuppressEvents = true;
      try {
        if (Content.SnapshotTimes.Any())
          snapshotsTextBox.Text = string.Join(" ; ", Content.SnapshotTimes.Select(x => TimeSpanHelper.FormatNatural(x, true)));
        else snapshotsTextBox.Text = String.Empty;
      } finally { SuppressEvents = false; }
    }
    #endregion

    #region Control events
    private void timeLimitTextBox_Validating(object sender, CancelEventArgs e) {
      if (SuppressEvents) return;
      TimeSpan ts;
      if (!TimeSpanHelper.TryGetFromNaturalFormat(timeLimitTextBox.Text, out ts)) {
        e.Cancel = !timeLimitTextBox.ReadOnly && timeLimitTextBox.Enabled;
        errorProvider.SetError(timeLimitTextBox, "Please enter a valid time span, e.g. 20 seconds ; 45s ; 4min ; 1h ; 3 hours ; 2 days ; 4d");
      } else {
        Content.MaximumExecutionTime = ts;
        e.Cancel = false;
        errorProvider.SetError(timeLimitTextBox, null);
      }
    }

    private void snapshotsTextBox_Validating(object sender, CancelEventArgs e) {
      if (SuppressEvents) return;
      e.Cancel = false;
      errorProvider.SetError(snapshotsTextBox, null);

      var snapshotTimes = new ObservableList<TimeSpan>();
      var matches = Regex.Matches(snapshotsTextBox.Text, @"(\d+[ ;,\t]*\w+)");
      foreach (Match m in matches) {
        TimeSpan value;
        if (!TimeSpanHelper.TryGetFromNaturalFormat(m.Value, out value)) {
          e.Cancel = !snapshotsTextBox.ReadOnly && snapshotsTextBox.Enabled; // don't cancel an operation that cannot be edited
          errorProvider.SetError(snapshotsTextBox, "Error parsing " + m.Value + ", please provide a valid time span, e.g. 20 seconds ; 45s ; 4min ; 1h ; 3 hours ; 2 days ; 4d");
          return;
        } else {
          snapshotTimes.Add(value);
        }
      }
      Content.SnapshotTimes = snapshotTimes;
    }

    private void storeAlgorithmInEachSnapshotCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (SuppressEvents) return;
      SuppressEvents = true;
      try {
        Content.StoreAlgorithmInEachSnapshot = storeAlgorithmInEachSnapshotCheckBox.Checked;
      } finally { SuppressEvents = false; }
    }

    private void newAlgorithmButton_Click(object sender, EventArgs e) {
      if (algorithmTypeSelectorDialog == null) {
        algorithmTypeSelectorDialog = new TypeSelectorDialog { Caption = "Select Algorithm" };
        algorithmTypeSelectorDialog.TypeSelector.Caption = "Available Algorithms";
        algorithmTypeSelectorDialog.TypeSelector.Configure(typeof(IAlgorithm), false, true);
      }
      if (algorithmTypeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.Algorithm = (IAlgorithm)algorithmTypeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    private void openAlgorithmButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Algorithm";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        newAlgorithmButton.Enabled = openAlgorithmButton.Enabled = false;
        algorithmViewHost.Enabled = false;

        ContentManager.LoadAsync(openFileDialog.FileName, delegate(IStorableContent content, Exception error) {
          try {
            if (error != null) throw error;
            var algorithm = content as IAlgorithm;
            if (algorithm == null)
              MessageBox.Show(this, "The selected file does not contain an algorithm.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
              Content.Algorithm = algorithm;
          } catch (Exception ex) {
            ErrorHandling.ShowErrorDialog(this, ex);
          } finally {
            Invoke(new Action(delegate() {
              algorithmViewHost.Enabled = true;
              newAlgorithmButton.Enabled = openAlgorithmButton.Enabled = true;
            }));
          }
        });
      }
    }

    private void algorithmTabPage_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (!ReadOnly && (e.Data.GetData(Constants.DragDropDataFormat) is IAlgorithm)) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }

    private void algorithmTabPage_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        var algorithm = e.Data.GetData(Constants.DragDropDataFormat) as IAlgorithm;
        if (e.Effect.HasFlag(DragDropEffects.Copy)) algorithm = (IAlgorithm)algorithm.Clone();
        Content.Algorithm = algorithm;
      }
    }

    private void snapshotButton_Click(object sender, EventArgs e) {
      Content.Snapshot();
    }

    private void sequenceButton_Click(object sender, EventArgs e) {
      using (var dialog = new DefineArithmeticTimeSpanProgressionDialog(TimeSpan.FromSeconds(1), Content.MaximumExecutionTime, TimeSpan.FromSeconds(1))) {
        if (dialog.ShowDialog() == DialogResult.OK) {
          if (dialog.Values.Any())
            Content.SnapshotTimes = new ObservableList<TimeSpan>(dialog.Values);
          else Content.SnapshotTimes = new ObservableList<TimeSpan>();
        }
      }
    }
    #endregion
    #endregion
  }
}
