#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// A base class for operators which evaluate TSP solutions.
  /// </summary>
  [Item("TSPEvaluator", "A base class for operators which evaluate TSP solutions.")]
  [StorableClass]
  public abstract class TSPEvaluator : SingleSuccessorOperator, ITSPEvaluator {
    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    [StorableConstructor]
    protected TSPEvaluator(bool deserializing) : base(deserializing) { }
    protected TSPEvaluator(TSPEvaluator original, Cloner cloner) : base(original, cloner) { }
    protected TSPEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The evaluated quality of the TSP solution."));
    }
  }
}
