#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers;
using HeuristicLab.Problems.DataAnalysis.SupportVectorMachine;

namespace HeuristicLab.Problems.DataAnalysis.Regression.SupportVectorRegression {
  [Item("BestSupportVectorRegressionSolutionAnalyzer", "An operator for analyzing the best solution of support vector regression problems.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class BestSupportVectorRegressionSolutionAnalyzer : RegressionSolutionAnalyzer {
    private const string SupportVectorRegressionModelParameterName = "SupportVectorRegressionModel";
    private const string BestSolutionInputvariableCountResultName = "Variables used by best solution";
    private const string BestSolutionParameterName = "BestSolution";

    #region parameter properties
    public ScopeTreeLookupParameter<SupportVectorMachineModel> SupportVectorRegressionModelParameter {
      get { return (ScopeTreeLookupParameter<SupportVectorMachineModel>)Parameters[SupportVectorRegressionModelParameterName]; }
    }
    public ILookupParameter<SupportVectorRegressionSolution> BestSolutionParameter {
      get { return (ILookupParameter<SupportVectorRegressionSolution>)Parameters[BestSolutionParameterName]; }
    }
    #endregion
    #region properties
    public ItemArray<SupportVectorMachineModel> SupportVectorMachineModel {
      get { return SupportVectorRegressionModelParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private BestSupportVectorRegressionSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private BestSupportVectorRegressionSolutionAnalyzer(BestSupportVectorRegressionSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public BestSupportVectorRegressionSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<SupportVectorMachineModel>(SupportVectorRegressionModelParameterName, "The support vector regression models to analyze."));
      Parameters.Add(new LookupParameter<SupportVectorRegressionSolution>(BestSolutionParameterName, "The best support vector regression solution."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestSupportVectorRegressionSolutionAnalyzer(this, cloner);
    }

    protected override DataAnalysisSolution UpdateBestSolution() {
      double upperEstimationLimit = UpperEstimationLimit != null ? UpperEstimationLimit.Value : double.PositiveInfinity;
      double lowerEstimationLimit = LowerEstimationLimit != null ? LowerEstimationLimit.Value : double.NegativeInfinity;

      int i = Quality.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;

      if (BestSolutionQualityParameter.ActualValue == null || BestSolutionQualityParameter.ActualValue.Value > Quality[i].Value) {
        IEnumerable<string> inputVariables = from var in ProblemData.InputVariables
                                             where ProblemData.InputVariables.ItemChecked(var)
                                             select var.Value;
        var solution = new SupportVectorRegressionSolution((DataAnalysisProblemData)ProblemData.Clone(), SupportVectorMachineModel[i], inputVariables, lowerEstimationLimit, upperEstimationLimit);

        BestSolutionParameter.ActualValue = solution;
        BestSolutionQualityParameter.ActualValue = Quality[i];

        if (Results.ContainsKey(BestSolutionInputvariableCountResultName)) {
          Results[BestSolutionInputvariableCountResultName].Value = new IntValue(inputVariables.Count());
        } else {
          Results.Add(new Result(BestSolutionInputvariableCountResultName, new IntValue(inputVariables.Count())));
        }
      }
      return BestSolutionParameter.ActualValue;
    }
  }
}
