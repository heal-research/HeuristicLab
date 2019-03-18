#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Knapsack {
  /// <summary>
  /// An operator that improves knapsack solutions.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Laguna, M. and Martí, R. (2003). Scatter Search: Methodology and Implementations in C. Operations Research/Computer Science Interfaces Series, Vol. 24. Springer.<br />
  /// The operator first orders the elements of the knapsack according to their value-to-weight ratio, then, if the solution is not feasible, removes the element with the lowest ratio until the solution is feasible and tries to add new elements with the best ratio that are not already included yet until the knapsack is full.
  /// </remarks>
  [Item("KnapsackImprovementOperator", "An operator that improves knapsack solutions. It is implemented as described in Laguna, M. and Martí, R. (2003). Scatter Search: Methodology and Implementations in C. Operations Research/Computer Science Interfaces Series, Vol. 24. Springer.")]
  [StorableClass]
  public sealed class KnapsackImprovementOperator : SingleSuccessorOperator, ISingleObjectiveImprovementOperator {
    #region Parameter properties
    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public ILookupParameter<IntValue> KnapsackCapacityParameter {
      get { return (ILookupParameter<IntValue>)Parameters["KnapsackCapacity"]; }
    }
    public ILookupParameter<DoubleValue> PenaltyParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Penalty"]; }
    }
    public IValueLookupParameter<IItem> SolutionParameter {
      get { return (IValueLookupParameter<IItem>)Parameters["Solution"]; }
    }
    public ILookupParameter<IntArray> ValuesParameter {
      get { return (ILookupParameter<IntArray>)Parameters["Values"]; }
    }
    public ILookupParameter<IntArray> WeightsParameter {
      get { return (ILookupParameter<IntArray>)Parameters["Weights"]; }
    }
    #endregion

    #region Properties
    private IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    private IntValue KnapsackCapacity {
      get { return KnapsackCapacityParameter.ActualValue; }
      set { KnapsackCapacityParameter.ActualValue = value; }
    }
    private DoubleValue Penalty {
      get { return PenaltyParameter.ActualValue; }
      set { PenaltyParameter.ActualValue = value; }
    }
    private IntArray Values {
      get { return ValuesParameter.ActualValue; }
      set { ValuesParameter.ActualValue = value; }
    }
    private IntArray Weights {
      get { return WeightsParameter.ActualValue; }
      set { WeightsParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    private KnapsackImprovementOperator(bool deserializing) : base(deserializing) { }
    private KnapsackImprovementOperator(KnapsackImprovementOperator original, Cloner cloner) : base(original, cloner) { }
    public KnapsackImprovementOperator()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that contains the solution to be improved."));
      Parameters.Add(new LookupParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack."));
      Parameters.Add(new LookupParameter<DoubleValue>("Penalty", "The penalty value for each unit of overweight."));
      Parameters.Add(new ValueLookupParameter<IItem>("Solution", "The solution to be improved. This parameter is used for name translation only."));
      Parameters.Add(new LookupParameter<IntArray>("Values", "The values of the items."));
      Parameters.Add(new LookupParameter<IntArray>("Weights", "The weights of the items."));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new KnapsackImprovementOperator(this, cloner);
    }

    public override IOperation Apply() {
      BinaryVector sol = CurrentScope.Variables[SolutionParameter.ActualName].Value as BinaryVector;
      if (sol == null)
        throw new ArgumentException("Cannot improve solution because it has the wrong type.");

      // calculate value-to-weight ratio
      double[] ratio = new double[Values.Length];
      for (int i = 0; i < ratio.Length; i++)
        ratio[i] = (double)Values[i] / (double)Weights[i];


      // calculate order for ratio
      int[] order = new int[ratio.Length];
      foreach (var x in ratio.Select((x, index) => new { Value = x, ValueIndex = index })
                             .OrderByDescending(x => x.Value)
                             .Select((x, index) => new { ValueIndex = x.ValueIndex, ItemIndex = index })) {
        order[x.ItemIndex] = x.ValueIndex;
      }

      int evaluatedSolutions = 0;
      int j = sol.Length - 1;
      var totalWeight = 0.0;
      for (var i = 0; i < sol.Length; i++) {
        if (sol[i]) totalWeight += Weights[i];
      }
      evaluatedSolutions++;

      while (totalWeight > KnapsackCapacity.Value && j >= 0) {
        totalWeight -= Weights[order[j]];
        sol[order[j--]] = false;
      }

      // calculate weight
      int weight = 0;
      for (int i = 0; i < sol.Length; i++)
        if (sol[i]) weight += Weights[i];

      // improve solution
      bool feasible = true; j = 0;
      while (feasible && j < sol.Length) {
        while (sol[order[j]]) j++;
        if (weight + Weights[order[j]] <= KnapsackCapacity.Value) {
          sol[order[j]] = true;
          weight += Weights[order[j]];
        } else feasible = false;
        evaluatedSolutions++;
      }

      CurrentScope.Variables[SolutionParameter.ActualName].Value = sol;
      CurrentScope.Variables.Add(new Variable("LocalEvaluatedSolutions", new IntValue(evaluatedSolutions)));

      return base.Apply();
    }
  }
}
