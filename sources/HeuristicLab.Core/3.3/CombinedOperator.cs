#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Xml;
using System.Drawing;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Collections;

namespace HeuristicLab.Core {
  /// <summary>
  /// The base class for all operators.
  /// </summary>
  [Item("CombinedOperator", "An operator which contains an operator graph.")]
  [Creatable("Test")]
  [EmptyStorableClass]
  public sealed class CombinedOperator : StandardOperatorBase, IOperator {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Module; }
    }

    public new ParameterCollection Parameters {
      get {
        return base.Parameters;
      }
    }
    IObservableKeyedCollection<string, IParameter> IOperator.Parameters {
      get { return Parameters; }
    }

    public CombinedOperator()
      : base() {
    }

    public override ExecutionContextCollection Apply(ExecutionContext context) {
      return base.Apply(context);
    }
  }
}
