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
    private OperatorLibrary operatorLibrary;
    private List<IOperator> problemInitializers;
    private int selectedProblemInitializer;
    private List<IOperator> solutionGenerators;
    private int selectedSolutionGenerator;
    private List<IOperator> mutators;
    private int selectedMutator;
    private List<IOperator> evaluators;
    private int selectedEvaluator;
    private List<IOperator> recombinators;
    private int selectedRecombinator;

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
      problemInitializers = new List<IOperator>();
      selectedProblemInitializer = -1;
      solutionGenerators = new List<IOperator>();
      selectedSolutionGenerator = -1;
      mutators = new List<IOperator>();
      selectedMutator = -1;
      evaluators = new List<IOperator>();
      selectedEvaluator = -1;
      recombinators = new List<IOperator>();
      selectedRecombinator = -1;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="ESEditor"/> with the given <paramref name="es"/>.
    /// </summary>
    /// <param name="es">The evolution strategy to display.</param>
    public ESEditor(ES es)
      : this() {
      ES = es;
    }

    private int AddOperator(ComboBox comboBox, IOperator iOperator, List<IOperator> list) {
      for (int i = 0; i < comboBox.Items.Count - 1; i++) {
        if ((comboBox.Items[i] as string).CompareTo(iOperator.Name) > 0) {
          comboBox.Items.Insert(i, iOperator.Name);
          list.Insert(i, iOperator);
          return i;
        }
      }
      int index = comboBox.Items.Count - 1;
      comboBox.Items.Insert(index, iOperator.Name);
      list.Add(iOperator);
      return index;
    }
    private int SetOperator(ComboBox comboBox, IOperator iOperator, List<IOperator> list) {
      int index;
      if ((index = list.FindIndex(op => op.Name.Equals(iOperator.Name))) >= 0) {
        comboBox.Items.RemoveAt(index);
        list.RemoveAt(index);
        return AddOperator(comboBox, iOperator, list);
      } else return -1;
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
        int index;
        if (!((index = problemInitializers.FindIndex(op => op.Name.Equals(ES.ProblemInjector.Name))) >= 0)) {
          index = AddOperator(problemInitializationComboBox, ES.ProblemInjector, problemInitializers);
          if (index <= selectedProblemInitializer) selectedProblemInitializer++;
        }
        problemInitializationComboBox.SelectedIndex = index;
        if (!((index = solutionGenerators.FindIndex(op => op.Name.Equals(ES.SolutionGenerator.Name))) >= 0)) {
          index = AddOperator(solutionGenerationComboBox, ES.SolutionGenerator, solutionGenerators);
          if (index <= selectedSolutionGenerator) selectedSolutionGenerator++;
        }
        solutionGenerationComboBox.SelectedIndex = index;
        if (!((index = evaluators.FindIndex(op => op.Name.Equals(ES.Evaluator.Name))) >= 0)) {
          index = AddOperator(evaluationComboBox, ES.Evaluator, evaluators);
          if (index <= selectedEvaluator) selectedEvaluator++;
        }
        evaluationComboBox.SelectedIndex = index;
        if (!((index = mutators.FindIndex(op => op.Name.Equals(ES.Mutator.Name))) >= 0)) {
          index = AddOperator(mutationComboBox, ES.Mutator, mutators);
          if (index <= selectedMutator) selectedMutator++;
        }
        mutationComboBox.SelectedIndex = index;
        if (!((index = recombinators.FindIndex(op => op.Name.Equals(ES.Recombinator.Name))) >= 0)) {
          index = AddOperator(recombinationComboBox, ES.Recombinator, recombinators);
          if (index <= selectedRecombinator) selectedRecombinator++;
        }
        recombinationComboBox.SelectedIndex = index;
        plusRadioButton.Checked = ES.PlusNotation;
        commaRadioButton.Checked = !ES.PlusNotation;
      }
    }

    private void SetDataBinding() {
      setRandomSeedRandomlyCheckBox.DataBindings.Add("Checked", ES, "SetSeedRandomly");
      randomSeedTextBox.DataBindings.Add("Text", ES, "Seed");
      muTextBox.DataBindings.Add("Text", ES, "Mu");
      rhoTextBox.DataBindings.Add("Text", ES, "Rho");
      lambdaTextBox.DataBindings.Add("Text", ES, "Lambda");
      maximumGenerationsTextBox.DataBindings.Add("Text", ES, "MaximumGenerations");
      shakingFactorsLowerBoundTextBox.DataBindings.Add("Text", ES, "ShakingFactorsMin");
      shakingFactorsUpperBoundTextBox.DataBindings.Add("Text", ES, "ShakingFactorsMax");
      problemDimensionTextBox.DataBindings.Add("Text", ES, "ProblemDimension");
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

    private void problemDimensionTextBox_Validated(object sender, EventArgs e) {
      UpdateLearningRates();
      Refresh();
    }

    private void UpdateLearningRates() {
      if (ES != null) {
        int dimension = int.Parse(problemDimensionTextBox.Text);
        ES.GeneralLearningRate = 1 / Math.Sqrt(2 * dimension);
        ES.LearningRate = 1 / Math.Sqrt(2 * Math.Sqrt(dimension));
      }
    }

    private void openOperatorLibraryButton_Click(object sender, EventArgs e) {
      if (openOperatorLibraryFileDialog.ShowDialog() == DialogResult.OK) {

        operatorLibrary = (PersistenceManager.Load(openOperatorLibraryFileDialog.FileName) as OperatorLibrary);
        if (operatorLibrary == null) {
          MessageBox.Show("The selected file is not an operator library");
          return;
        }
        foreach (IOperatorGroup topLevelGroup in operatorLibrary.Group.SubGroups) {
          if (topLevelGroup.Name.Equals("Problem")) {
            foreach (IOperatorGroup group in topLevelGroup.SubGroups) {
              if (group.Name.Equals("Initialization")) {
                foreach (IOperator op in group.Operators) {
                  int index = SetOperator(problemInitializationComboBox, (IOperator)op.Clone(), problemInitializers);
                  if (index < 0) {
                    index = AddOperator(problemInitializationComboBox, (IOperator)op.Clone(), problemInitializers);
                    if (index <= selectedProblemInitializer) selectedProblemInitializer++;
                  }
                }
              } else if (group.Name.Equals("Solution generation")) {
                foreach (IOperator op in group.Operators) {
                  int index = SetOperator(solutionGenerationComboBox, (IOperator)op.Clone(), solutionGenerators);
                  if (index < 0) {
                    index = AddOperator(solutionGenerationComboBox, (IOperator)op.Clone(), solutionGenerators);
                    if (index <= selectedSolutionGenerator) selectedSolutionGenerator++;
                  }
                }
              } else if (group.Name.Equals("Evaluation")) {
                foreach (IOperator op in group.Operators) {
                  int index = SetOperator(evaluationComboBox, (IOperator)op.Clone(), evaluators);
                  if (index < 0) {
                    index = AddOperator(evaluationComboBox, (IOperator)op.Clone(), evaluators);
                    if (index <= selectedEvaluator) selectedEvaluator++;
                  }
                }
              }
            }
          } else if (topLevelGroup.Name.Equals("ES")) {
            foreach (IOperatorGroup group in topLevelGroup.SubGroups) {
              if (group.Name.Equals("Mutation")) {
                foreach (IOperator op in group.Operators) {
                  int index = SetOperator(mutationComboBox, (IOperator)op.Clone(), mutators);
                  if (index < 0) {
                    index = AddOperator(mutationComboBox, (IOperator)op.Clone(), mutators);
                    if (index <= selectedMutator) selectedMutator++;
                  }
                }
              } else if (group.Name.Equals("Recombination")) {
                foreach (IOperator op in group.Operators) {
                  int index = SetOperator(recombinationComboBox, (IOperator)op.Clone(), recombinators);
                  if (index < 0) {
                    index = AddOperator(recombinationComboBox, (IOperator)op.Clone(), recombinators);
                    if (index <= selectedRecombinator) selectedRecombinator++;
                  }
                }
              }
            }
          }
        }
        problemInitializationComboBox.SelectedIndexChanged -= new EventHandler(problemInitializationComboBox_SelectedIndexChanged);
        problemInitializationComboBox.SelectedIndex = selectedProblemInitializer;
        problemInitializationComboBox.SelectedIndexChanged += new EventHandler(problemInitializationComboBox_SelectedIndexChanged);
        solutionGenerationComboBox.SelectedIndexChanged -= new EventHandler(solutionGenerationComboBox_SelectedIndexChanged);
        solutionGenerationComboBox.SelectedIndex = selectedSolutionGenerator;
        solutionGenerationComboBox.SelectedIndexChanged += new EventHandler(solutionGenerationComboBox_SelectedIndexChanged);
        evaluationComboBox.SelectedIndexChanged -= new EventHandler(evaluationComboBox_SelectedIndexChanged);
        evaluationComboBox.SelectedIndex = selectedEvaluator;
        evaluationComboBox.SelectedIndexChanged += new EventHandler(evaluationComboBox_SelectedIndexChanged);
        mutationComboBox.SelectedIndexChanged -= new EventHandler(mutationComboBox_SelectedIndexChanged);
        mutationComboBox.SelectedIndex = selectedMutator;
        mutationComboBox.SelectedIndexChanged += new EventHandler(mutationComboBox_SelectedIndexChanged);
        recombinationComboBox.SelectedIndexChanged -= new EventHandler(recombinationComboBox_SelectedIndexChanged);
        recombinationComboBox.SelectedIndex = selectedRecombinator;
        recombinationComboBox.SelectedIndexChanged += new EventHandler(recombinationComboBox_SelectedIndexChanged);
      }
    }

    private void problemInitializationComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      int index = problemInitializationComboBox.SelectedIndex;
      if (index != selectedProblemInitializer) {
        if (index == problemInitializationComboBox.Items.Count - 1) {
          if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
          if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
            selectedProblemInitializer = -1;
            index = SetOperator(problemInitializationComboBox, chooseOperatorDialog.Operator, problemInitializers);
            if (index < 0) index = AddOperator(problemInitializationComboBox, chooseOperatorDialog.Operator, problemInitializers);
          } else {
            problemInitializationComboBox.SelectedIndex = selectedProblemInitializer;
            return;
          }
        }
        if (index >= 0) {
          ES.ProblemInjector = problemInitializers[index];
          selectedProblemInitializer = index;
          problemInitializationComboBox.SelectedIndex = index;
        }
      }
    }

    private void solutionGenerationComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      int index = solutionGenerationComboBox.SelectedIndex;
      if (index != selectedSolutionGenerator) {
        if (index == solutionGenerationComboBox.Items.Count - 1) {
          if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
          if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
            selectedSolutionGenerator = -1;
            index = SetOperator(solutionGenerationComboBox, chooseOperatorDialog.Operator, solutionGenerators);
            if (index < 0) AddOperator(solutionGenerationComboBox, chooseOperatorDialog.Operator, solutionGenerators);
          } else {
            solutionGenerationComboBox.SelectedIndex = selectedSolutionGenerator;
            return;
          }
        }
        if (index >= 0) {
          ES.SolutionGenerator = solutionGenerators[index];
          selectedSolutionGenerator = index;
          solutionGenerationComboBox.SelectedIndex = selectedSolutionGenerator;
        }
      }
    }

    private void evaluationComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      int index = evaluationComboBox.SelectedIndex;
      if (index != selectedEvaluator) {
        if (index == evaluationComboBox.Items.Count - 1) {
          if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
          if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
            selectedEvaluator = -1;
            index = SetOperator(evaluationComboBox, chooseOperatorDialog.Operator, evaluators);
            if (index < 0) index = AddOperator(evaluationComboBox, chooseOperatorDialog.Operator, evaluators);
          } else {
            evaluationComboBox.SelectedIndex = selectedEvaluator;
            return;
          }
        }
        if (index >= 0) {
          ES.Evaluator = evaluators[index];
          selectedEvaluator = index;
          evaluationComboBox.SelectedIndex = index;
        }
      }
    }

    private void mutationComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      int index = mutationComboBox.SelectedIndex;
      if (index != selectedMutator) {
        if (index == mutationComboBox.Items.Count - 1) {
          if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
          if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
            selectedMutator = -1;
            index = SetOperator(mutationComboBox, chooseOperatorDialog.Operator, mutators);
            if (index < 0) index = AddOperator(mutationComboBox, chooseOperatorDialog.Operator, mutators);
          } else {
            mutationComboBox.SelectedIndex = selectedMutator;
            return;
          }
        }
        if (index >= 0) {
          ES.Mutator = mutators[index];
          selectedMutator = index;
          mutationComboBox.SelectedIndex = index;
        }
      }
    }

    private void recombinationComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      int index = recombinationComboBox.SelectedIndex;
      if (index != selectedRecombinator) {
        if (index == recombinationComboBox.Items.Count - 1) {
          if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
          if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
            selectedRecombinator = -1;
            index = SetOperator(recombinationComboBox, chooseOperatorDialog.Operator, recombinators);
            if (index < 0) index = AddOperator(recombinationComboBox, chooseOperatorDialog.Operator, recombinators);
          } else {
            recombinationComboBox.SelectedIndex = selectedRecombinator;
            return;
          }
        }
        if (index >= 0) {
          ES.Recombinator = recombinators[index];
          selectedRecombinator = index;
          recombinationComboBox.SelectedIndex = index;
        }
      }
    }
  }
}
