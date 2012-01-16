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
  internal sealed class EnforceAfterDeserializationHookName : HeuristicLabFxCopRuleBase {
    public EnforceAfterDeserializationHookName()
      : base("EnforceAfterDeserializationHookName") { }

    public override ProblemCollection Check(Member member) {
      Method method = member as Method;
      if (method == null) return null;

      if (method.Attributes.Any(x => x.Type.FullName == "HeuristicLab.Persistence.Default.CompositeSerializers.Storable.StorableHookAttribute")
        && method.Name.Name != "AfterDeserialization") {
        var resolution = GetResolution(method.FullName);
        Problems.Add(new Problem(resolution));
      }

      return Problems;
    }
  }
}