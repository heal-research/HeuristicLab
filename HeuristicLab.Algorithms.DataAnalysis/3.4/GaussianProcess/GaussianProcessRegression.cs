
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

using System;
using System.Linq;
using HeuristicLab.Algorithms.GradientDescent;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  ///Gaussian process regression data analysis algorithm.
  /// </summary>
  [Item("Gaussian Process Regression", "Gaussian process regression data analysis algorithm.")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class GaussianProcessRegression : GaussianProcessBase, IStorableContent {
    public string Filename { get; set; }

    public override Type ProblemType { get { return typeof(IRegressionProblem); } }
    public new IRegressionProblem Problem {
      get { return (IRegressionProblem)base.Problem; }
      set { base.Problem = value; }
    }

    private const string ModelParameterName = "Model";

    #region parameter properties
    public IConstrainedValueParameter<IGaussianProcessRegressionModelCreator> GaussianProcessModelCreatorParameter {
      get { return (IConstrainedValueParameter<IGaussianProcessRegressionModelCreator>)Parameters[ModelCreatorParameterName]; }
    }
    public IFixedValueParameter<GaussianProcessRegressionSolutionCreator> GaussianProcessSolutionCreatorParameter {
      get { return (IFixedValueParameter<GaussianProcessRegressionSolutionCreator>)Parameters[SolutionCreatorParameterName]; }
    }
    #endregion

    [StorableConstructor]
    private GaussianProcessRegression(bool deserializing) : base(deserializing) { }
    private GaussianProcessRegression(GaussianProcessRegression original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public GaussianProcessRegression()
      : base(new RegressionProblem()) {
      this.name = ItemName;
      this.description = ItemDescription;

      var modelCreators = ApplicationManager.Manager.GetInstances<IGaussianProcessRegressionModelCreator>();
      var defaultModelCreator = modelCreators.First(c => c is GaussianProcessRegressionModelCreator);

      // GP regression and classification algorithms only differ in the model and solution creators,
      // thus we use a common base class and use operator parameters to implement the specific versions.
      // Different model creators can be implemented,
      // but the solution creator is implemented in a generic fashion already and we don't allow derived solution creators
      Parameters.Add(new ConstrainedValueParameter<IGaussianProcessRegressionModelCreator>(ModelCreatorParameterName, "The operator to create the Gaussian process model.",
        new ItemSet<IGaussianProcessRegressionModelCreator>(modelCreators), defaultModelCreator));
      // this parameter is not intended to be changed, 
      Parameters.Add(new FixedValueParameter<GaussianProcessRegressionSolutionCreator>(SolutionCreatorParameterName, "The solution creator for the algorithm",
        new GaussianProcessRegressionSolutionCreator()));
      Parameters[SolutionCreatorParameterName].Hidden = true;

      ParameterizedModelCreators();
      ParameterizeSolutionCreator(GaussianProcessSolutionCreatorParameter.Value);
      RegisterEventHandlers();
    }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessRegression(this, cloner);
    }

    #region events
    private void RegisterEventHandlers() {
      GaussianProcessModelCreatorParameter.ValueChanged += ModelCreatorParameter_ValueChanged;
    }

    private void ModelCreatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizedModelCreator(GaussianProcessModelCreatorParameter.Value);
    }
    #endregion

    private void ParameterizedModelCreators() {
      foreach (var creator in GaussianProcessModelCreatorParameter.ValidValues) {
        ParameterizedModelCreator(creator);
      }
    }

    private void ParameterizedModelCreator(IGaussianProcessRegressionModelCreator modelCreator) {
      modelCreator.ProblemDataParameter.ActualName = Problem.ProblemDataParameter.Name;
      modelCreator.MeanFunctionParameter.ActualName = MeanFunctionParameterName;
      modelCreator.CovarianceFunctionParameter.ActualName = CovarianceFunctionParameterName;

      // parameter names fixed by the algorithm
      modelCreator.ModelParameter.ActualName = ModelParameterName;
      modelCreator.HyperparameterParameter.ActualName = HyperparameterParameterName;
      modelCreator.HyperparameterGradientsParameter.ActualName = HyperparameterGradientsParameterName;
      modelCreator.NegativeLogLikelihoodParameter.ActualName = NegativeLogLikelihoodParameterName;
    }

    private void ParameterizeSolutionCreator(GaussianProcessRegressionSolutionCreator solutionCreator) {
      solutionCreator.ModelParameter.ActualName = ModelParameterName;
      solutionCreator.ProblemDataParameter.ActualName = Problem.ProblemDataParameter.Name;
    }
  }
}
