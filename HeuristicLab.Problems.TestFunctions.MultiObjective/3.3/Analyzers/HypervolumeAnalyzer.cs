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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableType("13D363E4-76FF-4A5A-9B2C-767D9E880E4B")]
  [Item("HypervolumeAnalyzer", "Computes the enclosed Hypervolume between the current front and a given reference Point")]
  public class HypervolumeAnalyzer : MOTFAnalyzer {

    public ILookupParameter<DoubleArray> ReferencePointParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["ReferencePoint"]; }
    }
    public IResultParameter<DoubleValue> HypervolumeResultParameter {
      get { return (IResultParameter<DoubleValue>)Parameters["Hypervolume"]; }
    }
    public IResultParameter<DoubleValue> BestKnownHypervolumeResultParameter {
      get { return (IResultParameter<DoubleValue>)Parameters["Best known hypervolume"]; }
    }
    public IResultParameter<DoubleValue> HypervolumeDistanceResultParameter {
      get { return (IResultParameter<DoubleValue>)Parameters["Absolute Distance to BestKnownHypervolume"]; }
    }


    [StorableConstructor]
    protected HypervolumeAnalyzer(StorableConstructorFlag _) : base(_) {
    }

    protected HypervolumeAnalyzer(HypervolumeAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HypervolumeAnalyzer(this, cloner);
    }

    public HypervolumeAnalyzer() {
      Parameters.Add(new LookupParameter<DoubleArray>("ReferencePoint", "The reference point for hypervolume calculation"));
      Parameters.Add(new ResultParameter<DoubleValue>("Hypervolume", "The hypervolume of the current generation"));
      Parameters.Add(new ResultParameter<DoubleValue>("Best known hypervolume", "The optimal hypervolume"));
      Parameters.Add(new ResultParameter<DoubleValue>("Absolute Distance to BestKnownHypervolume", "The difference between the best known and the current hypervolume"));
      HypervolumeResultParameter.DefaultValue = new DoubleValue(0);
      BestKnownHypervolumeResultParameter.DefaultValue = new DoubleValue(0);
      HypervolumeDistanceResultParameter.DefaultValue = new DoubleValue(0);


    }

    public override IOperation Apply() {
      var qualities = QualitiesParameter.ActualValue;
      var testFunction = TestFunctionParameter.ActualValue;
      int objectives = qualities[0].Length;
      var referencePoint = ReferencePointParameter.ActualValue;

      double best = BestKnownHypervolumeResultParameter.ActualValue.Value;
      if (referencePoint.SequenceEqual(testFunction.ReferencePoint(objectives))) {
        best = Math.Max(best, testFunction.OptimalHypervolume(objectives));
      }

      IEnumerable<double[]> front = NonDominatedSelect.SelectNonDominatedVectors(qualities.Select(q => q.ToArray()), testFunction.Maximization(objectives), true);

      double hv = Hypervolume.Calculate(front, referencePoint.ToArray(), testFunction.Maximization(objectives));

      if (hv > best) {
        best = hv;
      }

      HypervolumeResultParameter.ActualValue.Value = hv;
      BestKnownHypervolumeResultParameter.ActualValue.Value = best;
      HypervolumeDistanceResultParameter.ActualValue.Value = best - hv;

      return base.Apply();
    }

  }
}
