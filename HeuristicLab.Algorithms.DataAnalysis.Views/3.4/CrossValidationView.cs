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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("CrossValidation View")]
  [Content(typeof(CrossValidation), true)]
  public sealed partial class CrossValidationView : NamedItemView {
    private TypeSelectorDialog algorithmTypeSelectorDialog;
    private TypeSelectorDialog problemTypeSelectorDialog;

    public CrossValidationView() {
      InitializeComponent();
    }

    public new CrossValidation Content {
      get { return (CrossValidation)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        workersNumericUpDown.Value = 1;
        foldsNumericUpDown.Value = 2;
        samplesStartStringConvertibleValueView.Content = null;
        samplesEndStringConvertibleValueView.Content = null;
        algorithmNamedItemView.Content = null;
        algorithmProblemViewHost.Content = null;
        algorithmParameterCollectionView.Content = null;
        resultCollectionView.Content = null;
        runCollectionView.Content = null;
        storeAlgorithmInEachRunCheckBox.Checked = true;
      } else {
        Locked = ReadOnly = Content.ExecutionState == ExecutionState.Started;
        workersNumericUpDown.Value = Content.NumberOfWorkers.Value;
        foldsNumericUpDown.Value = Content.Folds.Value;
        samplesStartStringConvertibleValueView.Content = Content.SamplesStart;
        samplesEndStringConvertibleValueView.Content = Content.SamplesEnd;
        UpdateAlgorithmView();
        UpdateProblemView();
        runCollectionView.Content = Content.Runs;
        algorithmParameterCollectionView.Content = ((IParameterizedNamedItem)Content).Parameters;
        resultCollectionView.Content = Content.Results;
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
        storeAlgorithmInEachRunCheckBox.Checked = Content.StoreAlgorithmInEachRun;
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.AlgorithmChanged += new EventHandler(Content_AlgorithmChanged);
      Content.ProblemChanged += new EventHandler(Content_ProblemChanged);
      Content.Folds.ValueChanged += new EventHandler(Content_Folds_ValueChanged);
      Content.NumberOfWorkers.ValueChanged += new EventHandler(Content_NumberOfWorker_ValueChanged);

      Content.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionStateChanged += new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
      Content.StoreAlgorithmInEachRunChanged += new EventHandler(Content_StoreAlgorithmInEachRunChanged);
    }

    protected override void DeregisterContentEvents() {
      Content.AlgorithmChanged -= new EventHandler(Content_AlgorithmChanged);
      Content.ProblemChanged -= new EventHandler(Content_ProblemChanged);
      Content.Folds.ValueChanged -= new EventHandler(Content_Folds_ValueChanged);
      Content.NumberOfWorkers.ValueChanged -= new EventHandler(Content_NumberOfWorker_ValueChanged);

      Content.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      Content.ExecutionStateChanged -= new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged -= new EventHandler(Content_ExecutionTimeChanged);
      Content.StoreAlgorithmInEachRunChanged -= new EventHandler(Content_StoreAlgorithmInEachRunChanged);
      base.DeregisterContentEvents();
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

    protected override void SetEnabledStateOfControls() {
      if (InvokeRequired) Invoke((Action)SetEnabledStateOfControls);
      else {
        base.SetEnabledStateOfControls();
        this.Enabled = Content != null;

        if (Content != null) {
          storeAlgorithmInEachRunCheckBox.Enabled = !ReadOnly;
          openAlgorithmButton.Enabled = !ReadOnly;
          newAlgorithmButton.Enabled = !ReadOnly;

          algorithmNamedItemView.Enabled = Content.Algorithm != null && (Content.ExecutionState == ExecutionState.Prepared || Content.ExecutionState == ExecutionState.Stopped);
          algorithmTabControl.Enabled = Content.Algorithm != null && (Content.ExecutionState == ExecutionState.Prepared || Content.ExecutionState == ExecutionState.Stopped);
          foldsNumericUpDown.Enabled = Content.ExecutionState == ExecutionState.Prepared;
          samplesStartStringConvertibleValueView.Enabled = Content.ExecutionState == ExecutionState.Prepared;
          samplesEndStringConvertibleValueView.Enabled = Content.ExecutionState == ExecutionState.Prepared;
          workersNumericUpDown.Enabled = (Content.ExecutionState == ExecutionState.Prepared) || (Content.ExecutionState == ExecutionState.Paused);

          startButton.Enabled = (Content.ExecutionState == ExecutionState.Prepared) || (Content.ExecutionState == ExecutionState.Paused);
          pauseButton.Enabled = Content.ExecutionState == ExecutionState.Started;
          stopButton.Enabled = (Content.ExecutionState == ExecutionState.Started) || (Content.ExecutionState == ExecutionState.Paused);
          resetButton.Enabled = Content.ExecutionState != ExecutionState.Started;
        }
      }
    }

    #region Content Events
    private void Content_AlgorithmChanged(object sender, EventArgs e) {
      UpdateAlgorithmView();
      UpdateProblemView();
      SetEnabledStateOfControls();
    }
    private void UpdateAlgorithmView() {
      algorithmNamedItemView.Content = Content.Algorithm;
      UpdateProblemView();
    }

    private void Content_ProblemChanged(object sender, EventArgs e) {
      UpdateProblemView();
      SetEnabledStateOfControls();
    }
    private void UpdateProblemView() {
      algorithmProblemViewHost.Content = Content.Problem;
    }

    private void Content_Folds_ValueChanged(object sender, EventArgs e) {
      foldsNumericUpDown.Value = Content.Folds.Value;
    }
    private void Content_NumberOfWorker_ValueChanged(object sender, EventArgs e) {
      workersNumericUpDown.Value = Content.NumberOfWorkers.Value;
    }

    private void Content_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionStateChanged), sender, e);
      else {
        Locked = ReadOnly = Content.ExecutionState == ExecutionState.Started;
        SetEnabledStateOfControls();
      }
    }

    private void Content_ExecutionTimeChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionTimeChanged), sender, e);
      else
        executionTimeTextBox.Text = Content == null ? "-" : Content.ExecutionTime.ToString();
    }
    private void Content_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred), sender, e);
      else
        ErrorHandling.ShowErrorDialog(this, e.Value);
    }
    private void Content_StoreAlgorithmInEachRunChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_StoreAlgorithmInEachRunChanged), sender, e);
      else
        storeAlgorithmInEachRunCheckBox.Checked = Content.StoreAlgorithmInEachRun;
    }
    #endregion

    #region GUI events
    private void foldsNumericUpDown_Validated(object sender, EventArgs e) {
      if (foldsNumericUpDown.Text == string.Empty)
        foldsNumericUpDown.Text = foldsNumericUpDown.Value.ToString();
    }
    private void foldsNumericUpDown_ValueChanged(object sender, EventArgs e) {
      if (Content != null)
        Content.Folds.Value = (int)foldsNumericUpDown.Value;
    }

    private void workersNumericUpDown_Validated(object sender, EventArgs e) {
      if (workersNumericUpDown.Text == string.Empty)
        workersNumericUpDown.Text = workersNumericUpDown.Value.ToString();
    }
    private void workersNumericUpDown_ValueChanged(object sender, EventArgs e) {
      if (Content != null)
        Content.NumberOfWorkers.Value = (int)workersNumericUpDown.Value;
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

    private void newAlgorithmButton_Click(object sender, EventArgs e) {
      if (algorithmTypeSelectorDialog == null) {
        algorithmTypeSelectorDialog = new TypeSelectorDialog();
        algorithmTypeSelectorDialog.Caption = "Select Algorithm";
        algorithmTypeSelectorDialog.TypeSelector.Caption = "Available Algorithms";
        algorithmTypeSelectorDialog.TypeSelector.Configure(typeof(IAlgorithm), false, true);
      }
      if (algorithmTypeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.Algorithm = (IAlgorithm)algorithmTypeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    private void openAlgorithmButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Algorithm";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        algorithmTabControl.Enabled = false;

        ContentManager.LoadAsync(openFileDialog.FileName, delegate(IStorableContent content, Exception error) {
          try {
            if (error != null) throw error;
            IAlgorithm algorithm = content as IAlgorithm;
            if (algorithm == null || !(algorithm.Problem is IDataAnalysisProblem))
              MessageBox.Show(this, "The selected file does not contain an algorithm or the problem of the algorithm is not a DataAnalysisProblem.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
              Content.Algorithm = algorithm;
          }
          catch (Exception ex) {
            ErrorHandling.ShowErrorDialog(this, ex);
          }
          finally {
            Invoke(new Action(delegate() {
              algorithmTabControl.Enabled = true;
            }));
          }
        });
      }
    }

    private void newProblemButton_Click(object sender, EventArgs e) {
      if (problemTypeSelectorDialog == null) {
        problemTypeSelectorDialog = new TypeSelectorDialog();
        problemTypeSelectorDialog.Caption = "Select Problem";
        problemTypeSelectorDialog.TypeSelector.Caption = "Available Problems";
      }
      problemTypeSelectorDialog.TypeSelector.Configure(new List<Type>() { Content.ProblemType, Content.Algorithm.ProblemType }, false, true, true);
      if (problemTypeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        Content.Problem = (IDataAnalysisProblem)problemTypeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
      }
    }

    private void openProblemButton_Click(object sender, EventArgs e) {
      openFileDialog.Title = "Open Problem";
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        newProblemButton.Enabled = openProblemButton.Enabled = false;
        algorithmProblemViewHost.Enabled = false;

        ContentManager.LoadAsync(openFileDialog.FileName, delegate(IStorableContent content, Exception error) {
          try {
            if (error != null) throw error;
            IDataAnalysisProblem problem = content as IDataAnalysisProblem;
            if (problem == null && (Content.Algorithm.ProblemType.IsAssignableFrom(content.GetType())))
              Invoke(new Action(() =>
                MessageBox.Show(this, "The selected file does not contain a DataAnalysisProblem problem.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            else
              Content.Problem = problem;
          }
          catch (Exception ex) {
            Invoke(new Action(() => ErrorHandling.ShowErrorDialog(this, ex)));
          }
          finally {
            Invoke(new Action(delegate() {
              algorithmProblemViewHost.Enabled = true;
              newProblemButton.Enabled = openProblemButton.Enabled = true;
            }));
          }
        });
      }
    }

    private void algorithmTabPage_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      IAlgorithm algorithm = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IAlgorithm;
      if (!ReadOnly && algorithm != null &&
        (algorithm.ProblemType != null || Content.ProblemType.IsAssignableFrom(algorithm.Problem.GetType()))) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }
    private void algorithmTabPage_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IAlgorithm algorithm = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IAlgorithm;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) algorithm = (IAlgorithm)algorithm.Clone();
        Content.Algorithm = algorithm;
      }
    }

    private void algorithmProblemTabPage_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (ReadOnly) return;
      IProblem problem = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IProblem;
      if (problem != null && Content.ProblemType.IsAssignableFrom(problem.GetType()) &&
        Content.Algorithm.ProblemType.IsAssignableFrom(problem.GetType())) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) e.Effect = DragDropEffects.Copy;
        else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) e.Effect = DragDropEffects.Move;
        else if ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) e.Effect = DragDropEffects.Link;
      }
    }
    private void algorithmProblemTabPage_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IDataAnalysisProblem problem = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IDataAnalysisProblem;
        if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy) problem = (IDataAnalysisProblem)problem.Clone();
        Content.Problem = problem;
      }
    }

    private void storeAlgorithmInEachRunCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content != null) Content.StoreAlgorithmInEachRun = storeAlgorithmInEachRunCheckBox.Checked;
    }
    #endregion

  }
}
