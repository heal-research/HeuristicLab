#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  /// <summary>
  /// A base class for stochastic selection operators which consider a single double quality value for selection.
  /// </summary>
  [Item("StochasticSingleObjectiveSelector", "A base class for stochastic selection operators which consider a single double quality value for selection.")]
  [EmptyStorableClass]
  public abstract class StochasticSingleObjectiveSelector : StochasticSelector, ISingleObjectiveSelector {
    public IValueLookupParameter<BoolData> MaximizationParameter {
      get { return (IValueLookupParameter<BoolData>)Parameters["Maximization"]; }
    }
    public ILookupParameter<ItemArray<DoubleData>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleData>>)Parameters["Quality"]; }
    }

    protected StochasticSingleObjectiveSelector()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolData>("Maximization", "True if the current problem is a maximization problem, otherwise false."));
      Parameters.Add(new SubScopesLookupParameter<DoubleData>("Quality", "The quality value contained in each sub-scope which is used for selection."));
    }
  }
}
