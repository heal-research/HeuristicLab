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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Operators {
  [Item("LocalRandomCreator", "Creates a local pseudo random number generator from a global random number generator.")]
  [StorableType("C1E56840-D71E-46F0-A964-AB8E9340333C")]
  public sealed class LocalRandomCreator : SingleSuccessorOperator {
    #region Parameter Properties
    public LookupParameter<IRandom> GlobalRandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["GlobalRandom"]; }
    }
    public LookupParameter<IRandom> LocalRandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["LocalRandom"]; }
    }
    #endregion

    [StorableConstructor]
    private LocalRandomCreator(StorableConstructorFlag _) : base(_) { }
    private LocalRandomCreator(LocalRandomCreator original, Cloner cloner)
      : base(original, cloner) {
    }
    public LocalRandomCreator()
      : base() {
      #region Create parameters
      Parameters.Add(new LookupParameter<IRandom>("GlobalRandom", "The global random number generator which is used to initialize the local random number generator."));
      Parameters.Add(new LookupParameter<IRandom>("LocalRandom", "The local pseudo random number generator."));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LocalRandomCreator(this, cloner);
    }

    public override IOperation Apply() {
      IRandom globalRandom = GlobalRandomParameter.ActualValue;
      IRandom localRandom = (IRandom)globalRandom.Clone();
      localRandom.Reset(globalRandom.Next());
      LocalRandomParameter.ActualValue = localRandom;
      return base.Apply();
    }
  }
}
