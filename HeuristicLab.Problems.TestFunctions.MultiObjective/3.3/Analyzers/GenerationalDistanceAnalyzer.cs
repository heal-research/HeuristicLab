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
  [StorableType("EBC72F16-E329-4D18-800C-8642EFD0F05C")]
  [Item("GenerationalDistanceAnalyzer", "The generational distance between the current and the best known front (see Multi-Objective Performance Metrics - Shodhganga for more information)")]
  public class GenerationalDistanceAnalyzer : MOTFAnalyzer {

    private IFixedValueParameter<DoubleValue> DampeningParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["Dampening"]; }
      set { Parameters["Dampening"].ActualValue = value; }
    }

    public double Dampening {
      get { return DampeningParameter.Value.Value; }
      set { DampeningParameter.Value.Value = value; }
    }

    public IResultParameter<DoubleValue> GenerationalDistanceResultParameter {
      get { return (IResultParameter<DoubleValue>)Parameters["Generational Distance"]; }
    }

    [StorableConstructor]
    protected GenerationalDistanceAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected GenerationalDistanceAnalyzer(GenerationalDistanceAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GenerationalDistanceAnalyzer(this, cloner);
    }

    public GenerationalDistanceAnalyzer() {
      Parameters.Add(new FixedValueParameter<DoubleValue>("Dampening", "", new DoubleValue(1)));
      Parameters.Add(new ResultParameter<DoubleValue>("Generational Distance", "The genrational distance between the current front and the optimal front"));
      GenerationalDistanceResultParameter.DefaultValue = new DoubleValue(double.NaN);

    }

    public override IOperation Apply() {
      var qualities = QualitiesParameter.ActualValue;
      int objectives = qualities[0].Length;

      var optimalfront = TestFunctionParameter.ActualValue.OptimalParetoFront(objectives);
      if (optimalfront == null) return base.Apply();

      var distance = GenerationalDistance.Calculate(qualities.Select(x => x.CloneAsArray()), optimalfront, Dampening);
      GenerationalDistanceResultParameter.ActualValue.Value = distance;

      return base.Apply();
    }
  }
}
