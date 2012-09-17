#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  /// <summary>
  /// Represents a symbolic classification model
  /// </summary>
  [StorableClass]
  [Item(Name = "SymbolicClassificationModel", Description = "Represents a symbolic classification model.")]
  public abstract class
    SymbolicClassificationModel : SymbolicDataAnalysisModel, ISymbolicClassificationModel {
    [Storable]
    private double lowerEstimationLimit;
    public double LowerEstimationLimit { get { return lowerEstimationLimit; } }
    [Storable]
    private double upperEstimationLimit;
    public double UpperEstimationLimit { get { return upperEstimationLimit; } }

    [StorableConstructor]
    protected SymbolicClassificationModel(bool deserializing) : base(deserializing) { }
    protected SymbolicClassificationModel(SymbolicClassificationModel original, Cloner cloner)
      : base(original, cloner) {
      lowerEstimationLimit = original.lowerEstimationLimit;
      upperEstimationLimit = original.upperEstimationLimit;
    }
    protected SymbolicClassificationModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue)
      : base(tree, interpreter) {
      this.lowerEstimationLimit = lowerEstimationLimit;
      this.upperEstimationLimit = upperEstimationLimit;
    }

    public abstract IEnumerable<double> GetEstimatedClassValues(Dataset dataset, IEnumerable<int> rows);
    public abstract void RecalculateModelParameters(IClassificationProblemData problemData, IEnumerable<int> rows);

    public abstract ISymbolicClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData);

    IClassificationSolution IClassificationModel.CreateClassificationSolution(IClassificationProblemData problemData) {
      return CreateClassificationSolution(problemData);
    }
  }
}
