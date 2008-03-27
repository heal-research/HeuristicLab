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
  /// <summary>
  /// Mühlenbein, Schlierkamp-Voosen (1993)
  /// Predictive Models for the Breeder Genetic Algorithm I. Continuous Parameter Optimization
  /// </summary>
  public class BreederGeneticAlgorithmManipulator : RealVectorManipulatorBase {
    public override string Description {
      get {
        return
@"Breeder Genetic Algorithm Manipulator (Mühlenbein, Schlierkamp-Voosen, 1993). Changes one position of a real vector by adding/substracting a value of the interval [(2^-15)*range, ..., (2^0)*range], where range is SearchIntervalFactor * (max - min).

Mühlenbein, Schlierkamp-Voosen (1993). Predictive Models for the Breeder Genetic Algorithm I. Continuous Parameter Optimization";
      }
    }

    public BreederGeneticAlgorithmManipulator()
      : base() {
      AddVariableInfo(new VariableInfo("Minimum", "Minimum of the sampling range for the vector element (included)", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximum", "Maximum of the sampling range for the vector element (excluded)", typeof(DoubleData), VariableKind.In));
      VariableInfo sifInfo = new VariableInfo("SearchIntervalFactor", "The factor determining the size of the search interval, that will be added/removed to/from the allele selected for manipulation.", typeof(DoubleData), VariableKind.In);
      sifInfo.Local = true;
      AddVariableInfo(sifInfo);
      AddVariable(new Variable("SearchIntervalFactor", new DoubleData(0.1)));
    }

    public static double[] Apply(IScope scope, IRandom random, double[] vector, double min, double max, double searchIntervalFactor) {
      int length = vector.Length;
      int pos = random.Next(length);
      double range = searchIntervalFactor * (max - min);
      double value = range * Sigma(random);

      if (random.NextDouble() < 0.5) {
        vector[pos] = vector[pos] + value;
      } else {
        vector[pos] = vector[pos] - value;
      }

      return vector;
    }

    private static double Sigma(IRandom random) {
      double sigma = 0;
      int limit = 16;

      for (int i = 0; i < limit; i++) {
        if (random.Next(limit) == 15) {
          // execute this statement with a probability of 1/16
          sigma += Math.Pow(2, -i);
        }
      }

      return sigma;
    }

    protected override double[] Manipulate(IScope scope, IRandom random, double[] vector) {
      double min = GetVariableValue<DoubleData>("Minimum", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Maximum", scope, true).Data;
      double searchIntervalFactor = GetVariableValue<DoubleData>("SearchIntervalFactor", scope, true).Data;
      return Apply(scope, random, vector, min, max, searchIntervalFactor);
    }
  }
}
