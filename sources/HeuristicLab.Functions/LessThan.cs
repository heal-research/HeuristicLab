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
using System.Linq;
using System.Text;
using HeuristicLab.Constraints;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Functions {
  public class LessThan : FunctionBase {
    public override string Description {
      get {
        return @"Less-than condition. Returns 1.0 if the value of the first sub-function is less than the value of the second sub-function and 0.0 otherwise.";
      }
    }

    public LessThan()
      : base() {
      AddConstraint(new NumberOfSubOperatorsConstraint(2, 2));
    }

    public LessThan(LessThan source, IDictionary<Guid, object> clonedObjects)
      : base(source, clonedObjects) {
    }


    public override double Evaluate(Dataset dataset, int sampleIndex) {
      if(SubFunctions[0].Evaluate(dataset, sampleIndex) < SubFunctions[1].Evaluate(dataset, sampleIndex)) return 1.0;
      else return 0.0;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      LessThan clone = new LessThan(this, clonedObjects);
      clonedObjects.Add(clone.Guid, clone);
      return clone;
    }

    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
