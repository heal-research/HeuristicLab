#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// A base class for operators creating int-valued vectors.
  /// </summary>
  [Item("IntegerVectorCreator", "A base class for operators creating int-valued vectors.")]
  [StorableClass]
  public abstract class IntegerVectorCreator : InstrumentedOperator, IIntegerVectorCreator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<IntegerVector> IntegerVectorParameter {
      get { return (ILookupParameter<IntegerVector>)Parameters["IntegerVector"]; }
    }
    public IValueLookupParameter<IntValue> LengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Length"]; }
    }
    public IValueLookupParameter<IntMatrix> BoundsParameter {
      get { return (IValueLookupParameter<IntMatrix>)Parameters["Bounds"]; }
    }

    [StorableConstructor]
    protected IntegerVectorCreator(bool deserializing) : base(deserializing) { }
    protected IntegerVectorCreator(IntegerVectorCreator original, Cloner cloner) : base(original, cloner) { }
    protected IntegerVectorCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<IntegerVector>("IntegerVector", "The vector which should be manipulated."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Length", "The length of the vector."));
      Parameters.Add(new ValueLookupParameter<IntMatrix>("Bounds", "The bounds matrix can contain one row for each dimension with three columns specifying minimum (inclusive), maximum (exclusive), and step size. If less rows are given the matrix is cycled."));
    }

    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("Bounds")) {
        var min = ((IValueLookupParameter<IntValue>)Parameters["Minimum"]).Value as IntValue;
        var max = ((IValueLookupParameter<IntValue>)Parameters["Maximum"]).Value as IntValue;
        Parameters.Remove("Minimum");
        Parameters.Remove("Maximum");
        Parameters.Add(new ValueLookupParameter<IntMatrix>("Bounds", "The bounds matrix can contain one row for each dimension with three columns specifying minimum (inclusive), maximum (exclusive), and step size. If less rows are given the matrix is cycled."));
        if (min != null && max != null) {
          BoundsParameter.Value = new IntMatrix(new int[,] { { min.Value, max.Value, 1 } });
        }
      }
    }
    #endregion

    public sealed override IOperation InstrumentedApply() {
      IntegerVectorParameter.ActualValue = Create(RandomParameter.ActualValue, LengthParameter.ActualValue, BoundsParameter.ActualValue);
      return base.InstrumentedApply();
    }

    protected abstract IntegerVector Create(IRandom random, IntValue length, IntMatrix bounds);
  }
}
