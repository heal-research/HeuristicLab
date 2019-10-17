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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.TravelingSalesman {
  [StorableType("1a9bf60c-b6a6-4c95-8e99-5a2dec0ee892")]
  public interface ITSPData : INamedItem {
    int Cities { get; }

    double GetDistance(int fromCity, int toCity);
    ITSPSolution GetSolution(Permutation tspTour, double tourLength);
    TSPData Export();
  }

  [Item("Matrix-based TSP Data", "TSP that is representd by a distance matrix.")]
  [StorableType("4df58a35-679d-4414-b815-9450ae100823")]
  public sealed class MatrixTSPData : NamedItem, ITSPData {
    [Storable] public DoubleMatrix Matrix { get; private set; }
    [Storable] public DoubleMatrix DisplayCoordinates { get; private set; }

    public int Cities => Matrix.Rows;

    [StorableConstructor]
    private  MatrixTSPData(StorableConstructorFlag _) : base(_) { }
    private MatrixTSPData(MatrixTSPData original, Cloner cloner) : base(original, cloner) {
      Matrix = original.Matrix;
      DisplayCoordinates = original.DisplayCoordinates;
    }
    public MatrixTSPData() {
      Name = TSPDefaultInstance.Name;
      Matrix = TSPDefaultInstance.Distances;
      DisplayCoordinates = TSPDefaultInstance.Coordinates;
    }
    public MatrixTSPData(string name, double[,] matrix, double[,] coordinates = null) {
      if (coordinates != null && coordinates.GetLength(1) != 2)
        throw new ArgumentException("Argument must have exactly two columns.", nameof(coordinates));
      if (coordinates != null && coordinates.GetLength(0) != matrix.GetLength(0))
        throw new ArgumentException("Unequal number of rows in " + nameof(matrix) + " and " + nameof(coordinates) + ".");
      Name = name;
      Matrix = new DoubleMatrix(matrix, @readonly: true);
      if (coordinates != null) DisplayCoordinates = new DoubleMatrix(coordinates, @readonly: true);
    }
    public MatrixTSPData(string name, DoubleMatrix matrix, DoubleMatrix coordinates = null) {
      if (coordinates != null && coordinates.Columns != 2)
        throw new ArgumentException("Argument must have exactly two columns.", nameof(coordinates));
      if (coordinates != null && coordinates.Rows != matrix.Rows)
        throw new ArgumentException("Unequal number of rows in " + nameof(matrix) + " and " + nameof(coordinates) + ".");
      Name = name;
      Matrix = matrix.AsReadOnly();
      DisplayCoordinates = coordinates?.AsReadOnly();
    }

    public ITSPSolution GetSolution(Permutation tour, double tourLength) {
      return new TSPSolution(this, tour, new DoubleValue(tourLength));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MatrixTSPData(this, cloner);
    }

    public double GetDistance(int fromCity, int toCity) => Matrix[fromCity, toCity];

    public TSPData Export() {
      return new TSPData() {
        Name = name,
        Description = description,
        Coordinates = DisplayCoordinates?.CloneAsMatrix(),
        Dimension = Matrix.Rows,
        DistanceMeasure = DistanceMeasure.Direct,
        Distances = Matrix.CloneAsMatrix()
      };
    }
  }

  [Item("Coordinates-based TSP Data", "TSP that is represented by coordinates of locations.")]
  [StorableType("3955d07a-d43c-4a01-9505-d2effb1ea865")]
  public abstract class CoordinatesTSPData : NamedItem, ITSPData {
    [Storable] public DoubleMatrix Coordinates { get; protected set; }

    public int Cities => Coordinates.Rows;

    [StorableConstructor]
    protected CoordinatesTSPData(StorableConstructorFlag _) : base(_) { }
    protected CoordinatesTSPData(CoordinatesTSPData original, Cloner cloner) : base(original, cloner) {
      Coordinates = original.Coordinates;
    }
    protected CoordinatesTSPData() : base() {
      Name = TSPDefaultInstance.Name;
      Coordinates = TSPDefaultInstance.Coordinates;
    }
    protected CoordinatesTSPData(string name, double[,] coordinates) : base() {
      if (coordinates == null) throw new ArgumentNullException(nameof(coordinates));
      if (coordinates.GetLength(1) != 2) throw new ArgumentException("Argument must have exactly two columns.", nameof(coordinates));
      Name = name;
      Coordinates = new DoubleMatrix(coordinates, @readonly: true);
    }
    protected CoordinatesTSPData(string name, DoubleMatrix coordinates) : base() {
      if (coordinates == null) throw new ArgumentNullException(nameof(coordinates));
      if (coordinates.Columns != 2) throw new ArgumentException("Argument must have exactly two columns.", nameof(coordinates));
      Name = name;
      Coordinates = coordinates.AsReadOnly();
    }

    public double GetDistance(int fromCity, int toCity) {
      return GetDistance(Coordinates[fromCity, 0], Coordinates[fromCity, 1],
        Coordinates[toCity, 0], Coordinates[toCity, 1]);
    }

    public abstract double GetDistance(double fromX, double fromY, double toX, double toY);

    public virtual ITSPSolution GetSolution(Permutation tour, double tourLength) {
      return new TSPSolution(this, tour, new DoubleValue(tourLength));
    }

    public abstract TSPData Export();
  }

  [Item("Euclidean TSP Data", "TSP that is represented by coordinates in an Euclidean plane.")]
  [StorableType("4bf58348-cd98-46c5-a4c0-55f486ca88b4")]
  public sealed class EuclideanTSPData : CoordinatesTSPData {
    public enum DistanceRounding { None, Midpoint, Ceiling }

    [Storable]
    public DistanceRounding Rounding { get; private set; }

    [StorableConstructor]
    private EuclideanTSPData(StorableConstructorFlag _) : base(_) { }
    private EuclideanTSPData(EuclideanTSPData original, Cloner cloner) : base(original, cloner) {
      Rounding = original.Rounding;
    }
    public EuclideanTSPData()
      : base() {
      Rounding = DistanceRounding.Midpoint;
    }
    public EuclideanTSPData(string name, double[,] coordinates, DistanceRounding rounding = DistanceRounding.None)
      : base(name, coordinates) {
      Rounding = rounding;
    }
    public EuclideanTSPData(string name, DoubleMatrix coordinates, DistanceRounding rounding = DistanceRounding.None)
      : base(name, coordinates) {
      Rounding = rounding;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EuclideanTSPData(this, cloner);
    }

    public override double GetDistance(double fromX, double fromY, double toX, double toY) {
      var dist = DistanceHelper.EuclideanDistance(fromX, fromY, toX, toY);
      switch (Rounding) {
        case DistanceRounding.None: return dist;
        case DistanceRounding.Midpoint: return Math.Round(dist);
        case DistanceRounding.Ceiling: return Math.Ceiling(dist);
        default: throw new InvalidOperationException("Unknown rounding mode " + Rounding);
      }
    }

    public override TSPData Export() {
      var data = new TSPData() {
        Name = name,
        Description = description,
        Coordinates = Coordinates.CloneAsMatrix(),
        Dimension = Coordinates.Rows
      };
      switch (Rounding) {
        case DistanceRounding.None: data.DistanceMeasure = DistanceMeasure.Euclidean; break;
        case DistanceRounding.Midpoint: data.DistanceMeasure = DistanceMeasure.RoundedEuclidean; break;
        case DistanceRounding.Ceiling: data.DistanceMeasure = DistanceMeasure.UpperEuclidean; break;
      }
      return data;
    }
  }

  [Item("Geo TSP Data", "TSP that is represented by geo coordinates.")]
  [StorableType("dc859a89-e6c2-4af3-a3b6-9aa3041b14a9")]
  public sealed class GeoTSPData : CoordinatesTSPData {
    [StorableConstructor]
    private GeoTSPData(StorableConstructorFlag _) : base(_) { }
    private GeoTSPData(GeoTSPData original, Cloner cloner) : base(original, cloner) { }
    public GeoTSPData() : base() { }
    public GeoTSPData(string name, double[,] coordinates) : base(name, coordinates) { }
    public GeoTSPData(string name, DoubleMatrix coordinates) : base(name, coordinates) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GeoTSPData(this, cloner);
    }

    public override double GetDistance(double fromX, double fromY, double toX, double toY) {
      return DistanceHelper.GeoDistance(fromX, fromY, toX, toY);
    }

    public override TSPData Export() {
      return new TSPData() {
        Name = name,
        Description = description,
        Coordinates = Coordinates.CloneAsMatrix(),
        Dimension = Coordinates.Rows,
        DistanceMeasure = DistanceMeasure.Geo
      };
    }
  }

  [Item("ATT TSP Data", "TSP that is represented by ATT coordinates.")]
  [StorableType("d7a0add3-6ec1-42e0-b1d7-b6454694d485")]
  public sealed class AttTSPData : CoordinatesTSPData {
    [StorableConstructor]
    private AttTSPData(StorableConstructorFlag _) : base(_) { }
    private AttTSPData(AttTSPData original, Cloner cloner) : base(original, cloner) { }
    public AttTSPData() : base() { }
    public AttTSPData(string name, double[,] coordinates) : base(name, coordinates) { }
    public AttTSPData(string name, DoubleMatrix coordinates) : base(name, coordinates) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AttTSPData(this, cloner);
    }

    public override double GetDistance(double fromX, double fromY, double toX, double toY) {
      return DistanceHelper.AttDistance(fromX, fromY, toX, toY);
    }

    public override TSPData Export() {
      return new TSPData() {
        Name = name,
        Description = description,
        Coordinates = Coordinates.CloneAsMatrix(),
        Dimension = Coordinates.Rows,
        DistanceMeasure = DistanceMeasure.Att
      };
    }
  }

  [Item("Manhattan TSP Data", "TSP that is represented by Manhattan coordinates.")]
  [StorableType("5f1ef9e2-cbd1-400e-8f87-6855f091fc9e")]
  public sealed class ManhattanTSPData : CoordinatesTSPData {
    [StorableConstructor]
    private ManhattanTSPData(StorableConstructorFlag _) : base(_) { }
    private ManhattanTSPData(ManhattanTSPData original, Cloner cloner) : base(original, cloner) { }
    public ManhattanTSPData() : base() { }
    public ManhattanTSPData(string name, double[,] coordinates) : base(name, coordinates) { }
    public ManhattanTSPData(string name, DoubleMatrix coordinates) : base(name, coordinates) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ManhattanTSPData(this, cloner);
    }

    public override double GetDistance(double fromX, double fromY, double toX, double toY) {
      return DistanceHelper.ManhattanDistance(fromX, fromY, toX, toY);
    }

    public override TSPData Export() {
      return new TSPData() {
        Name = name,
        Description = description,
        Coordinates = Coordinates.CloneAsMatrix(),
        Dimension = Coordinates.Rows,
        DistanceMeasure = DistanceMeasure.Manhattan
      };
    }
  }

  [Item("Manhattan TSP Data", "TSP that is represented by the maximum absolute distance in either x or y coordinates.")]
  [StorableType("c6294a6c-fe62-4906-9765-4bc306d3e4a8")]
  public sealed class MaximumTSPData : CoordinatesTSPData {
    [StorableConstructor]
    private MaximumTSPData(StorableConstructorFlag _) : base(_) { }
    private MaximumTSPData(MaximumTSPData original, Cloner cloner) : base(original, cloner) { }
    public MaximumTSPData() : base() { }
    public MaximumTSPData(string name, double[,] coordinates) : base(name, coordinates) { }
    public MaximumTSPData(string name, DoubleMatrix coordinates) : base(name, coordinates) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MaximumTSPData(this, cloner);
    }

    public override double GetDistance(double fromX, double fromY, double toX, double toY) {
      return DistanceHelper.MaximumDistance(fromX, fromY, toX, toY);
    }

    public override TSPData Export() {
      return new TSPData() {
        Name = name,
        Description = description,
        Coordinates = Coordinates.CloneAsMatrix(),
        Dimension = Coordinates.Rows,
        DistanceMeasure = DistanceMeasure.Maximum
      };
    }
  }

  internal static class TSPDefaultInstance {
    internal static readonly DoubleMatrix Distances = new DoubleMatrix(new double[,] {
{ 0, 100, 200, 300, 100, 141, 224, 316, 200, 224, 283, 361, 300, 316, 361, 424 },
{ 100, 0, 100, 200, 141, 100, 141, 224, 224, 200, 224, 283, 316, 300, 316, 361 },
{ 200, 100, 0, 100, 224, 141, 100, 141, 283, 224, 200, 224, 361, 316, 300, 316 },
{ 300, 200, 100, 0, 316, 224, 141, 100, 361, 283, 224, 200, 424, 361, 316, 300 },
{ 100, 141, 224, 316, 0, 100, 200, 300, 100, 141, 224, 316, 200, 224, 283, 361 },
{ 141, 100, 141, 224, 100, 0, 100, 200, 141, 100, 141, 224, 224, 200, 224, 283 },
{ 224, 141, 100, 141, 200, 100, 0, 100, 224, 141, 100, 141, 283, 224, 200, 224 },
{ 316, 224, 141, 100, 300, 200, 100, 0, 316, 224, 141, 100, 361, 283, 224, 200 },
{ 200, 224, 283, 361, 100, 141, 224, 316, 0, 100, 200, 300, 100, 141, 224, 316 },
{ 224, 200, 224, 283, 141, 100, 141, 224, 100, 0, 100, 200, 141, 100, 141, 224 },
{ 283, 224, 200, 224, 224, 141, 100, 141, 200, 100, 0, 100, 224, 141, 100, 141 },
{ 361, 283, 224, 200, 316, 224, 141, 100, 300, 200, 100, 0, 316, 224, 141, 100 },
{ 300, 316, 361, 424, 200, 224, 283, 361, 100, 141, 224, 316, 0, 100, 200, 300 },
{ 316, 300, 316, 361, 224, 200, 224, 283, 141, 100, 141, 224, 100, 0, 100, 200 },
{ 361, 316, 300, 316, 283, 224, 200, 224, 224, 141, 100, 141, 200, 100, 0, 100 },
{ 424, 361, 316, 300, 361, 283, 224, 200, 316, 224, 141, 100, 300, 200, 100, 0 },
}, @readonly: true);
    internal static readonly DoubleMatrix Coordinates = new DoubleMatrix(new double[,] {
{ 100, 100 }, { 100, 200 }, { 100, 300 }, { 100, 400 },
{ 200, 100 }, { 200, 200 }, { 200, 300 }, { 200, 400 },
{ 300, 100 }, { 300, 200 }, { 300, 300 }, { 300, 400 },
{ 400, 100 }, { 400, 200 }, { 400, 300 }, { 400, 400 }
}, @readonly: true);
    internal static readonly string Name = "HL TSP Default";
  }
}
