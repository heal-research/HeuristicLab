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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("SingleDepotVRPProblemInstance", "Represents a single depot VRP instance.")]
  [StorableClass]
  public class SingleDepotVRPProblemInstance : VRPProblemInstance, ISingleDepotProblemInstance {
    protected override IEnumerable<IOperator> GetOperators() {
      return ApplicationManager.Manager.GetInstances<ISingleDepotOperator>().Cast<IOperator>();
    }

    protected override IEnumerable<IOperator> GetAnalyzers() {
      return ApplicationManager.Manager.GetInstances<ISingleDepotOperator>()
        .Where(o => o is IAnalyzer)
        .Cast<IOperator>();
    }

    public override IntValue Cities {
      get {
        return new IntValue(Demand.Length - 1);
      }
    }

    protected override IVRPEvaluator Evaluator {
      get {
        return new SingleDepotVRPEvaluator();
      }
    }

    protected override IVRPCreator Creator {
      get {
        return new HeuristicLab.Problems.VehicleRouting.Encodings.Alba.RandomCreator();
      }
    }

    [StorableConstructor]
    protected SingleDepotVRPProblemInstance(bool deserializing) : base(deserializing) { }

    public SingleDepotVRPProblemInstance() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleDepotVRPProblemInstance(this, cloner);
    }

    protected SingleDepotVRPProblemInstance(SingleDepotVRPProblemInstance original, Cloner cloner)
      : base(original, cloner) {
    }
  }
}