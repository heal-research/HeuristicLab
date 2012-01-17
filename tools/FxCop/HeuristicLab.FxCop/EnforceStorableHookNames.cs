#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using Microsoft.FxCop.Sdk;

namespace HeuristicLab.FxCop {
  internal sealed class EnforceStorableHookNames : HeuristicLabFxCopRuleBase {
    public EnforceStorableHookNames()
      : base("EnforceStorableHookNames") { }

    public override ProblemCollection Check(Member member) {
      Method method = member as Method;
      if (method == null) return null;

      var attribute = method.Attributes.FirstOrDefault(x => x.Type.FullName == "HeuristicLab.Persistence.Default.CompositeSerializers.Storable.StorableHookAttribute");
      if (attribute != null && attribute.Expressions.Count > 0) {
        var literal = (Literal)attribute.Expressions[0];
        if (literal != null && literal.Value is int) {
          int hookType = (int)literal.Value;
          if (hookType == 1 && method.Name.Name != "AfterDeserialization") {
            Problems.Add(new Problem(GetResolution(method.FullName, "AfterDeserialization")));
          } else if (hookType == 0 && method.Name.Name != "BeforeSerialization") {
            Problems.Add(new Problem(GetResolution(method.FullName, "BeforeSerialization")));
          }
        }
      }
      return Problems;
    }
  }
}