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
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableType("720E2726-7F31-4425-B478-327D24BA2FF3")]
  [Item("ScatterPlotAnalyzer", "Creates a Scatterplot for the current and the best known front (see Multi-Objective Performance Metrics - Shodhganga for more information)")]
  public class ScatterPlotAnalyzer : MOTFAnalyzer {

    public IScopeTreeLookupParameter<RealVector> IndividualsParameter {
      get { return (IScopeTreeLookupParameter<RealVector>)Parameters["Individuals"]; }
    }

    public IResultParameter<ParetoFrontScatterPlot<RealVector>> ScatterPlotResultParameter {
      get { return (IResultParameter<ParetoFrontScatterPlot<RealVector>>)Parameters["Scatterplot"]; }
    }

    [StorableConstructor]
    protected ScatterPlotAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected ScatterPlotAnalyzer(ScatterPlotAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScatterPlotAnalyzer(this, cloner);
    }

    public ScatterPlotAnalyzer() {
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("Individuals", "The individual solutions to the problem"));
      Parameters.Add(new ResultParameter<ParetoFrontScatterPlot<RealVector>>("Scatterplot", "The scatterplot for the current and optimal (if known front)"));
    }

    public override IOperation Apply() {
      var qualities = QualitiesParameter.ActualValue;
      var individuals = IndividualsParameter.ActualValue;
      var testFunction = TestFunctionParameter.ActualValue;
      var objectives = qualities.Length != 0 ? qualities[0].Length:0;

      var optimalFront = new double[0][];               
      if (testFunction != null) {
        var front = testFunction.OptimalParetoFront(objectives);
        if (front != null) optimalFront = front.ToArray();
      }
      else {
        var mat = BestKnownFrontParameter.ActualValue;
        optimalFront = mat == null ? null : Enumerable.Range(0, mat.Rows).Select(r => Enumerable.Range(0, mat.Columns).Select(c => mat[r, c]).ToArray()).ToArray();
      }

      var fronts = DominationCalculator.CalculateAllParetoFronts(individuals.ToArray(), qualities.Select(x => x.ToArray()).ToArray(), testFunction.Maximization(objectives), out var rank);
      
      ScatterPlotResultParameter.ActualValue = new ParetoFrontScatterPlot<RealVector>(
        fronts.Select(x => x.Select(y => y.Item2).ToArray()).ToArray(),
        fronts.Select(x => x.Select(y => y.Item1).ToArray()).ToArray(),
        optimalFront, objectives);
      return base.Apply();
    }
  }
}
