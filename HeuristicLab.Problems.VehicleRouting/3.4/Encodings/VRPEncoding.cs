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
using HeuristicLab.Optimization;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings {
  [Item("VRPEncoding", "")]
  [StorableType("2bb0d7d5-d842-4cf7-8242-1d49940aa4b6")]
  public abstract class VRPEncoding : Encoding, IVRPEncoding {
    private static HashSet<Type> encodingOperatorTypes = new HashSet<Type>() { typeof(Alba.IAlbaOperator), typeof(GVR.IGVROperator),
      typeof(Potvin.IPotvinOperator), typeof(Prins.IPrinsOperator), typeof(Zhu.IZhuOperator) };
    public static IReadOnlyCollection<Type> EncodingOperatorTypes => encodingOperatorTypes.ToList().AsReadOnly();

    [StorableConstructor]
    protected VRPEncoding(StorableConstructorFlag _) : base(_) { }
    protected VRPEncoding(VRPEncoding original, Cloner cloner) : base(original, cloner) { }
    protected VRPEncoding(string name) : base(name) { }

    public void FilterOperators(IVRPProblemInstance instance) {
      DiscoverOperators();
      var operators = instance.FilterOperators(Operators).ToList();
      foreach (var op in operators.OfType<IMultiVRPOperator>().ToList()) {
        var subOps = instance.FilterOperators(op.Operators).ToList();
        if (subOps.Count == 0) operators.Remove(op);
        else {
          foreach (var dm in op.Operators.Except(subOps).ToList())
            op.RemoveOperator(dm);
        }
      }
      ReplaceOperators(operators);
    }

    protected abstract void DiscoverOperators();
  }
}
