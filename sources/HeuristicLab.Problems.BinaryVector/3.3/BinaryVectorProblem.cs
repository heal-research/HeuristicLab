#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.BinaryVector {
  [StorableClass]
  public abstract class BinaryVectorProblem : Problem, IBinaryVectorProblem {
    private const string LengthParameterName = "Length";

    public IFixedValueParameter<IntValue> LengthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[LengthParameterName]; }
    }

    public int Length {
      get { return LengthParameter.Value.Value; }
      set { LengthParameter.Value.Value = value; }
    }

    public abstract bool Maximization {
      get;
    }

    [StorableConstructor]
    protected BinaryVectorProblem(bool deserializing) : base(deserializing) { }
    protected BinaryVectorProblem(BinaryVectorProblem original, Cloner cloner) : base(original, cloner) { }
    public bool IsBetter(double quality, double bestQuality) {
      return (Maximization && quality > bestQuality || !Maximization && quality < bestQuality);
    }

    public BinaryVectorProblem()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(LengthParameterName, "", new IntValue(20)));
    }

    public abstract double Evaluate(bool[] individual);


  }
}
