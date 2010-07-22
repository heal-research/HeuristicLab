#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Analyzers;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Encodings.BinaryVectorEncoding;

namespace HeuristicLab.Problems.DataAnalysis.FeatureSelection {
  [Item("Feature Selection Problem", "Represents a feature selection problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public class FeatureSelectionProblem : DataAnalysisProblem, IMultiObjectiveProblem {

    #region Parameter Properties
    public ValueParameter<BoolArray> MaximizationParameter {
      get { return (ValueParameter<BoolArray>)Parameters["Maximization"]; }
    }
    IParameter IMultiObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public new ValueParameter<IBinaryVectorCreator> SolutionCreatorParameter {
      get { return (ValueParameter<IBinaryVectorCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public new ValueParameter<IFeatureSelectionEvaluator> EvaluatorParameter {
      get { return (ValueParameter<IFeatureSelectionEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    #endregion

    #region Properties
    public new IBinaryVectorCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public new IFeatureSelectionEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    IMultiObjectiveEvaluator IMultiObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    private List<IOperator> operators;
    public override IEnumerable<IOperator> Operators {
      get { return operators; }
    }
    #endregion

    public FeatureSelectionProblem()
      : base() {
      BinaryVectorCreator creator = new RandomBinaryVectorCreator();
      var evaluator = new LinearRegressionFeatureSelectionEvaluator();
      Parameters.Add(new ValueParameter<BoolArray>("Maximization", "Both objectives should be minimized.", new BoolArray(new bool[] {false, false})));
      Parameters.Add(new ValueParameter<IBinaryVectorCreator>("SolutionCreator", "The operator which should be used to create new solutions.", creator));
      Parameters.Add(new ValueParameter<IFeatureSelectionEvaluator>("Evaluator", "The evaluator that should be used for the feature selection problem.", evaluator));

      creator.BinaryVectorParameter.ActualName = "FeatureArray";
      evaluator.QualitiesParameter.ActualName = "SizeAndCVMeanSquaredError";

      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      Initialize();
    }

    [StorableConstructor]
    private FeatureSelectionProblem(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      FeatureSelectionProblem clone = (FeatureSelectionProblem)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    private void RegisterParameterValueEvents() {
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
    }

    private void RegisterParameterEvents() {
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_BinaryVectorParameter_ActualNameChanged);
    }

    #region event handling
    protected override void OnDataAnalysisProblemChanged(EventArgs e) {
      base.OnDataAnalysisProblemChanged(e);
      ParameterizeSolutionCreator();
    }
    protected virtual void OnOperatorsChanged(EventArgs e) { RaiseOperatorsChanged(e); }
    protected virtual void OnSolutionCreatorChanged(EventArgs e) {
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_BinaryVectorParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      OnSolutionParameterNameChanged(e);
      RaiseSolutionCreatorChanged(e);
    }

    protected virtual void OnSolutionParameterNameChanged(EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeOperators();
    }

    protected virtual void OnEvaluatorChanged(EventArgs e) {
      ParameterizeEvaluator();
      RaiseEvaluatorChanged(e);
    }
    #endregion

    #region event handlers
    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      OnSolutionCreatorChanged(e);
    }
    private void SolutionCreator_BinaryVectorParameter_ActualNameChanged(object sender, EventArgs e) {
      OnSolutionParameterNameChanged(e);
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      OnEvaluatorChanged(e);
    }
    #endregion

    #region Helpers
    private void Initialize() {
      InitializeOperators();
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    private void InitializeOperators() {
      operators = new List<IOperator>();
      operators.AddRange(ApplicationManager.Manager.GetInstances<IBinaryVectorOperator>().OfType<IOperator>());
      ParameterizeOperators();
    }

    private void ParameterizeSolutionCreator() {
      SolutionCreator.LengthParameter.Value = new IntValue(DataAnalysisProblemData.InputVariables.CheckedItems.Count());
    }

    private void ParameterizeEvaluator() {
      Evaluator.SolutionParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
    }

    private void ParameterizeOperators() {
      foreach (IBinaryVectorCrossover op in Operators.OfType<IBinaryVectorCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      }
      foreach (IBinaryVectorManipulator op in Operators.OfType<IBinaryVectorManipulator>()) {
        op.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      }
    }
    #endregion
  }
}
