#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [StorableType("c54d5084-fee2-4c94-be62-924b014d180d")]
  public interface IRunCollectionConstraint : IConstraint {
    new RunCollection ConstrainedValue { get; set; }
  }

  [StorableType("9e35dfa2-f197-4212-841f-d52e91a297e0")]
  public interface IRunCollectionColumnConstraint : IRunCollectionConstraint {
    string ConstraintColumn { get; set; }
    event EventHandler ConstraintColumnChanged;
  }
}
