#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("F30B6562-ECEC-42A0-B7D6-E55390FC0412")]
  [NonDiscoverableType]
  public class SymbolicDataAnalysisExpressionTreeAverageSimilarityCalculator : InstrumentedOperator {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string AverageSimilarityParameterName = "AverageSimilarity";
    private const string StrictSimilarityParameterName = "StrictSimilarity";

    public SymbolicDataAnalysisExpressionTreeAverageSimilarityCalculator() {
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(AverageSimilarityParameterName));
      Parameters.Add(new FixedValueParameter<BoolValue>(StrictSimilarityParameterName));
    }

    protected SymbolicDataAnalysisExpressionTreeAverageSimilarityCalculator(SymbolicDataAnalysisExpressionTreeAverageSimilarityCalculator rhs, Cloner cloner) { }

    public IScopeTreeLookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (IScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }

    public IFixedValueParameter<BoolValue> StrictSimilarityParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[StrictSimilarityParameterName]; }
    }

    public bool StrictSimilarity {
      get { return StrictSimilarityParameter.Value.Value; }
      set { StrictSimilarityParameter.Value.Value = value; }
    }

    public IScopeTreeLookupParameter<DoubleValue> AverageSimilarityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters[AverageSimilarityParameterName]; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionTreeAverageSimilarityCalculator(this, cloner);
    }

    [StorableConstructor]
    protected SymbolicDataAnalysisExpressionTreeAverageSimilarityCalculator(StorableConstructorFlag _) : base(_) { }

    public override IOperation InstrumentedApply() {
      var trees = SymbolicExpressionTreeParameter.ActualValue;
      var similarityMatrix = SymbolicExpressionTreeHash.ComputeSimilarityMatrix(trees, false, StrictSimilarity);
      DoubleValue averageSimilarity(int i) => new DoubleValue(Enumerable.Range(0, trees.Length).Where(j => i != j).Average(j => similarityMatrix[i, j]));
      AverageSimilarityParameter.ActualValue = new ItemArray<DoubleValue>(Enumerable.Range(0, trees.Length).Select(averageSimilarity));
      return base.InstrumentedApply();
    }
  }
}
