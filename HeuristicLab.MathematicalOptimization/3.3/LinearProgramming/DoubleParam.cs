#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.MathematicalOptimization.LinearProgramming {

  // Enumeration of parameters that take continuous values.
  public enum DoubleParam {

    // Limit for relative MIP gap.
    RELATIVE_MIP_GAP = 0,

    // Advanced usage: tolerance for primal feasibility of basic
    // solutions. This does not control the integer feasibility
    // tolerance of integer solutions for MIP or the tolerance used
    // during presolve.
    PRIMAL_TOLERANCE = 1,

    // Advanced usage: tolerance for dual feasibility of basic solutions.
    DUAL_TOLERANCE = 2
  };
}
