#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Problems.DataAnalysis.Regression.SupportVectorRegression;
using HeuristicLab.Problems.DataAnalysis.SupportVectorMachine;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// A support vector machine.
  /// </summary>
  [NonDiscoverableType]
  [Item("Support Vector Machine", "Support vector machine data analysis algorithm.")]
  [StorableClass]
  public sealed class SupportVectorMachine : EngineAlgorithm, IStorableContent {
    private const string TrainingSamplesStartParameterName = "Training start";
    private const string TrainingSamplesEndParameterName = "Training end";
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";
    private const string SvmTypeParameterName = "SvmType";
    private const string KernelTypeParameterName = "KernelType";
    private const string CostParameterName = "Cost";
    private const string NuParameterName = "Nu";
    private const string GammaParameterName = "Gamma";
    private const string EpsilonParameterName = "Epsilon";
    private const string ModelParameterName = "SupportVectorMachineModel";

    public string Filename { get; set; }

    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(DataAnalysisProblem); }
    }
    public new DataAnalysisProblem Problem {
      get { return (DataAnalysisProblem)base.Problem; }
      set { base.Problem = value; }
    }
    #endregion

    #region parameter properties
    public IValueParameter<IntValue> TrainingSamplesStartParameter {
      get { return (IValueParameter<IntValue>)Parameters[TrainingSamplesStartParameterName]; }
    }
    public IValueParameter<IntValue> TrainingSamplesEndParameter {
      get { return (IValueParameter<IntValue>)Parameters[TrainingSamplesEndParameterName]; }
    }
    public IValueParameter<StringValue> SvmTypeParameter {
      get { return (IValueParameter<StringValue>)Parameters[SvmTypeParameterName]; }
    }
    public IValueParameter<StringValue> KernelTypeParameter {
      get { return (IValueParameter<StringValue>)Parameters[KernelTypeParameterName]; }
    }
    public IValueParameter<DoubleValue> NuParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[NuParameterName]; }
    }
    public IValueParameter<DoubleValue> CostParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[CostParameterName]; }
    }
    public IValueParameter<DoubleValue> GammaParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[GammaParameterName]; }
    }
    public IValueParameter<DoubleValue> EpsilonParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[EpsilonParameterName]; }
    }
    #endregion

    [Storable]
    private SupportVectorMachineModelCreator solutionCreator;
    [Storable]
    private SupportVectorMachineModelEvaluator evaluator;
    [Storable]
    private SimpleMSEEvaluator mseEvaluator;
    [Storable]
    private BestSupportVectorRegressionSolutionAnalyzer analyzer;
    public SupportVectorMachine()
      : base() {
      #region svm types
      StringValue cSvcType = new StringValue("C_SVC").AsReadOnly();
      StringValue nuSvcType = new StringValue("NU_SVC").AsReadOnly();
      StringValue eSvrType = new StringValue("EPSILON_SVR").AsReadOnly();
      StringValue nuSvrType = new StringValue("NU_SVR").AsReadOnly();
      ItemSet<StringValue> allowedSvmTypes = new ItemSet<StringValue>();
      allowedSvmTypes.Add(cSvcType);
      allowedSvmTypes.Add(nuSvcType);
      allowedSvmTypes.Add(eSvrType);
      allowedSvmTypes.Add(nuSvrType);
      #endregion
      #region kernel types
      StringValue rbfKernelType = new StringValue("RBF").AsReadOnly();
      StringValue linearKernelType = new StringValue("LINEAR").AsReadOnly();
      StringValue polynomialKernelType = new StringValue("POLY").AsReadOnly();
      StringValue sigmoidKernelType = new StringValue("SIGMOID").AsReadOnly();
      ItemSet<StringValue> allowedKernelTypes = new ItemSet<StringValue>();
      allowedKernelTypes.Add(rbfKernelType);
      allowedKernelTypes.Add(linearKernelType);
      allowedKernelTypes.Add(polynomialKernelType);
      allowedKernelTypes.Add(sigmoidKernelType);
      #endregion
      Parameters.Add(new ValueParameter<IntValue>(TrainingSamplesStartParameterName, "The first index of the data set partition to use for training."));
      Parameters.Add(new ValueParameter<IntValue>(TrainingSamplesEndParameterName, "The last index of the data set partition to use for training."));
      Parameters.Add(new ConstrainedValueParameter<StringValue>(SvmTypeParameterName, "The type of SVM to use.", allowedSvmTypes, nuSvrType));
      Parameters.Add(new ConstrainedValueParameter<StringValue>(KernelTypeParameterName, "The kernel type to use for the SVM.", allowedKernelTypes, rbfKernelType));
      Parameters.Add(new ValueParameter<DoubleValue>(NuParameterName, "The value of the nu parameter nu-SVC, one-class SVM and nu-SVR.", new DoubleValue(0.5)));
      Parameters.Add(new ValueParameter<DoubleValue>(CostParameterName, "The value of the C (cost) parameter of C-SVC, epsilon-SVR and nu-SVR.", new DoubleValue(1.0)));
      Parameters.Add(new ValueParameter<DoubleValue>(GammaParameterName, "The value of the gamma parameter in the kernel function.", new DoubleValue(1.0)));
      Parameters.Add(new ValueLookupParameter<DoubleValue>(EpsilonParameterName, "The value of the epsilon parameter (only for epsilon-SVR).", new DoubleValue(1.0)));

      solutionCreator = new SupportVectorMachineModelCreator();
      evaluator = new SupportVectorMachineModelEvaluator();
      mseEvaluator = new SimpleMSEEvaluator();
      analyzer = new BestSupportVectorRegressionSolutionAnalyzer();

      OperatorGraph.InitialOperator = solutionCreator;
      solutionCreator.Successor = evaluator;
      evaluator.Successor = mseEvaluator;
      mseEvaluator.Successor = analyzer;

      Initialize();
    }
    [StorableConstructor]
    private SupportVectorMachine(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private SupportVectorMachine(SupportVectorMachine original, Cloner cloner)
      : base(original, cloner) {
      solutionCreator = cloner.Clone(original.solutionCreator);
      evaluator = cloner.Clone(original.evaluator);
      mseEvaluator = cloner.Clone(original.mseEvaluator);
      analyzer = cloner.Clone(original.analyzer);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SupportVectorMachine(this, cloner);
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    protected override void Problem_Reset(object sender, EventArgs e) {
      UpdateAlgorithmParameters();
      base.Problem_Reset(sender, e);
    }

    #region Events
    protected override void OnProblemChanged() {
      solutionCreator.DataAnalysisProblemDataParameter.ActualName = Problem.DataAnalysisProblemDataParameter.Name;
      evaluator.DataAnalysisProblemDataParameter.ActualName = Problem.DataAnalysisProblemDataParameter.Name;
      analyzer.ProblemDataParameter.ActualName = Problem.DataAnalysisProblemDataParameter.Name;
      UpdateAlgorithmParameters();
      Problem.Reset += new EventHandler(Problem_Reset);
      base.OnProblemChanged();
    }

    #endregion

    #region Helpers
    private void Initialize() {
      solutionCreator.SvmTypeParameter.ActualName = SvmTypeParameter.Name;
      solutionCreator.KernelTypeParameter.ActualName = KernelTypeParameter.Name;
      solutionCreator.CostParameter.ActualName = CostParameter.Name;
      solutionCreator.GammaParameter.ActualName = GammaParameter.Name;
      solutionCreator.NuParameter.ActualName = NuParameter.Name;
      solutionCreator.SamplesStartParameter.ActualName = TrainingSamplesStartParameter.Name;
      solutionCreator.SamplesEndParameter.ActualName = TrainingSamplesEndParameter.Name;

      evaluator.SamplesStartParameter.ActualName = TrainingSamplesStartParameter.Name;
      evaluator.SamplesEndParameter.ActualName = TrainingSamplesEndParameter.Name;
      evaluator.SupportVectorMachineModelParameter.ActualName = solutionCreator.SupportVectorMachineModelParameter.ActualName;
      evaluator.ValuesParameter.ActualName = "Training values";

      mseEvaluator.ValuesParameter.ActualName = "Training values";
      mseEvaluator.MeanSquaredErrorParameter.ActualName = "Training MSE";

      analyzer.SupportVectorRegressionModelParameter.ActualName = solutionCreator.SupportVectorMachineModelParameter.ActualName;
      analyzer.SupportVectorRegressionModelParameter.Depth = 0;
      analyzer.QualityParameter.ActualName = mseEvaluator.MeanSquaredErrorParameter.ActualName;
      analyzer.QualityParameter.Depth = 0;

      if (Problem != null) {
        solutionCreator.DataAnalysisProblemDataParameter.ActualName = Problem.DataAnalysisProblemDataParameter.Name;
        evaluator.DataAnalysisProblemDataParameter.ActualName = Problem.DataAnalysisProblemDataParameter.Name;
        analyzer.ProblemDataParameter.ActualName = Problem.DataAnalysisProblemDataParameter.Name;
        Problem.Reset += new EventHandler(Problem_Reset);
      }
    }

    private void UpdateAlgorithmParameters() {
      TrainingSamplesStartParameter.ActualValue = Problem.DataAnalysisProblemData.TrainingSamplesStart;
      TrainingSamplesEndParameter.ActualValue = Problem.DataAnalysisProblemData.TrainingSamplesEnd;
    }
    #endregion
  }
}
