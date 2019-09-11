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
using System.ComponentModel;
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

  [Item("Matrix-based TSP Data", "TSP that is representd by a distance matrix.")]
  [StorableType("4df58a35-679d-4414-b815-9450ae100823")]
  public sealed class MatrixTSPData : Item, ITSPData, INotifyPropertyChanged {

    [Storable]
    public DoubleMatrix Matrix { get; private set; }

    [Storable]
    private DoubleMatrix displayCoordinates;
    public DoubleMatrix DisplayCoordinates {
      get => displayCoordinates;
      set {
        if (displayCoordinates == value) return;
        displayCoordinates = value;
        OnPropertyChanged(nameof(DisplayCoordinates));
      }
    }

    [StorableConstructor]
    private MatrixTSPData(StorableConstructorFlag _) : base(_) { }
    private MatrixTSPData(MatrixTSPData original, Cloner cloner) : base(original, cloner) {
      Matrix = original.Matrix;
      displayCoordinates = cloner.Clone(original.displayCoordinates);
    }
    public MatrixTSPData() {
      Matrix = new DoubleMatrix(new double[0, 0], @readonly: true);
      DisplayCoordinates = null;
    }
    public MatrixTSPData(double[,] matrix, double[,] coordinates = null) {
      Matrix = new DoubleMatrix(matrix, @readonly: true);
      if (coordinates != null) DisplayCoordinates = new DoubleMatrix(coordinates);
      if (DisplayCoordinates != null && DisplayCoordinates.Columns != 2)
        throw new ArgumentException("Argument must have exactly two columns.", nameof(coordinates));
      if (DisplayCoordinates != null && DisplayCoordinates.Rows != Matrix.Rows)
        throw new ArgumentException("Unequal number of rows in " + nameof(matrix) + " and " + nameof(coordinates) + ".");
    }

    public ITSPSolution GetSolution(Permutation tour, double tourLength) {
      return new TSPSolution(DisplayCoordinates, tour, new DoubleValue(tourLength));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MatrixTSPData(this, cloner);
    }

    public void SetMatrix(double[,] matrix, double[,] coordinates = null) {
      Matrix = new DoubleMatrix(matrix, @readonly: true);
      OnPropertyChanged(nameof(Matrix));
      if (coordinates == null) DisplayCoordinates = null;
      else DisplayCoordinates = new DoubleMatrix(coordinates);
      if (DisplayCoordinates != null && DisplayCoordinates.Columns != 2)
        throw new ArgumentException("Argument must have exactly two columns.", nameof(coordinates));
      if (DisplayCoordinates != null && DisplayCoordinates.Rows != Matrix.Rows)
        throw new ArgumentException("Unequal number of rows in " + nameof(matrix) + " and " + nameof(coordinates) + ".");
    }

    public void SetMatrix(DoubleMatrix matrix, DoubleMatrix coordinates = null) {
      Matrix = (DoubleMatrix)matrix.AsReadOnly();
      OnPropertyChanged(nameof(Matrix));
      DisplayCoordinates = (DoubleMatrix)coordinates?.Clone();
      if (DisplayCoordinates != null && DisplayCoordinates.Columns != 2)
        throw new ArgumentException("Argument must have exactly two columns.", nameof(coordinates));
      if (DisplayCoordinates != null && DisplayCoordinates.Rows != Matrix.Rows)
        throw new ArgumentException("Unequal number of rows in " + nameof(matrix) + " and " + nameof(coordinates) + ".");
    }

    public double GetDistance(int fromCity, int toCity) => Matrix[fromCity, toCity];


    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string property) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
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
      return new TSPSolution(Coordinates, tour, new DoubleValue(tourLength));
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
}
