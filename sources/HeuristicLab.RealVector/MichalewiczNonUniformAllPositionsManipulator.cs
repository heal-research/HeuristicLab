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
  public class MichalewiczNonUniformAllPositionsManipulator : RealVectorManipulatorBase {
    public override string Description {
      get { return
@"Non-uniformly distributed change of all positions of a real vector (Michalewicz 1992)
Initially, the space will be searched uniformly and very locally at later stages. This increases the probability of generating the new number closer to its successor instead of a random number.

Michalewicz, Z. (1992). Genetic Algorithms + Data Structures = Evolution Programs. Springer Verlag.";
      }
    }

    public MichalewiczNonUniformAllPositionsManipulator()
      : base() {
      AddVariableInfo(new VariableInfo("Minimum", "Minimum of the sampling range for the vector element (included)", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Maximum", "Maximum of the sampling range for the vector element (excluded)", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("CurrentGeneration", "Current Generation of the algorithm", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaximumGenerations", "Maximum number of Generations", typeof(IntData), VariableKind.In));
      VariableInfo genDepInfo = new VariableInfo("GenerationsDependency", "Specifies the degree of dependency on the number of generations", typeof(IntData), VariableKind.In);
      genDepInfo.Local = true;
      AddVariableInfo(genDepInfo);
      AddVariable(new Variable("GenerationsDependency", new IntData(5)));
    }

    protected override double[] Manipulate(IScope scope, IRandom random, double[] vector) {
      double min = GetVariableValue<DoubleData>("Minimum", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Maximum", scope, true).Data;
      int currentGeneration = GetVariableValue<IntData>("CurrentGeneration", scope, true).Data;
      int maximumGenerations = GetVariableValue<IntData>("MaximumGenerations", scope, true).Data;
      int generationsDependency = GetVariableValue<IntData>("GenerationsDependency", scope, true).Data;
      return Apply(random, vector, min, max, currentGeneration, maximumGenerations, generationsDependency);
    }

    public static double[] Apply(IRandom random, double[] vector, double min, double max, int currentGeneration, int maximumGenerations, int generationsDependency) {
      int length = vector.Length;
      double[] result = new double[length];

      for (int i = 0; i < length; i++) {
        if (random.NextDouble() < 0.5) {
          vector[i] = vector[i] + Delta(random, currentGeneration, max - vector[i], maximumGenerations, generationsDependency);
        } else {
          vector[i] = vector[i] - Delta(random, currentGeneration, vector[i] - min, maximumGenerations, generationsDependency);
        }
      }

      return vector;
    }

    // returns a value between 0 and y (both included)
    private static double Delta(IRandom random, int currentGeneration, double y, int maximumGenerations, int generationsDependency) {
      return y * (1 - Math.Pow(random.NextDouble(), Math.Pow(1 - currentGeneration / maximumGenerations, generationsDependency)));
    }
  }
}
