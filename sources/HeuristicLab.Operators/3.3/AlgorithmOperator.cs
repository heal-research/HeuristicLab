#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Drawing;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which represents an algorithm represented as an operator graph.
  /// </summary>
  [Item("AlgorithmOperator", "An operator which represents an algorithm represented as an operator graph.")]
  public abstract class AlgorithmOperator : SingleSuccessorOperator {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Module; }
    }
    [Storable]
    private OperatorGraph operatorGraph;
    public OperatorGraph OperatorGraph {
      get { return operatorGraph; }
    }

    protected AlgorithmOperator()
      : base() {
      operatorGraph = new OperatorGraph();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      AlgorithmOperator clone = (AlgorithmOperator)base.Clone(cloner);
      clone.operatorGraph = (OperatorGraph)cloner.Clone(operatorGraph);
      return clone;
    }

    public override IOperation Apply() {
      OperationCollection next = new OperationCollection(base.Apply());
      if (operatorGraph.InitialOperator != null)
        next.Insert(0, ExecutionContext.CreateChildOperation(operatorGraph.InitialOperator));
      return next;
    }
  }
}
