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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.Binary {
  [Item("Deceptive Trap Problem", "Genome encodes completely separable blocks, where each block is fully deceptive.")]
  [StorableType("399FFE01-2B76-4DBF-B363-8BB65FE95A5D")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 230)]
  public class DeceptiveTrapProblem : BinaryProblem {
    [StorableConstructor]
    protected DeceptiveTrapProblem(StorableConstructorFlag _) : base(_) { }
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
      Encoding.Length = 49;
    }

    // In the GECCO paper, calculates Equation 3
    protected virtual int Score(BinaryVector individual, int trapIndex, int trapSize) {
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

    public override double Evaluate(BinaryVector individual, IRandom random) {
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
