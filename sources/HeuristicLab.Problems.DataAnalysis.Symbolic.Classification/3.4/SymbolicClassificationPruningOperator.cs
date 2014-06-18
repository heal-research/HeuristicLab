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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [StorableClass]
  [Item("SymbolicClassificationPruningOperator", "An operator which prunes symbolic classificaton trees.")]
  public class SymbolicClassificationPruningOperator : SymbolicDataAnalysisExpressionPruningOperator {
    private const string ImpactValuesCalculatorParameterName = "ImpactValuesCalculator";
    private const string ModelCreatorParameterName = "ModelCreator";

    #region parameter properties
    public ILookupParameter<ISymbolicClassificationModelCreator> ModelCreatorParameter {
      get { return (ILookupParameter<ISymbolicClassificationModelCreator>)Parameters[ModelCreatorParameterName]; }
    }
    #endregion

    protected SymbolicClassificationPruningOperator(SymbolicClassificationPruningOperator original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationPruningOperator(this, cloner);
    }

    [StorableConstructor]
    protected SymbolicClassificationPruningOperator(bool deserializing) : base(deserializing) { }

    public SymbolicClassificationPruningOperator() {
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisSolutionImpactValuesCalculator>(ImpactValuesCalculatorParameterName, new SymbolicClassificationSolutionImpactValuesCalculator()));
      Parameters.Add(new LookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName));
    }

    protected override ISymbolicDataAnalysisModel CreateModel() {
      var model = ModelCreatorParameter.ActualValue.CreateSymbolicClassificationModel(SymbolicExpressionTree, Interpreter, EstimationLimits.Lower, EstimationLimits.Upper);
      var problemData = (IClassificationProblemData)ProblemData;
      var rows = problemData.TrainingIndices;
      model.RecalculateModelParameters(problemData, rows);
      return model;
    }

    protected override double Evaluate(IDataAnalysisModel model) {
      var classificationModel = (IClassificationModel)model;
      var classificationProblemData = (IClassificationProblemData)ProblemData;
      var trainingIndices = Enumerable.Range(FitnessCalculationPartition.Start, FitnessCalculationPartition.Size);
      var estimatedValues = classificationModel.GetEstimatedClassValues(ProblemData.Dataset, trainingIndices);
      var targetValues = ProblemData.Dataset.GetDoubleValues(classificationProblemData.TargetVariable, trainingIndices);
      OnlineCalculatorError errorState;
      var quality = OnlineAccuracyCalculator.Calculate(targetValues, estimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return quality;
    }
  }
}
