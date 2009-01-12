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
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Core;

namespace HeuristicLab.ES {
  /// <summary>
  /// Class for visual representation of an <see cref="ES"/>.
  /// </summary>
  public partial class ESEditor : EditorBase {
    private ChooseOperatorDialog chooseOperatorDialog;

    /// <summary>
    /// Gets or sets the evolution strategy to display.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="EditorBase"/>. 
    /// No own data storage present.</remarks>
    public ES ES {
      get { return (ES)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ESEditor"/>.
    /// </summary>
    public ESEditor() {
      InitializeComponent();
      problemDimensionTextBox.Text = "1";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="ESEditor"/> with the given <paramref name="es"/>.
    /// </summary>
    /// <param name="es">The evolution strategy to display.</param>
    public ESEditor(ES es)
      : this() {
      ES = es;
      int dimension = es.ShakingFactors.Length;
      problemDimensionTextBox.Text = dimension.ToString();
    }

    /// <summary>
    /// Removes all event handlers from the underlying <see cref="ES"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="EditorBase"/>.</remarks>
    protected override void RemoveItemEvents() {
      ES.Engine.ExceptionOccurred -= new EventHandler<ExceptionEventArgs>(Engine_ExceptionOccurred);
      ES.Engine.Finished -= new EventHandler(Engine_Finished);
      ES.Changed -= new EventHandler(ES_Changed);
      scopeView.Scope = null;
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds event handlers to the underlying <see cref="ES"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="EditorBase"/>.</remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ES.Engine.ExceptionOccurred += new EventHandler<ExceptionEventArgs>(Engine_ExceptionOccurred);
      ES.Engine.Finished += new EventHandler(Engine_Finished);
      ES.Changed += new EventHandler(ES_Changed);
      SetDataBinding();
      scopeView.Scope = ES.Engine.GlobalScope;
    }

    void ES_Changed(object sender, EventArgs e) {
      // neither Refresh() nor Update() work
      randomSeedTextBox.Text = ES.Seed.ToString();
      muTextBox.Text = ES.Mu.ToString();
      rhoTextBox.Text = ES.Rho.ToString();
      lambdaTextBox.Text = ES.Lambda.ToString();
      learningRateTextBox.Text = ES.LearningRate.ToString();
      generalLearningRateTextBox.Text = ES.GeneralLearningRate.ToString();
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="EditorBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (ES == null) {
        tabControl.Enabled = false;
      } else {
        tabControl.Enabled = true;
        problemInitializationTextBox.Text = ES.ProblemInjector.GetType().Name;
        solutionGenerationTextBox.Text = ES.SolutionGenerator.GetType().Name;
        mutationTextBox.Text = ES.Mutator.GetType().Name;
        evaluationTextBox.Text = ES.Evaluator.GetType().Name;
        recombinationTextBox.Text = ES.Recombinator.GetType().Name;
        initialMutationStrengthVectorTextBox.Text = ArrayToString<double>(ES.ShakingFactors);
      }
    }

    private void SetDataBinding() {
      setRandomSeedRandomlyCheckBox.DataBindings.Add("Checked", ES, "SetSeedRandomly");
      randomSeedTextBox.DataBindings.Add("Text", ES, "Seed");
      muTextBox.DataBindings.Add("Text", ES, "Mu");
      rhoTextBox.DataBindings.Add("Text", ES, "Rho");
      lambdaTextBox.DataBindings.Add("Text", ES, "Lambda");
      maximumGenerationsTextBox.DataBindings.Add("Text", ES, "MaximumGenerations");
      generalLearningRateTextBox.DataBindings.Add("Text", ES, "GeneralLearningRate");
      learningRateTextBox.DataBindings.Add("Text", ES, "LearningRate");
    }

    #region Button Events
    private void viewProblemInitializationButton_Click(object sender, EventArgs e) {
      IView view = ES.ProblemInjector.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void viewSolutionGenerationButton_Click(object sender, EventArgs e) {
      IView view = ES.SolutionGenerator.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void viewMutationButton_Click(object sender, EventArgs e) {
      IView view = ES.Mutator.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void viewEvaluationButton_Click(object sender, EventArgs e) {
      IView view = ES.Evaluator.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void viewRecombinationButton_Click(object sender, EventArgs e) {
      IView view = ES.Recombinator.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void setProblemInitializationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        ES.ProblemInjector = chooseOperatorDialog.Operator;
        problemInitializationTextBox.Text = ES.ProblemInjector.GetType().Name;
      }
    }
    private void setSolutionGenerationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        ES.SolutionGenerator= chooseOperatorDialog.Operator;
        solutionGenerationTextBox.Text = ES.SolutionGenerator.GetType().Name;
      }
    }
    private void setMutationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        ES.Mutator = chooseOperatorDialog.Operator;
        mutationTextBox.Text = ES.Mutator.GetType().Name;
      }
    }
    private void setEvaluationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        ES.Evaluator = chooseOperatorDialog.Operator;
        evaluationTextBox.Text = ES.Evaluator.GetType().Name;
      }
    }
    private void setRecombinationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        ES.Recombinator = chooseOperatorDialog.Operator;
        recombinationTextBox.Text = ES.Recombinator.GetType().Name;
      }
    }
    private void executeButton_Click(object sender, EventArgs e) {
      executeButton.Enabled = false;
      abortButton.Enabled = true;
      ES.Engine.Execute();
    }
    private void abortButton_Click(object sender, EventArgs e) {
      ES.Engine.Abort();
    }
    private void resetButton_Click(object sender, EventArgs e) {
      ES.Engine.Reset();
    }
    private void cloneEngineButton_Click(object sender, EventArgs e) {
      IEngine clone = (IEngine)ES.Engine.Clone();
      IEditor editor = ((IEditable)clone).CreateEditor();
      PluginManager.ControlManager.ShowControl(editor);
    }
    #endregion

    #region Engine Events
    private delegate void OnExceptionEventDelegate(object sender, ExceptionEventArgs e);
    private void Engine_ExceptionOccurred(object sender, ExceptionEventArgs e) {
      if (InvokeRequired)
        Invoke(new OnExceptionEventDelegate(Engine_ExceptionOccurred), sender, e);
      else
        Auxiliary.ShowErrorMessageBox(e.Exception);
    }
    private void Engine_Finished(object sender, EventArgs e) {
      scopeView.Refresh();
      if (executeButton.InvokeRequired) {
        executeButton.Invoke(new MethodInvoker(EnableExecute));
      } else {
        executeButton.Enabled = true;
        abortButton.Enabled = false;
      }
    }
    private void EnableExecute() {
      executeButton.Enabled = true;
      abortButton.Enabled = false;
    }
    #endregion

    #region RadioButton Events
    private void plusRadioButton_CheckedChanged(object sender, EventArgs e) {
      if (plusRadioButton.Checked) ES.PlusNotation = true;
    }

    private void commaRadioButton_CheckedChanged(object sender, EventArgs e) {
      if (commaRadioButton.Checked) ES.PlusNotation = false;
    }
    #endregion

    private string ArrayToString<T>(T[] array) {
      StringBuilder s = new StringBuilder();
      foreach (T element in array)
        s.Append(element + "; ");
      s.Remove(s.Length - 2, 2);
      return s.ToString();
    }

    private double[] StringToDoubleArray(string str) {
      
      string[] s = str.Split(new char[] { ';' });
      double[] tmp = new double[s.Length];
      try {
        for (int i = 0; i < s.Length; i++) {
          tmp[i] = double.Parse(s[i]);
        }
      } catch (FormatException) {        
        return null;
      }
      return tmp;
    }

    private void initialMutationStrengthVectorTextBox_Validated(object sender, EventArgs e) {
      double[] tmp = StringToDoubleArray(initialMutationStrengthVectorTextBox.Text);
      if (tmp != null) ES.ShakingFactors = tmp;
      else MessageBox.Show("Please use colons \";\" (without the quotes) to delimite the items like this: " + (1.2).ToString() + ";" + (1.1).ToString() + ";" + (3.453).ToString());
      int dim = int.Parse(problemDimensionTextBox.Text);
      if (ES.ShakingFactors.Length != dim) {
        problemDimensionTextBox.Text = ES.ShakingFactors.Length.ToString();
        UpdateLearningRates();
      }
      Refresh();
    }

    private void problemDimensionTextBox_Validated(object sender, EventArgs e) {
      double[] tmp = StringToDoubleArray(initialMutationStrengthVectorTextBox.Text);
      if (tmp != null) {
        int dim = 0;
        try {
          dim = int.Parse(problemDimensionTextBox.Text);
          if (dim < 1) throw new FormatException();
        } catch (FormatException) {
          MessageBox.Show("Problem Dimension must contain an integer > 0");
        }
        double[] shakingFactors = new double[dim];
        for (int i = 0; i < dim; i++) {
          shakingFactors[i] = tmp[i % tmp.Length];
        }
        ES.ShakingFactors = shakingFactors;
        UpdateLearningRates();
        Refresh();
      }
    }

    private void UpdateLearningRates() {
      if (ES != null) {
        int dimension = int.Parse(problemDimensionTextBox.Text);
        ES.GeneralLearningRate = 1 / Math.Sqrt(2 * dimension);
        ES.LearningRate = 1 / Math.Sqrt(2 * Math.Sqrt(dimension));
      }
    }
  }
}
