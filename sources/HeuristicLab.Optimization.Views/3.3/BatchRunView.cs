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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("BatchRun View")]
  [Content(typeof(BatchRun), true)]
  public sealed partial class BatchRunView : IOptimizerView {
    private TypeSelectorDialog optimizerTypeSelectorDialog;

    public new BatchRun Content {
      get { return (BatchRun)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public BatchRunView() {
      InitializeComponent();
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (optimizerTypeSelectorDialog != null) optimizerTypeSelectorDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void DeregisterContentEvents() {
      Content.OptimizerChanged -= new EventHandler(Content_OptimizerChanged);
      Content.RepetitionsChanged -= new EventHandler(Content_RepetitionsChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.OptimizerChanged += new EventHandler(Content_OptimizerChanged);
      Content.RepetitionsChanged += new EventHandler(Content_RepetitionsChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        repetitionsNumericUpDown.Value = 1;
        optimizerViewHost.Content = null;
        runsView.Content = null;
      } else {
        repetitionsNumericUpDown.Value = Content.Repetitions;
        optimizerViewHost.Content = Content.Optimizer;
        runsView.Content = Content.Runs;
      }
    }
    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      repetitionsNumericUpDown.Enabled = Content != null && !ReadOnly;
      newOptimizerButton.Enabled = Content != null && !ReadOnly;
      openOptimizerButton.Enabled = Content != null && !ReadOnly;
      optimizerViewHost.Enabled = Content != null;
      runsView.Enabled = Content != null;
    }

    protected override void OnClosed(FormClosedEventArgs e) {
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

    #region Content Events
    private void Content_OptimizerChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_OptimizerChanged), sender, e);
      else {
        optimizerViewHost.Content = Content.Optimizer;
      }
    }
    private void Content_RepetitionsChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_RepetitionsChanged), sender, e);
      else
        repetitionsNumericUpDown.Value = Content.Repetitions;
    }
    #endregion

    #region Control events
    private void repetitionsNumericUpDown_Validated(object sender, System.EventArgs e) {
      if (repetitionsNumericUpDown.Text == string.Empty)
        repetitionsNumericUpDown.Text = repetitionsNumericUpDown.Value.ToString();
    }
    private void repetitionsNumericUpDown_ValueChanged(object sender, EventArgs e) {
      if (Content != null)
        Content.Repetitions = (int)repetitionsNumericUpDown.Value;
    }
    private void newOptimizerButton_Click(object sender, EventArgs e) {
      if (optimizerTypeSelectorDialog == null) {
        optimizerTypeSelectorDialog = new TypeSelectorDialog();
        optimizerTypeSelectorDialog.Caption = "Select Optimizer";
        optimizerTypeSelectorDialog.TypeSelector.Caption = "Available Optimizers";
        optimizerTypeSelectorDialog.TypeSelector.Configure(typeof(IOptimizer), false, true);
      }
      if (optimizerTypeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.Optimizer = (IOptimizer)optimizerTypeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }
    private void openOptimizerButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Optimizer";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        newOptimizerButton.Enabled = openOptimizerButton.Enabled = false;
        optimizerViewHost.Enabled = false;

        ContentManager.LoadAsync(openFileDialog.FileName, delegate(IStorableContent content, Exception error) {
          try {
            if (error != null) throw error;
            IOptimizer optimizer = content as IOptimizer;
            if (optimizer == null)
              MessageBox.Show(this, "The selected file does not contain an optimizer.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
              Content.Optimizer = optimizer;
          }
          catch (Exception ex) {
            ErrorHandling.ShowErrorDialog(this, ex);
          }
          finally {
            Invoke(new Action(delegate() {
              optimizerViewHost.Enabled = true;
              newOptimizerButton.Enabled = openOptimizerButton.Enabled = true;
            }));
          }
        });
      }
    }

    private void optimizerTabPage_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (!ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IOptimizer)) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }
    private void optimizerTabPage_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IOptimizer optimizer = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IOptimizer;
        if (e.Effect.HasFlag(DragDropEffects.Copy)) optimizer = (IOptimizer)optimizer.Clone();
        Content.Optimizer = optimizer;
      }
    }
    #endregion

  }
}
