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
using HeuristicLab.Evolutionary;

namespace HeuristicLab.RealVector {
  class HeuristicCrossover : CrossoverBase {
    public HeuristicCrossover()
      : base() {
      AddVariableInfo(new VariableInfo("Maximization", "Maximization problem", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "Quality value", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("RealVector", "Parent and child real vector", typeof(DoubleArrayData), VariableKind.In | VariableKind.New));
    }

    public override string Description {
      get { return "Heuristic crossover for real vectors."; }
    }

    public static double[] Apply(IRandom random, bool maximization, double[] parent1, double quality1, double[] parent2, double quality2) {
      int length = parent1.Length;
      double[] result = new double[length];
      double factor = random.NextDouble();

      for (int i = 0; i < length; i++) {
        if ((maximization && (quality1 > quality2)) || ((!maximization) && (quality1 < quality2)))
          result[i] = parent1[i] + factor * (parent1[i] - parent2[i]);
        else
          result[i] = parent2[i] + factor * (parent2[i] - parent1[i]);
      }
      return result;
    }

    protected sealed override void Cross(IScope scope, IRandom random, IScope parent1, IScope parent2, IScope child) {
      bool maximization = GetVariableValue<BoolData>("Maximization", scope, true).Data;
      DoubleArrayData vector1 = parent1.GetVariableValue<DoubleArrayData>("RealVector", false);
      DoubleData quality1 = parent1.GetVariableValue<DoubleData>("Quality", false);
      DoubleArrayData vector2 = parent2.GetVariableValue<DoubleArrayData>("RealVector", false);
      DoubleData quality2 = parent2.GetVariableValue<DoubleData>("Quality", false);

      if (vector1.Data.Length != vector2.Data.Length) throw new InvalidOperationException("Cannot apply crossover to real vectors of different length.");

      double[] result = Apply(random, maximization, vector1.Data, quality1.Data, vector2.Data, quality2.Data);
      child.AddVariable(new Variable(child.TranslateName("RealVector"), new DoubleArrayData(result)));
    }
  }
}
