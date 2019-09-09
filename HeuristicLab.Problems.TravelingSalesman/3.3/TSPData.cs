using System;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.TravelingSalesman {
  [StorableType("1a9bf60c-b6a6-4c95-8e99-5a2dec0ee892")]
  public interface ITSPData : IItem {
    double GetDistance(int fromCity, int toCity);
  }

  [Item("Coordinates-based TSP Data", "TSP that is represented by coordinates of cities.")]
  [StorableType("3955d07a-d43c-4a01-9505-d2effb1ea865")]
  public abstract class CoordinatesTSPData : Item, ITSPData {
    [Storable]
    public DoubleMatrix Coordinates { get; set; }

    [StorableConstructor]
    protected CoordinatesTSPData(StorableConstructorFlag _) : base(_) { }
    protected CoordinatesTSPData(CoordinatesTSPData original, Cloner cloner) : base(original, cloner) {
      Coordinates = cloner.Clone(original.Coordinates);
    }
    protected CoordinatesTSPData() : base() {
      Coordinates = new DoubleMatrix();
    }
    protected CoordinatesTSPData(double[,] coordinates) : base() {
      if (coordinates == null) throw new ArgumentNullException(nameof(coordinates));
      if (coordinates.GetLength(1) != 2) throw new ArgumentException("Argument must have exactly two columns.", nameof(coordinates));
      Coordinates = new DoubleMatrix(coordinates);
    }

    public double GetDistance(int fromCity, int toCity) {
      return GetDistance(Coordinates[fromCity, 0], Coordinates[fromCity, 1],
        Coordinates[toCity, 0], Coordinates[toCity, 1]);
    }

    public abstract double GetDistance(double fromX, double fromY, double toX, double toY);
  }

  [Item("Euclidean (rounded) TSP Data", "TSP that is represented by coordinates and a rounded euclidean distance measure.")]
  [StorableType("4bf58348-cd98-46c5-a4c0-55f486ca88b4")]
  public sealed class RoundedEuclideanCoordinatesTSPData : CoordinatesTSPData {

    [StorableConstructor]
    private RoundedEuclideanCoordinatesTSPData(StorableConstructorFlag _) : base(_) { }
    private RoundedEuclideanCoordinatesTSPData(RoundedEuclideanCoordinatesTSPData original, Cloner cloner) : base(original, cloner) { }
    public RoundedEuclideanCoordinatesTSPData() : base() { }
    public RoundedEuclideanCoordinatesTSPData(double[,] coordinates) : base(coordinates) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RoundedEuclideanCoordinatesTSPData(this, cloner);
    }

    public override double GetDistance(double fromX, double fromY, double toX, double toY) {
      return Math.Round(Math.Sqrt((fromX - toX) * (fromX - toX) + (fromY - toY) * (fromY - toY)));
    }
  }

  [Item("Matrix-based TSP Data", "TSP that is representd by a distance matrix.")]
  [StorableType("4df58a35-679d-4414-b815-9450ae100823")]
  public class MatrixTSPData : Item, ITSPData {
    [Storable]
    private double[,] Matrix { get; set; }

    [Storable]
    public DoubleMatrix DisplayCoordinates { get; set; }

    [StorableConstructor]
    protected MatrixTSPData(StorableConstructorFlag _) : base(_) { }
    protected MatrixTSPData(MatrixTSPData original, Cloner cloner) : base(original, cloner) {
      Matrix = original.Matrix;
      DisplayCoordinates = cloner.Clone(original.DisplayCoordinates);
    }
    public MatrixTSPData() {
      Matrix = new double[0, 0];
      DisplayCoordinates = null;
    }
    public MatrixTSPData(double[,] matrix, double[,] coordinates = null) {
      Matrix = (double[,])matrix.Clone();
      if (coordinates != null) DisplayCoordinates = new DoubleMatrix(coordinates);
      if (DisplayCoordinates != null && DisplayCoordinates.Columns != 2)
        throw new ArgumentException("Argument must have exactly two columns.", nameof(coordinates));
      if (DisplayCoordinates != null && DisplayCoordinates.Rows != Matrix.GetLength(0))
        throw new ArgumentException("Unequal number of rows in " + nameof(matrix) + " and " + nameof(coordinates) + ".");
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MatrixTSPData(this, cloner);
    }

    public void SetMatrix(double[,] matrix, double[,] coordinates = null) {
      Matrix = (double[,])matrix.Clone();
      if (coordinates == null) DisplayCoordinates = null;
      else DisplayCoordinates = new DoubleMatrix(coordinates);
      if (DisplayCoordinates != null && DisplayCoordinates.Columns != 2)
        throw new ArgumentException("Argument must have exactly two columns.", nameof(coordinates));
      if (DisplayCoordinates != null && DisplayCoordinates.Rows != Matrix.GetLength(0))
        throw new ArgumentException("Unequal number of rows in " + nameof(matrix) + " and " + nameof(coordinates) + ".");
    }

    public void SetMatrix(ValueTypeMatrix<double> matrix, DoubleMatrix coordinates = null) {
      Matrix = matrix.CloneAsMatrix();
      DisplayCoordinates = (DoubleMatrix)coordinates?.Clone();
      if (DisplayCoordinates != null && DisplayCoordinates.Columns != 2)
        throw new ArgumentException("Argument must have exactly two columns.", nameof(coordinates));
      if (DisplayCoordinates != null && DisplayCoordinates.Rows != Matrix.GetLength(0))
        throw new ArgumentException("Unequal number of rows in " + nameof(matrix) + " and " + nameof(coordinates) + ".");
    }

    public double GetDistance(int fromCity, int toCity) => Matrix[fromCity, toCity];
  }
}
