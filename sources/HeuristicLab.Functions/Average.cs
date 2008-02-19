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
using HeuristicLab.DataAnalysis;
using HeuristicLab.Constraints;

namespace HeuristicLab.Functions {
  public class Average : FunctionBase {
    public override string Description {
      get {
        return @"Returns the average (arithmetic mean) of all sub-operator results.";
      }
    }

    public Average()
      : base() {
      AddConstraint(new NumberOfSubOperatorsConstraint(2, 3));
    }

    public Average(Average source, IDictionary<Guid, object> clonedObjects)
      : base(source, clonedObjects) {
    }


    public override double Evaluate(Dataset dataset, int sampleIndex) {
      double sum = 0.0;
      for(int i = 0; i < SubFunctions.Count; i++) {
        sum += SubFunctions[i].Evaluate(dataset, sampleIndex);
      }
      return sum / SubFunctions.Count;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Average clone = new Average(this, clonedObjects);
      clonedObjects.Add(clone.Guid, clone);
      return clone;
    }

    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
