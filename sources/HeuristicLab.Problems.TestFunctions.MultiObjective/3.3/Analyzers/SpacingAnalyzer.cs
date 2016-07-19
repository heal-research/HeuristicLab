#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableClass]
  [Item("SpacingAnalyzer", "The spacing of the current front (see Multi-Objective Performance Metrics - Shodhganga for more information)")]
  public class SpacingAnalyzer : MOTFAnalyzer {

    public IResultParameter<DoubleValue> SpacingResultParameter {
      get { return (IResultParameter<DoubleValue>)Parameters["Spacing"]; }
    }
    [StorableConstructor]
    protected SpacingAnalyzer(bool deserializing) : base(deserializing) { }


    protected SpacingAnalyzer(SpacingAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SpacingAnalyzer(this, cloner);
    }

    public SpacingAnalyzer() {
      Parameters.Add(new ResultParameter<DoubleValue>("Spacing", "The spacing of the current front"));
      SpacingResultParameter.DefaultValue = new DoubleValue(double.NaN);
    }

    public override IOperation Apply() {
      var qualities = QualitiesParameter.ActualValue;
      var spacing = Spacing.Calculate(qualities.Select(q => q.ToArray()));
      SpacingResultParameter.ActualValue.Value = spacing;

      return base.Apply();
    }
  }
}
