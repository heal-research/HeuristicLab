﻿#region License Information
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

using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("E9876DF8-ACFA-41C8-93B7-FA40C57CE459")]
  public abstract class SymbolicDataAnalysisMultiObjectiveProblem<T, U, V> : SymbolicDataAnalysisProblem<T, U, V>, ISymbolicDataAnalysisMultiObjectiveProblem
    where T : class, IDataAnalysisProblemData
    where U : class, ISymbolicDataAnalysisMultiObjectiveEvaluator<T>
    where V : class, ISymbolicDataAnalysisSolutionCreator {
    private const string MaximizationParameterName = "Maximization";
    private const string BestKnownQualityParameterName = "BestKnownQuality";

    #region parameter properties
    public IValueParameter<BoolArray> MaximizationParameter {
      get { return (IValueParameter<BoolArray>)Parameters[MaximizationParameterName]; }
    }
    IParameter IMultiObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    #endregion

    #region properties
    public BoolArray Maximization {
      get { return MaximizationParameter.Value; }
      set { MaximizationParameter.Value = value; }
    }
    IMultiObjectiveEvaluator IMultiObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return Evaluator; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisMultiObjectiveProblem(StorableConstructorFlag _) : base(_) { }
    protected SymbolicDataAnalysisMultiObjectiveProblem(SymbolicDataAnalysisMultiObjectiveProblem<T, U, V> original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandler();
    }

    public SymbolicDataAnalysisMultiObjectiveProblem(T problemData, U evaluator, V solutionCreator)
      : base(problemData, evaluator, solutionCreator) {
      Parameters.Add(new ValueParameter<BoolArray>(MaximizationParameterName, "Set to false if the problem should be minimized."));

      ParameterizeOperators();
      RegisterEventHandler();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandler();
    }

    private void RegisterEventHandler() {
      Evaluator.QualitiesParameter.ActualNameChanged += new System.EventHandler(QualitiesParameter_ActualNameChanged);
    }

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      Evaluator.QualitiesParameter.ActualNameChanged += new System.EventHandler(QualitiesParameter_ActualNameChanged);
      Maximization = new BoolArray(Evaluator.Maximization.ToArray());
      ParameterizeOperators();
    }

    private void QualitiesParameter_ActualNameChanged(object sender, System.EventArgs e) {
      ParameterizeOperators();
    }

    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
      foreach (var op in Operators.OfType<ISymbolicDataAnalysisMultiObjectiveAnalyzer>()) {
        op.QualitiesParameter.ActualName = Evaluator.QualitiesParameter.ActualName;
        op.MaximizationParameter.ActualName = MaximizationParameterName;
      }

      // these two crossover operators are compatible with single-objective problems only so we remove them from the operators collection
      bool pred(IItem x) {
        return x is SymbolicDataAnalysisExpressionDeterministicBestCrossover<T> || x is SymbolicDataAnalysisExpressionContextAwareCrossover<T>;
      };
      Operators.RemoveAll(pred);
      // if a multi crossover is present, remove them from its operator collection
      var cx = Operators.FirstOrDefault(x => x is MultiSymbolicDataAnalysisExpressionCrossover<T>);
      if (cx != null) {
        var multiCrossover = (MultiSymbolicDataAnalysisExpressionCrossover<T>)cx;
        var items = multiCrossover.Operators.Where(pred).Cast<ISymbolicExpressionTreeCrossover>().ToList();
        foreach (var item in items) {
          multiCrossover.Operators.Remove(item);
        }
      }
    }
  }
}
