#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A base class for stochastic selection operators.
  /// </summary>
  [Item("StochasticSelector", "A base class for stochastic selection operators.")]
  [StorableType("311180FE-97EE-48FF-A4C0-40982D6B862A")]
  public abstract class StochasticSelector : Selector, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected StochasticSelector(StorableConstructorFlag _) : base(_) { }
    protected StochasticSelector(StochasticSelector original, Cloner cloner) : base(original, cloner) { }
    protected StochasticSelector()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator used for stochastic selection."));
    }
  }
}
