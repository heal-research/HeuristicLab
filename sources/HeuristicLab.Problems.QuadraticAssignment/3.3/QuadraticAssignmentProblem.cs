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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.QuadraticAssignment {
  [Item("Quadratic Assignment Problem", "The Quadratic Assignment Problem (QAP) can be described as the problem of assigning N facilities to N fixed locations such that there is exactly one facility in each location and that the sum of the distances multiplied by the connection strength between the facilities becomes minimal.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class QuadraticAssignmentProblem : SingleObjectiveHeuristicOptimizationProblem<IQAPEvaluator, IPermutationCreator>, IStorableContent {
    private static string InstancePrefix = "HeuristicLab.Problems.QuadraticAssignment.Data.";

    public string Filename { get; set; }

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    #region Parameter Properties
    public IValueParameter<Permutation> BestKnownSolutionParameter {
      get { return (IValueParameter<Permutation>)Parameters["BestKnownSolution"]; }
    }
    public IValueParameter<DoubleMatrix> WeightsParameter {
      get { return (IValueParameter<DoubleMatrix>)Parameters["Weights"]; }
    }
    public IValueParameter<DoubleMatrix> DistancesParameter {
      get { return (IValueParameter<DoubleMatrix>)Parameters["Distances"]; }
    }
    #endregion

    #region Properties
    public Permutation BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    public DoubleMatrix Weights {
      get { return WeightsParameter.Value; }
      set { WeightsParameter.Value = value; }
    }
    public DoubleMatrix Distances {
      get { return DistancesParameter.Value; }
      set { DistancesParameter.Value = value; }
    }

    public IEnumerable<string> EmbeddedInstances {
      get {
        return Assembly.GetExecutingAssembly()
          .GetManifestResourceNames()
          .Where(x => x.EndsWith(".dat"))
          .OrderBy(x => x)
          .Select(x => x.Replace(".dat", String.Empty))
          .Select(x => x.Replace(InstancePrefix, String.Empty));
      }
    }

    private BestQAPSolutionAnalyzer BestQAPSolutionAnalyzer {
      get { return Operators.OfType<BestQAPSolutionAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    [StorableConstructor]
    private QuadraticAssignmentProblem(bool deserializing) : base(deserializing) { }
    private QuadraticAssignmentProblem(QuadraticAssignmentProblem original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }
    public QuadraticAssignmentProblem()
      : base(new QAPEvaluator(), new RandomPermutationCreator()) {
      Parameters.Add(new OptionalValueParameter<Permutation>("BestKnownSolution", "The best known solution which is updated whenever a new better solution is found or may be the optimal solution if it is known beforehand.", null));
      Parameters.Add(new ValueParameter<DoubleMatrix>("Weights", "The strength of the connection between the facilities.", new DoubleMatrix(5, 5)));
      Parameters.Add(new ValueParameter<DoubleMatrix>("Distances", "The distance matrix which can either be specified directly without the coordinates, or can be calculated automatically from the coordinates.", new DoubleMatrix(5, 5)));

      Maximization = new BoolValue(false);

      Weights = new DoubleMatrix(new double[,] {
        { 0, 1, 0, 0, 1 },
        { 1, 0, 1, 0, 0 },
        { 0, 1, 0, 1, 0 },
        { 0, 0, 1, 0, 1 },
        { 1, 0, 0, 1, 0 }
      });

      Distances = new DoubleMatrix(new double[,] {
        {   0, 360, 582, 582, 360 },
        { 360,   0, 360, 582, 582 },
        { 582, 360,   0, 360, 582 },
        { 582, 582, 360,   0, 360 },
        { 360, 582, 582, 360,   0 }
      });

      SolutionCreator.PermutationParameter.ActualName = "Assignment";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      InitializeOperators();
      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QuadraticAssignmentProblem(this, cloner);
    }

    #region Events
    protected override void OnSolutionCreatorChanged() {
      SolutionCreator.PermutationParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      ParameterizeOperators();
      base.OnSolutionCreatorChanged();
    }
    protected override void OnEvaluatorChanged() {
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      ParameterizeOperators();
      base.OnEvaluatorChanged();
    }

    private void SolutionCreator_PermutationParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    private void WeightsParameter_ValueChanged(object sender, EventArgs e) {
      Weights.RowsChanged += new EventHandler(Weights_RowsChanged);
      Weights.ColumnsChanged += new EventHandler(Weights_ColumnsChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeOperators();
      AdjustDistanceMatrix();
    }
    private void Weights_RowsChanged(object sender, EventArgs e) {
      if (Weights.Rows != Weights.Columns)
        ((IStringConvertibleMatrix)Weights).Columns = Weights.Rows;
      else {
        ParameterizeSolutionCreator();
        ParameterizeEvaluator();
        ParameterizeOperators();
        AdjustDistanceMatrix();
      }
    }
    private void Weights_ColumnsChanged(object sender, EventArgs e) {
      if (Weights.Rows != Weights.Columns)
        ((IStringConvertibleMatrix)Weights).Rows = Weights.Columns;
      else {
        ParameterizeSolutionCreator();
        ParameterizeEvaluator();
        ParameterizeOperators();
        AdjustDistanceMatrix();
      }
    }
    private void DistancesParameter_ValueChanged(object sender, EventArgs e) {
      Distances.RowsChanged += new EventHandler(Distances_RowsChanged);
      Distances.ColumnsChanged += new EventHandler(Distances_ColumnsChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeOperators();
      AdjustWeightsMatrix();
    }
    private void Distances_RowsChanged(object sender, EventArgs e) {
      if (Distances.Rows != Distances.Columns)
        ((IStringConvertibleMatrix)Distances).Columns = Distances.Rows;
      else {
        ParameterizeSolutionCreator();
        ParameterizeEvaluator();
        ParameterizeOperators();
        AdjustWeightsMatrix();
      }
    }
    private void Distances_ColumnsChanged(object sender, EventArgs e) {
      if (Distances.Rows != Distances.Columns)
        ((IStringConvertibleMatrix)Distances).Rows = Distances.Columns;
      else {
        ParameterizeSolutionCreator();
        ParameterizeEvaluator();
        ParameterizeOperators();
        AdjustWeightsMatrix();
      }
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      SolutionCreator.PermutationParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      WeightsParameter.ValueChanged += new EventHandler(WeightsParameter_ValueChanged);
      Weights.RowsChanged += new EventHandler(Weights_RowsChanged);
      Weights.ColumnsChanged += new EventHandler(Weights_ColumnsChanged);
      DistancesParameter.ValueChanged += new EventHandler(DistancesParameter_ValueChanged);
      Distances.RowsChanged += new EventHandler(Distances_RowsChanged);
      Distances.ColumnsChanged += new EventHandler(Distances_ColumnsChanged);
    }

    private void InitializeOperators() {
      Operators.AddRange(ApplicationManager.Manager.GetInstances<IPermutationOperator>());
      Operators.Add(new BestQAPSolutionAnalyzer());
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }
    private void ParameterizeSolutionCreator() {
      if (SolutionCreator != null) {
        SolutionCreator.PermutationTypeParameter.Value = new PermutationType(PermutationTypes.Absolute);
        SolutionCreator.LengthParameter.Value = new IntValue(Weights.Rows);
      }
    }
    private void ParameterizeEvaluator() {
      if (Evaluator != null) {
        Evaluator.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        Evaluator.DistancesParameter.ActualName = DistancesParameter.Name;
        Evaluator.WeightsParameter.ActualName = WeightsParameter.Name;
      }
    }
    private void ParameterizeAnalyzers() {
      if (BestQAPSolutionAnalyzer != null) {
        BestQAPSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        BestQAPSolutionAnalyzer.DistancesParameter.ActualName = DistancesParameter.Name;
        BestQAPSolutionAnalyzer.WeightsParameter.ActualName = WeightsParameter.Name;
        BestQAPSolutionAnalyzer.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        BestQAPSolutionAnalyzer.ResultsParameter.ActualName = "Results";
        BestQAPSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
        BestQAPSolutionAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
        BestQAPSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
      }
    }
    private void ParameterizeOperators() {
      foreach (IPermutationCrossover op in Operators.OfType<IPermutationCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      }
      foreach (IPermutationManipulator op in Operators.OfType<IPermutationManipulator>()) {
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      }
      foreach (IPermutationMoveOperator op in Operators.OfType<IPermutationMoveOperator>()) {
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      }
      if (Operators.OfType<IMoveGenerator>().Any()) {
        string inversionMove = Operators.OfType<IMoveGenerator>().OfType<IPermutationInversionMoveOperator>().First().InversionMoveParameter.ActualName;
        foreach (IPermutationInversionMoveOperator op in Operators.OfType<IPermutationInversionMoveOperator>())
          op.InversionMoveParameter.ActualName = inversionMove;
        string translocationMove = Operators.OfType<IMoveGenerator>().OfType<IPermutationTranslocationMoveOperator>().First().TranslocationMoveParameter.ActualName;
        foreach (IPermutationTranslocationMoveOperator op in Operators.OfType<IPermutationTranslocationMoveOperator>())
          op.TranslocationMoveParameter.ActualName = translocationMove;
        string swapMove = Operators.OfType<IMoveGenerator>().OfType<IPermutationSwap2MoveOperator>().First().Swap2MoveParameter.ActualName;
        foreach (IPermutationSwap2MoveOperator op in Operators.OfType<IPermutationSwap2MoveOperator>()) {
          op.Swap2MoveParameter.ActualName = swapMove;
        }
      }
      foreach (var op in Operators.OfType<IPermutationMultiNeighborhoodShakingOperator>())
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
    }

    private void AdjustDistanceMatrix() {
      if (Distances.Rows != Weights.Rows || Distances.Columns != Weights.Columns) {
        ((IStringConvertibleMatrix)Distances).Rows = Weights.Rows;
      }
    }

    private void AdjustWeightsMatrix() {
      if (Weights.Rows != Distances.Rows || Weights.Columns != Distances.Columns) {
        ((IStringConvertibleMatrix)Weights).Rows = Distances.Rows;
      }
    }
    #endregion

    public void ImportFileInstance(string filename) {
      QAPLIBParser parser = new QAPLIBParser();
      parser.Parse(filename);
      if (parser.Error != null) throw parser.Error;
      Distances = new DoubleMatrix(parser.Distances);
      Weights = new DoubleMatrix(parser.Weights);
      Name = "Quadratic Assignment Problem (imported from " + Path.GetFileNameWithoutExtension(filename) + ")";
      Description = "Imported problem data using QAPLIBParser " + Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).Cast<AssemblyFileVersionAttribute>().FirstOrDefault().Version + ".";
      BestKnownQuality = null;
      BestKnownSolution = null;
      OnReset();
    }

    public void LoadEmbeddedInstance(string instance) {
      using (Stream stream = Assembly.GetExecutingAssembly()
        .GetManifestResourceStream(InstancePrefix + instance + ".dat")) {
        QAPLIBParser parser = new QAPLIBParser();
        parser.Parse(stream);
        if (parser.Error != null) throw parser.Error;
        Distances = new DoubleMatrix(parser.Distances);
        Weights = new DoubleMatrix(parser.Weights);
        Name = "Quadratic Assignment Problem (loaded instance " + instance + ")";
        Description = "Loaded embedded problem data of instance " + instance + ".";
        OnReset();
      }
      bool solutionExists = Assembly.GetExecutingAssembly()
          .GetManifestResourceNames()
          .Where(x => x.EndsWith(instance + ".sln"))
          .Any();
      if (solutionExists) {
        using (Stream solStream = Assembly.GetExecutingAssembly()
          .GetManifestResourceStream(InstancePrefix + instance + ".sln")) {
          QAPLIBSolutionParser solParser = new QAPLIBSolutionParser();
          solParser.Parse(solStream, true); // most sln's seem to be of the type index = "facility" => value = "location"
          if (solParser.Error != null) throw solParser.Error;
          if (!solParser.Quality.IsAlmost(QAPEvaluator.Apply(new Permutation(PermutationTypes.Absolute, solParser.Assignment), Weights, Distances))) {
            solStream.Seek(0, SeekOrigin.Begin);
            solParser.Reset();
            solParser.Parse(solStream, false); // some sln's seem to be of the type index = "location" => value = "facility"
            if (solParser.Error != null) throw solParser.Error;
            if (solParser.Quality.IsAlmost(QAPEvaluator.Apply(new Permutation(PermutationTypes.Absolute, solParser.Assignment), Weights, Distances))) {
              BestKnownQuality = new DoubleValue(solParser.Quality);
              BestKnownSolution = new Permutation(PermutationTypes.Absolute, solParser.Assignment);
            } else {
              BestKnownQuality = new DoubleValue(solParser.Quality);
              BestKnownSolution = null;
            }
          } else {
            BestKnownQuality = new DoubleValue(solParser.Quality);
            BestKnownSolution = new Permutation(PermutationTypes.Absolute, solParser.Assignment);
          }
        }
      } else {
        BestKnownQuality = null;
        BestKnownSolution = null;
      }
    }
  }
}
