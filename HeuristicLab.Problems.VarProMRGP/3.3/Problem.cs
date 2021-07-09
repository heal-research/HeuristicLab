#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.VarProMRGP {
  [Item("VarPro Multi-Regression Genetic Programming", "Similar to MRGP but MRGP is a inappropriate name, we should think about a new name.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 999)]
  [StorableType("8B84830E-0DEB-44FD-B7E8-6DA2F64C0FF2")]
  public sealed class Problem : SingleObjectiveBasicProblem<BinaryVectorEncoding>, IProblemInstanceConsumer<IRegressionProblemData> {

    public IValueParameter<IRegressionProblemData> RegressionProblemDataParameter => (IValueParameter<IRegressionProblemData>)Parameters["ProblemData"];
    public IValueParameter<VarProGrammar> GrammarParameter => (IValueParameter<VarProGrammar>)Parameters["Grammar"];
    public IFixedValueParameter<IntValue> MaxDepthParameter => (IFixedValueParameter<IntValue>)Parameters["MaxDepth"];
    public IFixedValueParameter<IntValue> MaxSizeParameter => (IFixedValueParameter<IntValue>)Parameters["MaxSize"];
    public OptionalValueParameter<ReadOnlyItemArray<StringValue>> FeaturesParameter => (OptionalValueParameter<ReadOnlyItemArray<StringValue>>)Parameters["Features"];
    public OptionalValueParameter<BinaryVector> BestKnownSolutionParameter => (OptionalValueParameter<BinaryVector>)Parameters["BestKnownSolution"];
    public IRegressionProblemData RegressionProblemData {
      get => RegressionProblemDataParameter.Value;
      set => RegressionProblemDataParameter.Value = value;
    }
    public VarProGrammar Grammar {
      get => GrammarParameter.Value;
      set => GrammarParameter.Value = value;
    }

    public int MaxSize {
      get => MaxSizeParameter.Value.Value;
      set => MaxSizeParameter.Value.Value = value;
    }

    public int MaxDepth {
      get => MaxDepthParameter.Value.Value;
      set => MaxDepthParameter.Value.Value = value;
    }

    public ReadOnlyItemArray<StringValue> Features {
      get => FeaturesParameter.Value;
      private set {
        FeaturesParameter.ReadOnly = false;
        FeaturesParameter.Value = value;
        FeaturesParameter.ReadOnly = true;
      }
    }

    public BinaryVector BestKnownSolution {
      get => BestKnownSolutionParameter.Value;
      set => BestKnownSolutionParameter.Value = value;
    }


    public override bool Maximization => false;
    // public override bool[] Maximization => new[] { false, false };


    #region not cloned or stored
    ISymbolicExpressionTree[] features;
    private List<TreeToAutoDiffTermConverter.ParametricFunctionGradient> featCode; // AutoDiff delegates for the features
    private List<double[]> featParam; // parameters for the features
    private List<double[][]> featVariables;
    #endregion


    [StorableConstructor]
    private Problem(StorableConstructorFlag _) : base(_) { }
    private Problem(Problem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    public Problem() {
      var g = new VarProGrammar();

      // TODO optionally: scale dataset

      Parameters.Add(new ValueParameter<IRegressionProblemData>("ProblemData", "", new RegressionProblemData()));
      Parameters.Add(new ValueParameter<VarProGrammar>("Grammar", "", g));
      Parameters.Add(new FixedValueParameter<IntValue>("MaxSize", "", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<IntValue>("MaxDepth", "", new IntValue(6)));
      Parameters.Add(new OptionalValueParameter<ReadOnlyItemArray<StringValue>>("Features", "autogenerated"));
      Parameters.Add(new OptionalValueParameter<BinaryVector>("BestKnownSolution", ""));
      FeaturesParameter.ReadOnly = true;

      Encoding = new BinaryVectorEncoding("b");
      Encoding.Length = 10000; // default for number of features

      g.ConfigureVariableSymbols(RegressionProblemData);

      InitializeOperators();
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Problem(this, cloner);
    }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    #region event handling
    // Dependencies of parameters and fields
    // ProblemData
    //   |
    // Grammar             MaxSize           MaxDepth          MaxInteractions
    //   |                    |                 |                 |
    //   +--------------------+-----------------+-----------------+
    //   |
    //  Features
    //   Code
    //   |
    //  Encoding (Length)
    //   |
    //   +--------------------+
    //   |                    |
    // BestKnownSolution      Operators (Parameter)
    // BestKnownQuality

    private void RegisterEventHandlers() {
      RegressionProblemDataParameter.ValueChanged += RegressionProblemDataParameter_ValueChanged;
      RegressionProblemData.Changed += RegressionProblemData_Changed;
      GrammarParameter.ValueChanged += GrammarParameter_ValueChanged;
      Grammar.Changed += Grammar_Changed;
      MaxSizeParameter.Value.ValueChanged += Value_ValueChanged;
      MaxDepthParameter.Value.ValueChanged += Value_ValueChanged;
      FeaturesParameter.ValueChanged += FeaturesParameter_ValueChanged;
    }

    private void FeaturesParameter_ValueChanged(object sender, EventArgs e) {
      if (Encoding.Length != Features.Length) {
        Encoding.Length = Features.Length;
        OnEncodingChanged();
      }
    }

    private void Value_ValueChanged(object sender, EventArgs e) {
      UpdateFeaturesAndCode();
    }

    private void Grammar_Changed(object sender, EventArgs e) {
      UpdateFeaturesAndCode();
    }

    private void GrammarParameter_ValueChanged(object sender, EventArgs e) {
      Grammar.Changed += Grammar_Changed;
      UpdateFeaturesAndCode();
    }

    private void RegressionProblemData_Changed(object sender, EventArgs e) {
      Grammar.ConfigureVariableSymbols(RegressionProblemData);
    }

    private void RegressionProblemDataParameter_ValueChanged(object sender, EventArgs e) {
      RegressionProblemData.Changed += RegressionProblemData_Changed;
      Grammar.ConfigureVariableSymbols(RegressionProblemData);
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      OnReset();
      ParameterizeOperators();
    }

    protected override void OnReset() {
      base.OnReset();
      BestKnownQualityParameter.ActualValue = null;
      BestKnownSolutionParameter.ActualValue = null;
    }

    private void UpdateFeaturesAndCode() {
      features = GenerateFeaturesSystematic(10000, new MersenneTwister(31415), Grammar, MaxSize, MaxDepth, maxVariables: 3);
      GenerateCode(features, RegressionProblemData);
      var formatter = new InfixExpressionFormatter();
      Features = new ItemArray<StringValue>(features.Select(fi => new StringValue(formatter.Format(fi, System.Globalization.NumberFormatInfo.InvariantInfo, formatString: "0.0")).AsReadOnly())).AsReadOnly();
    }


    #endregion

    public override double Evaluate(Individual individual, IRandom random) {
      if (featCode == null) {
        UpdateFeaturesAndCode();
      }
      var b = individual.BinaryVector(Encoding.Name);

      var rows = RegressionProblemData.TrainingIndices.ToArray();

      var allRows = rows.ToArray();
      var nRows = allRows.Length;
      var termIndexList = new List<int>();
      for (int i = 0; i < b.Length; i++) {
        if (b[i] == true) {
          termIndexList.Add(i);
        }
      }

      var oldParameterValues = ExtractParameters(termIndexList);
      var alpha = (double[])oldParameterValues.Clone();

      var target = RegressionProblemData.TargetVariableTrainingValues.ToArray();

      // local function for feature evaluation
      void Phi(double[] a, ref double[,] phi) {
        if (phi == null) {
          phi = new double[nRows, termIndexList.Count + 1]; // + offset term
          // last term is constant = 1
          for (int i = 0; i < nRows; i++)
            phi[i, termIndexList.Count] = 1.0;
        }
        var offset = 0;
        // for each term
        for (int i = 0; i < termIndexList.Count; i++) {
          var termIdx = termIndexList[i];
          var numFeatParam = this.featParam[termIdx].Length;
          var variableValues = new double[featVariables[termIdx].Length];
          var featParam = new double[numFeatParam];
          Array.Copy(a, offset, featParam, 0, featParam.Length);
          // for each row
          for (int j = 0; j < nRows; j++) {
            // copy row values
            for (int k = 0; k < variableValues.Length; k++) {
              variableValues[k] = featVariables[termIdx][k][j]; // featVariables is column-order
            }
            var tup = featCode[termIdx].Invoke(featParam, variableValues); // TODO for phi we do not actually need g
            phi[j, i] = tup.Item2;
          }
          offset += numFeatParam;
        }
      }

      // local function for Jacobian evaluation
      void Jac(double[] a, ref double[,] J, ref int[,] ind) {
        if (J == null) {
          J = new double[nRows, featParam.Sum(fp => fp.Length)]; // all parameters
          ind = new int[2, featParam.Sum(fp => fp.Length)];
        }
        var offset = 0;
        // for each term
        for (int i = 0; i < termIndexList.Count; i++) {
          var termIdx = termIndexList[i];
          var numFeatParam = this.featParam[termIdx].Length;
          var variableValues = new double[featVariables[termIdx].Length];
          var featParam = new double[numFeatParam];
          Array.Copy(a, offset, featParam, 0, featParam.Length);

          // for each parameter
          for (int k = 0; k < featParam.Length; k++) {
            ind[0, offset + k] = i; // column idx in phi
            ind[1, offset + k] = offset + k; // parameter idx (no parameter is used twice)
          }

          // for each row
          for (int j = 0; j < nRows; j++) {
            // copy row values
            for (int k = 0; k < variableValues.Length; k++) {
              variableValues[k] = featVariables[termIdx][k][j]; // featVariables is column-order
            }
            var tup = featCode[termIdx].Invoke(featParam, variableValues);
            // for each parameter
            for (int k = 0; k < featParam.Length; k++) {
              J[j, offset + k] = tup.Item1[k];
            }
          }
          offset += numFeatParam;
        }
      }

      try {
        HEAL.VarPro.VariableProjection.Fit(Phi, Jac, target, alpha, out var coeff, out var report);


        if (report.residNorm < 0) throw new InvalidProgramException();
        UpdateParameter(termIndexList, alpha);


        individual["Parameter"] = new DoubleArray(alpha); // store the parameter which worked for this individual for solution creation
        individual["Coeff"] = new DoubleArray(coeff);

        return report.residNormSqr / nRows;
      } catch (Exception _) {
        individual["Parameter"] = new DoubleArray(alpha); // store the parameter which worked for this individual for solution creation
        individual["Coeff"] = new DoubleArray(termIndexList.Count + 1);
        return double.MaxValue;
      }
    }


    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(individuals, qualities, results, random);

      var orderedIndividuals = individuals.Zip(qualities, (i, q) => new { Individual = i, Quality = q }).OrderBy(z => z.Quality);
      var bestIndividual = orderedIndividuals.First().Individual;
      var bestQ = orderedIndividuals.First().Quality;
      if (double.IsNaN(BestKnownQuality) || bestQ < BestKnownQuality) {
        BestKnownQuality = bestQ;
        BestKnownSolution = bestIndividual.BinaryVector(Encoding.Name);
      }

      var curBestQuality = results.ContainsKey("BestQuality") ? ((DoubleValue)results["BestQuality"].Value).Value : double.NaN;
      if (double.IsNaN(curBestQuality) || bestQ < curBestQuality) {
        var bestVector = bestIndividual.BinaryVector(Encoding.Name);
        var bestParams = ((DoubleArray)bestIndividual["Parameter"]).ToArray();
        var bestCoeff = ((DoubleArray)bestIndividual["Coeff"]).ToArray();
        // var rows = RegressionProblemData.TrainingIndices.ToArray();
        // var target = RegressionProblemData.TargetVariableTrainingValues.ToArray();
        // 
        // var rowsArray = rows.ToArray();
        // var nRows = rowsArray.Length;
        // var result = new double[nRows];
        var termIndexList = new List<int>();
        // var predictorNames = new List<string>();
        for (int i = 0; i < bestVector.Length; i++) {
          if (bestVector[i] == true) {
            termIndexList.Add(i);
          }
        }

        results.AddOrUpdateResult("Solution", CreateRegressionSolution(termIndexList.ToArray(), bestParams, bestCoeff, RegressionProblemData));
        results.AddOrUpdateResult("BestQuality", new DoubleValue(bestQ));
      }
    }

    #region retrieval / update of non-linear parameters
    private double[] ExtractParameters(List<int> termIndexList) {
      var p = new List<double>();
      for (int i = 0; i < termIndexList.Count; i++) {
        p.AddRange(featParam[termIndexList[i]]);
      }
      return p.ToArray();
    }


    // parameters are given as a flat array
    private void UpdateParameter(List<int> termIndexList, double[] p) {
      var offset = 0;
      for (int i = 0; i < termIndexList.Count; i++) {
        var numFeatParam = featParam[termIndexList[i]].Length;
        Array.Copy(p, offset, featParam[termIndexList[i]], 0, numFeatParam);
        offset += numFeatParam;
      }
    }
    #endregion

    #region feature generation
    /*
    private static ISymbolicExpressionTree[] GenerateFeatures(int n, IRandom random, ISymbolicDataAnalysisGrammar grammar, int maxSize, int maxDepth) {
      var features = new ISymbolicExpressionTree[n];
      var hashes = new HashSet<ulong>();
      int i = 0;
      while (i < features.Length) {
        var t = ProbabilisticTreeCreator.Create(random, grammar, maxSize, maxDepth);
        t = TreeSimplifier.Simplify(t);
        var th = SymbolicExpressionTreeHash.ComputeHash(t);
        if (!hashes.Contains(th)) {
          features[i++] = t;
          hashes.Add(th);
        }
      }
      return features;
    }
    */
    private static ISymbolicExpressionTree[] GenerateFeaturesSystematic(int n, IRandom random, ISymbolicDataAnalysisGrammar grammar, int maxSize, int maxDepth, int maxVariables) {
      var hashes = new HashSet<ulong>();

      var root = grammar.ProgramRootSymbol.CreateTreeNode();
      var trees = new List<ISymbolicExpressionTreeNode>();
      var incompleteTrees = new Queue<ISymbolicExpressionTreeNode>();
      incompleteTrees.Enqueue(root);
      while (incompleteTrees.Any() && trees.Count < n) {
        var t = incompleteTrees.Dequeue();
        // find first extension point
        ISymbolicExpressionTreeNode parent = null;
        var numVariables = 0;
        int size = 0;
        int depth = t.GetDepth();
        foreach (var node in t.IterateNodesPrefix()) {
          size++;
          if (node is VariableTreeNodeBase) numVariables++;
          if (node.SubtreeCount < grammar.GetMinimumSubtreeCount(node.Symbol)) {
            parent = node;
            break;
          }
        }
        if (numVariables > maxVariables || size > maxSize || depth > maxDepth) continue;
        if (parent == null) {
          // no extension point found => sentence is complete
          var hash = SymbolicExpressionTreeHash.ComputeHash(t);
          if (hashes.Add(SymbolicExpressionTreeHash.ComputeHash(t))) {
            trees.Add((ISymbolicExpressionTreeNode)t.Clone());
          }

          // check if the (complete) sentence can be extended
          foreach (var node in t.IterateNodesPrefix()) {
            if (node.SubtreeCount < grammar.GetMaximumSubtreeCount(node.Symbol)) {
              parent = node;
              break;
            }
          }
          if (parent == null) {
            // no extension possible => continue with next tree in queue
            continue;
          }
        }

        if (parent == null) throw new InvalidProgramException(); // assertion

        // the sentence must / can be extended
        var allowedChildSy = grammar.GetAllowedChildSymbols(parent.Symbol, parent.SubtreeCount).OrderBy(sy => sy.MinimumArity == 0 ? 0 : 1); // terminals first
        if (!allowedChildSy.Any()) throw new InvalidProgramException(); // grammar fail

        // make new variants and add them to the queue of incomplete trees
        foreach (var sy in allowedChildSy) {
          if (sy is DataAnalysis.Symbolic.Variable variableSy) {
            // generate all variables
            foreach (var varName in variableSy.VariableNames) {
              var varNode = (VariableTreeNode)variableSy.CreateTreeNode();
              varNode.ResetLocalParameters(random);
              varNode.VariableName = varName;
              varNode.Weight = 1.0;
              parent.AddSubtree(varNode);
              incompleteTrees.Enqueue((ISymbolicExpressionTreeNode)t.Clone());
              parent.RemoveSubtree(parent.SubtreeCount - 1); // prepare for next iteration
            }
          } else {
            var node = sy.CreateTreeNode();
            node.ResetLocalParameters(random);
            parent.AddSubtree(node);
            incompleteTrees.Enqueue((ISymbolicExpressionTreeNode)t.Clone());
            parent.RemoveSubtree(parent.SubtreeCount - 1); // prepare for next iteration
          }
        }

      }
      return trees.Select(r => new SymbolicExpressionTree(r)).ToArray();
    }


    private void GenerateCode(ISymbolicExpressionTree[] features, IRegressionProblemData problemData) {
      this.featCode = new List<TreeToAutoDiffTermConverter.ParametricFunctionGradient>();
      this.featParam = new List<double[]>();
      this.featVariables = new List<double[][]>();
      foreach (var f in features) {
        var featureCode = Compile(f, problemData, out var initialParamValues, out var variableValues);

        featCode.Add(featureCode);
        featParam.Add(initialParamValues);
        featVariables.Add(variableValues);
      }
    }


    private static readonly HashSet<byte> supportedOpCodes = new HashSet<byte>() {
      (byte)OpCode.Constant,
      (byte)OpCode.Variable,
      (byte)OpCode.Add,
      (byte)OpCode.Sub,
      (byte)OpCode.Mul,
      (byte)OpCode.Div,
      (byte)OpCode.Exp,
      (byte)OpCode.Log,
      (byte)OpCode.Sin,
      (byte)OpCode.Cos,
      (byte)OpCode.Tan,
      (byte)OpCode.Tanh,
      // (byte)OpCode.Power,
      // (byte)OpCode.Root,
      (byte)OpCode.SquareRoot,
      (byte)OpCode.Square,
      (byte)OpCode.CubeRoot,
      (byte)OpCode.Cube,
      (byte)OpCode.Absolute,
      (byte)OpCode.AnalyticQuotient
    };
    private TreeToAutoDiffTermConverter.ParametricFunctionGradient Compile(ISymbolicExpressionTree tree, IRegressionProblemData problemData,
      out double[] initialParameterValues, out double[][] variableValues) {
      TreeToAutoDiffTermConverter.TryConvertToAutoDiff(tree, makeVariableWeightsVariable: false, addLinearScalingTerms: false,
        out var parameters, out initialParameterValues, out var func, out var func_grad);
      variableValues = new double[parameters.Count][];
      for (int i = 0; i < parameters.Count; i++) {
        variableValues[i] = problemData.Dataset.GetDoubleValues(parameters[i].variableName, problemData.TrainingIndices).ToArray(); // TODO: we could reuse the arrays
      }
      return func_grad;
    }
    #endregion

    #region solution creation
    private IRegressionSolution CreateRegressionSolution(int[] featIdx, double[] parameters, double[] coefficients, IRegressionProblemData problemData) {
      var root = (new ProgramRootSymbol()).CreateTreeNode();
      var start = (new StartSymbol()).CreateTreeNode();
      var add = (new Addition()).CreateTreeNode();
      root.AddSubtree(start);
      start.AddSubtree(add);
      var offset = 0;
      for (int i = 0; i < featIdx.Length; i++) {
        var term = (ISymbolicExpressionTreeNode)features[featIdx[i]].Root.GetSubtree(0).GetSubtree(0).Clone();

        var termParameters = new double[featParam[featIdx[i]].Length];
        Array.Copy(parameters, offset, termParameters, 0, termParameters.Length);
        ReplaceParameters(term, termParameters);
        offset += termParameters.Length;

        var mul = (new Multiplication()).CreateTreeNode();
        mul.AddSubtree(term);
        mul.AddSubtree(CreateConstant(coefficients[i]));
        add.AddSubtree(mul);
      }
      // last coeff is offset
      add.AddSubtree(CreateConstant(coefficients[coefficients.Length - 1]));

      var tree = new SymbolicExpressionTree(root);
      var ds = problemData.Dataset;
      var scaledDataset = new Dataset(ds.DoubleVariables, ds.ToArray(ds.DoubleVariables, Enumerable.Range(0, ds.Rows)));
      var scaledProblemData = new RegressionProblemData(scaledDataset, problemData.AllowedInputVariables, problemData.TargetVariable);
      scaledProblemData.TrainingPartition.Start = problemData.TrainingPartition.Start;
      scaledProblemData.TrainingPartition.End = problemData.TrainingPartition.End;
      scaledProblemData.TestPartition.Start = problemData.TestPartition.Start;
      scaledProblemData.TestPartition.End = problemData.TestPartition.End;
      return new SymbolicRegressionSolution(
        new SymbolicRegressionModel(problemData.TargetVariable, tree, new SymbolicDataAnalysisExpressionTreeNativeInterpreter()), scaledProblemData);
    }

    private void ReplaceParameters(ISymbolicExpressionTreeNode term, double[] termParameters) {
      // Autodiff converter extracts parameters using a pre-order tree traversal.
      // Therefore, we must use a pre-order tree traversal here as well.
      // Only ConstantTreeNode values are optimized.
      var paramIdx = 0;
      foreach (var node in term.IterateNodesPrefix().OfType<ConstantTreeNode>()) {
        node.Value = termParameters[paramIdx++];
      }
      if (paramIdx != termParameters.Length) throw new InvalidProgramException();
    }

    private ISymbolicExpressionTreeNode CreateConstant(double coeff) {
      var constNode = (ConstantTreeNode)(new Constant()).CreateTreeNode();
      constNode.Value = coeff;
      return constNode;
    }

    Dictionary<byte, Symbol> symbols = new Dictionary<byte, Symbol>() {
      {(byte)OpCode.Add, new Addition()  },
      {(byte)OpCode.Sub, new Subtraction()  },
      {(byte)OpCode.Mul, new Multiplication()  },
      {(byte)OpCode.Div, new Division()  },
      {(byte)OpCode.Exp, new Exponential()  },
      {(byte)OpCode.Log, new Logarithm()  },
      {(byte)OpCode.Sin, new Sine()  },
      {(byte)OpCode.Cos, new Cosine()  },
      {(byte)OpCode.Tan, new Tangent()  },
      {(byte)OpCode.Tanh, new HyperbolicTangent()  },
      {(byte)OpCode.Square, new Square()  },
      {(byte)OpCode.SquareRoot, new SquareRoot()  },
      {(byte)OpCode.Cube, new Cube()  },
      {(byte)OpCode.CubeRoot, new CubeRoot()  },
      {(byte)OpCode.Absolute, new Absolute()  },
      {(byte)OpCode.AnalyticQuotient, new AnalyticQuotient()  },
    };

    // used for solutions only
    Symbol constSy = new Constant();
    Symbol varSy = new DataAnalysis.Symbolic.Variable();


    #endregion

    public void Load(IRegressionProblemData data) {
      RegressionProblemData = data;
    }

    private void InitializeOperators() {
      Operators.Add(new AlleleFrequencyAnalyzer());

      // var cvMSEAnalyzer = new BestAverageWorstQualityAnalyzer();
      // cvMSEAnalyzer.Name = "CVMSE Analzer";
      // ParameterizeAnalyzer(cvMSEAnalyzer, "CV MSE (avg)");
      // Operators.Add(cvMSEAnalyzer);
      // 
      // var trainingMSEAnalyzer = new BestAverageWorstQualityAnalyzer();
      // trainingMSEAnalyzer.Name = "Training MSE Analzer";
      // ParameterizeAnalyzer(trainingMSEAnalyzer, "Train MSE (avg)");
      // Operators.Add(trainingMSEAnalyzer);

      ParameterizeOperators();
    }

    private void ParameterizeAnalyzer(BestAverageWorstQualityAnalyzer analyzer, string qualityName) {
      analyzer.QualityParameter.ActualName = qualityName;
      analyzer.QualitiesParameter.ActualName = qualityName + " " + analyzer.QualitiesParameter.ActualName;
      analyzer.BestQualityParameter.ActualName += " " + qualityName;
      analyzer.CurrentAverageQualityParameter.ActualName += " " + qualityName;
      analyzer.CurrentBestQualityParameter.ActualName += " " + qualityName;
      analyzer.CurrentWorstQualityParameter.ActualName += " " + qualityName;
      analyzer.BestKnownQualityParameter.ActualName += " " + qualityName;
      analyzer.AbsoluteDifferenceBestKnownToBestParameter.ActualName += " " + qualityName;
      analyzer.RelativeDifferenceBestKnownToBestParameter.ActualName += " " + qualityName;
    }

    private void ParameterizeOperators() {
      foreach (var op in Operators) {
        if (op is AlleleFrequencyAnalyzer alleleAnalyzer) {
          alleleAnalyzer.SolutionParameter.ActualName = Encoding.Name;
        }
        if (op is MultiAnalyzer multiAnalyzer) {
          var freqAnalyzer = Operators.OfType<AlleleFrequencyAnalyzer>().First();
          multiAnalyzer.Operators.SetItemCheckedState(freqAnalyzer, true);
        }
      }
      foreach (var op in Encoding.Operators) {
        if (op is SomePositionsBitflipManipulator multiFlipManipulator) {
          multiFlipManipulator.MutationProbabilityParameter.Value.Value = 1.0 / Encoding.Length; // one feature on average
        } else if (op is RandomBinaryVectorCreator creator) {
          creator.TrueProbability.Value = 20.0 / Encoding.Length; // 20 features on average
        }
      }
    }
  }
}
