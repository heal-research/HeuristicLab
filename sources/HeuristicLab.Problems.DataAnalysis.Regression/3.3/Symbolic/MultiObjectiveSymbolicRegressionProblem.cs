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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("Symbolic Regression Problem (multi objective)", "Represents a multi objective symbolic regression problem.")]
  [StorableClass]
  [NonDiscoverableType]
  public class MultiObjectiveSymbolicRegressionProblem : SymbolicRegressionProblemBase, IMultiObjectiveHeuristicOptimizationProblem {

    #region Parameter Properties
    public ValueParameter<BoolArray> MaximizationParameter {
      get { return (ValueParameter<BoolArray>)Parameters["Maximization"]; }
    }
    IParameter IMultiObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public new ValueParameter<IMultiObjectiveSymbolicRegressionEvaluator> EvaluatorParameter {
      get { return (ValueParameter<IMultiObjectiveSymbolicRegressionEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IHeuristicOptimizationProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    #endregion

    #region Properties
    public new IMultiObjectiveSymbolicRegressionEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    IMultiObjectiveEvaluator IMultiObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IHeuristicOptimizationProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected MultiObjectiveSymbolicRegressionProblem(bool deserializing) : base(deserializing) { }
    protected MultiObjectiveSymbolicRegressionProblem(MultiObjectiveSymbolicRegressionProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }
    public MultiObjectiveSymbolicRegressionProblem()
      : base() {
      var evaluator = new MultiObjectiveSymbolicRegressionPearsonsRSquaredEvaluator();
      Parameters.Add(new ValueParameter<BoolArray>("Maximization", "Set to false as the error of the regression model should be minimized.", new BoolArray(new bool[] { true, false })));
      Parameters.Add(new ValueParameter<IMultiObjectiveSymbolicRegressionEvaluator>("Evaluator", "The operator which should be used to evaluate symbolic regression solutions.", evaluator));

      evaluator.QualitiesParameter.ActualName = "TrainingRSquared/Size";

      ParameterizeEvaluator();

      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveSymbolicRegressionProblem(this, cloner);
    }

    private void RegisterParameterValueEvents() {
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
    }

    private void RegisterParameterEvents() {
    }

    #region event handling
    protected override void OnDataAnalysisProblemChanged(EventArgs e) {
      base.OnDataAnalysisProblemChanged(e);
      // paritions could be changed
      ParameterizeEvaluator();
    }
    protected override void OnSolutionParameterNameChanged(EventArgs e) {
      ParameterizeEvaluator();
    }

    protected override void OnEvaluatorChanged(EventArgs e) {
      base.OnEvaluatorChanged(e);
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
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    private void ParameterizeEvaluator() {
      Evaluator.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      Evaluator.RegressionProblemDataParameter.ActualName = DataAnalysisProblemDataParameter.Name;
      Evaluator.SamplesStartParameter.Value = TrainingSamplesStart;
      Evaluator.SamplesEndParameter.Value = TrainingSamplesEnd;
    }
    #endregion
  }
}
