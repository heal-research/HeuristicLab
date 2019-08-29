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

using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;

namespace HeuristicLab.Analysis {
  [StorableType("d84ac1f1-a6d4-425f-bcff-28472ef7529a")]
  [Item("SpacingAnalyzer", "The spacing of the current front (see Multi-Objective Performance Metrics - Shodhganga for more information)")]
  public class SpacingAnalyzer : MultiObjectiveSuccessAnalyzer {
    public override string ResultName {
      get { return "Spacing"; }
    }

    [StorableConstructor]
    protected SpacingAnalyzer(StorableConstructorFlag _) : base(_) { }


    protected SpacingAnalyzer(SpacingAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SpacingAnalyzer(this, cloner);
    }

    public SpacingAnalyzer() {
      Parameters.Add(new ResultParameter<DoubleValue>("Spacing", "The spacing of the current front", "Results", new DoubleValue(double.NaN)));
    }

    public override IOperation Apply() {
      var qualities = QualitiesParameter.ActualValue;
      ResultParameter.ActualValue.Value = CalculateSpacing(qualities.Select(x => x.ToArray()));
      return base.Apply();
    }

    public static double CalculateSpacing(IEnumerable<double[]> qualities) {
      if (qualities == null) throw new ArgumentNullException("qualities");
      var l = qualities.ToList();
      if (l.Count == 0) throw new ArgumentException("Front must not be empty.");
      if (l.Count == 1) return 0;

      var mat = l.ToMatrix();
      alglib.kdtree tree;
      alglib.kdtreebuild(mat, mat.GetLength(0), mat.GetLength(1), 0, 2, out tree);
      var summand = new double[2];
      var dists = new List<double>();
      foreach (var point in l) {
        alglib.kdtreequeryknn(tree, point.ToArray(), 2, true);
        alglib.kdtreequeryresultsdistances(tree, ref summand);
        dists.Add(summand[1]);
      }
      return dists.StandardDeviationPop();
    }
  }
}