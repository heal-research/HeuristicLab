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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("MultiVRPSolutionCreator", "Randomly selects and applies one of its creator every time it is called.")]
  [StorableClass]
  public class MultiVRPSolutionCreator : StochasticMultiBranch<IVRPCreator>, IVRPCreator, IGeneralVRPOperator, IMultiVRPOperator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (LookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }

    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }

    [StorableConstructor]
    protected MultiVRPSolutionCreator(bool deserializing) : base(deserializing) { }
    public MultiVRPSolutionCreator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The new VRP tours."));

      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiVRPSolutionCreator(this, cloner);
    }

    protected MultiVRPSolutionCreator(MultiVRPSolutionCreator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPCreator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeCreators();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPCreator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeCreators();
    }

    public void SetOperators(IEnumerable<IOperator> operators) {
      foreach (IOperator op in operators) {
        if (op is IVRPCreator && !(op is MultiVRPSolutionCreator)) {
          Operators.Add(op.Clone() as IVRPCreator, true);
        }
      }
    }

    private void ParameterizeCreators() {
      foreach (IVRPCreator creator in Operators.OfType<IVRPCreator>()) {
        creator.VRPToursParameter.ActualName = VRPToursParameter.Name;
        creator.ProblemInstanceParameter.ActualName = ProblemInstanceParameter.Name;
      }
      foreach (IStochasticOperator creator in Operators.OfType<IStochasticOperator>()) {
        creator.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation InstrumentedApply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one VRP creator to choose from.");
      return base.InstrumentedApply();
    }
  }
}
