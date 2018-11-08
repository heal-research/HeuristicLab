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

  // Enumeration of parameters that take integer or categorical values.
  public enum IntegerParam {

    // Advanced usage: presolve mode.
    PRESOLVE = 1000,

    // Algorithm to solve linear programs.
    LP_ALGORITHM = 1001,

    // Advanced usage: incrementality from one solve to the next.
    INCREMENTALITY = 1002,

    // Advanced usage: enable or disable matrix scaling.
    SCALING = 1003
  };
}
