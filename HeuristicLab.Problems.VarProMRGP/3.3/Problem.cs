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
using HeuristicLab.Analysis.Statistics;

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
    private int[] featureIdx = null; // code[featureIdx[i]] is the last operation of feature i
    [ExcludeFromObjectGraphTraversal]
    private NativeInterpreter.NativeInstruction[] code = null;
    private Dictionary<string, GCHandle> cachedData = null;
    private LinearTransformation[] transformations;
    #endregion


    [StorableConstructor]
    private Problem(StorableConstructorFlag _) : base(_) { }
    private Problem(Problem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    public Problem() {
      var g = new VarProGrammar();

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
    //   +------------------+
    //   |                  |
    // cachedData         Grammar             MaxSize           MaxDepth          MaxInteractions
    // cachedDataSet        |                    |                 |                 |
    //                      +--------------------+-----------------+-----------------+
    //                      |
    //                     Features
    //                      Code
    //                      |
    //                     Encoding (Length)
    //                      |
    //                      +--------------------+
    //                      |                    |
    //                   BestKnownSolution      Operators (Parameter)
    //                   BestKnownQuality

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
      UpdateDataCache();
      Grammar.ConfigureVariableSymbols(RegressionProblemData);
    }

    private void RegressionProblemDataParameter_ValueChanged(object sender, EventArgs e) {
      RegressionProblemData.Changed += RegressionProblemData_Changed;
      UpdateDataCache();
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

    private void UpdateDataCache() {
      if (cachedData != null) {
        foreach (var gch in cachedData.Values) {
          gch.Free();
        }
        cachedData = null;
      }

      var dataset = RegressionProblemData.Dataset;

      // free handles to old data
      if (cachedData != null) {
        foreach (var gch in cachedData.Values) {
          gch.Free();
        }
        cachedData = null;
      }

      // cache new data
      cachedData = new Dictionary<string, GCHandle>();
      transformations = new LinearTransformation[dataset.DoubleVariables.Count()];
      int varIdx = 0;
      foreach (var v in dataset.DoubleVariables) {
        var values = dataset.GetDoubleValues(v).ToArray();
        if (v == RegressionProblemData.TargetVariable) {
          // do not scale target
          var linTransform = new LinearTransformation(dataset.DoubleVariables);
          linTransform.Addend = 0;
          linTransform.Multiplier = 1.0;
          transformations[varIdx++] = linTransform;
        } else {
          // Scale to 0 .. 1
          var max = values.Max();
          var min = values.Min();
          var range = max - min;
          var linTransform = new LinearTransformation(dataset.DoubleVariables);
          transformations[varIdx++] = linTransform;
          linTransform.Addend = -1 * min / range - 0.0;
          linTransform.Multiplier = 1.0 / range;
          for (int i = 0; i < values.Length; i++) {
            values[i] = values[i] * linTransform.Multiplier + linTransform.Addend;
          }
        }
        var gch = GCHandle.Alloc(values, GCHandleType.Pinned);
        cachedData[v] = gch;
      }
    }

    private void UpdateFeaturesAndCode() {
      var features = GenerateFeaturesSystematic(10000, new MersenneTwister(31415), Grammar, MaxSize, MaxDepth, maxVariables: 3);
      GenerateCode(features, RegressionProblemData.Dataset);
      var formatter = new InfixExpressionFormatter();
      Features = new ItemArray<StringValue>(features.Select(fi => new StringValue(formatter.Format(fi, System.Globalization.NumberFormatInfo.InvariantInfo, formatString: "0.0")).AsReadOnly())).AsReadOnly();
    }


    #endregion

    public override double Evaluate(Individual individual, IRandom random) {
      if (cachedData == null || code == null) {
        // after loading from file or cloning the data cache and code must be restored
        UpdateDataCache();
        UpdateFeaturesAndCode();
      }
      var b = individual.BinaryVector(Encoding.Name);
      var fittingOptions = new NativeInterpreter.SolverOptions();
      fittingOptions.Iterations = 10;
      fittingOptions.Algorithm = NativeInterpreter.Algorithm.Krogh;
      fittingOptions.LinearSolver = NativeInterpreter.CeresTypes.LinearSolverType.DENSE_QR;
      fittingOptions.Minimizer = NativeInterpreter.CeresTypes.MinimizerType.TRUST_REGION;
      fittingOptions.TrustRegionStrategy = NativeInterpreter.CeresTypes.TrustRegionStrategyType.LEVENBERG_MARQUARDT;

      var evalOptions = new NativeInterpreter.SolverOptions();
      evalOptions.Iterations = 0;
      evalOptions.Algorithm = NativeInterpreter.Algorithm.Krogh;
      evalOptions.LinearSolver = NativeInterpreter.CeresTypes.LinearSolverType.DENSE_QR;
      evalOptions.Minimizer = NativeInterpreter.CeresTypes.MinimizerType.TRUST_REGION;
      evalOptions.TrustRegionStrategy = NativeInterpreter.CeresTypes.TrustRegionStrategyType.LEVENBERG_MARQUARDT;

      var rows = RegressionProblemData.TrainingIndices.ToArray();

      var allRows = rows.ToArray();
      var nRows = allRows.Length;
      var termIndexList = new List<int>();
      for (int i = 0; i < b.Length; i++) {
        if (b[i] == true) {
          termIndexList.Add(featureIdx[i]);
        }
      }

      var coefficients = new double[termIndexList.Count + 1]; // coefficients for all terms + offset 
                                                              // var avgCoefficients = new double[Features.Length];

      // 5-fold CV
      var nFolds = 5;
      var nFoldRows = nRows / nFolds;
      var fittingRows = new int[(nFolds - 1) * nFoldRows];
      var fittingResult = new double[fittingRows.Length];

      var testRows = new int[nFoldRows];
      var testResult = new double[nFoldRows];
      var cvMSE = new List<double>();
      var trainMSE = new List<double>();
      for (int testFold = 0; testFold < nFolds; testFold++) {
        // TODO: THREAD SAFETY FOR PARALLEL EVALUATION
        // CODE IS MANIPULATED
        lock (code) {
          var oldParameterValues = ExtractParameters(code, termIndexList);

          #region fit to training folds
          // copy rows from the four training folds
          var nextFoldIdx = 0;
          for (int fold = 0; fold < nFolds; fold++) {
            if (fold == testFold) {
              Array.Copy(allRows, fold * nFoldRows, testRows, 0, nFoldRows);
            } else {
              Array.Copy(allRows, fold * nFoldRows, fittingRows, nextFoldIdx, nFoldRows);
              nextFoldIdx += nFoldRows;
            }
          }

          var fittingTarget = RegressionProblemData.Dataset.GetDoubleValues(RegressionProblemData.TargetVariable, fittingRows).ToArray();

          Array.Clear(fittingResult, 0, fittingResult.Length);
          Array.Clear(coefficients, 0, coefficients.Length);


          NativeInterpreter.NativeWrapper.GetValuesVarPro(code, code.Length, termIndexList.ToArray(), termIndexList.Count, fittingRows, fittingRows.Length,
            coefficients, fittingOptions, fittingResult, fittingTarget, out var optSummary);
          #endregion

          if (optSummary.InitialCost < 0 || optSummary.FinalCost < 0) throw new InvalidProgramException();
          trainMSE.Add(optSummary.FinalCost * 2.0 / fittingRows.Length); // ceres cost is 0.5 * sum of squared residuals
                                                                         // average of all coefficients
                                                                         // TODO return a statistic for the relevance of the term instead of the coefficient
                                                                         // for (int k = 0; k < termIndexList.Count; k++) {
                                                                         //   avgCoefficients[Array.IndexOf(featureIdx, termIndexList[k])] += coefficients[k] / nFolds; // TODO perf
                                                                         // }

          #region calculate output on test fold to determine CV MSE

          // evaluate all terms for test rows
          var sumOutput = new double[testResult.Length];
          for (int r = 0; r < sumOutput.Length; r++) {
            sumOutput[r] = coefficients[coefficients.Length - 1]; // offset
          }

          for (int k = 0; k < termIndexList.Count; k++) {
            var termIdx = termIndexList[k];
            // copy relevant part of the code
            var termCode = new NativeInterpreter.NativeInstruction[code[termIdx].length];
            Array.Copy(code, termIdx - termCode.Length + 1, termCode, 0, termCode.Length);
            NativeInterpreter.NativeWrapper.GetValues(termCode, termCode.Length, testRows, testRows.Length, evalOptions, testResult, null, out var evalSummary);

            for (int r = 0; r < sumOutput.Length; r++) {
              sumOutput[r] += coefficients[k] * testResult[r];
            }
          }

          var evalTarget = RegressionProblemData.Dataset.GetDoubleValues(RegressionProblemData.TargetVariable, testRows);
          if (sumOutput.Any(d => double.IsNaN(d))) cvMSE.Add(evalTarget.VariancePop()); // variance of target is MSE of constant model
          else cvMSE.Add(sumOutput.Zip(evalTarget, (ri, ti) => { var res = ti - ri; return res * res; }).Average());

          #endregion

          #region prepare for next varpro call
          var newParameterValues = ExtractParameters(code, termIndexList);
          // keep the updated parameter values only when the coefficient is significantly <> 0
          for (int i = 0; i < termIndexList.Count; i++) {
            // TODO would actually need to take variance of term into account
            if (Math.Abs(coefficients[i]) > 1e-5) {
              oldParameterValues[i] = newParameterValues[i];
            }
          }
          UpdateParameter(code, termIndexList, oldParameterValues);
        }
        #endregion
      }


      individual["Train MSE (avg)"] = new DoubleValue(trainMSE.Average());
      individual["Parameter"] = new DoubleArray(ExtractParameters(code, termIndexList).SelectMany(arr => arr).ToArray()); // store the parameter which worked for this individual for solution creation
                                                                                                                          // individual["Coefficients"] = new DoubleArray(avgCoefficients); // for allele frequency analysis
                                                                                                                          // return new double[] { avgCost, coefficients.Count(ci => Math.Abs(ci) > 1e-9) };
                                                                                                                          // average of averages 
      individual["CV MSE (avg)"] = new DoubleValue(cvMSE.Average());
      individual["CV MSE (stdev)"] = new DoubleValue(cvMSE.StandardDeviation());
      return cvMSE.Average() + cvMSE.StandardDeviation();
      // return new double[] { cvMSE.Average() + cvMSE.StandardDeviation(), termIndexList.Count };
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

        var options = new NativeInterpreter.SolverOptions();
        options.Iterations = 10; // optimize on full dataset
        options.Algorithm = NativeInterpreter.Algorithm.Krogh;
        options.LinearSolver = NativeInterpreter.CeresTypes.LinearSolverType.DENSE_QR;
        options.Minimizer = NativeInterpreter.CeresTypes.MinimizerType.TRUST_REGION;
        options.TrustRegionStrategy = NativeInterpreter.CeresTypes.TrustRegionStrategyType.LEVENBERG_MARQUARDT;

        var rows = RegressionProblemData.TrainingIndices.ToArray();
        var target = RegressionProblemData.TargetVariableTrainingValues.ToArray();

        var rowsArray = rows.ToArray();
        var nRows = rowsArray.Length;
        var result = new double[nRows];
        var termIndexList = new List<int>();
        var predictorNames = new List<string>();
        for (int i = 0; i < bestVector.Length; i++) {
          if (bestVector[i] == true) {
            termIndexList.Add(featureIdx[i]);
            predictorNames.Add(Features[i].Value);
          }
        }
        var coefficients = new double[termIndexList.Count + 1]; // coefficients for all terms + offset 
                                                                // TODO: thread-safety
        lock (code) {
          UpdateParameter(code, termIndexList, ((DoubleArray)bestIndividual["Parameter"]).ToArray());

          NativeInterpreter.NativeWrapper.GetValuesVarPro(code, code.Length, termIndexList.ToArray(), termIndexList.Count, rows, nRows, coefficients, options, result, target, out var optSummary);
        }

        #region linear model statistics
        var xy = new double[nRows, termIndexList.Count + 1];
        for(int j=0;j<termIndexList.Count;j++) {
          var t = termIndexList[j];
          var termCode = new NativeInterpreter.NativeInstruction[code[t].length];
          Array.Copy(code, t - code[t].length + 1, termCode, 0, code[t].length);
          options.Iterations = 0;
          NativeInterpreter.NativeWrapper.GetValues(termCode, termCode.Length, rows, nRows, options, result, null, out _);
          for(int i=0;i<nRows;i++) { xy[i, j] = result[i]; } // copy to matrix
        }
        // copy target to matrix
        for (int i = 0; i < nRows; i++) { xy[i, termIndexList.Count] = target[i]; } // copy to matrix
        predictorNames.Add("<const>"); // last coefficient is offset
        // var stats = Statistics.CalculateLinearModelStatistics(xy, coefficients);
        // results.AddOrUpdateResult("Statistics", stats.AsResultCollection(predictorNames));
        #endregion

        results.AddOrUpdateResult("Solution", CreateRegressionSolution(code, termIndexList, coefficients, RegressionProblemData));
        results.AddOrUpdateResult("BestQuality", new DoubleValue(bestQ));
        results.AddOrUpdateResult("CV MSE (avg)", bestIndividual["CV MSE (avg)"]);
        results.AddOrUpdateResult("CV MSE (stdev)", bestIndividual["CV MSE (stdev)"]);
      }
    }

    #region retrieval / update of non-linear parameters
    private double[][] ExtractParameters(NativeInterpreter.NativeInstruction[] code, List<int> termIndexList) {
      var p = new double[termIndexList.Count][];
      for (int i = 0; i < termIndexList.Count; i++) {
        var termIdx = termIndexList[i];
        var pList = new List<double>();
        var start = termIdx - code[termIdx].length + 1;
        for (int codeIdx = start; codeIdx <= termIdx; codeIdx++) {
          if (code[codeIdx].optimize)
            pList.Add(code[codeIdx].value);
        }
        p[i] = pList.ToArray();
      }
      return p;
    }

    // parameters are given as array for each term
    private void UpdateParameter(NativeInterpreter.NativeInstruction[] code, List<int> termIndexList, double[][] p) {
      for (int i = 0; i < termIndexList.Count; i++) {
        if (p[i].Length == 0) continue; // nothing to update
        var termIdx = termIndexList[i];
        var pIdx = 0;
        var start = termIdx - code[termIdx].length + 1;
        for (int codeIdx = start; codeIdx <= termIdx; codeIdx++) {
          if (code[codeIdx].optimize)
            code[codeIdx].value = p[i][pIdx++];
        }
      }
    }

    // parameters are given as a flat array
    private void UpdateParameter(NativeInterpreter.NativeInstruction[] code, List<int> termIndexList, double[] p) {
      var pIdx = 0;
      for (int i = 0; i < termIndexList.Count; i++) {
        var termIdx = termIndexList[i];
        var start = termIdx - code[termIdx].length + 1;
        for (int codeIdx = start; codeIdx <= termIdx; codeIdx++) {
          if (code[codeIdx].optimize)
            code[codeIdx].value = p[pIdx++];
        }
      }
    }
    #endregion



    #region feature generation
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


    private void GenerateCode(ISymbolicExpressionTree[] features, IDataset dataset) {
      byte mapSupportedSymbols(ISymbolicExpressionTreeNode node) {
        var opCode = OpCodes.MapSymbolToOpCode(node);
        if (supportedOpCodes.Contains(opCode)) return opCode;
        else throw new NotSupportedException($"VarPro does not support {node.Symbol.Name}");
      };

      var i = 0;
      featureIdx = new int[features.Length];

      var code = new List<NativeInterpreter.NativeInstruction>(capacity: (int)(features.Sum(fi => fi.Length - 2) * 1.2));
      foreach (var f in features) {
        var featureCode = Compile(f, mapSupportedSymbols);

        code.AddRange(featureCode);
        featureIdx[i++] = code.Count - 1;
      }
      this.code = code.ToArray();
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
    private NativeInterpreter.NativeInstruction[] Compile(ISymbolicExpressionTree tree, Func<ISymbolicExpressionTreeNode, byte> opCodeMapper) {
      var root = tree.Root.GetSubtree(0).GetSubtree(0);
      var code = new List<NativeInterpreter.NativeInstruction>();
      foreach (var n in root.IterateNodesPrefix()) {
        var instr = new NativeInterpreter.NativeInstruction { narg = (ushort)n.SubtreeCount, opcode = opCodeMapper(n), length = 1 }; // length is updated in a second pass below
        if (n is VariableTreeNode variable) {
          instr.value = variable.Weight;
          instr.optimize = false;
          instr.data = cachedData[variable.VariableName].AddrOfPinnedObject();
        } else if (n is ConstantTreeNode constant) {
          instr.value = constant.Value;
          if (n.Symbol.Name != "<1.0>") // HACK TODO this depends on the name given in the grammar!
            instr.optimize = true; // the VarPro grammar is specifically designed to make sure we have all necessary and only necessary non-linear parameters
        }
        if (n.Symbol is Logarithm) {
          // for log(f(x)) generate code log ( sqrt(f(x)²)) to ensure argument to log is positive
          code.Add(instr);
          code.Add(new NativeInterpreter.NativeInstruction { narg = 1, opcode = (byte)OpCode.SquareRoot, length = 1 }); // length is updated in a second pass below
          code.Add(new NativeInterpreter.NativeInstruction { narg = 1, opcode = (byte)OpCode.Square, length = 1 });
        } else {
          // all other operations are added verbatim
          code.Add(instr);
        }
      }

      code.Reverse();
      var codeArr = code.ToArray();

      // second pass to calculate lengths
      for (int i = 0; i < codeArr.Length; i++) {
        var c = i - 1;
        for (int j = 0; j < codeArr[i].narg; ++j) {
          codeArr[i].length += codeArr[c].length;
          c -= codeArr[c].length;
        }
      }

      return codeArr;
    }
    #endregion

    #region solution creation
    private IRegressionSolution CreateRegressionSolution(NativeInterpreter.NativeInstruction[] code, IList<int> termIndexList, double[] coefficients, IRegressionProblemData problemData) {
      // parse back solution from code (required because we need to used optimized parameter values)
      var addSy = symbols[(byte)OpCode.Add];
      var mulSy = symbols[(byte)OpCode.Mul];
      var sum = addSy.CreateTreeNode();
      for (int i = 0; i < termIndexList.Count; i++) {
        if (Math.Abs(coefficients[i]) < 1e-8) continue;
        var termIdx = termIndexList[i];
        var prod = mulSy.CreateTreeNode();
        var constNode = (ConstantTreeNode)constSy.CreateTreeNode();
        constNode.Value = coefficients[i];
        var term = CreateTree(code, termIdx);
        prod.AddSubtree(constNode);
        prod.AddSubtree(term);
        sum.AddSubtree(prod);
      }
      {
        var constNode = (ConstantTreeNode)constSy.CreateTreeNode();
        constNode.Value = coefficients.Last();
        sum.AddSubtree(constNode);
      }
      var root = (new ProgramRootSymbol()).CreateTreeNode();
      var start = (new StartSymbol()).CreateTreeNode();
      root.AddSubtree(start);
      start.AddSubtree(sum);
      var tree = new SymbolicExpressionTree(root);
      var ds = problemData.Dataset;
      var scaledDataset = new Dataset(ds.DoubleVariables, ds.ToArray(ds.DoubleVariables, transformations, Enumerable.Range(0, ds.Rows)));
      var scaledProblemData = new RegressionProblemData(scaledDataset, problemData.AllowedInputVariables, problemData.TargetVariable, transformations);
      scaledProblemData.TrainingPartition.Start = problemData.TrainingPartition.Start;
      scaledProblemData.TrainingPartition.End = problemData.TrainingPartition.End;
      scaledProblemData.TestPartition.Start = problemData.TestPartition.Start;
      scaledProblemData.TestPartition.End = problemData.TestPartition.End;
      return new SymbolicRegressionSolution(
        new SymbolicRegressionModel(problemData.TargetVariable, tree, new SymbolicDataAnalysisExpressionTreeNativeInterpreter()), scaledProblemData);
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


    private ISymbolicExpressionTreeNode CreateTree(NativeInterpreter.NativeInstruction[] code, int i) {
      switch (code[i].opcode) {
        case (byte)OpCode.Add:
        case (byte)OpCode.Sub:
        case (byte)OpCode.Mul:
        case (byte)OpCode.Div:
        case (byte)OpCode.Exp:
        case (byte)OpCode.Log:
        case (byte)OpCode.Sin:
        case (byte)OpCode.Cos:
        case (byte)OpCode.Tan:
        case (byte)OpCode.Tanh:
        case (byte)OpCode.Square:
        case (byte)OpCode.SquareRoot:
        case (byte)OpCode.Cube:
        case (byte)OpCode.CubeRoot:
        case (byte)OpCode.Absolute:
        case (byte)OpCode.AnalyticQuotient: {
            var node = symbols[code[i].opcode].CreateTreeNode();
            var c = i - 1;
            for (int childIdx = 0; childIdx < code[i].narg; childIdx++) {
              node.AddSubtree(CreateTree(code, c));
              c = c - code[c].length;
            }
            return node;
          }
        case (byte)OpCode.Constant: {
            var node = (ConstantTreeNode)constSy.CreateTreeNode();
            node.Value = code[i].value;
            return node;
          }
        case (byte)OpCode.Variable: {
            var node = (VariableTreeNode)varSy.CreateTreeNode();
            node.Weight = code[i].value;
            // TODO perf
            node.VariableName = string.Empty;
            foreach (var tup in this.cachedData) {
              if (tup.Value.AddrOfPinnedObject() == code[i].data) {
                node.VariableName = tup.Key;
                break;
              }
            }
            return node;
          }
        default: throw new NotSupportedException("unknown opcode");
      }
    }
    #endregion

    public void Load(IRegressionProblemData data) {
      RegressionProblemData = data;
    }

    private void InitializeOperators() {
      Operators.Add(new AlleleFrequencyAnalyzer());

      var cvMSEAnalyzer = new BestAverageWorstQualityAnalyzer();
      cvMSEAnalyzer.Name = "CVMSE Analzer";
      ParameterizeAnalyzer(cvMSEAnalyzer, "CV MSE (avg)");
      Operators.Add(cvMSEAnalyzer);
      
      var trainingMSEAnalyzer = new BestAverageWorstQualityAnalyzer();
      trainingMSEAnalyzer.Name = "Training MSE Analzer";
      ParameterizeAnalyzer(trainingMSEAnalyzer, "Train MSE (avg)");
      Operators.Add(trainingMSEAnalyzer);

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
