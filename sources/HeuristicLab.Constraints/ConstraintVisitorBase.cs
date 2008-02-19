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

namespace HeuristicLab.Constraints {
  public class ConstraintVisitorBase : IConstraintVisitor {
    public virtual void Visit(AndConstraint constraint) {
    }

    public virtual void Visit(OrConstraint constraint) {
    }

    public virtual void Visit(NotConstraint constraint) {
    }

    public virtual void Visit(TrueConstraint constraint) {
    }

    public virtual void Visit(FalseConstraint constraint) {
    }

    public virtual void Visit(IsIntegerConstraint constraint) {
    }

    public virtual void Visit(DoubleBoundedConstraint constraint) {
    }

    public virtual void Visit(IntBoundedConstraint constraint) {
    }

    public virtual void Visit(NumberOfSubOperatorsConstraint constraint) {
    }

    public virtual void Visit(SubOperatorTypeConstraint constraint) {
    }

    public virtual void Visit(AllSubOperatorsTypeConstraint constraint) {
    }

    public virtual void Visit(ItemTypeConstraint constraint) {
    }
  }
}
