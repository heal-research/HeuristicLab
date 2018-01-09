#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Neighborhood Components Analysis
  /// </summary>
  [Item("Neighborhood Components Analysis (NCA)", @"Implementation of Neighborhood Components Analysis
based on the description of J. Goldberger, S. Roweis, G. Hinton, R. Salakhutdinov. 2005.
Neighbourhood Component Analysis. Advances in Neural Information Processing Systems, 17. pp. 513-520
with additional regularizations described in Z. Yang, J. Laaksonen. 2007.
Regularized Neighborhood Component Analysis. Lecture Notes in Computer Science, 4522. pp. 253-262.")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisClassification, Priority = 170)]
  [StorableClass]
  public sealed class NcaAlgorithm : EngineAlgorithm {
    #region Parameter Names
    private const string SeedParameterName = "Seed";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    private const string KParameterName = "K";
    private const string DimensionsParameterName = "Dimensions";
    private const string InitializationParameterName = "Initialization";
    private const string NeighborSamplesParameterName = "NeighborSamples";
    private const string IterationsParameterName = "Iterations";
    private const string RegularizationParameterName = "Regularization";
    private const string NcaModelCreatorParameterName = "NcaModelCreator";
    private const string NcaSolutionCreatorParameterName = "NcaSolutionCreator";
    private const string ApproximateGradientsParameterName = "ApproximateGradients";
    private const string NcaMatrixParameterName = "NcaMatrix";
    private const string NcaMatrixGradientsParameterName = "NcaMatrixGradients";
    private const string QualityParameterName = "Quality";
    #endregion

    public override Type ProblemType { get { return typeof(IClassificationProblem); } }
    public new IClassificationProblem Problem {
      get { return (IClassificationProblem)base.Problem; }
      set { base.Problem = value; }
    }

    #region Parameter Properties
    public IValueParameter<IntValue> SeedParameter {
      get { return (IValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }
    public IValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
    }
    public IFixedValueParameter<IntValue> KParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[KParameterName]; }
    }
    public IFixedValueParameter<IntValue> DimensionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[DimensionsParameterName]; }
    }
    public IConstrainedValueParameter<INcaInitializer> InitializationParameter {
      get { return (IConstrainedValueParameter<INcaInitializer>)Parameters[InitializationParameterName]; }
    }
    public IFixedValueParameter<IntValue> NeighborSamplesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[NeighborSamplesParameterName]; }
    }
    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[IterationsParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> RegularizationParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[RegularizationParameterName]; }
    }
    public IValueParameter<BoolValue> ApproximateGradientsParameter {
      get { return (IValueParameter<BoolValue>)Parameters[ApproximateGradientsParameterName]; }
    }
    public IValueParameter<INcaModelCreator> NcaModelCreatorParameter {
      get { return (IValueParameter<INcaModelCreator>)Parameters[NcaModelCreatorParameterName]; }
    }
    public IValueParameter<INcaSolutionCreator> NcaSolutionCreatorParameter {
      get { return (IValueParameter<INcaSolutionCreator>)Parameters[NcaSolutionCreatorParameterName]; }
    }
    #endregion

    #region Properties
    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }
    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }
    public int K {
      get { return KParameter.Value.Value; }
      set { KParameter.Value.Value = value; }
    }
    public int Dimensions {
      get { return DimensionsParameter.Value.Value; }
      set { DimensionsParameter.Value.Value = value; }
    }
    public int NeighborSamples {
      get { return NeighborSamplesParameter.Value.Value; }
      set { NeighborSamplesParameter.Value.Value = value; }
    }
    public int Iterations {
      get { return IterationsParameter.Value.Value; }
      set { IterationsParameter.Value.Value = value; }
    }
    public double Regularization {
      get { return RegularizationParameter.Value.Value; }
      set { RegularizationParameter.Value.Value = value; }
    }
    public INcaModelCreator NcaModelCreator {
      get { return NcaModelCreatorParameter.Value; }
      set { NcaModelCreatorParameter.Value = value; }
    }
    public INcaSolutionCreator NcaSolutionCreator {
      get { return NcaSolutionCreatorParameter.Value; }
      set { NcaSolutionCreatorParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private NcaAlgorithm(bool deserializing) : base(deserializing) { }
    private NcaAlgorithm(NcaAlgorithm original, Cloner cloner) : base(original, cloner) { }
    public NcaAlgorithm()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>(SeedParameterName, "The seed of the random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>(SetSeedRandomlyParameterName, "A boolean flag that indicates whether the seed should be randomly reset each time the algorithm is run.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>(KParameterName, "The K for the nearest neighbor.", new IntValue(3)));
      Parameters.Add(new FixedValueParameter<IntValue>(DimensionsParameterName, "The number of dimensions that NCA should reduce the data to.", new IntValue(2)));
      Parameters.Add(new ConstrainedValueParameter<INcaInitializer>(InitializationParameterName, "Which method should be used to initialize the matrix. Typically LDA (linear discriminant analysis) should provide a good estimate."));
      Parameters.Add(new FixedValueParameter<IntValue>(NeighborSamplesParameterName, "How many of the neighbors should be sampled in order to speed up the calculation. This should be at least the value of k and at most the number of training instances minus one will be used.", new IntValue(60)));
      Parameters.Add(new FixedValueParameter<IntValue>(IterationsParameterName, "How many iterations the conjugate gradient (CG) method should be allowed to perform. The method might still terminate earlier if a local optima has already been reached.", new IntValue(50)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(RegularizationParameterName, "A non-negative paramter which can be set to increase generalization and avoid overfitting. If set to 0 the algorithm is similar to NCA as proposed by Goldberger et al.", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<INcaModelCreator>(NcaModelCreatorParameterName, "Creates an NCA model out of the matrix.", new NcaModelCreator()));
      Parameters.Add(new ValueParameter<INcaSolutionCreator>(NcaSolutionCreatorParameterName, "Creates an NCA solution given a model and some data.", new NcaSolutionCreator()));
      Parameters.Add(new ValueParameter<BoolValue>(ApproximateGradientsParameterName, "True if the gradient should be approximated otherwise they are computed exactly.", new BoolValue()));

      NcaSolutionCreatorParameter.Hidden = true;
      ApproximateGradientsParameter.Hidden = true;

      INcaInitializer defaultInitializer = null;
      foreach (var initializer in ApplicationManager.Manager.GetInstances<INcaInitializer>().OrderBy(x => x.ItemName)) {
        if (initializer is LdaInitializer) defaultInitializer = initializer;
        InitializationParameter.ValidValues.Add(initializer);
      }
      if (defaultInitializer != null) InitializationParameter.Value = defaultInitializer;

      var randomCreator = new RandomCreator();
      var ncaInitializer = new Placeholder();
      var bfgsInitializer = new LbfgsInitializer();
      var makeStep = new LbfgsMakeStep();
      var branch = new ConditionalBranch();
      var gradientCalculator = new NcaGradientCalculator();
      var modelCreator = new Placeholder();
      var updateResults = new LbfgsUpdateResults();
      var analyzer = new LbfgsAnalyzer();
      var finalModelCreator = new Placeholder();
      var finalAnalyzer = new LbfgsAnalyzer();
      var solutionCreator = new Placeholder();

      OperatorGraph.InitialOperator = randomCreator;
      randomCreator.SeedParameter.ActualName = SeedParameterName;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameterName;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = ncaInitializer;

      ncaInitializer.Name = "(NcaInitializer)";
      ncaInitializer.OperatorParameter.ActualName = InitializationParameterName;
      ncaInitializer.Successor = bfgsInitializer;

      bfgsInitializer.IterationsParameter.ActualName = IterationsParameterName;
      bfgsInitializer.PointParameter.ActualName = NcaMatrixParameterName;
      bfgsInitializer.ApproximateGradientsParameter.ActualName = ApproximateGradientsParameterName;
      bfgsInitializer.Successor = makeStep;

      makeStep.StateParameter.ActualName = bfgsInitializer.StateParameter.Name;
      makeStep.PointParameter.ActualName = NcaMatrixParameterName;
      makeStep.Successor = branch;

      branch.ConditionParameter.ActualName = makeStep.TerminationCriterionParameter.Name;
      branch.FalseBranch = gradientCalculator;
      branch.TrueBranch = finalModelCreator;

      gradientCalculator.Successor = modelCreator;

      modelCreator.OperatorParameter.ActualName = NcaModelCreatorParameterName;
      modelCreator.Successor = updateResults;

      updateResults.StateParameter.ActualName = bfgsInitializer.StateParameter.Name;
      updateResults.QualityParameter.ActualName = QualityParameterName;
      updateResults.QualityGradientsParameter.ActualName = NcaMatrixGradientsParameterName;
      updateResults.ApproximateGradientsParameter.ActualName = ApproximateGradientsParameterName;
      updateResults.Successor = analyzer;

      analyzer.QualityParameter.ActualName = QualityParameterName;
      analyzer.PointParameter.ActualName = NcaMatrixParameterName;
      analyzer.QualityGradientsParameter.ActualName = NcaMatrixGradientsParameterName;
      analyzer.StateParameter.ActualName = bfgsInitializer.StateParameter.Name;
      analyzer.PointsTableParameter.ActualName = "Matrix table";
      analyzer.QualityGradientsTableParameter.ActualName = "Gradients table";
      analyzer.QualitiesTableParameter.ActualName = "Qualities";
      analyzer.Successor = makeStep;

      finalModelCreator.OperatorParameter.ActualName = NcaModelCreatorParameterName;
      finalModelCreator.Successor = finalAnalyzer;

      finalAnalyzer.QualityParameter.ActualName = QualityParameterName;
      finalAnalyzer.PointParameter.ActualName = NcaMatrixParameterName;
      finalAnalyzer.QualityGradientsParameter.ActualName = NcaMatrixGradientsParameterName;
      finalAnalyzer.PointsTableParameter.ActualName = analyzer.PointsTableParameter.ActualName;
      finalAnalyzer.QualityGradientsTableParameter.ActualName = analyzer.QualityGradientsTableParameter.ActualName;
      finalAnalyzer.QualitiesTableParameter.ActualName = analyzer.QualitiesTableParameter.ActualName;
      finalAnalyzer.Successor = solutionCreator;

      solutionCreator.OperatorParameter.ActualName = NcaSolutionCreatorParameterName;

      Problem = new ClassificationProblem();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NcaAlgorithm(this, cloner);
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }
  }
}
