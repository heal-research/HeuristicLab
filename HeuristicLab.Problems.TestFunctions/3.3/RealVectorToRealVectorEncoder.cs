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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions {
  // BackwardsCompatibility3.3
  #region Backwards compatible code (remove with 3.4)
  [Obsolete("This operator should not be used anymore.")]
  internal class RealVectorToRealVectorEncoder : SingleSuccessorOperator {

    [StorableConstructor]
    protected RealVectorToRealVectorEncoder(bool deserializing) : base(deserializing) { }
    protected RealVectorToRealVectorEncoder(RealVectorToRealVectorEncoder original, Cloner cloner) : base(original, cloner) { }
    public RealVectorToRealVectorEncoder()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVectorToRealVectorEncoder(this, cloner);
    }

    public override IOperation Apply() {
      return base.Apply();
    }

    public override bool CanChangeName {
      get { return false; }
    }
  }
  #endregion
}
