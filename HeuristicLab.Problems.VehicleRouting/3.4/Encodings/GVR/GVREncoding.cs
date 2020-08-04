#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.GVR {
  [Item("GVR Encoding", "Represents the genetic vehicle representation encoding for GVR encoded solutions.")]
  [StorableType("e868041e-0c91-4734-940a-e339ec9dca89")]
  public sealed class GVREncoding : VRPEncoding {

    [StorableConstructor]
    private GVREncoding(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      DiscoverOperators();
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new GVREncoding(this, cloner); }
    private GVREncoding(GVREncoding original, Cloner cloner) : base(original, cloner) { }


    public GVREncoding() : this("VRPTours") { }
    public GVREncoding(string name) : base(name) {
      DiscoverOperators();
    }

    #region Operator Discovery
    private static readonly IEnumerable<Type> encodingSpecificOperatorTypes;
    static GVREncoding() {
      encodingSpecificOperatorTypes = new List<Type>() {
          typeof (IGVROperator),
          typeof (IVRPCreator),
          typeof (IMultiVRPOperator)
      };
    }
    protected override void DiscoverOperators() {
      var assembly = typeof(IGVROperator).Assembly;
      var discoveredTypes = ApplicationManager.Manager.GetTypes(encodingSpecificOperatorTypes, assembly, true, false, false);
      var operators = discoveredTypes.Select(t => (IOperator)Activator.CreateInstance(t));
      var newOperators = operators.Except(Operators, new TypeEqualityComparer<IOperator>()).ToList();

      foreach (var op in newOperators.OfType<IMultiVRPOperator>().ToList()) {
        op.SetOperators(Operators.Concat(newOperators));
        if (!op.Operators.Any()) newOperators.Remove(op);
      }
      foreach (var op in Operators.OfType<IMultiVRPOperator>()) {
        op.SetOperators(newOperators);
      }
      ConfigureOperators(newOperators);
      foreach (var @operator in newOperators)
        AddOperator(@operator);
    }
    #endregion

    public override void ConfigureOperators(IEnumerable<IItem> operators) {
      base.ConfigureOperators(operators);
    }

    #region specific operator wiring

    #endregion
  }
}
