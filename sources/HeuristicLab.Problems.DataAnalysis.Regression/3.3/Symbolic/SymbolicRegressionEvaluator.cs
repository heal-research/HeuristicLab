#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Operators;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("SymbolicRegressionEvaluator", "Evaluates a symbolic regression solution.")]
  [StorableClass]
  public abstract class SymbolicRegressionEvaluator : SingleSuccessorOperator, ISymbolicRegressionEvaluator {
    private const string QualityParameterName = "Quality";
    private const string FunctionTreeParameterName = "FunctionTree";
    private const string RegressionProblemDataParameterName = "RegressionProblemData";
    private const string NumberOfEvaluatedNodexParameterName = "NumberOfEvaluatedNodes";
    #region ISymbolicRegressionEvaluator Members

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }

    public ILookupParameter<SymbolicExpressionTree> FunctionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[FunctionTreeParameterName]; }
    }

    public ILookupParameter<RegressionProblemData> RegressionProblemDataParameter {
      get { return (ILookupParameter<RegressionProblemData>)Parameters[RegressionProblemDataParameterName]; }
    }

    //public ILookupParameter<IntValue> SamplesStartParameter {
    //  get { return (ILookupParameter<IntValue>)Parameters["SamplesStart"]; }
    //}

    //public ILookupParameter<IntValue> SamplesEndParameter {
    //  get { return (ILookupParameter<IntValue>)Parameters["SamplesEnd"]; }
    //}

    public ILookupParameter<DoubleValue> NumberOfEvaluatedNodesParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[NumberOfEvaluatedNodexParameterName]; }
    }

    #endregion

    public SymbolicRegressionEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The quality of the evaluated symbolic regression solution."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(FunctionTreeParameterName, "The symbolic regression solution encoded as a symbolic expression tree."));
      Parameters.Add(new LookupParameter<RegressionProblemData>(RegressionProblemDataParameterName, "The data set on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new LookupParameter<DoubleValue>(NumberOfEvaluatedNodexParameterName, "The number of evaluated nodes so far (for performance measurements.)"));
    }

    public override IOperation Apply() {
      SymbolicExpressionTree solution = FunctionTreeParameter.ActualValue;
      RegressionProblemData regressionProblemData = RegressionProblemDataParameter.ActualValue;
      DoubleValue numberOfEvaluatedNodes = NumberOfEvaluatedNodesParameter.ActualValue;
      
      QualityParameter.ActualValue = new DoubleValue(Evaluate(solution, regressionProblemData.Dataset, regressionProblemData.TargetVariable, regressionProblemData.TrainingSamplesStart, regressionProblemData.TrainingSamplesEnd, numberOfEvaluatedNodes));
      return null;
    }

    protected abstract double Evaluate(SymbolicExpressionTree solution, Dataset dataset, StringValue targetVariable, IntValue samplesStart, IntValue samplesEnd, DoubleValue numberOfEvaluatedNodes);
  }
}
