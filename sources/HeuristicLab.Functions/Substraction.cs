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
using System.Linq;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Functions {
  public class Substraction : FunctionBase {
    public override string Description {
      get {
        return @"Substracts the results of sub-operators 2..n from the result of the first sub-operator.
    (- 3) => -3
    (- 2 3) => -1
    (- 3 4 5) => -6";
      }
    }

    public Substraction()
      : base() {
      // 2 - 3 seems like an reasonable defaut (used for +,-,*,/) (discussion with swinkler and maffenze)
      AddConstraint(new NumberOfSubOperatorsConstraint(2, 3));
    }

    public Substraction(Substraction source, IDictionary<Guid, object> clonedObjects)
      : base(source, clonedObjects) {
    }


    public override double Evaluate(Dataset dataset, int sampleIndex) {

      if(SubFunctions.Count == 1) {
        return -SubFunctions[0].Evaluate(dataset, sampleIndex);
      } else {
        double result = SubFunctions[0].Evaluate(dataset, sampleIndex);
        for(int i = 1; i < SubFunctions.Count; i++) {
          result -= SubFunctions[i].Evaluate(dataset, sampleIndex);
        }
        return result;
      }
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Substraction clone = new Substraction(this, clonedObjects);
      clonedObjects.Add(clone.Guid, clone);
      return clone;
    }

    public override void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
