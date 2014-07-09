#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// A base class for operators that manipulate real-valued vectors.
  /// </summary>
  [Item("RealVectorManipulator", "A base class for operators that manipulate real-valued vectors.")]
  [StorableClass]
  public abstract class RealVectorManipulator : InstrumentedOperator, IRealVectorManipulator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    public IValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }
    protected OptionalValueParameter<IRealVectorBoundsChecker> BoundsCheckerParameter {
      get { return (OptionalValueParameter<IRealVectorBoundsChecker>)Parameters["BoundsChecker"]; }
    }
    public IRealVectorBoundsChecker BoundsChecker {
      get { return BoundsCheckerParameter.Value; }
      set { BoundsCheckerParameter.Value = value; }
    }

    [StorableConstructor]
    protected RealVectorManipulator(bool deserializing) : base(deserializing) { }
    protected RealVectorManipulator(RealVectorManipulator original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    protected RealVectorManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The vector which should be manipulated."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds of the real vector."));
      Parameters.Add(new OptionalValueParameter<IRealVectorBoundsChecker>("BoundsChecker", "The bounds checker that ensures that the values stay within the bounds.", new BoundsChecker()));

      RegisterEventHandlers();
      ParameterizeBoundsChecker();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (!Parameters.ContainsKey("BoundsChecker")) {
        Parameters.Add(new OptionalValueParameter<IRealVectorBoundsChecker>("BoundsChecker", "The bounds checker that ensures that the values stay within the bounds.", new BoundsChecker()));
        ParameterizeBoundsChecker();
      }
      #endregion
      RegisterEventHandlers();
    }

    protected virtual void RegisterEventHandlers() {
      BoundsCheckerParameter.ValueChanged += new System.EventHandler(BoundsCheckerParameter_ValueChanged);
    }

    public sealed override IOperation InstrumentedApply() {
      RealVector vector = RealVectorParameter.ActualValue;
      Manipulate(RandomParameter.ActualValue, vector);

      IOperation successor = base.InstrumentedApply();
      if (BoundsChecker != null) {
        IOperation checkerOperation = ExecutionContext.CreateChildOperation(BoundsChecker);
        if (successor == null) return checkerOperation;
        else return new OperationCollection(checkerOperation, successor);
      } else return successor;
    }

    protected abstract void Manipulate(IRandom random, RealVector realVector);

    protected virtual void BoundsCheckerParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeBoundsChecker();
    }

    protected virtual void ParameterizeBoundsChecker() {
      if (BoundsChecker != null) {
        BoundsChecker.BoundsParameter.ActualName = BoundsParameter.Name;
        BoundsChecker.RealVectorParameter.ActualName = RealVectorParameter.Name;
      }
    }
  }
}
