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
    #region ISymbolicRegressionEvaluator Members

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<SymbolicExpressionTree> FunctionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters["FunctionTree"]; }
    }

    public ILookupParameter<Dataset> DatasetParameter {
      get { return (ILookupParameter<Dataset>)Parameters["Dataset"]; }
    }

    public ILookupParameter<StringValue> TargetVariableParameter {
      get { return (ILookupParameter<StringValue>)Parameters["TargetVariable"]; }
    }

    public ILookupParameter<IntValue> SamplesStartParameter {
      get { return (ILookupParameter<IntValue>)Parameters["SamplesStart"]; }
    }

    public ILookupParameter<IntValue> SamplesEndParameter {
      get { return (ILookupParameter<IntValue>)Parameters["SamplesEnd"]; }
    }

    public ILookupParameter<DoubleValue> NumberOfEvaluatedNodesParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["NumberOfEvaluatedNodes"]; }
    }

    #endregion

    public SymbolicRegressionEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the evaluated symbolic regression solution."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>("FunctionTree", "The symbolic regression solution encoded as a symbolic expression tree."));
      Parameters.Add(new LookupParameter<Dataset>("Dataset", "The data set on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new LookupParameter<StringValue>("TargetVariable", "The target variable of the symbolic regression solution."));
      Parameters.Add(new LookupParameter<IntValue>("SamplesStart", "The start index of the partition of the data set on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new LookupParameter<IntValue>("SamplesEnd", "The end index of the partition of the data set on which the symbolic regression solution should be evaluated."));
      Parameters.Add(new LookupParameter<DoubleValue>("NumberOfEvaluatedNodes", "The number of evaluated nodes so far (for performance measurements.)"));
    }

    public override IOperation Apply() {
      SymbolicExpressionTree solution = FunctionTreeParameter.ActualValue;
      Dataset dataset = DatasetParameter.ActualValue;
      StringValue targetVariable = TargetVariableParameter.ActualValue;
      IntValue samplesStart = SamplesStartParameter.ActualValue;
      IntValue samplesEnd = SamplesEndParameter.ActualValue;
      DoubleValue numberOfEvaluatedNodes = NumberOfEvaluatedNodesParameter.ActualValue;
      
      QualityParameter.ActualValue = new DoubleValue(Evaluate(solution, dataset, targetVariable, samplesStart, samplesEnd, numberOfEvaluatedNodes));
      return null;
    }

    protected abstract double Evaluate(SymbolicExpressionTree solution, Dataset dataset, StringValue targetVariable, IntValue samplesStart, IntValue samplesEnd, DoubleValue numberOfEvaluatedNodes);
  }
}
