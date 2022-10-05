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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableType("F06FB45C-051E-4AD8-BD82-16DA9DCBCACB")]
  [Item("CrowdingAnalyzer", "The mean crowding distance for each point of the Front (see Multi-Objective Performance Metrics - Shodhganga for more information)")]
  public class CrowdingAnalyzer : MOTFAnalyzer {

    public ILookupParameter<DoubleMatrix> BoundsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    public IResultParameter<DoubleValue> CrowdingResultParameter {
      get { return (IResultParameter<DoubleValue>)Parameters["Crowding"]; }
    }

    [StorableConstructor]
    protected CrowdingAnalyzer(StorableConstructorFlag _) : base(_) { }
    public CrowdingAnalyzer(CrowdingAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CrowdingAnalyzer(this, cloner);
    }

    public CrowdingAnalyzer() {
      Parameters.Add(new LookupParameter<DoubleMatrix>("Bounds",
        "The bounds of the solution given as either one line for all variables or a line for each variable. The first column specifies lower bound, the second upper bound."));
      Parameters.Add(new ResultParameter<DoubleValue>("Crowding", "The average corwding value of all points (excluding infinities)"));
      CrowdingResultParameter.DefaultValue = new DoubleValue(double.NaN);

    }

    public override IOperation Apply() {
      var qualities = QualitiesParameter.ActualValue;
      var bounds = BoundsParameter.ActualValue;

      var crowdingDistance = Crowding.Calculate(qualities.Select(x => x.ToArray()), bounds.CloneAsMatrix());
      CrowdingResultParameter.ActualValue.Value = crowdingDistance;

      return base.Apply();
    }

  }
}
