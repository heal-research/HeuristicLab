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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.PTSP {
  [Item("Probabilistic Traveling Salesman Problem (PTSP)", "Represents a Probabilistic Traveling Salesman Problem.")]
  [StorableClass]
  public abstract class ProbabilisticTravelingSalesmanProblem : SingleObjectiveBasicProblem<PermutationEncoding>,
  IProblemInstanceConsumer<PTSPData> {
    protected bool SuppressEvents { get; set; }

    private static readonly int DistanceMatrixSizeLimit = 1000;

    #region Parameter Properties
    public OptionalValueParameter<DoubleMatrix> CoordinatesParameter {
      get { return (OptionalValueParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public OptionalValueParameter<DistanceCalculator> DistanceCalculatorParameter {
      get { return (OptionalValueParameter<DistanceCalculator>)Parameters["DistanceCalculator"]; }
    }
    public OptionalValueParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (OptionalValueParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }
    public IFixedValueParameter<BoolValue> UseDistanceMatrixParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["UseDistanceMatrix"]; }
    }
    public OptionalValueParameter<Permutation> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<Permutation>)Parameters["BestKnownSolution"]; }
    }
    public IValueParameter<DoubleArray> ProbabilitiesParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["Probabilities"]; }
    }
    #endregion

    #region Properties
    public DoubleMatrix Coordinates {
      get { return CoordinatesParameter.Value; }
      set { CoordinatesParameter.Value = value; }
    }
    public DistanceCalculator DistanceCalculator {
      get { return DistanceCalculatorParameter.Value; }
      set { DistanceCalculatorParameter.Value = value; }
    }
    public DistanceMatrix DistanceMatrix {
      get { return DistanceMatrixParameter.Value; }
      set { DistanceMatrixParameter.Value = value; }
    }
    public bool UseDistanceMatrix {
      get { return UseDistanceMatrixParameter.Value.Value; }
      set { UseDistanceMatrixParameter.Value.Value = value; }
    }
    public Permutation BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    public DoubleArray Probabilities {
      get { return ProbabilitiesParameter.Value; }
      set { ProbabilitiesParameter.Value = value; }
    }

    #endregion

    public override bool Maximization {
      get { return false; }
    }

    [StorableConstructor]
    protected ProbabilisticTravelingSalesmanProblem(bool deserializing) : base(deserializing) { }
    protected ProbabilisticTravelingSalesmanProblem(ProbabilisticTravelingSalesmanProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    protected ProbabilisticTravelingSalesmanProblem() {
      Parameters.Add(new OptionalValueParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(new OptionalValueParameter<DistanceCalculator>("DistanceCalculator", "Calculates the distance between two rows in the coordinates matrix."));
      Parameters.Add(new OptionalValueParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new FixedValueParameter<BoolValue>("UseDistanceMatrix", "True if the coordinates based evaluators should calculate the distance matrix from the coordinates and use it for evaluation similar to the distance matrix evaluator, otherwise false.", new BoolValue(true)));
      Parameters.Add(new OptionalValueParameter<Permutation>("BestKnownSolution", "The best known solution of this TSP instance."));
      Parameters.Add(new ValueParameter<DoubleArray>("Probabilities", "This list describes for each city the probability of appearing in a realized instance."));

      var coordinates = new DoubleMatrix(new double[,] {
        { 100, 100 }, { 100, 200 }, { 100, 300 }, { 100, 400 },
        { 200, 100 }, { 200, 200 }, { 200, 300 }, { 200, 400 },
        { 300, 100 }, { 300, 200 }, { 300, 300 }, { 300, 400 },
        { 400, 100 }, { 400, 200 }, { 400, 300 }, { 400, 400 }
      });
      Coordinates = coordinates;
      Encoding.Length = coordinates.Rows;
      DistanceCalculator = new EuclideanDistance();
      DistanceMatrix = new DistanceMatrix(CalculateDistances());
      Probabilities = new DoubleArray(Enumerable.Range(0, coordinates.Rows).Select(x => 0.5).ToArray());

      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      CoordinatesParameter.ValueChanged += CoordinatesParameterOnValueChanged;
      if (Coordinates != null) {
        Coordinates.RowsChanged += CoordinatesOnChanged;
        Coordinates.ItemChanged += CoordinatesOnChanged;
      }
      UseDistanceMatrixParameter.Value.ValueChanged += UseDistanceMatrixValueChanged;
      DistanceCalculatorParameter.ValueChanged += DistanceCalculatorParameterOnValueChanged;
    }

    private void CoordinatesParameterOnValueChanged(object sender, EventArgs eventArgs) {
      if (Coordinates != null) {
        Coordinates.RowsChanged += CoordinatesOnChanged;
        Coordinates.ItemChanged += CoordinatesOnChanged;
      }
      if (SuppressEvents) return;
      UpdateInstance();
    }

    private void CoordinatesOnChanged(object sender, EventArgs eventArgs) {
      if (SuppressEvents) return;
      UpdateInstance();
    }

    private void UseDistanceMatrixValueChanged(object sender, EventArgs eventArgs) {
      if (SuppressEvents) return;
      UpdateInstance();
    }

    private void DistanceCalculatorParameterOnValueChanged(object sender, EventArgs eventArgs) {
      if (SuppressEvents) return;
      UpdateInstance();
    }

    public override double Evaluate(Individual individual, IRandom random) {
      return Evaluate(individual.Permutation(), random);
    }

    public abstract double Evaluate(Permutation tour, IRandom random);

    public double[,] CalculateDistances() {
      var coords = Coordinates;
      var len = coords.Rows;
      var dist = DistanceCalculator;

      var matrix = new double[len, len];
      for (var i = 0; i < len - 1; i++)
        for (var j = i + 1; j < len; j++)
          matrix[i, j] = matrix[j, i] = dist.Calculate(i, j, coords);

      return matrix;
    }

    public virtual void Load(PTSPData data) {
      try {
        SuppressEvents = true;
        if (data.Coordinates == null && data.Distances == null)
          throw new System.IO.InvalidDataException("The given instance specifies neither coordinates nor distances!");
        if (data.Dimension > DistanceMatrixSizeLimit && (data.Coordinates == null || data.Coordinates.GetLength(0) != data.Dimension || data.Coordinates.GetLength(1) != 2))
          throw new System.IO.InvalidDataException("The given instance is too large for using a distance matrix and there is a problem with the coordinates.");
        if (data.Coordinates != null && (data.Coordinates.GetLength(0) != data.Dimension || data.Coordinates.GetLength(1) != 2))
          throw new System.IO.InvalidDataException("The coordinates of the given instance are not in the right format, there need to be one row for each customer and two columns for the x and y coordinates respectively.");

        switch (data.DistanceMeasure) {
          case DistanceMeasure.Direct:
            DistanceCalculator = null;
            if (data.Dimension > DistanceMatrixSizeLimit && Coordinates != null) {
              DistanceCalculator = new EuclideanDistance();
              UseDistanceMatrix = false;
            } else UseDistanceMatrix = true;
            break;
          case DistanceMeasure.Att: DistanceCalculator = new AttDistance(); break;
          case DistanceMeasure.Euclidean: DistanceCalculator = new EuclideanDistance(); break;
          case DistanceMeasure.Geo: DistanceCalculator = new GeoDistance(); break;
          case DistanceMeasure.Manhattan: DistanceCalculator = new ManhattanDistance(); break;
          case DistanceMeasure.Maximum: DistanceCalculator = new MaximumDistance(); break;
          case DistanceMeasure.RoundedEuclidean: DistanceCalculator = new RoundedEuclideanDistance(); break;
          case DistanceMeasure.UpperEuclidean: DistanceCalculator = new UpperEuclideanDistance(); break;
          default: throw new ArgumentException("Distance measure is unknown");
        }

        Name = data.Name;
        Description = data.Description;

        Probabilities = new DoubleArray(data.Probabilities);
        BestKnownSolution = data.BestKnownTour != null ? new Permutation(PermutationTypes.RelativeUndirected, data.BestKnownTour) : null;
        Coordinates = data.Coordinates != null && data.Coordinates.GetLength(0) > 0 ? new DoubleMatrix(data.Coordinates) : null;
        DistanceMatrix = data.Dimension <= DistanceMatrixSizeLimit && UseDistanceMatrix ? new DistanceMatrix(data.GetDistanceMatrix()) : null;

        Encoding.Length = data.Dimension;
      } finally { SuppressEvents = false; }
      OnReset();
    }

    private void UpdateInstance() {
      var len = GetProblemDimension();
      if (Coordinates != null && Coordinates.Rows <= DistanceMatrixSizeLimit
        && DistanceCalculator != null && UseDistanceMatrix)
        DistanceMatrix = new DistanceMatrix(CalculateDistances());
      if (!UseDistanceMatrix) DistanceMatrix = null;
      Encoding.Length = len;

      OnReset();
    }

    private int GetProblemDimension() {
      if (Coordinates == null && DistanceMatrix == null) throw new InvalidOperationException("Both coordinates and distance matrix are null, please specify at least one of them.");
      return Coordinates != null ? Coordinates.Rows : DistanceMatrix.Rows;
    }
  }
}
