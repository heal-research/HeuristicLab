#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances;


namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("Gaussian Process Covariance Optimization Problem", "")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 300)]
  [StorableClass]
  public sealed class GaussianProcessCovarianceOptimizationProblem : SymbolicExpressionTreeProblem, IStatefulItem, IRegressionProblem, IProblemInstanceConsumer<IRegressionProblemData>, IProblemInstanceExporter<IRegressionProblemData> {
    #region static variables and ctor
    private static readonly CovarianceMaternIso maternIso1;
    private static readonly CovarianceMaternIso maternIso3;
    private static readonly CovarianceMaternIso maternIso5;
    private static readonly CovariancePiecewisePolynomial piecewisePoly0;
    private static readonly CovariancePiecewisePolynomial piecewisePoly1;
    private static readonly CovariancePiecewisePolynomial piecewisePoly2;
    private static readonly CovariancePiecewisePolynomial piecewisePoly3;
    private static readonly CovariancePolynomial poly2;
    private static readonly CovariancePolynomial poly3;
    private static readonly CovarianceSpectralMixture spectralMixture1;
    private static readonly CovarianceSpectralMixture spectralMixture3;
    private static readonly CovarianceSpectralMixture spectralMixture5;
    private static readonly CovarianceLinear linear;
    private static readonly CovarianceLinearArd linearArd;
    private static readonly CovarianceNeuralNetwork neuralNetwork;
    private static readonly CovariancePeriodic periodic;
    private static readonly CovarianceRationalQuadraticIso ratQuadraticIso;
    private static readonly CovarianceRationalQuadraticArd ratQuadraticArd;
    private static readonly CovarianceSquaredExponentialArd sqrExpArd;
    private static readonly CovarianceSquaredExponentialIso sqrExpIso;

    static GaussianProcessCovarianceOptimizationProblem() {
      // cumbersome initialization because of ConstrainedValueParameters
      maternIso1 = new CovarianceMaternIso(); SetConstrainedValueParameter(maternIso1.DParameter, 1);
      maternIso3 = new CovarianceMaternIso(); SetConstrainedValueParameter(maternIso3.DParameter, 3);
      maternIso5 = new CovarianceMaternIso(); SetConstrainedValueParameter(maternIso5.DParameter, 5);

      piecewisePoly0 = new CovariancePiecewisePolynomial(); SetConstrainedValueParameter(piecewisePoly0.VParameter, 0);
      piecewisePoly1 = new CovariancePiecewisePolynomial(); SetConstrainedValueParameter(piecewisePoly1.VParameter, 1);
      piecewisePoly2 = new CovariancePiecewisePolynomial(); SetConstrainedValueParameter(piecewisePoly2.VParameter, 2);
      piecewisePoly3 = new CovariancePiecewisePolynomial(); SetConstrainedValueParameter(piecewisePoly3.VParameter, 3);

      poly2 = new CovariancePolynomial(); poly2.DegreeParameter.Value.Value = 2;
      poly3 = new CovariancePolynomial(); poly3.DegreeParameter.Value.Value = 3;

      spectralMixture1 = new CovarianceSpectralMixture(); spectralMixture1.QParameter.Value.Value = 1;
      spectralMixture3 = new CovarianceSpectralMixture(); spectralMixture3.QParameter.Value.Value = 3;
      spectralMixture5 = new CovarianceSpectralMixture(); spectralMixture5.QParameter.Value.Value = 5;

      linear = new CovarianceLinear();
      linearArd = new CovarianceLinearArd();
      neuralNetwork = new CovarianceNeuralNetwork();
      periodic = new CovariancePeriodic();
      ratQuadraticArd = new CovarianceRationalQuadraticArd();
      ratQuadraticIso = new CovarianceRationalQuadraticIso();
      sqrExpArd = new CovarianceSquaredExponentialArd();
      sqrExpIso = new CovarianceSquaredExponentialIso();
    }

    private static void SetConstrainedValueParameter(IConstrainedValueParameter<IntValue> param, int val) {
      param.Value = param.ValidValues.Single(v => v.Value == val);
    }

    #endregion

    #region parameter names

    private const string ProblemDataParameterName = "ProblemData";
    private const string ConstantOptIterationsParameterName = "Constant optimization steps";
    private const string RestartsParameterName = "Restarts";

    #endregion

    #region Parameter Properties
    IParameter IDataAnalysisProblem.ProblemDataParameter { get { return ProblemDataParameter; } }

    public IValueParameter<IRegressionProblemData> ProblemDataParameter {
      get { return (IValueParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IFixedValueParameter<IntValue> ConstantOptIterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[ConstantOptIterationsParameterName]; }
    }
    public IFixedValueParameter<IntValue> RestartsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[RestartsParameterName]; }
    }
    #endregion

    #region Properties

    public IRegressionProblemData ProblemData {
      get { return ProblemDataParameter.Value; }
      set { ProblemDataParameter.Value = value; }
    }
    IDataAnalysisProblemData IDataAnalysisProblem.ProblemData { get { return ProblemData; } }

    public int ConstantOptIterations {
      get { return ConstantOptIterationsParameter.Value.Value; }
      set { ConstantOptIterationsParameter.Value.Value = value; }
    }

    public int Restarts {
      get { return RestartsParameter.Value.Value; }
      set { RestartsParameter.Value.Value = value; }
    }
    #endregion

    public override bool Maximization {
      get { return true; } // return log likelihood (instead of negative log likelihood as in GPR
    }

    // problem stores a few variables for information exchange from Evaluate() to Analyze()
    private readonly object problemStateLocker = new object();
    [Storable]
    private double bestQ;
    [Storable]
    private double[] bestHyperParameters;
    [Storable]
    private IMeanFunction meanFunc;
    [Storable]
    private ICovarianceFunction covFunc;

    public GaussianProcessCovarianceOptimizationProblem()
      : base() {
      Parameters.Add(new ValueParameter<IRegressionProblemData>(ProblemDataParameterName, "The data for the regression problem", new RegressionProblemData()));
      Parameters.Add(new FixedValueParameter<IntValue>(ConstantOptIterationsParameterName, "Number of optimization steps for hyperparameter values", new IntValue(50)));
      Parameters.Add(new FixedValueParameter<IntValue>(RestartsParameterName, "The number of random restarts for constant optimization.", new IntValue(10)));
      Parameters["Restarts"].Hidden = true;
      var g = new SimpleSymbolicExpressionGrammar();
      g.AddSymbols(new string[] { "Sum", "Product" }, 2, 2);
      g.AddTerminalSymbols(new string[]
      {
        "Linear",
        "LinearArd",
        "MaternIso1",
        "MaternIso3",
        "MaternIso5",
        "NeuralNetwork",
        "Periodic",
        "PiecewisePolynomial0",
        "PiecewisePolynomial1",
        "PiecewisePolynomial2",
        "PiecewisePolynomial3",
        "Polynomial2",
        "Polynomial3",
        "RationalQuadraticArd",
        "RationalQuadraticIso",
        "SpectralMixture1",
        "SpectralMixture3",
        "SpectralMixture5",
        "SquaredExponentialArd",
        "SquaredExponentialIso"
      });
      base.Encoding = new SymbolicExpressionTreeEncoding(g, 10, 5);
    }

    public void InitializeState() { ClearState(); }
    public void ClearState() {
      meanFunc = null;
      covFunc = null;
      bestQ = double.NegativeInfinity;
      bestHyperParameters = null;
    }

    private readonly object syncRoot = new object();
    // Does not produce the same result for the same seed when using parallel engine (see below)!
    public override double Evaluate(ISymbolicExpressionTree tree, IRandom random) {
      var meanFunction = new MeanConst();
      var problemData = ProblemData;
      var ds = problemData.Dataset;
      var targetVariable = problemData.TargetVariable;
      var allowedInputVariables = problemData.AllowedInputVariables.ToArray();
      var nVars = allowedInputVariables.Length;
      var trainingRows = problemData.TrainingIndices.ToArray();

      // use the same covariance function for each restart
      var covarianceFunction = TreeToCovarianceFunction(tree);

      // allocate hyperparameters
      var hyperParameters = new double[meanFunction.GetNumberOfParameters(nVars) + covarianceFunction.GetNumberOfParameters(nVars) + 1]; // mean + cov + noise
      double[] bestHyperParameters = new double[hyperParameters.Length];
      var bestObjValue = new double[1] { double.MinValue };

      // data that is necessary for the objective function
      var data = Tuple.Create(ds, targetVariable, allowedInputVariables, trainingRows, (IMeanFunction)meanFunction, covarianceFunction, bestObjValue);

      for (int t = 0; t < Restarts; t++) {
        var prevBest = bestObjValue[0];
        var prevBestHyperParameters = new double[hyperParameters.Length];
        Array.Copy(bestHyperParameters, prevBestHyperParameters, bestHyperParameters.Length);

        // initialize hyperparameters
        hyperParameters[0] = ds.GetDoubleValues(targetVariable).Average(); // mean const

        // Evaluate might be called concurrently therefore access to random has to be synchronized.
        // However, results of multiple runs with the same seed will be different when using the parallel engine.
        lock (syncRoot) {
          for (int i = 0; i < covarianceFunction.GetNumberOfParameters(nVars); i++) {
            hyperParameters[1 + i] = random.NextDouble() * 2.0 - 1.0;
          }
        }
        hyperParameters[hyperParameters.Length - 1] = 1.0; // s² = exp(2), TODO: other inits better?

        // use alglib.bfgs for hyper-parameter optimization ...
        double epsg = 0;
        double epsf = 0.00001;
        double epsx = 0;
        double stpmax = 1;
        int maxits = ConstantOptIterations;
        alglib.mincgstate state;
        alglib.mincgreport rep;

        alglib.mincgcreate(hyperParameters, out state);
        alglib.mincgsetcond(state, epsg, epsf, epsx, maxits);
        alglib.mincgsetstpmax(state, stpmax);
        alglib.mincgoptimize(state, ObjectiveFunction, null, data);

        alglib.mincgresults(state, out bestHyperParameters, out rep);

        if (rep.terminationtype < 0) {
          // error -> restore previous best quality
          bestObjValue[0] = prevBest;
          Array.Copy(prevBestHyperParameters, bestHyperParameters, prevBestHyperParameters.Length);
        }
      }

      UpdateBestSoFar(bestObjValue[0], bestHyperParameters, meanFunction, covarianceFunction);

      return bestObjValue[0];
    }

    // updates the overall best quality and overall best model for Analyze()
    private void UpdateBestSoFar(double bestQ, double[] bestHyperParameters, IMeanFunction meanFunc, ICovarianceFunction covFunc) {
      lock (problemStateLocker) {
        if (bestQ > this.bestQ) {
          this.bestQ = bestQ;
          this.bestHyperParameters = new double[bestHyperParameters.Length];
          Array.Copy(bestHyperParameters, this.bestHyperParameters, this.bestHyperParameters.Length);
          this.meanFunc = meanFunc;
          this.covFunc = covFunc;
        }
      }
    }

    public override void Analyze(ISymbolicExpressionTree[] trees, double[] qualities, ResultCollection results, IRandom random) {
      if (!results.ContainsKey("Best Solution Quality")) {
        results.Add(new Result("Best Solution Quality", typeof(DoubleValue)));
      }
      if (!results.ContainsKey("Best Tree")) {
        results.Add(new Result("Best Tree", typeof(ISymbolicExpressionTree)));
      }
      if (!results.ContainsKey("Best Solution")) {
        results.Add(new Result("Best Solution", typeof(GaussianProcessRegressionSolution)));
      }

      var bestQuality = qualities.Max();

      if (results["Best Solution Quality"].Value == null || bestQuality > ((DoubleValue)results["Best Solution Quality"].Value).Value) {
        var bestIdx = Array.IndexOf(qualities, bestQuality);
        var bestClone = (ISymbolicExpressionTree)trees[bestIdx].Clone();
        results["Best Tree"].Value = bestClone;
        results["Best Solution Quality"].Value = new DoubleValue(bestQuality);
        results["Best Solution"].Value = CreateSolution();
      }
    }

    private IItem CreateSolution() {
      var problemData = ProblemData;
      var ds = problemData.Dataset;
      var targetVariable = problemData.TargetVariable;
      var allowedInputVariables = problemData.AllowedInputVariables.ToArray();
      var trainingRows = problemData.TrainingIndices.ToArray();

      lock (problemStateLocker) {
        var model = new GaussianProcessModel(ds, targetVariable, allowedInputVariables, trainingRows, bestHyperParameters, (IMeanFunction)meanFunc.Clone(), (ICovarianceFunction)covFunc.Clone());
        model.FixParameters();
        return model.CreateRegressionSolution((IRegressionProblemData)ProblemData.Clone());
      }
    }

    private void ObjectiveFunction(double[] x, ref double func, double[] grad, object obj) {
      // we want to optimize the model likelihood by changing the hyperparameters and also return the gradient for each hyperparameter
      var data = (Tuple<IDataset, string, string[], int[], IMeanFunction, ICovarianceFunction, double[]>)obj;
      var ds = data.Item1;
      var targetVariable = data.Item2;
      var allowedInputVariables = data.Item3;
      var trainingRows = data.Item4;
      var meanFunction = data.Item5;
      var covarianceFunction = data.Item6;
      var bestObjValue = data.Item7;
      var hyperParameters = x; // the decision variable vector

      try {
        var model = new GaussianProcessModel(ds, targetVariable, allowedInputVariables, trainingRows, hyperParameters, meanFunction, covarianceFunction);

        func = model.NegativeLogLikelihood; // mincgoptimize, so we return negative likelihood
        bestObjValue[0] = Math.Max(bestObjValue[0], -func); // problem itself is a maximization problem
        var gradients = model.HyperparameterGradients;
        Array.Copy(gradients, grad, gradients.Length);
      }
      catch (ArgumentException) {
        // building the GaussianProcessModel might fail, in this case we return the worst possible objective value
        func = 1.0E+300;
        Array.Clear(grad, 0, grad.Length);
      }
    }

    private ICovarianceFunction TreeToCovarianceFunction(ISymbolicExpressionTree tree) {
      return TreeToCovarianceFunction(tree.Root.GetSubtree(0).GetSubtree(0)); // skip programroot and startsymbol
    }

    private ICovarianceFunction TreeToCovarianceFunction(ISymbolicExpressionTreeNode node) {
      switch (node.Symbol.Name) {
        case "Sum": {
            var sum = new CovarianceSum();
            sum.Terms.Add(TreeToCovarianceFunction(node.GetSubtree(0)));
            sum.Terms.Add(TreeToCovarianceFunction(node.GetSubtree(1)));
            return sum;
          }
        case "Product": {
            var prod = new CovarianceProduct();
            prod.Factors.Add(TreeToCovarianceFunction(node.GetSubtree(0)));
            prod.Factors.Add(TreeToCovarianceFunction(node.GetSubtree(1)));
            return prod;
          }
        // covFunction is cloned by the model so we can reuse instances of terminal covariance functions
        case "Linear": return linear;
        case "LinearArd": return linearArd;
        case "MaternIso1": return maternIso1;
        case "MaternIso3": return maternIso3;
        case "MaternIso5": return maternIso5;
        case "NeuralNetwork": return neuralNetwork;
        case "Periodic": return periodic;
        case "PiecewisePolynomial0": return piecewisePoly0;
        case "PiecewisePolynomial1": return piecewisePoly1;
        case "PiecewisePolynomial2": return piecewisePoly2;
        case "PiecewisePolynomial3": return piecewisePoly3;
        case "Polynomial2": return poly2;
        case "Polynomial3": return poly3;
        case "RationalQuadraticArd": return ratQuadraticArd;
        case "RationalQuadraticIso": return ratQuadraticIso;
        case "SpectralMixture1": return spectralMixture1;
        case "SpectralMixture3": return spectralMixture3;
        case "SpectralMixture5": return spectralMixture5;
        case "SquaredExponentialArd": return sqrExpArd;
        case "SquaredExponentialIso": return sqrExpIso;
        default: throw new InvalidProgramException(string.Format("Found invalid symbol {0}", node.Symbol.Name));
      }
    }


    // persistence
    [StorableConstructor]
    private GaussianProcessCovarianceOptimizationProblem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    // cloning 
    private GaussianProcessCovarianceOptimizationProblem(GaussianProcessCovarianceOptimizationProblem original, Cloner cloner)
      : base(original, cloner) {
      bestQ = original.bestQ;
      meanFunc = cloner.Clone(original.meanFunc);
      covFunc = cloner.Clone(original.covFunc);
      if (bestHyperParameters != null) {
        bestHyperParameters = new double[original.bestHyperParameters.Length];
        Array.Copy(original.bestHyperParameters, bestHyperParameters, bestHyperParameters.Length);
      }
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessCovarianceOptimizationProblem(this, cloner);
    }

    public void Load(IRegressionProblemData data) {
      this.ProblemData = data;
      OnProblemDataChanged();
    }

    public IRegressionProblemData Export() {
      return ProblemData;
    }

    #region events
    public event EventHandler ProblemDataChanged;


    private void OnProblemDataChanged() {
      var handler = ProblemDataChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    #endregion

  }
}
