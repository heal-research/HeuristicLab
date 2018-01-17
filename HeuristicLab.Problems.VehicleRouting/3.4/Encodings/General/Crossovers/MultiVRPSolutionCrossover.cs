#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("MultiVRPSolutionCrossover", "Randomly selects and applies one of its crossovers every time it is called.")]
  [StorableClass]
  public class MultiVRPSolutionCrossover : StochasticMultiBranch<IVRPCrossover>, IVRPCrossover, IGeneralVRPOperator, IMultiVRPOperator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (LookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }

    public ILookupParameter<ItemArray<IVRPEncoding>> ParentsParameter {
      get { return (ScopeTreeLookupParameter<IVRPEncoding>)Parameters["Parents"]; }
    }

    public ILookupParameter<IVRPEncoding> ChildParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["Child"]; }
    }


    [StorableConstructor]
    protected MultiVRPSolutionCrossover(bool deserializing) : base(deserializing) { }
    public MultiVRPSolutionCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));

      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncoding>("Parents", "The parent permutations which should be crossed."));
      ParentsParameter.ActualName = "VRPTours";
      Parameters.Add(new LookupParameter<IVRPEncoding>("Child", "The child permutation resulting from the crossover."));
      ChildParameter.ActualName = "VRPTours";

      SelectedOperatorParameter.ActualName = "SelectedCrossoverOperator";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiVRPSolutionCrossover(this, cloner);
    }

    protected MultiVRPSolutionCrossover(MultiVRPSolutionCrossover original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPCrossover>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeCrossovers();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPCrossover>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeCrossovers();
    }

    public void SetOperators(IEnumerable<IOperator> operators) {
      foreach (IOperator op in operators) {
        if (op is IVRPCrossover && !(op is MultiVRPSolutionCrossover)) {
          Operators.Add(op.Clone() as IVRPCrossover, true);
        }
      }
    }

    private void ParameterizeCrossovers() {
      foreach (IVRPCrossover crossover in Operators.OfType<IVRPCrossover>()) {
        crossover.ChildParameter.ActualName = ChildParameter.Name;
        crossover.ParentsParameter.ActualName = ParentsParameter.Name;
        crossover.ProblemInstanceParameter.ActualName = ProblemInstanceParameter.Name;
      }
      foreach (IStochasticOperator crossover in Operators.OfType<IStochasticOperator>()) {
        crossover.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation InstrumentedApply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one permutation crossover to choose from.");
      return base.InstrumentedApply();
    }
  }
}
