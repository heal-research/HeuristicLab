#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using System;
using System.Linq;

namespace HeuristicLab.Algorithms.CMAEvolutionStrategy {
  [Item("Terminator", "Decides if the algorithm should terminate or not.")]
  [StorableType("BE0F0791-4B19-4AC0-ACD6-C0FBEF829DBC")]
  public class Terminator : Operator, IIterationBasedOperator, ISingleObjectiveOperator {

    protected OperatorParameter ContinueParameter {
      get { return (OperatorParameter)Parameters["Continue"]; }
    }
    protected OperatorParameter TerminateParameter {
      get { return (OperatorParameter)Parameters["Terminate"]; }
    }

    public IOperator Continue {
      get { return ContinueParameter.Value; }
      set { ContinueParameter.Value = value; }
    }

    public IOperator Terminate {
      get { return TerminateParameter.Value; }
      set { TerminateParameter.Value = value; }
    }

    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    public ILookupParameter<CMAParameters> StrategyParametersParameter {
      get { return (ILookupParameter<CMAParameters>)Parameters["StrategyParameters"]; }
    }

    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Iterations"]; }
    }

    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }

    public ILookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }

    public IValueLookupParameter<IntValue> MaximumEvaluatedSolutionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumEvaluatedSolutions"]; }
    }

    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public IValueLookupParameter<DoubleValue> TargetQualityParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["TargetQuality"]; }
    }

    public IValueLookupParameter<DoubleValue> MinimumQualityChangeParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["MinimumQualityChange"]; }
    }

    public IValueLookupParameter<DoubleValue> MinimumQualityHistoryChangeParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["MinimumQualityHistoryChange"]; }
    }

    public IValueLookupParameter<DoubleValue> MinimumStandardDeviationParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["MinimumStandardDeviation"]; }
    }

    public ILookupParameter<DoubleArray> InitialSigmaParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["InitialSigma"]; }
    }

    public IValueLookupParameter<DoubleValue> MaximumStandardDeviationChangeParameter {
      get { return (IValueLookupParameter<DoubleValue>)Parameters["MaximumStandardDeviationChange"]; }
    }

    public ILookupParameter<BoolValue> DegenerateStateParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["DegenerateState"]; }
    }

    [StorableConstructor]
    protected Terminator(StorableConstructorFlag _) : base(_) { }
    protected Terminator(Terminator original, Cloner cloner)
      : base(original, cloner) { }
    public Terminator() {
      Parameters.Add(new OperatorParameter("Continue", "The operator that is executed if the stop conditions have not been met!"));
      Parameters.Add(new OperatorParameter("Terminate", "The operator that is executed if the stop conditions have been met!"));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is to be maximized and false otherwise."));
      Parameters.Add(new LookupParameter<CMAParameters>("StrategyParameters", "The CMA-ES strategy parameters."));
      Parameters.Add(new LookupParameter<IntValue>("Iterations", "The number of iterations passed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum number of iterations."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of evaluated solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumEvaluatedSolutions", "The maximum number of evaluated solutions."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The quality of the offspring."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("TargetQuality", "(stopFitness) Surpassing this quality value terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MinimumQualityChange", "(stopTolFun) If the range of fitness values is less than a certain value the algorithm terminates (set to 0 or positive value to enable)."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MinimumQualityHistoryChange", "(stopTolFunHist) If the range of fitness values is less than a certain value for a certain time the algorithm terminates (set to 0 or positive to enable)."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MinimumStandardDeviation", "(stopTolXFactor) If the standard deviation falls below a certain value the algorithm terminates (set to 0 or positive to enable)."));
      Parameters.Add(new LookupParameter<DoubleArray>("InitialSigma", "The initial value for Sigma."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumStandardDeviationChange", "(stopTolUpXFactor) If the standard deviation changes by a value larger than this parameter the algorithm stops (set to a value > 0 to enable)."));
      Parameters.Add(new LookupParameter<BoolValue>("DegenerateState", "Whether the algorithm state has degenerated and should be terminated."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Terminator(this, cloner);
    }

    public override IOperation Apply() {
      var terminateOp = Terminate != null ? ExecutionContext.CreateOperation(Terminate) : null;

      var degenerated = DegenerateStateParameter.ActualValue.Value;
      if (degenerated) return terminateOp;

      var iterations = IterationsParameter.ActualValue.Value;
      var maxIterations = MaximumIterationsParameter.ActualValue.Value;
      if (iterations >= maxIterations) return terminateOp;

      var evals = EvaluatedSolutionsParameter.ActualValue.Value;
      var maxEvals = MaximumEvaluatedSolutionsParameter.ActualValue.Value;
      if (evals >= maxEvals) return terminateOp;

      var maximization = MaximizationParameter.ActualValue.Value;
      var bestQuality = QualityParameter.ActualValue.First().Value;
      var targetQuality = TargetQualityParameter.ActualValue.Value;
      if (iterations > 1 && (maximization && bestQuality >= targetQuality
        || !maximization && bestQuality <= targetQuality)) return terminateOp;

      var sp = StrategyParametersParameter.ActualValue;
      var worstQuality = QualityParameter.ActualValue.Last().Value;
      var minHist = sp.QualityHistory.Min();
      var maxHist = sp.QualityHistory.Max();
      var change = Math.Max(maxHist, Math.Max(bestQuality, worstQuality))
                   - Math.Min(minHist, Math.Min(bestQuality, worstQuality));
      var stopTolFun = MinimumQualityChangeParameter.ActualValue.Value;
      if (change <= stopTolFun) return terminateOp;

      if (iterations > sp.QualityHistorySize &&
          maxHist - minHist <= MinimumQualityHistoryChangeParameter.ActualValue.Value)
        return terminateOp;

      double minSqrtdiagC = int.MaxValue, maxSqrtdiagC = int.MinValue;
      for (int i = 0; i < sp.C.GetLength(0); i++) {
        if (Math.Sqrt(sp.C[i, i]) < minSqrtdiagC) minSqrtdiagC = Math.Sqrt(sp.C[i, i]);
        if (Math.Sqrt(sp.C[i, i]) > maxSqrtdiagC) maxSqrtdiagC = Math.Sqrt(sp.C[i, i]);
      }

      var tolx = MinimumStandardDeviationParameter.ActualValue.Value;
      if (sp.Sigma * maxSqrtdiagC < tolx
          && sp.Sigma * sp.PC.Select(x => Math.Abs(x)).Max() < tolx) return terminateOp;

      var stopTolUpXFactor = MaximumStandardDeviationChangeParameter.ActualValue.Value;
      if (sp.Sigma * maxSqrtdiagC > stopTolUpXFactor * InitialSigmaParameter.ActualValue.Max())
        return terminateOp;

      return Continue != null ? ExecutionContext.CreateOperation(Continue) : null;
    }
  }
}
