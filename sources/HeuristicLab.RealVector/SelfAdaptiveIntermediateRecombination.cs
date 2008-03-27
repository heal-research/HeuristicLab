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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.RealVector {
  public class SelfAdaptiveIntermediateRecombination : DiscreteRecombination {
    public override string Description {
      get {
        return @"Self adaptive intermediate recombination creates a new offspring by computing the centroid of the parents. It will also use the same strategy to combine the endogenous strategy parameter vector.";
      }
    }

    public SelfAdaptiveIntermediateRecombination()
      : base() {
      AddVariableInfo(new VariableInfo("StrategyVector", "Vector containing the endogenous strategy parameters", typeof(DoubleArrayData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      int rho = GetVariableValue<IntData>("Rho", scope, true).Data;
      // with just 1 parent no recombination is necessary/possible
      if (rho == 1) return null;
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);

      if (scope.SubScopes.Count % rho != 0)
        throw new InvalidOperationException("Number of parents is not a multiple of rho");
      int lambda = scope.SubScopes.Count / rho;
      IList<double[]> parents = new List<double[]>(rho);
      IList<double[]> parentsStrategy = new List<double[]>(rho);

      for (int i = 0; i < lambda; i++) {
        IScope childScope = new Scope(i.ToString());
        double[] childGene = (double[])scope.SubScopes[0].GetVariableValue<DoubleArrayData>("RealVector", false).Data.Clone();
        double[] strategyParams = (double[])scope.SubScopes[0].GetVariableValue<DoubleArrayData>("StrategyVector", false).Data.Clone();
        parents.Clear();
        for (int j = 0; j < rho; j++) {
          IScope parent = scope.SubScopes[0];
          parents.Add(parent.GetVariableValue<DoubleArrayData>("RealVector", false).Data);
          parentsStrategy.Add(parent.GetVariableValue<DoubleArrayData>("StrategyVector", false).Data);
          scope.RemoveSubScope(parent);
        }
        // actual discrete recombination
        if (childGene.Length != strategyParams.Length)
          throw new InvalidOperationException("ERROR: strategy vector must be as long as there are dimensions");

        for (int x = 0; x < childGene.Length; x++) {
          double sum = 0.0, sumStrategy = 0.0;
          for (int y = 0; y < rho; y++) {
            sum += parents[y][x];
            sumStrategy += parentsStrategy[y][x];
          }
          childGene[x] = sum / rho;
          strategyParams[x] = sumStrategy / rho;
        }
        childScope.AddVariable(new Variable(scope.SubScopes[0].TranslateName("RealVector"), new DoubleArrayData(childGene)));
        childScope.AddVariable(new Variable(scope.SubScopes[0].TranslateName("StrategyVector"), new DoubleArrayData(strategyParams)));
        scope.AddSubScope(childScope);
      }
      return null;
    }
  }
}
