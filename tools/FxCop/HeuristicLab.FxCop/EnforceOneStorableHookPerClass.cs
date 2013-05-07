#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  internal sealed class EnforceOneStorableHookPerClass : HeuristicLabFxCopRuleBase {
    public EnforceOneStorableHookPerClass()
      : base("EnforceOneStorableHookPerClass") { }

    public override ProblemCollection Check(TypeNode type) {
      var storableHooks = type.Members.Where(x => x.Attributes.Any(y => y.Type.FullName == "HeuristicLab.Persistence.Default.CompositeSerializers.Storable.StorableHookAttribute")).ToArray();
      if (storableHooks.Length < 2) return null;
      else {
        int afterDeserialization = 0;
        int beforeSerialization = 0;
        foreach (var hook in storableHooks) {
          var attribute = hook.Attributes.FirstOrDefault(x => x.Type.FullName == "HeuristicLab.Persistence.Default.CompositeSerializers.Storable.StorableHookAttribute");
          if (attribute != null && attribute.Expressions.Count > 0) {
            var literal = (Literal)attribute.Expressions[0];
            if (literal != null && literal.Value is int) {
              int hookType = (int)literal.Value;
              if (hookType == 1) afterDeserialization++;
              else if (hookType == 0) beforeSerialization++;
            }
          }
        }
        if (afterDeserialization > 1)
          Problems.Add(new Problem(GetResolution(type.Name.Name, afterDeserialization, "AfterDeserialization")));
        if (beforeSerialization > 1)
          Problems.Add(new Problem(GetResolution(type.Name.Name, beforeSerialization, "BeforeSerialization")));
      }
      return Problems;
    }
  }
}
