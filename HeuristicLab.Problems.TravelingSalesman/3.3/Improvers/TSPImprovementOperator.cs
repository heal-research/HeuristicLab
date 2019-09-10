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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator that improves traveling salesman solutions.
  /// </summary>
  /// <remarks>
  /// The operator tries to improve the traveling salesman solution by swapping two randomly chosen edges for a certain number of times.
  /// </remarks>
  [Item("TSPImprovementOperator", "An operator that improves traveling salesman solutions. The operator tries to improve the traveling salesman solution by swapping two randomly chosen edges for a certain number of times.")]
  [StorableType("1b3cbc66-6dcc-4f61-9cbe-8b50cc54413a")]
  public sealed class TSPImprovementOperator : SingleSuccessorOperator, ISingleObjectiveImprovementOperator {
    
    [Storable] public ILookupParameter<ITSPData> TSPDataParameter { get; private set; }
    [Storable] public IValueParameter<IntValue> ImprovementAttemptsParameter { get; private set; }
    [Storable] public ILookupParameter<IRandom> RandomParameter { get; private set; }
    [Storable] public IValueLookupParameter<IItem> SolutionParameter { get; private set; }

    public int ImprovementAttempts {
      get { return ImprovementAttemptsParameter.Value.Value; }
      set { ImprovementAttemptsParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private TSPImprovementOperator(StorableConstructorFlag _) : base(_) { }
    private TSPImprovementOperator(TSPImprovementOperator original, Cloner cloner)
      : base(original, cloner) {
      TSPDataParameter = cloner.Clone(original.TSPDataParameter);
      ImprovementAttemptsParameter = cloner.Clone(original.ImprovementAttemptsParameter);
      RandomParameter = cloner.Clone(original.RandomParameter);
      SolutionParameter = cloner.Clone(original.SolutionParameter);
    }
    public TSPImprovementOperator()
      : base() {
      #region Create parameters
      Parameters.Add(TSPDataParameter = new LookupParameter<ITSPData>("TSPData", "The main parameters of the TSP."));
      Parameters.Add(ImprovementAttemptsParameter = new ValueParameter<IntValue>("ImprovementAttempts", "The number of improvement attempts the operator should perform.", new IntValue(100)));
      Parameters.Add(RandomParameter = new LookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(SolutionParameter = new ValueLookupParameter<IItem>("Solution", "The solution to be improved. This parameter is used for name translation only."));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPImprovementOperator(this, cloner);
    }

    public override IOperation Apply() {
      var random = RandomParameter.ActualValue;
      var solution = ExecutionContext.Scope.Variables[SolutionParameter.ActualName].Value as Permutation;
      if (solution == null)
        throw new ArgumentException("Cannot improve solution because it has the wrong type.");
      if (solution.PermutationType != PermutationTypes.RelativeUndirected)
        throw new ArgumentException("Cannot improve solution because the permutation type is not supported.");
      var tspData = TSPDataParameter.ActualValue;

      for (int i = 0; i < ImprovementAttempts; i++) {
        var move = StochasticInversionSingleMoveGenerator.Apply(solution, random);
        double moveQualtiy = TSPInversionMoveEvaluator.CalculateTourLengthDelta(tspData, solution, move);
        if (moveQualtiy < 0)
          InversionManipulator.Apply(solution, move.Index1, move.Index2);
      }

      ExecutionContext.Scope.Variables.Add(new Variable("LocalEvaluatedSolutions", new IntValue(ImprovementAttempts)));

      return base.Apply();
    }
  }
}
