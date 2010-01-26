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
using System.Drawing;
using System.Text;
using System.Xml;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Operator which contains an operator graph.
  /// </summary>
  [Item("CombinedOperator", "An operator which contains an operator graph.")]
  [Creatable("Test")]
  public sealed class CombinedOperator : StandardOperator, IOperator {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Module; }
    }
    [Storable]
    private OperatorGraph operatorGraph;
    public OperatorGraph OperatorGraph {
      get { return operatorGraph; }
    }
    public new ParameterCollection Parameters {
      get {
        return base.Parameters;
      }
    }
    IObservableKeyedCollection<string, IParameter> IOperator.Parameters {
      get { return Parameters; }
    }
    public override bool CanChangeDescription {
      get { return true; }
    }

    public CombinedOperator()
      : base() {
      operatorGraph = new OperatorGraph();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      CombinedOperator clone = (CombinedOperator)base.Clone(cloner);
      clone.operatorGraph = (OperatorGraph)cloner.Clone(operatorGraph);
      return base.Clone(cloner);
    }

    public override ExecutionContextCollection Apply(ExecutionContext context) {
      ExecutionContextCollection next = base.Apply(context);
      if (operatorGraph.InitialOperator != null)
        next.Insert(0, new ExecutionContext(context, operatorGraph.InitialOperator, context.Scope));
      return next;
    }
  }
}
