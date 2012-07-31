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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "GaussianProcessSetHyperparameterLength",
    Description = "Determines the length of the hyperparameter vector based on the mean function, covariance function, and number of allowed input variables.")]
  public sealed class GaussianProcessSetHyperparameterLength : SingleSuccessorOperator {
    private const string MeanFunctionParameterName = "MeanFunction";
    private const string CovarianceFunctionParameterName = "CovarianceFunction";
    private const string ProblemDataParameterName = "ProblemData";
    private const string NumberOfHyperparameterParameterName = "NumberOfHyperparameter";

    #region Parameter Properties
    // in
    public ILookupParameter<IMeanFunction> MeanFunctionParameter {
      get { return (ILookupParameter<IMeanFunction>)Parameters[MeanFunctionParameterName]; }
    }
    public ILookupParameter<ICovarianceFunction> CovarianceFunctionParameter {
      get { return (ILookupParameter<ICovarianceFunction>)Parameters[CovarianceFunctionParameterName]; }
    }
    public ILookupParameter<IDataAnalysisProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IDataAnalysisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    // out
    public ILookupParameter<IntValue> NumberOfHyperparameterParameter {
      get { return (ILookupParameter<IntValue>)Parameters[NumberOfHyperparameterParameterName]; }
    }
    #endregion

    #region Properties
    public IMeanFunction MeanFunction { get { return MeanFunctionParameter.ActualValue; } }
    public ICovarianceFunction CovarianceFunction { get { return CovarianceFunctionParameter.ActualValue; } }
    public IDataAnalysisProblemData ProblemData { get { return ProblemDataParameter.ActualValue; } }
    #endregion

    [StorableConstructor]
    private GaussianProcessSetHyperparameterLength(bool deserializing) : base(deserializing) { }
    private GaussianProcessSetHyperparameterLength(GaussianProcessSetHyperparameterLength original, Cloner cloner) : base(original, cloner) { }
    public GaussianProcessSetHyperparameterLength()
      : base() {
      // in
      Parameters.Add(new LookupParameter<IMeanFunction>(MeanFunctionParameterName, "The mean function for the Gaussian process model."));
      Parameters.Add(new LookupParameter<ICovarianceFunction>(CovarianceFunctionParameterName, "The covariance function for the Gaussian process model."));
      Parameters.Add(new LookupParameter<IDataAnalysisProblemData>(ProblemDataParameterName, "The input data for the Gaussian process."));
      // out
      Parameters.Add(new LookupParameter<IntValue>(NumberOfHyperparameterParameterName, "The length of the hyperparameter vector for the Gaussian process model."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessSetHyperparameterLength(this, cloner);
    }

    public override IOperation Apply()
    {
      var inputVariablesCount = ProblemData.AllowedInputVariables.Count();
      int l = 1 + MeanFunction.GetNumberOfParameters(inputVariablesCount) +
              CovarianceFunction.GetNumberOfParameters(inputVariablesCount);
      NumberOfHyperparameterParameter.ActualValue = new IntValue(l);
      return base.Apply();
    }
  }
}
