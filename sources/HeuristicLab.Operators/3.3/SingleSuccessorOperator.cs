#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// A base class for operators which have only one successor.
  /// </summary>
  [Item("SingleSuccessorOperator", "A base class for operators which have only one successor.")]
  [StorableClass]
  public abstract class SingleSuccessorOperator : Operator {
    protected OperatorParameter SuccessorParameter {
      get { return (OperatorParameter)Parameters["Successor"]; }
    }
    public IOperator Successor {
      get { return SuccessorParameter.Value; }
      set { SuccessorParameter.Value = value; }
    }

    [StorableConstructor]
    protected SingleSuccessorOperator(bool deserializing) : base(deserializing) { }
    protected SingleSuccessorOperator(SingleSuccessorOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public SingleSuccessorOperator()
      : base() {
      Parameters.Add(new OperatorParameter("Successor", "Operator which is executed next."));
      SuccessorParameter.Hidden = true;
    }

    public override IOperation Apply() {
      if (Successor != null)
        return ExecutionContext.CreateOperation(Successor);
      else
        return null;
    }
  }
}
