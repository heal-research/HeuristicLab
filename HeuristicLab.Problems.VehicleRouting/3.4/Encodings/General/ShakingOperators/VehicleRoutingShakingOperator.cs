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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("VRPShakingOperator", "A shaking operator for VNS that applies available mutation operators.")]
  [StorableClass]
  public class VehicleRoutingShakingOperator : ShakingOperator<IVRPManipulator>, IVRPMultiNeighborhoodShakingOperator, IGeneralVRPOperator, IStochasticOperator {
    #region Parameters
    public ILookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ILookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (LookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }

    public IVRPProblemInstance ProblemInstance {
      get { return ProblemInstanceParameter.ActualValue; }
    }

    #endregion

    [StorableConstructor]
    protected VehicleRoutingShakingOperator(bool deserializing) : base(deserializing) { }
    protected VehicleRoutingShakingOperator(VehicleRoutingShakingOperator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new VehicleRoutingShakingOperator(this, cloner);
    }
    public VehicleRoutingShakingOperator()
      : base() {
      Parameters.Add(new LookupParameter<IVRPEncoding>("VRPTours", "The vrp tour encoding to shake."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator that will be used for stochastic shaking operators."));
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance"));
     
      foreach (IVRPManipulator shaker in ApplicationManager.Manager.GetInstances<IVRPManipulator>().OrderBy(x => x.Name))
        if (!(shaker is MultiVRPSolutionManipulator)) Operators.Add(shaker, !(shaker is IVRPLocalSearchManipulator));
    }

    #region Wiring of some parameters
    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeOperators(e.Items);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IVRPManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeOperators(e.Items);
    }

    private void ParameterizeOperators(IEnumerable<IndexedItem<IVRPManipulator>> items) {
      if (items.Any()) {
        foreach (IStochasticOperator op in items.Select(x => x.Value).OfType<IStochasticOperator>())
          op.RandomParameter.ActualName = RandomParameter.Name;
        foreach (IVRPManipulator op in items.Select(x => x.Value).OfType<IVRPManipulator>()) {
          op.VRPToursParameter.ActualName = VRPToursParameter.Name;
          if (op.ProblemInstanceParameter != null) op.ProblemInstanceParameter.ActualName = ProblemInstanceParameter.Name;
        }
      }
    }
    #endregion
  }
}
