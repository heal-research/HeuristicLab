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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic.Evaluators;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic.Interfaces;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic {
  [Item("Symbolic Vector Regression Problem (multi objective)", "Represents a multi objective symbolic vector regression problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public class MultiObjectiveSymbolicVectorRegressionProblem : SymbolicVectorRegressionProblem, IMultiObjectiveProblem {

    #region Parameter Properties
    public ValueParameter<BoolArray> MultiObjectiveMaximizationParameter {
      get { return (ValueParameter<BoolArray>)Parameters["MultiObjectiveMaximization"]; }
    }
    IParameter IMultiObjectiveProblem.MaximizationParameter {
      get { return MultiObjectiveMaximizationParameter; }
    }

    public new ValueParameter<IMultiObjectiveSymbolicVectorRegressionEvaluator> EvaluatorParameter {
      get { return (ValueParameter<IMultiObjectiveSymbolicVectorRegressionEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    #endregion

    #region Properties
    public new IMultiObjectiveSymbolicVectorRegressionEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    IMultiObjectiveEvaluator IMultiObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected MultiObjectiveSymbolicVectorRegressionProblem(bool deserializing) : base(deserializing) { }
    public MultiObjectiveSymbolicVectorRegressionProblem()
      : base() {
      var evaluator = new SymbolicVectorRegressionScaledMseEvaluator();
      Parameters.Add(new ValueParameter<BoolArray>("MultiObjectiveMaximization", "Set to false as the error of the regression model should be minimized.", new BoolArray(MultiVariateDataAnalysisProblemData.TargetVariables.CheckedItems.Count())));
      Parameters.Add(new ValueParameter<IMultiObjectiveSymbolicVectorRegressionEvaluator>("Evaluator", "The operator which should be used to evaluate symbolic regression solutions.", evaluator));

      ParameterizeEvaluator();
      Initialize();
    }
    
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      MultiObjectiveSymbolicVectorRegressionProblem clone = (MultiObjectiveSymbolicVectorRegressionProblem)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    private void RegisterParameterValueEvents() {
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
    }

    #region event handling
    protected override void OnMultiVariateDataAnalysisProblemChanged(EventArgs e) {
      base.OnMultiVariateDataAnalysisProblemChanged(e);
      MultiObjectiveMaximizationParameter.Value = new BoolArray(MultiVariateDataAnalysisProblemData.TargetVariables.CheckedItems.Count());
      // paritions could be changed
      ParameterizeEvaluator();
    }

    protected override void OnSolutionParameterNameChanged(EventArgs e) {
      base.OnSolutionParameterNameChanged(e);
      ParameterizeEvaluator();
    }

    protected virtual void OnEvaluatorChanged(EventArgs e) {
      ParameterizeEvaluator();
      RaiseEvaluatorChanged(e);
    }
    #endregion

    #region event handlers
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      OnEvaluatorChanged(e);
    }
    #endregion

    #region Helpers
    private void Initialize() {
      RegisterParameterValueEvents();
    }


    private void ParameterizeEvaluator() {
      Evaluator.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      Evaluator.MultiVariateDataAnalysisProblemDataParameter.ActualName = MultiVariateDataAnalysisProblemDataParameter.Name;
      Evaluator.SamplesStartParameter.Value = TrainingSamplesStart;
      Evaluator.SamplesEndParameter.Value = TrainingSamplesEnd;
    }
    #endregion
  }
}
