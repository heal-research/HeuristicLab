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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Random forest regression data analysis algorithm.
  /// </summary>
  [Item("Random Forest Regression", "Random forest regression data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 120)]
  [StorableClass]
  public sealed class RandomForestRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string RandomForestRegressionModelResultName = "Random forest regression solution";
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
    private RandomForestRegression(bool deserializing) : base(deserializing) { }
    private RandomForestRegression(RandomForestRegression original, Cloner cloner)
      : base(original, cloner) {
    }

    public RandomForestRegression()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(NumberOfTreesParameterName, "The number of trees in the forest. Should be between 50 and 100", new IntValue(50)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(RParameterName, "The ratio of the training set that will be used in the construction of individual trees (0<r<=1). Should be adjusted depending on the noise level in the dataset in the range from 0.66 (low noise) to 0.05 (high noise). This parameter should be adjusted to achieve good generalization error.", new DoubleValue(0.3)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(MParameterName, "The ratio of features that will be used in the construction of individual trees (0<m<=1)", new DoubleValue(0.5)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Problem = new RegressionProblem();
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
      return new RandomForestRegression(this, cloner);
    }

    #region random forest
    protected override void Run() {
      double rmsError, avgRelError, outOfBagRmsError, outOfBagAvgRelError;
      if (SetSeedRandomly) Seed = new System.Random().Next();

      var solution = CreateRandomForestRegressionSolution(Problem.ProblemData, NumberOfTrees, R, M, Seed, out rmsError, out avgRelError, out outOfBagRmsError, out outOfBagAvgRelError);
      Results.Add(new Result(RandomForestRegressionModelResultName, "The random forest regression solution.", solution));
      Results.Add(new Result("Root mean square error", "The root of the mean of squared errors of the random forest regression solution on the training set.", new DoubleValue(rmsError)));
      Results.Add(new Result("Average relative error", "The average of relative errors of the random forest regression solution on the training set.", new PercentValue(avgRelError)));
      Results.Add(new Result("Root mean square error (out-of-bag)", "The out-of-bag root of the mean of squared errors of the random forest regression solution.", new DoubleValue(outOfBagRmsError)));
      Results.Add(new Result("Average relative error (out-of-bag)", "The out-of-bag average of relative errors of the random forest regression solution.", new PercentValue(outOfBagAvgRelError)));
    }

    public static IRegressionSolution CreateRandomForestRegressionSolution(IRegressionProblemData problemData, int nTrees, double r, double m, int seed,
      out double rmsError, out double avgRelError, out double outOfBagRmsError, out double outOfBagAvgRelError) {
      var model = RandomForestModel.CreateRegressionModel(problemData, nTrees, r, m, seed, out rmsError, out avgRelError, out outOfBagRmsError, out outOfBagAvgRelError);
      return new RandomForestRegressionSolution((IRegressionProblemData)problemData.Clone(), model);
    }
    #endregion
  }
}
