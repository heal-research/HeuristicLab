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
  [Item("MultiVRPSolutionManipulator", "Randomly selects and applies one of its manipulators every time it is called.")]
  [StorableClass]
  public class MultiVRPSolutionManipulator : StochasticMultiBranch<IVRPManipulator>, IVRPManipulator, IGeneralVRPOperator, IMultiVRPOperator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }


    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (LookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }

    [StorableConstructor]
    protected MultiVRPSolutionManipulator(bool deserializing) : base(deserializing) { }
    public MultiVRPSolutionManipulator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The VRP tours to be manipulated."));

      SelectedOperatorParameter.ActualName = "SelectedManipulationOperator";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiVRPSolutionManipulator(this, cloner);
    }

    protected MultiVRPSolutionManipulator(MultiVRPSolutionManipulator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeManipulators();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeManipulators();
    }

    public void SetOperators(IEnumerable<IOperator> operators) {
      foreach (IOperator op in operators) {
        if (op is IVRPManipulator && !(op is MultiVRPSolutionManipulator)) {
          Operators.Add(op.Clone() as IVRPManipulator, !(op is IVRPLocalSearchManipulator));
        }
      }
    }

    private void ParameterizeManipulators() {
      foreach (IVRPManipulator manipulator in Operators.OfType<IVRPManipulator>()) {
        manipulator.VRPToursParameter.ActualName = VRPToursParameter.Name;
        manipulator.ProblemInstanceParameter.ActualName = ProblemInstanceParameter.Name;
      }
      foreach (IStochasticOperator manipulator in Operators.OfType<IStochasticOperator>()) {
        manipulator.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation InstrumentedApply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one permutation manipulator to choose from.");
      return base.InstrumentedApply();
    }
  }
}
