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
using HeuristicLab.Random;

namespace HeuristicLab.RealVector {
  public class SelfAdaptiveNormalAllPositionsManipulator : RealVectorManipulatorBase {
    public override string Description {
      get { return @"Manipulates each dimension in the real vector with the mutation strength given in the strategy parameter vector"; }
    }

    public SelfAdaptiveNormalAllPositionsManipulator()
      : base() {
      AddVariableInfo(new VariableInfo("StrategyVector", "The strategy vector determining the strength of the mutation", typeof(DoubleArrayData), VariableKind.In));
    }

    public static double[] Apply(double[] strategyParameters, IRandom random, double[] vector) {
      NormalDistributedRandom N = new NormalDistributedRandom(random, 0.0, 1.0);
      for (int i = 0; i < vector.Length; i++) {
        vector[i] = vector[i] + (N.NextDouble() * strategyParameters[i % strategyParameters.Length]);
      }
      return vector;
    }

    protected override double[] Manipulate(IScope scope, IRandom random, double[] vector) {
      double[] strategyVector = scope.GetVariableValue<DoubleArrayData>("StrategyVector", true).Data;
      return Apply(strategyVector, random, vector);
    }
  }
}
