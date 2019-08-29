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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;

namespace HeuristicLab.Optimization {
  [Item("MultiEncoding Manipulator", "Applies different manipulators to change a multi-encoding.")]
  [StorableType("574D0530-47E8-4FD9-8AC8-B8EA2DE3C203")]
  internal sealed class MultiEncodingManipulator : MultiEncodingOperator<IManipulator>, IManipulator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public override string OperatorPrefix {
      get { return "Manipulator"; }
    }
    [StorableConstructor]
    private MultiEncodingManipulator(StorableConstructorFlag _) : base(_) { }
    private MultiEncodingManipulator(MultiEncodingManipulator original, Cloner cloner) : base(original, cloner) { }

    public MultiEncodingManipulator() : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator used by the individual operators."));
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new MultiEncodingManipulator(this, cloner); }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("Random")) {
        Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator used by the individual operators."));
      }
    }

  }
}
