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

namespace HeuristicLab.Optimization {
  /// <summary>
  /// This is only a marker interface to prevent operators that expect to work with a single quality value
  /// and/or a single maximization flag to appear in multi-objective algorithms.
  /// </summary>
  /// <remarks>
  /// Marker interfaces are typically not a good design, however specifying a reasonable non-empty common
  /// interface for all single-objective operators is difficult. There are many different variants of
  /// single-ojective operators with specific needs that can currently only be abstracted by an empty interface.
  /// In the following, please find a list of different kinds of single-objective operators.
  /// <list type="bullet">
  ///   <item>
  ///     <term>Evaluators</term>
  ///     <description>These only need a quality parameter, but not a maximization parameter.</description>
  ///   </item>
  ///   <item>
  ///     <term>Population-based operators</term>
  ///     <description>These require the quality parameter to be a scope-tree lookup, e.g. analyzers, crossovers, single-objective replacers and selectors.</description>
  ///   </item>
  ///   <item>
  ///     <term>Problem-specific operators</term>
  ///     <description>They may perform multiple operations on a solution, evaluate and select without taking into account maximization or the evaluator, e.g. many improvement operators.</description>
  ///   </item>
  ///   <item>
  ///     <term>Other single-objective operators</term>
  ///     <description>They would need both a quality, and a maximization parameter, and may be population-based or not.</description>
  ///   </item>
  /// </list>
  /// </remarks>
  public interface ISingleObjectiveOperator { }
}
