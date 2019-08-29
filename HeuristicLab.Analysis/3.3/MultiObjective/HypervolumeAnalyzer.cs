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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Analysis {
  [StorableType("fc42ed56-15de-41df-bf59-0e5b11cd4f9b")]
  [Item("HypervolumeAnalyzer", "Computes the enclosed Hypervolume between the current front and a given reference Point")]
  public class HypervolumeAnalyzer : MultiObjectiveSuccessAnalyzer {
    public override string ResultName {
      get { return "Hypervolume"; }
    }

    public ILookupParameter<DoubleArray> ReferencePointParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["ReferencePoint"]; }
    }

    public IResultParameter<DoubleValue> BestKnownHypervolumeResultParameter {
      get { return (IResultParameter<DoubleValue>)Parameters["Best known hypervolume"]; }
    }

    public IResultParameter<DoubleValue> HypervolumeDistanceResultParameter {
      get { return (IResultParameter<DoubleValue>)Parameters["Absolute Distance to BestKnownHypervolume"]; }
    }


    [StorableConstructor]
    protected HypervolumeAnalyzer(StorableConstructorFlag _) : base(_) { }

    protected HypervolumeAnalyzer(HypervolumeAnalyzer original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new HypervolumeAnalyzer(this, cloner);
    }

    public HypervolumeAnalyzer() {
      Parameters.Add(new LookupParameter<DoubleArray>("ReferencePoint", "The reference point for hypervolume calculation"));
      Parameters.Add(new ResultParameter<DoubleValue>("Hypervolume", "The hypervolume of the current generation", "Results", new DoubleValue(double.NaN)));
      Parameters.Add(new ResultParameter<DoubleValue>("Best known hypervolume", "The optimal hypervolume"));
      Parameters.Add(new ResultParameter<DoubleValue>("Absolute Distance to BestKnownHypervolume", "The difference between the best known and the current hypervolume"));
      BestKnownHypervolumeResultParameter.DefaultValue = new DoubleValue(double.NaN);
      HypervolumeDistanceResultParameter.DefaultValue = new DoubleValue(double.NaN);
    }

    public override IOperation Apply() {
      var qualities = QualitiesParameter.ActualValue.Select(x => x.CloneAsArray()).ToArray();
      var referencePoint = ReferencePointParameter.ActualValue.ToArray();
      var best = BestKnownHypervolumeResultParameter.ActualValue.Value;
      var maximization = MaximizationParameter.ActualValue.ToArray();

      var hv = HypervolumeCalculator.CalculateHypervolume(qualities, referencePoint, maximization);
      if (hv > best || double.IsNaN(best)) best = hv;
      ResultParameter.ActualValue.Value = hv;
      BestKnownHypervolumeResultParameter.ActualValue.Value = best;
      HypervolumeDistanceResultParameter.ActualValue.Value = best - hv;

      return base.Apply();
    }
  }
}