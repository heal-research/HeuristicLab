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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HEAL.Attic;

namespace HeuristicLab.Analysis {
  [StorableType("6f09f028-039d-4da8-9585-2575f9aef074")]
  [Item("CrowdingAnalyzer", "The mean crowding distance for each point of the Front (see Multi-Objective Performance Metrics - Shodhganga for more information)")]
  public class CrowdingAnalyzer : MultiObjectiveSuccessAnalyzer {
    public override string ResultName {
      get { return "Crowding"; }
    }

    [StorableConstructor]
    protected CrowdingAnalyzer(StorableConstructorFlag _) : base(_) { }
    public CrowdingAnalyzer(CrowdingAnalyzer original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CrowdingAnalyzer(this, cloner);
    }

    public CrowdingAnalyzer() {
      Parameters.Add(new ResultParameter<DoubleValue>("Crowding", "The average corwding distance of all points (excluding infinities)", "Results", new DoubleValue(double.NaN)));
    }

    public override IOperation Apply() {
      var qualities = QualitiesParameter.ActualValue;
      var crowdingDistance = CrowdingCalculator.CalculateCrowding(qualities.Select(x => x.ToArray()));
      ResultParameter.ActualValue.Value = crowdingDistance;
      return base.Apply();
    }
  }
}