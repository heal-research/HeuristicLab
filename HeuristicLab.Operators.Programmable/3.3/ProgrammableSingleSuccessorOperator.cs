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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators.Programmable {

  [Item("ProgrammableSingleSuccessorOperator", "An operator that can be programmed for arbitrary needs and handle a single successor.")]
  [StorableClass]
  public class ProgrammableSingleSuccessorOperator : ProgrammableOperator {

    public IOperator Successor {
      get {
        IParameter parameter;
        Parameters.TryGetValue("Successor", out parameter);
        OperatorParameter successorParameter = parameter as OperatorParameter;
        if (successorParameter == null)
          return null;
        return successorParameter.Value;
      }
      set {
        ((OperatorParameter)Parameters["Successor"]).Value = value;
      }
    }

    [StorableConstructor]
    protected ProgrammableSingleSuccessorOperator(bool deserializing) : base(deserializing) { }
    protected ProgrammableSingleSuccessorOperator(ProgrammableSingleSuccessorOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public ProgrammableSingleSuccessorOperator()
      : base() {
      Parameters.Add(new OperatorParameter("Successor", "Operator that is executed next."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProgrammableSingleSuccessorOperator(this, cloner);
    }

    public override string MethodSuffix {
      get { return "return op.Successor == null ? null : context.CreateOperation(op.Successor);"; }
    }
  }
}
