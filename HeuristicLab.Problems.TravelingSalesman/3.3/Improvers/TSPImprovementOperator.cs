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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator that improves traveling salesman solutions.
  /// </summary>
  /// <remarks>
  /// The operator tries to improve the traveling salesman solution by swapping two randomly chosen edges for a certain number of times.
  /// </remarks>
  [Item("TSPImprovementOperator", "An operator that improves traveling salesman solutions. The operator tries to improve the traveling salesman solution by swapping two randomly chosen edges for a certain number of times.")]
  [StorableType("9C3B53A4-8FE7-45FC-833B-FF3DAE578010")]
  public sealed class TSPImprovementOperator : SingleSuccessorOperator, ISingleObjectiveImprovementOperator {
    #region Parameter properties
    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public ILookupParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }
    public IValueParameter<IntValue> ImprovementAttemptsParameter {
      get { return (IValueParameter<IntValue>)Parameters["ImprovementAttempts"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IItem> SolutionParameter {
      get { return (IValueLookupParameter<IItem>)Parameters["Solution"]; }
    }
    #endregion

    #region Properties
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    public DistanceMatrix DistanceMatrix {
      get { return DistanceMatrixParameter.ActualValue; }
      set { DistanceMatrixParameter.ActualValue = value; }
    }
    public IntValue ImprovementAttempts {
      get { return ImprovementAttemptsParameter.Value; }
      set { ImprovementAttemptsParameter.Value = value; }
    }
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
      set { RandomParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    private TSPImprovementOperator(StorableConstructorFlag _) : base(_) { }
    private TSPImprovementOperator(TSPImprovementOperator original, Cloner cloner) : base(original, cloner) { }
    public TSPImprovementOperator()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that contains the solution to be improved."));
      Parameters.Add(new LookupParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new ValueParameter<IntValue>("ImprovementAttempts", "The number of improvement attempts the operator should perform.", new IntValue(100)));
      Parameters.Add(new LookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<IItem>("Solution", "The solution to be improved. This parameter is used for name translation only."));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPImprovementOperator(this, cloner);
    }

    public override IOperation Apply() {
      var solution = CurrentScope.Variables[SolutionParameter.ActualName].Value as Permutation;
      if (solution == null)
        throw new ArgumentException("Cannot improve solution because it has the wrong type.");
      if (solution.PermutationType != PermutationTypes.RelativeUndirected)
        throw new ArgumentException("Cannot improve solution because the permutation type is not supported.");

      for (int i = 0; i < ImprovementAttempts.Value; i++) {
        var move = StochasticInversionSingleMoveGenerator.Apply(solution, Random);
        double moveQualtiy = TSPInversionMovePathEvaluator.EvaluateByDistanceMatrix(solution, move, DistanceMatrix);
        if (moveQualtiy < 0)
          InversionManipulator.Apply(solution, move.Index1, move.Index2);
      }

      CurrentScope.Variables.Add(new Variable("LocalEvaluatedSolutions", ImprovementAttempts));

      return base.Apply();
    }
  }
}
