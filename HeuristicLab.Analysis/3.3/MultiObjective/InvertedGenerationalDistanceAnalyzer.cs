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

using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;

namespace HeuristicLab.Analysis {
  [StorableType("1004980a-9b3d-4b6f-b5bf-81e75d288344")]
  [Item("InvertedGenerationalDistanceAnalyzer", "The inverted generational distance between the current and the best known front (see Multi-Objective Performance Metrics - Shodhganga for more information)")]
  public class InvertedGenerationalDistanceAnalyzer : GenerationalDistanceAnalyzer {
    public override bool EnabledByDefault {
      get { return false; }
    }
    public override string ResultName {
      get { return "Inverted Generational Distance"; }
    }

    public IResultParameter<DoubleValue> InvertedGenerationalDistanceResultParameter {
      get { return (IResultParameter<DoubleValue>)Parameters["Inverted Generational Distance"]; }
    }

    [StorableConstructor]
    protected InvertedGenerationalDistanceAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected InvertedGenerationalDistanceAnalyzer(InvertedGenerationalDistanceAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new InvertedGenerationalDistanceAnalyzer(this, cloner);
    }

    public InvertedGenerationalDistanceAnalyzer() : base() { }

    protected override double CalculateDistance(ItemArray<DoubleArray> qualities, IEnumerable<double[]> optimalFront) {
      return CalculateGenerationalDistance(optimalFront, qualities.Select(x => x.ToArray()), Dampening);
    }
  }
}