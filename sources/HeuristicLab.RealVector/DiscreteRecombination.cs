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
  public class DiscreteRecombination : OperatorBase {
    public override string Description {
      get {
        return @"Discrete/dominant recombination creates a new offspring by combining the alleles in the parents such that each allele is randomly selected from one parent";
      }
    }

    public DiscreteRecombination()
      : base() {
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("Rho", "Amount of parents to recombine", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("RealVector", "Parent and child real vector", typeof(DoubleArrayData), VariableKind.In | VariableKind.New));
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

      for (int i = 0; i < lambda; i++) {
        IScope childScope = new Scope(i.ToString());
        double[] childGene = (double[])scope.SubScopes[0].GetVariableValue<DoubleArrayData>("RealVector", false).Data.Clone();
        parents.Clear();
        for (int j = 0; j < rho; j++) {
          IScope parent = scope.SubScopes[0];
          parents.Add(parent.GetVariableValue<DoubleArrayData>("RealVector", false).Data);
          scope.RemoveSubScope(parent);
        }
        // actual discrete recombination
        for (int x = 0; x < childGene.Length; x++) {
          childGene[x] = parents[random.Next(rho)][x];
        }
        childScope.AddVariable(new Variable(scope.SubScopes[0].TranslateName("RealVector"), new DoubleArrayData(childGene)));
        scope.AddSubScope(childScope);
      }
      return null;
    }
  }
}
