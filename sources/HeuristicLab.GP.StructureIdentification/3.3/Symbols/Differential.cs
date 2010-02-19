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


using HeuristicLab.GP.Interfaces;
namespace HeuristicLab.GP.StructureIdentification {
  public sealed class Differential : Variable {

    public override string Description {
      get {
        return @"Differential returns the difference between the value of a variable at t and t-1. The weight is a coefficient that is multiplied to the the difference.";
      }
    }

    public Differential() : base() { }

    public override IFunctionTree GetTreeNode() {
      return new VariableFunctionTree(this);
    }

    public override HeuristicLab.Core.IView CreateView() {
      return new VariableView(this);
    }
  }
}
