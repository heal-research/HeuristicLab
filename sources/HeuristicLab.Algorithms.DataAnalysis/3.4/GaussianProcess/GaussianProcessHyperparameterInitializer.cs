#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "GaussianProcessHyperparameterInitializer",
    Description = "Initializers the hyperparameter vector based on the mean function, covariance function, and number of allowed input variables.")]
  public sealed class GaussianProcessHyperparameterInitializer : SingleSuccessorOperator {
    private const string MeanFunctionParameterName = "MeanFunction";
    private const string CovarianceFunctionParameterName = "CovarianceFunction";
    private const string ProblemDataParameterName = "ProblemData";
    private const string HyperparameterParameterName = "Hyperparameter";
    private const string RandomParameterName = "Random";

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
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters[RandomParameterName]; }
    }
    // out
    public ILookupParameter<RealVector> HyperparameterParameter {
      get { return (ILookupParameter<RealVector>)Parameters[HyperparameterParameterName]; }
    }
    #endregion

    #region Properties
    private IMeanFunction MeanFunction { get { return MeanFunctionParameter.ActualValue; } }
    private ICovarianceFunction CovarianceFunction { get { return CovarianceFunctionParameter.ActualValue; } }
    private IDataAnalysisProblemData ProblemData { get { return ProblemDataParameter.ActualValue; } }
    #endregion

    [StorableConstructor]
    private GaussianProcessHyperparameterInitializer(bool deserializing) : base(deserializing) { }
    private GaussianProcessHyperparameterInitializer(GaussianProcessHyperparameterInitializer original, Cloner cloner) : base(original, cloner) { }
    public GaussianProcessHyperparameterInitializer()
      : base() {
      // in
      Parameters.Add(new LookupParameter<IMeanFunction>(MeanFunctionParameterName, "The mean function for the Gaussian process model."));
      Parameters.Add(new LookupParameter<ICovarianceFunction>(CovarianceFunctionParameterName, "The covariance function for the Gaussian process model."));
      Parameters.Add(new LookupParameter<IDataAnalysisProblemData>(ProblemDataParameterName, "The input data for the Gaussian process."));
      Parameters.Add(new LookupParameter<IRandom>(RandomParameterName, "The pseudo random number generator to use for initializing the hyperparameter vector."));
      // out
      Parameters.Add(new LookupParameter<RealVector>(HyperparameterParameterName, "The initial hyperparameter vector for the Gaussian process model."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessHyperparameterInitializer(this, cloner);
    }

    public override IOperation Apply() {
      var inputVariablesCount = ProblemData.AllowedInputVariables.Count();
      int l = 1 + MeanFunction.GetNumberOfParameters(inputVariablesCount) +
              CovarianceFunction.GetNumberOfParameters(inputVariablesCount);
      var r = new RealVector(l);
      var rand = RandomParameter.ActualValue;
      for (int i = 0; i < r.Length; i++)
        r[i] = rand.NextDouble() * 10 - 5;

      HyperparameterParameter.ActualValue = r;
      return base.Apply();
    }
  }
}
