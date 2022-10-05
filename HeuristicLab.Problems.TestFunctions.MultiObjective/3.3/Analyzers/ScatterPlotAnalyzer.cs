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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableType("720E2726-7F31-4425-B478-327D24BA2FF3")]
  [Item("ScatterPlotAnalyzer", "Creates a Scatterplot for the current and the best known front (see Multi-Objective Performance Metrics - Shodhganga for more information)")]
  public class ScatterPlotAnalyzer : MOTFAnalyzer {

    public IScopeTreeLookupParameter<RealVector> IndividualsParameter {
      get { return (IScopeTreeLookupParameter<RealVector>)Parameters["Individuals"]; }
    }

    public IResultParameter<ParetoFrontScatterPlot> ScatterPlotResultParameter {
      get { return (IResultParameter<ParetoFrontScatterPlot>)Parameters["Scatterplot"]; }
    }


    [StorableConstructor]
    protected ScatterPlotAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected ScatterPlotAnalyzer(ScatterPlotAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScatterPlotAnalyzer(this, cloner);
    }

    public ScatterPlotAnalyzer() {
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("Individuals", "The individual solutions to the problem"));
      Parameters.Add(new ResultParameter<ParetoFrontScatterPlot>("Scatterplot", "The scatterplot for the current and optimal (if known front)"));

    }

    public override IOperation Apply() {
      var qualities = QualitiesParameter.ActualValue;
      var individuals = IndividualsParameter.ActualValue;
      var testFunction = TestFunctionParameter.ActualValue;
      int objectives = qualities[0].Length;
      int problemSize = individuals[0].Length;

      double[][] optimalFront = new double[0][];
      var front = testFunction.OptimalParetoFront(objectives);
      if (front != null) optimalFront = front.ToArray();

      var qualityClones = qualities.Select(s => s.ToArray()).ToArray();
      var solutionClones = individuals.Select(s => s.ToArray()).ToArray();

      ScatterPlotResultParameter.ActualValue = new ParetoFrontScatterPlot(qualityClones, solutionClones, optimalFront, objectives, problemSize);

      return base.Apply();
    }
  }
}
