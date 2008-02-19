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

namespace HeuristicLab.Constraints {
  public interface IConstraintVisitor {
    void Visit(AndConstraint constraint);

    void Visit(OrConstraint constraint);

    void Visit(NotConstraint constraint);

    void Visit(TrueConstraint constraint);

    void Visit(FalseConstraint constraint);

    void Visit(IsIntegerConstraint constraint);

    void Visit(DoubleBoundedConstraint constraint);

    void Visit(IntBoundedConstraint constraint);

    void Visit(NumberOfSubOperatorsConstraint constraint);

    void Visit(SubOperatorTypeConstraint constraint);

    void Visit(AllSubOperatorsTypeConstraint constraint);

    void Visit(ItemTypeConstraint constraint);
  }
}
