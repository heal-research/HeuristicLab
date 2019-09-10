using System;
using System.ComponentModel;
using System.Drawing;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Problems.TravelingSalesman {
  [StorableType("1a9bf60c-b6a6-4c95-8e99-5a2dec0ee892")]
  public interface ITSPData : IItem {
    double GetDistance(int fromCity, int toCity);
    ITSPSolution GetSolution(Permutation tspTour, double tourLength);
  }

  [StorableType("f08a63d9-0b83-4944-9251-42925baeb872")]
  public interface ITSPSolution : IItem {
    DoubleMatrix Coordinates { get; }
    Permutation Tour { get; }
    DoubleValue TourLength { get; }
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

    public ITSPSolution GetSolution(Permutation tour, double tourLength) {
      return new PathTSPTour(DisplayCoordinates, tour, new DoubleValue(tourLength));
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

  [Item("Coordinates-based TSP Data", "TSP that is represented by coordinates of locations.")]
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

    public ITSPSolution GetSolution(Permutation tour, double tourLength) {
      return new PathTSPTour(Coordinates, tour, new DoubleValue(tourLength));
    }
  }

  [Item("Euclidean TSP Data", "TSP that is represented by coordinates in an Euclidean plane.")]
  [StorableType("4bf58348-cd98-46c5-a4c0-55f486ca88b4")]
  public sealed class EuclideanTSPData : CoordinatesTSPData {
    public enum RoundingMode { None, Midpoint, Ceiling }

    [Storable]
    public RoundingMode Rounding { get; set; }

    [StorableConstructor]
    private EuclideanTSPData(StorableConstructorFlag _) : base(_) { }
    private EuclideanTSPData(EuclideanTSPData original, Cloner cloner) : base(original, cloner) {
      Rounding = original.Rounding;
    }
    public EuclideanTSPData() : base() { }
    public EuclideanTSPData(double[,] coordinates) : base(coordinates) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EuclideanTSPData(this, cloner);
    }

    public override double GetDistance(double fromX, double fromY, double toX, double toY) {
      var dist = Math.Sqrt((fromX - toX) * (fromX - toX) + (fromY - toY) * (fromY - toY));
      switch (Rounding) {
        case RoundingMode.None: return dist;
        case RoundingMode.Midpoint: return Math.Round(dist);
        case RoundingMode.Ceiling: return Math.Ceiling(dist);
        default: throw new InvalidOperationException("Unknown rounding mode " + Rounding);
      }
    }
  }

  [Item("Geo TSP Data", "TSP that is represented by geo coordinates.")]
  [StorableType("4bf58348-cd98-46c5-a4c0-55f486ca88b4")]
  public sealed class GeoTSPData : CoordinatesTSPData {
    public const double PI = 3.141592;
    public const double RADIUS = 6378.388;

    [StorableConstructor]
    private GeoTSPData(StorableConstructorFlag _) : base(_) { }
    private GeoTSPData(GeoTSPData original, Cloner cloner) : base(original, cloner) { }
    public GeoTSPData() : base() { }
    public GeoTSPData(double[,] coordinates) : base(coordinates) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GeoTSPData(this, cloner);
    }

    public override double GetDistance(double fromX, double fromY, double toX, double toY) {
      double latitude1, longitude1, latitude2, longitude2;
      double q1, q2, q3;
      double length;

      latitude1 = ConvertToRadian(fromX);
      longitude1 = ConvertToRadian(fromY);
      latitude2 = ConvertToRadian(toX);
      longitude2 = ConvertToRadian(toY);

      q1 = Math.Cos(longitude1 - longitude2);
      q2 = Math.Cos(latitude1 - latitude2);
      q3 = Math.Cos(latitude1 + latitude2);

      length = (int)(RADIUS * Math.Acos(0.5 * ((1.0 + q1) * q2 - (1.0 - q1) * q3)) + 1.0);
      return (length);
    }

    private double ConvertToRadian(double x) {
      return PI * (Math.Truncate(x) + 5.0 * (x - Math.Truncate(x)) / 3.0) / 180.0;
    }
  }
  /// <summary>
  /// Represents a tour of a Traveling Salesman Problem given in path representation which can be visualized in the GUI.
  /// </summary>
  [Item("PathTSPTour", "Represents a tour of a Traveling Salesman Problem given in path representation which can be visualized in the GUI.")]
  [StorableType("2CAE7C49-751B-4802-9025-62E2268E47AE")]
  public sealed class PathTSPTour : Item, ITSPSolution, INotifyPropertyChanged {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Image; }
    }

    [Storable]
    private DoubleMatrix coordinates;
    public DoubleMatrix Coordinates {
      get { return coordinates; }
      set {
        if (coordinates == value) return;
        coordinates = value;
        OnPropertyChanged(nameof(Coordinates));
      }
    }

    [Storable(Name = "tour", OldName = "permutation")]
    private Permutation tour;
    public Permutation Tour {
      get { return tour; }
      set {
        if (tour == value) return;
        tour = value;
        OnPropertyChanged(nameof(Tour));
      }
    }
    [Storable(Name = "tourLength", OldName = "quality")]
    private DoubleValue tourLength;
    public DoubleValue TourLength {
      get { return tourLength; }
      set {
        if (tourLength == value) return;
        tourLength = value;
        OnPropertyChanged(nameof(TourLength));
      }
    }

    [StorableConstructor]
    private PathTSPTour(StorableConstructorFlag _) : base(_) { }
    private PathTSPTour(PathTSPTour original, Cloner cloner)
      : base(original, cloner) {
      this.coordinates = cloner.Clone(original.coordinates);
      this.tour = cloner.Clone(original.tour);
      this.tourLength = cloner.Clone(original.tourLength);
    }
    public PathTSPTour() : base() { }
    public PathTSPTour(DoubleMatrix coordinates)
      : base() {
      this.coordinates = coordinates;
    }
    public PathTSPTour(DoubleMatrix coordinates, Permutation permutation)
      : base() {
      this.coordinates = coordinates;
      this.tour = permutation;
    }
    public PathTSPTour(DoubleMatrix coordinates, Permutation permutation, DoubleValue quality)
      : base() {
      this.coordinates = coordinates;
      this.tour = permutation;
      this.tourLength = quality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PathTSPTour(this, cloner);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string property) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
  }
}
