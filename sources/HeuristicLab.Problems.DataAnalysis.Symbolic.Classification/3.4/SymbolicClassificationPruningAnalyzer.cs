#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [Item("SymbolicClassificationPruningAnalyzer", "An analyzer that prunes introns from the population.")]
  [StorableClass]
  public sealed class SymbolicClassificationPruningAnalyzer : SymbolicDataAnalysisSingleObjectivePruningAnalyzer {
    private const string ModelCreatorParameterName = "ModelCreator";
    #region parameter properties
    public ILookupParameter<ISymbolicClassificationModelCreator> ModelCreatorParameter {
      get { return (ILookupParameter<ISymbolicClassificationModelCreator>)Parameters[ModelCreatorParameterName]; }
    }
    #endregion
    #region properties
    private ISymbolicClassificationModelCreator ModelCreator {
      get { return ModelCreatorParameter.ActualValue; }
      set { ModelCreatorParameter.ActualValue = value; }
    }
    #endregion

    private SymbolicClassificationPruningAnalyzer(SymbolicClassificationPruningAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationPruningAnalyzer(this, cloner);
    }

    [StorableConstructor]
    private SymbolicClassificationPruningAnalyzer(bool deserializing) : base(deserializing) { }

    public SymbolicClassificationPruningAnalyzer() {
      // pruning parameters
      Parameters.Add(new LookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName));
      impactValuesCalculator = new SymbolicClassificationSolutionImpactValuesCalculator();
    }

    protected override ISymbolicDataAnalysisModel CreateModel(ISymbolicExpressionTree tree,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double lowerEstimationLimit = Double.MinValue,
      double upperEstimationLimit = Double.MaxValue) {
      var model = ModelCreator.CreateSymbolicClassificationModel(tree, Interpreter, lowerEstimationLimit, upperEstimationLimit);
      model.RecalculateModelParameters((IClassificationProblemData)ProblemData, ProblemData.TrainingIndices);
      return model;
    }
  }
}
