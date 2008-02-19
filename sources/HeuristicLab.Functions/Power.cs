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
using HeuristicLab.Core;
using System.Diagnostics;
using HeuristicLab.Constraints;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Functions {
  public class Power : FunctionBase {
    public override string Description {
      get { return "Returns the result of the first sub-operator to the power of the second sub-operator (power(x, y))."; }
    }

    public Power()
      : base() {
      // must have exactly 2 suboperators base ^ exponent
      AddConstraint(new NumberOfSubOperatorsConstraint(2, 2));
    }

    public Power(Power source, IDictionary<Guid, object> clonedObjects)
      : base(source, clonedObjects) {
    }


    public override double Evaluate(Dataset dataset, int sampleIndex) {
      return Math.Pow(SubFunctions[0].Evaluate(dataset, sampleIndex), SubFunctions[1].Evaluate(dataset, sampleIndex));
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Power clone = new Power(this, clonedObjects);
      clonedObjects.Add(clone.Guid, clone);
      return clone;
    }


    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
