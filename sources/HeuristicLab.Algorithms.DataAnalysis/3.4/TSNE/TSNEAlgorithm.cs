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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// t-distributed stochastic neighbourhood embedding (tSNE) projects the data in a low dimensional 
  /// space to allow visual cluster identification.
  /// </summary>
  [Item("tSNE", "t-distributed stochastic neighbourhood embedding projects the data in a low " +
                "dimensional space to allow visual cluster identification. Implemented similar to: https://lvdmaaten.github.io/tsne/#implementations (Barnes-Hut t-SNE). Described in : https://lvdmaaten.github.io/publications/papers/JMLR_2014.pdf")]
  [Creatable(CreatableAttribute.Categories.DataAnalysis, Priority = 100)]
  [StorableClass]
  public sealed class TSNEAlgorithm : BasicAlgorithm {
    public override bool SupportsPause {
      get { return true; }
    }
    public override Type ProblemType {
      get { return typeof(IDataAnalysisProblem); }
    }
    public new IDataAnalysisProblem Problem {
      get { return (IDataAnalysisProblem)base.Problem; }
      set { base.Problem = value; }
    }

    #region parameter names
    private const string DistanceParameterName = "DistanceFunction";
    private const string PerplexityParameterName = "Perplexity";
    private const string ThetaParameterName = "Theta";
    private const string NewDimensionsParameterName = "Dimensions";
    private const string MaxIterationsParameterName = "MaxIterations";
    private const string StopLyingIterationParameterName = "StopLyingIteration";
    private const string MomentumSwitchIterationParameterName = "MomentumSwitchIteration";
    private const string InitialMomentumParameterName = "InitialMomentum";
    private const string FinalMomentumParameterName = "FinalMomentum";
    private const string EtaParameterName = "Eta";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    private const string SeedParameterName = "Seed";
    private const string ClassesParameterName = "ClassNames";
    private const string NormalizationParameterName = "Normalization";
    private const string UpdateIntervalParameterName = "UpdateInterval";
    #endregion

    #region result names
    private const string IterationResultName = "Iteration";
    private const string ErrorResultName = "Error";
    private const string ErrorPlotResultName = "Error plot";
    private const string ScatterPlotResultName = "Scatterplot";
    private const string DataResultName = "Projected data";
    #endregion

    #region parameter properties
    public IFixedValueParameter<DoubleValue> PerplexityParameter {
      get { return Parameters[PerplexityParameterName] as IFixedValueParameter<DoubleValue>; }
    }
    public IFixedValueParameter<PercentValue> ThetaParameter {
      get { return Parameters[ThetaParameterName] as IFixedValueParameter<PercentValue>; }
    }
    public IFixedValueParameter<IntValue> NewDimensionsParameter {
      get { return Parameters[NewDimensionsParameterName] as IFixedValueParameter<IntValue>; }
    }
    public IConstrainedValueParameter<IDistance<double[]>> DistanceParameter {
      get { return Parameters[DistanceParameterName] as IConstrainedValueParameter<IDistance<double[]>>; }
    }
    public IFixedValueParameter<IntValue> MaxIterationsParameter {
      get { return Parameters[MaxIterationsParameterName] as IFixedValueParameter<IntValue>; }
    }
    public IFixedValueParameter<IntValue> StopLyingIterationParameter {
      get { return Parameters[StopLyingIterationParameterName] as IFixedValueParameter<IntValue>; }
    }
    public IFixedValueParameter<IntValue> MomentumSwitchIterationParameter {
      get { return Parameters[MomentumSwitchIterationParameterName] as IFixedValueParameter<IntValue>; }
    }
    public IFixedValueParameter<DoubleValue> InitialMomentumParameter {
      get { return Parameters[InitialMomentumParameterName] as IFixedValueParameter<DoubleValue>; }
    }
    public IFixedValueParameter<DoubleValue> FinalMomentumParameter {
      get { return Parameters[FinalMomentumParameterName] as IFixedValueParameter<DoubleValue>; }
    }
    public IFixedValueParameter<DoubleValue> EtaParameter {
      get { return Parameters[EtaParameterName] as IFixedValueParameter<DoubleValue>; }
    }
    public IFixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return Parameters[SetSeedRandomlyParameterName] as IFixedValueParameter<BoolValue>; }
    }
    public IFixedValueParameter<IntValue> SeedParameter {
      get { return Parameters[SeedParameterName] as IFixedValueParameter<IntValue>; }
    }
    public IConstrainedValueParameter<StringValue> ClassesParameter {
      get { return Parameters[ClassesParameterName] as IConstrainedValueParameter<StringValue>; }
    }
    public IFixedValueParameter<BoolValue> NormalizationParameter {
      get { return Parameters[NormalizationParameterName] as IFixedValueParameter<BoolValue>; }
    }
    public IFixedValueParameter<IntValue> UpdateIntervalParameter {
      get { return Parameters[UpdateIntervalParameterName] as IFixedValueParameter<IntValue>; }
    }
    #endregion

    #region  Properties
    public IDistance<double[]> Distance {
      get { return DistanceParameter.Value; }
    }
    public double Perplexity {
      get { return PerplexityParameter.Value.Value; }
      set { PerplexityParameter.Value.Value = value; }
    }
    public double Theta {
      get { return ThetaParameter.Value.Value; }
      set { ThetaParameter.Value.Value = value; }
    }
    public int NewDimensions {
      get { return NewDimensionsParameter.Value.Value; }
      set { NewDimensionsParameter.Value.Value = value; }
    }
    public int MaxIterations {
      get { return MaxIterationsParameter.Value.Value; }
      set { MaxIterationsParameter.Value.Value = value; }
    }
    public int StopLyingIteration {
      get { return StopLyingIterationParameter.Value.Value; }
      set { StopLyingIterationParameter.Value.Value = value; }
    }
    public int MomentumSwitchIteration {
      get { return MomentumSwitchIterationParameter.Value.Value; }
      set { MomentumSwitchIterationParameter.Value.Value = value; }
    }
    public double InitialMomentum {
      get { return InitialMomentumParameter.Value.Value; }
      set { InitialMomentumParameter.Value.Value = value; }
    }
    public double FinalMomentum {
      get { return FinalMomentumParameter.Value.Value; }
      set { FinalMomentumParameter.Value.Value = value; }
    }
    public double Eta {
      get { return EtaParameter.Value.Value; }
      set { EtaParameter.Value.Value = value; }
    }
    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }
    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }
    public string Classes {
      get { return ClassesParameter.Value != null ? ClassesParameter.Value.Value : null; }
      set { ClassesParameter.Value.Value = value; }
    }
    public bool Normalization {
      get { return NormalizationParameter.Value.Value; }
      set { NormalizationParameter.Value.Value = value; }
    }

    public int UpdateInterval {
      get { return UpdateIntervalParameter.Value.Value; }
      set { UpdateIntervalParameter.Value.Value = value; }
    }
    #endregion

    #region Constructors & Cloning
    [StorableConstructor]
    private TSNEAlgorithm(bool deserializing) : base(deserializing) { }

    private TSNEAlgorithm(TSNEAlgorithm original, Cloner cloner) : base(original, cloner) {
      if (original.dataRowNames != null)
        this.dataRowNames = new Dictionary<string, List<int>>(original.dataRowNames);
      if (original.dataRows != null)
        this.dataRows = original.dataRows.ToDictionary(kvp => kvp.Key, kvp => cloner.Clone(kvp.Value));
      if (original.state != null)
        this.state = cloner.Clone(original.state);
      this.iter = original.iter;
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new TSNEAlgorithm(this, cloner); }
    public TSNEAlgorithm() {
      var distances = new ItemSet<IDistance<double[]>>(ApplicationManager.Manager.GetInstances<IDistance<double[]>>());
      Parameters.Add(new ConstrainedValueParameter<IDistance<double[]>>(DistanceParameterName, "The distance function used to differentiate similar from non-similar points", distances, distances.OfType<EuclideanDistance>().FirstOrDefault()));
      Parameters.Add(new FixedValueParameter<DoubleValue>(PerplexityParameterName, "Perplexity-parameter of tSNE. Comparable to k in a k-nearest neighbour algorithm. Recommended value is floor(number of points /3) or lower", new DoubleValue(25)));
      Parameters.Add(new FixedValueParameter<PercentValue>(ThetaParameterName, "Value describing how much appoximated " +
                                                                              "gradients my differ from exact gradients. Set to 0 for exact calculation and in [0,1] otherwise. " +
                                                                              "Appropriate values for theta are between 0.1 and 0.7 (default = 0.5). CAUTION: exact calculation of " +
                                                                              "forces requires building a non-sparse N*N matrix where N is the number of data points. This may " +
                                                                              "exceed memory limitations. The function is designed to run on large (N > 5000) data sets. It may give" +
                                                                              " poor performance on very small data sets(it is better to use a standard t - SNE implementation on such data).", new PercentValue(0)));
      Parameters.Add(new FixedValueParameter<IntValue>(NewDimensionsParameterName, "Dimensionality of projected space (usually 2 for easy visual analysis)", new IntValue(2)));
      Parameters.Add(new FixedValueParameter<IntValue>(MaxIterationsParameterName, "Maximum number of iterations for gradient descent.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>(StopLyingIterationParameterName, "Number of iterations after which p is no longer approximated.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<IntValue>(MomentumSwitchIterationParameterName, "Number of iterations after which the momentum in the gradient descent is switched.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(InitialMomentumParameterName, "The initial momentum in the gradient descent.", new DoubleValue(0.5)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(FinalMomentumParameterName, "The final momentum.", new DoubleValue(0.8)));
      Parameters.Add(new FixedValueParameter<DoubleValue>(EtaParameterName, "Gradient descent learning rate.", new DoubleValue(10)));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "If the seed should be random.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "The seed used if it should not be random.", new IntValue(0)));

      //Name of the column specifying the class lables of each data point.If the label column can not be found training/test is used as labels."
      Parameters.Add(new OptionalConstrainedValueParameter<StringValue>(ClassesParameterName, "Name of the column specifying the class lables of each data point."));
      Parameters.Add(new FixedValueParameter<BoolValue>(NormalizationParameterName, "Whether the data should be zero centered and have variance of 1 for each variable, so different scalings are ignored.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>(UpdateIntervalParameterName, "", new IntValue(50)));
      Parameters[UpdateIntervalParameterName].Hidden = true;

      MomentumSwitchIterationParameter.Hidden = true;
      InitialMomentumParameter.Hidden = true;
      FinalMomentumParameter.Hidden = true;
      StopLyingIterationParameter.Hidden = true;
      EtaParameter.Hidden = false;
      Problem = new RegressionProblem();
    }
    #endregion

    [Storable]
    private Dictionary<string, List<int>> dataRowNames;
    [Storable]
    private Dictionary<string, ScatterPlotDataRow> dataRows;
    [Storable]
    private TSNEStatic<double[]>.TSNEState state;
    [Storable]
    private int iter;

    public override void Prepare() {
      base.Prepare();
      dataRowNames = null;
      dataRows = null;
      state = null;
    }

    protected override void Run(CancellationToken cancellationToken) {
      var problemData = Problem.ProblemData;
      // set up and initialized everything if necessary
      if (state == null) {
        if (SetSeedRandomly) Seed = new System.Random().Next();
        var random = new MersenneTwister((uint)Seed);
        var dataset = problemData.Dataset;
        var allowedInputVariables = problemData.AllowedInputVariables.ToArray();
        var data = new double[dataset.Rows][];
        for (var row = 0; row < dataset.Rows; row++)
          data[row] = allowedInputVariables.Select(col => dataset.GetDoubleValue(col, row)).ToArray();

        if (Normalization) data = NormalizeData(data);

        state = TSNEStatic<double[]>.CreateState(data, Distance, random, NewDimensions, Perplexity, Theta,
          StopLyingIteration, MomentumSwitchIteration, InitialMomentum, FinalMomentum, Eta);

        SetUpResults(data);
        iter = 0;
      }
      for (; iter < MaxIterations && !cancellationToken.IsCancellationRequested; iter++) {
        if (iter % UpdateInterval == 0)
          Analyze(state);
        TSNEStatic<double[]>.Iterate(state);
      }
      Analyze(state);
    }

    #region Events
    protected override void OnProblemChanged() {
      base.OnProblemChanged();
      if (Problem == null) return;
      OnProblemDataChanged(this, null);
    }

    protected override void RegisterProblemEvents() {
      base.RegisterProblemEvents();
      Problem.ProblemDataChanged += OnProblemDataChanged;
    }
    protected override void DeregisterProblemEvents() {
      base.DeregisterProblemEvents();
      Problem.ProblemDataChanged -= OnProblemDataChanged;
    }

    private void OnProblemDataChanged(object sender, EventArgs args) {
      if (Problem == null || Problem.ProblemData == null) return;
      if (!Parameters.ContainsKey(ClassesParameterName)) return;
      ClassesParameter.ValidValues.Clear();
      foreach (var input in Problem.ProblemData.InputVariables) ClassesParameter.ValidValues.Add(input);
    }

    #endregion

    #region Helpers
    private void SetUpResults(IReadOnlyCollection<double[]> data) {
      if (Results == null) return;
      var results = Results;
      dataRowNames = new Dictionary<string, List<int>>();
      dataRows = new Dictionary<string, ScatterPlotDataRow>();
      var problemData = Problem.ProblemData;

      //color datapoints acording to classes variable (be it double or string)
      if (problemData.Dataset.VariableNames.Contains(Classes)) {
        if ((problemData.Dataset as Dataset).VariableHasType<string>(Classes)) {
          var classes = problemData.Dataset.GetStringValues(Classes).ToArray();
          for (var i = 0; i < classes.Length; i++) {
            if (!dataRowNames.ContainsKey(classes[i])) dataRowNames.Add(classes[i], new List<int>());
            dataRowNames[classes[i]].Add(i);
          }
        } else if ((problemData.Dataset as Dataset).VariableHasType<double>(Classes)) {
          var classValues = problemData.Dataset.GetDoubleValues(Classes).ToArray();
          var max = classValues.Max() + 0.1;
          var min = classValues.Min() - 0.1;
          const int contours = 8;
          for (var i = 0; i < contours; i++) {
            var contourname = GetContourName(i, min, max, contours);
            dataRowNames.Add(contourname, new List<int>());
            dataRows.Add(contourname, new ScatterPlotDataRow(contourname, "", new List<Point2D<double>>()));
            dataRows[contourname].VisualProperties.Color = GetHeatMapColor(i, contours);
            dataRows[contourname].VisualProperties.PointSize = i + 3;
          }
          for (var i = 0; i < classValues.Length; i++) {
            dataRowNames[GetContourName(classValues[i], min, max, contours)].Add(i);
          }
        }
      } else {
        dataRowNames.Add("Training", problemData.TrainingIndices.ToList());
        dataRowNames.Add("Test", problemData.TestIndices.ToList());
      }

      if (!results.ContainsKey(IterationResultName)) results.Add(new Result(IterationResultName, new IntValue(0)));
      else ((IntValue)results[IterationResultName].Value).Value = 0;

      if (!results.ContainsKey(ErrorResultName)) results.Add(new Result(ErrorResultName, new DoubleValue(0)));
      else ((DoubleValue)results[ErrorResultName].Value).Value = 0;

      if (!results.ContainsKey(ErrorPlotResultName)) results.Add(new Result(ErrorPlotResultName, new DataTable(ErrorPlotResultName, "Development of errors during gradient descent")));
      else results[ErrorPlotResultName].Value = new DataTable(ErrorPlotResultName, "Development of errors during gradient descent");

      var plot = results[ErrorPlotResultName].Value as DataTable;
      if (plot == null) throw new ArgumentException("could not create/access error data table in results collection");

      if (!plot.Rows.ContainsKey("errors")) plot.Rows.Add(new DataRow("errors"));
      plot.Rows["errors"].Values.Clear();
      plot.Rows["errors"].VisualProperties.StartIndexZero = true;

      results.Add(new Result(ScatterPlotResultName, "Plot of the projected data", new ScatterPlot(DataResultName, "")));
      results.Add(new Result(DataResultName, "Projected Data", new DoubleMatrix()));
    }

    private void Analyze(TSNEStatic<double[]>.TSNEState tsneState) {
      if (Results == null) return;
      var results = Results;
      var plot = results[ErrorPlotResultName].Value as DataTable;
      if (plot == null) throw new ArgumentException("Could not create/access error data table in results collection.");
      var errors = plot.Rows["errors"].Values;
      var c = tsneState.EvaluateError();
      errors.Add(c);
      ((IntValue)results[IterationResultName].Value).Value = tsneState.iter;
      ((DoubleValue)results[ErrorResultName].Value).Value = errors.Last();

      var ndata = Normalize(tsneState.newData);
      results[DataResultName].Value = new DoubleMatrix(ndata);
      var splot = results[ScatterPlotResultName].Value as ScatterPlot;
      FillScatterPlot(ndata, splot);
    }

    private void FillScatterPlot(double[,] lowDimData, ScatterPlot plot) {
      foreach (var rowName in dataRowNames.Keys) {
        if (!plot.Rows.ContainsKey(rowName))
          plot.Rows.Add(dataRows.ContainsKey(rowName) ? dataRows[rowName] : new ScatterPlotDataRow(rowName, "", new List<Point2D<double>>()));
        plot.Rows[rowName].Points.Replace(dataRowNames[rowName].Select(i => new Point2D<double>(lowDimData[i, 0], lowDimData[i, 1])));
      }
    }

    private static double[,] Normalize(double[,] data) {
      var max = new double[data.GetLength(1)];
      var min = new double[data.GetLength(1)];
      var res = new double[data.GetLength(0), data.GetLength(1)];
      for (var i = 0; i < max.Length; i++) max[i] = min[i] = data[0, i];
      for (var i = 0; i < data.GetLength(0); i++)
        for (var j = 0; j < data.GetLength(1); j++) {
          var v = data[i, j];
          max[j] = Math.Max(max[j], v);
          min[j] = Math.Min(min[j], v);
        }
      for (var i = 0; i < data.GetLength(0); i++) {
        for (var j = 0; j < data.GetLength(1); j++) {
          var d = max[j] - min[j];
          var s = data[i, j] - (max[j] + min[j]) / 2;  //shift data
          if (d.IsAlmost(0)) res[i, j] = data[i, j];   //no scaling possible
          else res[i, j] = s / d;  //scale data
        }
      }
      return res;
    }

    private static double[][] NormalizeData(IReadOnlyList<double[]> data) {
      // as in tSNE implementation by van der Maaten
      var n = data[0].Length;
      var mean = new double[n];
      var max = new double[n];
      var nData = new double[data.Count][];
      for (var i = 0; i < n; i++) {
        mean[i] = Enumerable.Range(0, data.Count).Select(x => data[x][i]).Average();
        max[i] = Enumerable.Range(0, data.Count).Max(x => Math.Abs(data[x][i]));
      }
      for (var i = 0; i < data.Count; i++) {
        nData[i] = new double[n];
        for (var j = 0; j < n; j++) nData[i][j] = max[j].IsAlmost(0) ? data[i][j] - mean[j] : (data[i][j] - mean[j]) / max[j];
      }
      return nData;
    }

    private static Color GetHeatMapColor(int contourNr, int noContours) {
      var q = (double)contourNr / noContours;  // q in [0,1]
      var c = q < 0.5 ? Color.FromArgb((int)(q * 2 * 255), 255, 0) : Color.FromArgb(255, (int)((1 - q) * 2 * 255), 0);
      return c;
    }

    private static string GetContourName(double value, double min, double max, int noContours) {
      var size = (max - min) / noContours;
      var contourNr = (int)((value - min) / size);
      return GetContourName(contourNr, min, max, noContours);
    }

    private static string GetContourName(int i, double min, double max, int noContours) {
      var size = (max - min) / noContours;
      return "[" + (min + i * size) + ";" + (min + (i + 1) * size) + ")";
    }
    #endregion
  }
}
