#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Random forest classification data analysis algorithm.
  /// </summary>
  [Item("Random Forest Classification", "Random forest classification data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class RandomForestClassification : FixedDataAnalysisAlgorithm<IClassificationProblem> {
    private const string RandomForestClassificationModelResultName = "Random forest classification solution";
    private const string NumberOfTreesParameterName = "Number of trees";
    private const string RParameterName = "R";
    private const string MParameterName = "M";
    private const string SeedParameterName = "Seed";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";

    #region parameter properties
    public IFixedValueParameter<IntValue> NumberOfTreesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[NumberOfTreesParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> RParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[RParameterName]; }
    }
    public IFixedValueParameter<DoubleValue> MParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[MParameterName]; }
    }
    public IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }
    public IFixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
    }
    #endregion
    #region properties
    public int NumberOfTrees {
      get { return NumberOfTreesParameter.Value.Value; }
      set { NumberOfTreesParameter.Value.Value = value; }
    }
    public double R {
      get { return RParameter.Value.Value; }
      set { RParameter.Value.Value = value; }
    }
    public double M {
      get { return MParameter.Value.Value; }
      set { MParameter.Value.Value = value; }
    }
    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }
    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private RandomForestClassification(bool deserializing) : base(deserializing) { }
    private RandomForestClassification(RandomForestClassification original, Cloner cloner)
      : base(original, cloner) {
    }

    public RandomForestClassification()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(NumberOfTreesParameterName, "The number of trees in the forest. Should be between 50 and 100", new IntValue(50)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(RParameterName, "The ratio of the training set that will be used in the construction of individual trees (0<r<=1). Should be adjusted depending on the noise level in the dataset in the range from 0.66 (low noise) to 0.05 (high noise). This parameter should be adjusted to achieve good generalization error.", new DoubleValue(0.3)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(MParameterName, "The ratio of features that will be used in the construction of individual trees (0<m<=1)", new DoubleValue(0.5)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Problem = new ClassificationProblem();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(MParameterName))
        Parameters.Add(new FixedValueParameter<DoubleValue>(MParameterName, "The ratio of features that will be used in the construction of individual trees (0<m<=1)", new DoubleValue(0.5)));
      if (!Parameters.ContainsKey(SeedParameterName))
        Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      if (!Parameters.ContainsKey((SetSeedRandomlyParameterName)))
        Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomForestClassification(this, cloner);
    }

    #region random forest
    protected override void Run() {
      double rmsError, relClassificationError, outOfBagRmsError, outOfBagRelClassificationError;
      if (SetSeedRandomly) Seed = new System.Random().Next();

      var solution = CreateRandomForestClassificationSolution(Problem.ProblemData, NumberOfTrees, R, M, Seed, out rmsError, out relClassificationError, out outOfBagRmsError, out outOfBagRelClassificationError);
      Results.Add(new Result(RandomForestClassificationModelResultName, "The random forest classification solution.", solution));
      Results.Add(new Result("Root mean square error", "The root of the mean of squared errors of the random forest regression solution on the training set.", new DoubleValue(rmsError)));
      Results.Add(new Result("Relative classification error", "Relative classification error of the random forest regression solution on the training set.", new PercentValue(relClassificationError)));
      Results.Add(new Result("Root mean square error (out-of-bag)", "The out-of-bag root of the mean of squared errors of the random forest regression solution.", new DoubleValue(outOfBagRmsError)));
      Results.Add(new Result("Relative classification error (out-of-bag)", "The out-of-bag relative classification error  of the random forest regression solution.", new PercentValue(outOfBagRelClassificationError)));
    }

    public static IClassificationSolution CreateRandomForestClassificationSolution(IClassificationProblemData problemData, int nTrees, double r, double m, int seed,
      out double rmsError, out double relClassificationError, out double outOfBagRmsError, out double outOfBagRelClassificationError) {
      if (r <= 0 || r > 1) throw new ArgumentException("The R parameter in the random forest regression must be between 0 and 1.");
      if (m <= 0 || m > 1) throw new ArgumentException("The M parameter in the random forest regression must be between 0 and 1.");

      alglib.math.rndobject = new System.Random(seed);

      Dataset dataset = problemData.Dataset;
      string targetVariable = problemData.TargetVariable;
      IEnumerable<string> allowedInputVariables = problemData.AllowedInputVariables;
      IEnumerable<int> rows = problemData.TrainingIndices;
      double[,] inputMatrix = AlglibUtil.PrepareInputMatrix(dataset, allowedInputVariables.Concat(new string[] { targetVariable }), rows);
      if (inputMatrix.Cast<double>().Any(x => double.IsNaN(x) || double.IsInfinity(x)))
        throw new NotSupportedException("Random forest classification does not support NaN or infinity values in the input dataset.");

      int info = 0;
      alglib.decisionforest dForest = new alglib.decisionforest();
      alglib.dfreport rep = new alglib.dfreport(); ;
      int nRows = inputMatrix.GetLength(0);
      int nColumns = inputMatrix.GetLength(1);
      int sampleSize = Math.Max((int)Math.Round(r * nRows), 1);
      int nFeatures = Math.Max((int)Math.Round(m * (nColumns - 1)), 1);


      double[] classValues = problemData.ClassValues.ToArray();
      int nClasses = problemData.Classes;
      // map original class values to values [0..nClasses-1]
      Dictionary<double, double> classIndices = new Dictionary<double, double>();
      for (int i = 0; i < nClasses; i++) {
        classIndices[classValues[i]] = i;
      }
      for (int row = 0; row < nRows; row++) {
        inputMatrix[row, nColumns - 1] = classIndices[inputMatrix[row, nColumns - 1]];
      }
      // execute random forest algorithm      
      alglib.dforest.dfbuildinternal(inputMatrix, nRows, nColumns - 1, nClasses, nTrees, sampleSize, nFeatures, alglib.dforest.dfusestrongsplits + alglib.dforest.dfuseevs, ref info, dForest.innerobj, rep.innerobj);
      if (info != 1) throw new ArgumentException("Error in calculation of random forest classification solution");

      rmsError = rep.rmserror;
      outOfBagRmsError = rep.oobrmserror;
      relClassificationError = rep.relclserror;
      outOfBagRelClassificationError = rep.oobrelclserror;
      return new RandomForestClassificationSolution((IClassificationProblemData)problemData.Clone(), new RandomForestModel(dForest, targetVariable, allowedInputVariables, classValues));
    }
    #endregion
  }
}
