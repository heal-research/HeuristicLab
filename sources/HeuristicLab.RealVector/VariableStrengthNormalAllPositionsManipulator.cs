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
  public class VariableStrengthNormalAllPositionsManipulator : RealVectorManipulatorBase {
    public override string Description {
      get { return @"Adds a N(0, 1*factor) distributed random variable to each dimension in the real vector"; }
    }

    public VariableStrengthNormalAllPositionsManipulator()
      : base() {
      AddVariableInfo(new VariableInfo("ShakingFactor", "The factor determining the strength of the mutation", typeof(DoubleData), VariableKind.In));
    }

    public static double[] Apply(double shakingFactor, IRandom random, double[] vector) {
      NormalDistributedRandom N = new NormalDistributedRandom(random, 0.0, 1.0 * shakingFactor);
      for (int i = 0; i < vector.Length; i++) {
        vector[i] = vector[i] + N.NextDouble();
      }
      return vector;
    }

    protected override double[] Manipulate(IScope scope, IRandom random, double[] vector) {
      double shakingFactor = GetVariableValue<DoubleData>("ShakingFactor", scope, true).Data;
      return Apply(shakingFactor, random, vector);
    }
  }
}
