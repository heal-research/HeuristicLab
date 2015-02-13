#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Base class for threshold calculators for discriminant function classification models.
  /// </summary>
  [StorableClass]
  public abstract class ThresholdCalculator : NamedItem, IDiscriminantFunctionThresholdCalculator {

    [StorableConstructor]
    protected ThresholdCalculator(bool deserializing) : base(deserializing) { }
    protected ThresholdCalculator(ThresholdCalculator original, Cloner cloner)
      : base(original, cloner) {
    }
    public ThresholdCalculator()
      : base() {
      this.name = ItemName;
      this.description = ItemDescription;
    }

    public abstract void Calculate(IClassificationProblemData problemData, IEnumerable<double> estimatedValues, IEnumerable<double> targetClassValues, out double[] classValues, out double[] thresholds);
  }
}
