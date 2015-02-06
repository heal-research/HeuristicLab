#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ParameterlessPopulationPyramid {
  // This code is based off the publication
  // B. W. Goldman and W. F. Punch, "Parameter-less Population Pyramid," GECCO, pp. 785–792, 2014
  // and the original source code in C++11 available from: https://github.com/brianwgoldman/Parameter-less_Population_Pyramid
  [Item("Deceptive Trap Problem", "Genome encodes completely separable blocks, where each block is fully deceptive.")]
  [StorableClass]
  [Creatable("Parameterless Population Pyramid")]
  public class DeceptiveTrapProblem : BinaryVectorProblem {
    [StorableConstructor]
    protected DeceptiveTrapProblem(bool deserializing) : base(deserializing) { }
    protected DeceptiveTrapProblem(DeceptiveTrapProblem original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DeceptiveTrapProblem(this, cloner);
    }

    public override bool Maximization {
      get { return true; }
    }

    private const string TrapSizeParameterName = "Trap Size";

    public IFixedValueParameter<IntValue> TrapSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[TrapSizeParameterName]; }
    }

    public int TrapSize {
      get { return TrapSizeParameter.Value.Value; }
      set { TrapSizeParameter.Value.Value = value; }
    }

    protected virtual int TrapMaximum {
      get { return TrapSize; }
    }

    public DeceptiveTrapProblem()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(TrapSizeParameterName, "", new IntValue(7)));
      Length = 49;
    }

    // In the GECCO paper, calculates Equation 3
    protected virtual int Score(bool[] individual, int trapIndex, int trapSize) {
      int result = 0;
      // count number of bits in trap set to 1
      for (int index = trapIndex; index < trapIndex + trapSize; index++) {
        if (individual[index]) result++;
      }

      // Make it deceptive
      if (result < trapSize) {
        result = trapSize - result - 1;
      }
      return result;
    }

    public override double Evaluate(bool[] individual) {
      if (individual.Length != Length) throw new ArgumentException("The individual has not the correct length.");
      int total = 0;
      var trapSize = TrapSize;
      for (int i = 0; i < individual.Length; i += trapSize) {
        total += Score(individual, i, trapSize);
      }
      return (double)(total * trapSize) / (TrapMaximum * individual.Length);
    }
  }
}
