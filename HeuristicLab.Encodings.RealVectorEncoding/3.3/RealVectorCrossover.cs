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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// A base class for operators that perform a crossover of real-valued vectors.
  /// </summary>
  [Item("RealVectorCrossover", "A base class for operators that perform a crossover of real-valued vectors.")]
  [StorableType("4E51D1FF-056B-4A4D-8D90-3EB640EAF44A")]
  public abstract class RealVectorCrossover : InstrumentedOperator, IRealVectorCrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<ItemArray<RealVector>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<RealVector>)Parameters["Parents"]; }
    }
    public ILookupParameter<RealVector> ChildParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Child"]; }
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
    protected RealVectorCrossover(StorableConstructorFlag _) : base(_) { }
    protected RealVectorCrossover(RealVectorCrossover original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    protected RealVectorCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic crossover operators."));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("Parents", "The parent vectors which should be crossed."));
      ParentsParameter.ActualName = "RealVector";
      Parameters.Add(new LookupParameter<RealVector>("Child", "The child vector resulting from the crossover."));
      ChildParameter.ActualName = "RealVector";
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
      RealVector result = Cross(RandomParameter.ActualValue, ParentsParameter.ActualValue);
      ChildParameter.ActualValue = result;

      IOperation successor = base.InstrumentedApply();
      if (BoundsChecker != null) {
        IOperation checkerOperation = ExecutionContext.CreateChildOperation(BoundsChecker);
        if (successor == null) return checkerOperation;
        else return new OperationCollection(checkerOperation, successor);
      } else return successor;
    }

    protected abstract RealVector Cross(IRandom random, ItemArray<RealVector> parents);

    protected virtual void BoundsCheckerParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeBoundsChecker();
    }

    protected virtual void ParameterizeBoundsChecker() {
      if (BoundsChecker != null) {
        BoundsChecker.BoundsParameter.ActualName = BoundsParameter.Name;
        BoundsChecker.RealVectorParameter.ActualName = ChildParameter.Name;
      }
    }
  }
}
