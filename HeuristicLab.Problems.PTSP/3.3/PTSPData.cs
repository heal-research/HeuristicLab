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
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.TravelingSalesman;

namespace HeuristicLab.Problems.PTSP {
  [StorableType("dd2d0ecc-372e-46f1-846f-fb4ca2afa124")]
  public interface IProbabilisticTSPData : ITSPData {
    double GetProbability(int city);
    new PTSPData Export();
  }

  [Item("Matrix-based p-TSP Data", "p-TSP that is representd by a distance matrix.")]
  [StorableType("30ebade1-d28c-4e45-b195-c7fa32a15df5")]
  public class MatrixProbabilisticTSPData : MatrixTSPData, IProbabilisticTSPData {
    [Storable] public PercentArray Probabilities { get; protected set; }

    [StorableConstructor]
    protected MatrixProbabilisticTSPData(StorableConstructorFlag _) : base(_) { }
    protected MatrixProbabilisticTSPData(MatrixProbabilisticTSPData original, Cloner cloner) : base(original, cloner) {
      Probabilities = original.Probabilities;
    }
    public MatrixProbabilisticTSPData() {
      Name = PTSPDefaultInstance.Name;
      Matrix = PTSPDefaultInstance.Distances;
      Probabilities = PTSPDefaultInstance.Probabilities;
      DisplayCoordinates = PTSPDefaultInstance.Coordinates;
    }
    public MatrixProbabilisticTSPData(string name, double[,] matrix, double[] probabilities, double[,] coordinates = null)
      : base(name, matrix, coordinates) {
      Probabilities = new PercentArray(probabilities, @readonly: true);
    }
    public MatrixProbabilisticTSPData(string name, DoubleMatrix matrix, PercentArray probabilities, DoubleMatrix coordinates = null)
      : base(name, matrix, coordinates) {
      Probabilities = probabilities.AsReadOnly();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MatrixProbabilisticTSPData(this, cloner);
    }

    public double GetProbability(int city) => Probabilities[city];

    public new PTSPData Export() {
      return new PTSPData() {
        Name = name,
        Description = description,
        Coordinates = DisplayCoordinates?.CloneAsMatrix(),
        Dimension = Matrix.Rows,
        DistanceMeasure = DistanceMeasure.Direct,
        Distances = Matrix.CloneAsMatrix(),
        Probabilities = Probabilities.CloneAsArray()
      };
    }
  }

  [Item("Euclidean p-TSP Data", "p-TSP that is represented by coordinates in an Euclidean plane.")]
  [StorableType("8d4cf257-9013-4746-bc6c-37615954c3fb")]
  public class EuclideanPTSPData : EuclideanTSPData, IProbabilisticTSPData {
    [Storable] public PercentArray Probabilities { get; protected set; }

    [StorableConstructor]
    protected EuclideanPTSPData(StorableConstructorFlag _) : base(_) { }
    protected EuclideanPTSPData(EuclideanPTSPData original, Cloner cloner) : base(original, cloner) {
      Probabilities = original.Probabilities;
    }
    public EuclideanPTSPData() : base() {
      Name = PTSPDefaultInstance.Name;
      Coordinates = PTSPDefaultInstance.Coordinates;
      Probabilities = PTSPDefaultInstance.Probabilities;
      Rounding = DistanceRounding.Midpoint;
    }
    public EuclideanPTSPData(string name, double[,] coordinates, double[] probabilities, DistanceRounding rounding = DistanceRounding.None)
      : base(name, coordinates) {
      if (coordinates.GetLength(0) != probabilities.Length) throw new InvalidOperationException("Number of cities is ambiguous between " + nameof(coordinates) + " and " + nameof(probabilities) + ".");
      Probabilities = new PercentArray(probabilities, @readonly: true);
    }
    public EuclideanPTSPData(string name, DoubleMatrix coordinates, PercentArray probabilities, DistanceRounding rounding = DistanceRounding.None)
      : base(name, coordinates, rounding) {
      if (coordinates.Rows != probabilities.Length) throw new InvalidOperationException("Number of cities is ambiguous between " + nameof(coordinates) + " and " + nameof(probabilities) + ".");
      Probabilities = probabilities.AsReadOnly();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EuclideanPTSPData(this, cloner);
    }

    public double GetProbability(int city) => Probabilities[city];

    public new PTSPData Export() {
      var data = new PTSPData() {
        Name = name,
        Description = description,
        Coordinates = Coordinates.CloneAsMatrix(),
        Probabilities = Probabilities.CloneAsArray(),
        Dimension = Coordinates.Rows,
      };
      switch (Rounding) {
        case DistanceRounding.None: data.DistanceMeasure = DistanceMeasure.Euclidean; break;
        case DistanceRounding.Midpoint: data.DistanceMeasure = DistanceMeasure.RoundedEuclidean; break;
        case DistanceRounding.Ceiling: data.DistanceMeasure = DistanceMeasure.UpperEuclidean; break;
      }
      return data;
    }
  }

  [Item("Geo p-TSP Data", "p-TSP that is represented by geo coordinates.")]
  [StorableType("b175e0be-5706-4c44-b3f0-dcb948d5c47a")]
  public class GeoPTSPData : GeoTSPData, IProbabilisticTSPData {
    [Storable] public PercentArray Probabilities { get; protected set; }

    [StorableConstructor]
    protected GeoPTSPData(StorableConstructorFlag _) : base(_) { }
    protected GeoPTSPData(GeoPTSPData original, Cloner cloner)
      : base(original, cloner) {
      Probabilities = original.Probabilities;
    }
    public GeoPTSPData() : base() {
      Name = PTSPDefaultInstance.Name;
      Coordinates = PTSPDefaultInstance.Coordinates;
      Probabilities = PTSPDefaultInstance.Probabilities;
    }
    public GeoPTSPData(string name, double[,] coordinates, double[] probabilities)
      : base(name, coordinates) {
      if (coordinates.GetLength(0) != probabilities.Length) throw new InvalidOperationException("Number of cities is ambiguous between " + nameof(coordinates) + " and " + nameof(probabilities) + ".");
      Probabilities = new PercentArray(probabilities, @readonly: true);
    }
    public GeoPTSPData(string name, DoubleMatrix coordinates, PercentArray probabilities)
      : base(name, coordinates) {
      if (coordinates.Rows != probabilities.Length) throw new InvalidOperationException("Number of cities is ambiguous between " + nameof(coordinates) + " and " + nameof(probabilities) + ".");
      Probabilities = probabilities.AsReadOnly();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GeoPTSPData(this, cloner);
    }

    public double GetProbability(int city) => Probabilities[city];

    public new PTSPData Export() {
      return new PTSPData() {
        Name = name,
        Description = description,
        Coordinates = Coordinates.CloneAsMatrix(),
        Probabilities = Probabilities.CloneAsArray(),
        Dimension = Coordinates.Rows,
        DistanceMeasure = DistanceMeasure.Geo,
      };
    }
  }

  [Item("ATT p-TSP Data", "p-TSP that is represented by ATT distance.")]
  [StorableType("7a2aa605-58d2-4533-a763-3d474e185460")]
  public class AttPTSPData : AttTSPData, IProbabilisticTSPData {
    [Storable] public PercentArray Probabilities { get; protected set; }

    [StorableConstructor]
    protected AttPTSPData(StorableConstructorFlag _) : base(_) { }
    protected AttPTSPData(AttPTSPData original, Cloner cloner)
      : base(original, cloner) {
      Probabilities = original.Probabilities;
    }
    public AttPTSPData() : base() {
      Name = PTSPDefaultInstance.Name;
      Coordinates = PTSPDefaultInstance.Coordinates;
      Probabilities = PTSPDefaultInstance.Probabilities;
    }
    public AttPTSPData(string name, double[,] coordinates, double[] probabilities)
      : base(name, coordinates) {
      if (coordinates.GetLength(0) != probabilities.Length) throw new InvalidOperationException("Number of cities is ambiguous between " + nameof(coordinates) + " and " + nameof(probabilities) + ".");
      Probabilities = new PercentArray(probabilities, @readonly: true);
    }
    public AttPTSPData(string name, DoubleMatrix coordinates, PercentArray probabilities)
      : base(name, coordinates) {
      if (coordinates.Rows != probabilities.Length) throw new InvalidOperationException("Number of cities is ambiguous between " + nameof(coordinates) + " and " + nameof(probabilities) + ".");
      Probabilities = probabilities.AsReadOnly();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AttPTSPData(this, cloner);
    }

    public double GetProbability(int city) => Probabilities[city];

    public new PTSPData Export() {
      return new PTSPData() {
        Name = name,
        Description = description,
        Coordinates = Coordinates.CloneAsMatrix(),
        Probabilities = Probabilities.CloneAsArray(),
        Dimension = Coordinates.Rows,
        DistanceMeasure = DistanceMeasure.Att,
      };
    }
  }

  [Item("Manhattan p-TSP Data", "p-TSP that is represented by Manhattan distance.")]
  [StorableType("cc43d1db-3da9-4d6e-becb-6b475b42fc59")]
  public class ManhattanPTSPData : ManhattanTSPData, IProbabilisticTSPData {
    [Storable] public PercentArray Probabilities { get; protected set; }

    [StorableConstructor]
    protected ManhattanPTSPData(StorableConstructorFlag _) : base(_) { }
    protected ManhattanPTSPData(ManhattanPTSPData original, Cloner cloner)
      : base(original, cloner) {
      Probabilities = original.Probabilities;
    }
    public ManhattanPTSPData() : base() {
      Name = PTSPDefaultInstance.Name;
      Coordinates = PTSPDefaultInstance.Coordinates;
      Probabilities = PTSPDefaultInstance.Probabilities;
    }
    public ManhattanPTSPData(string name, double[,] coordinates, double[] probabilities)
      : base(name, coordinates) {
      if (coordinates.GetLength(0) != probabilities.Length) throw new InvalidOperationException("Number of cities is ambiguous between " + nameof(coordinates) + " and " + nameof(probabilities) + ".");
      Probabilities = new PercentArray(probabilities, @readonly: true);
    }
    public ManhattanPTSPData(string name, DoubleMatrix coordinates, PercentArray probabilities)
      : base(name, coordinates) {
      if (coordinates.Rows != probabilities.Length) throw new InvalidOperationException("Number of cities is ambiguous between " + nameof(coordinates) + " and " + nameof(probabilities) + ".");
      Probabilities = probabilities.AsReadOnly();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ManhattanPTSPData(this, cloner);
    }

    public double GetProbability(int city) => Probabilities[city];

    public new PTSPData Export() {
      return new PTSPData() {
        Name = name,
        Description = description,
        Coordinates = Coordinates.CloneAsMatrix(),
        Probabilities = Probabilities.CloneAsArray(),
        Dimension = Coordinates.Rows,
        DistanceMeasure = DistanceMeasure.Manhattan,
      };
    }
  }

  [Item("Maximum p-TSP Data", "p-TSP that is represented by maximum absolute distance in either the x or y coordinates.")]
  [StorableType("82e9cde2-a942-403a-80ff-8deae4fa8214")]
  public class MaximumPTSPData : MaximumTSPData, IProbabilisticTSPData {
    [Storable] public PercentArray Probabilities { get; protected set; }

    [StorableConstructor]
    protected MaximumPTSPData(StorableConstructorFlag _) : base(_) { }
    protected MaximumPTSPData(MaximumPTSPData original, Cloner cloner)
      : base(original, cloner) {
      Probabilities = original.Probabilities;
    }
    public MaximumPTSPData() : base() {
      Name = PTSPDefaultInstance.Name;
      Coordinates = PTSPDefaultInstance.Coordinates;
      Probabilities = PTSPDefaultInstance.Probabilities;
    }
    public MaximumPTSPData(string name, double[,] coordinates, double[] probabilities)
      : base(name, coordinates) {
      if (coordinates.GetLength(0) != probabilities.Length) throw new InvalidOperationException("Number of cities is ambiguous between " + nameof(coordinates) + " and " + nameof(probabilities) + ".");
      Probabilities = new PercentArray(probabilities, @readonly: true);
    }
    public MaximumPTSPData(string name, DoubleMatrix coordinates, PercentArray probabilities)
      : base(name, coordinates) {
      if (coordinates.Rows != probabilities.Length) throw new InvalidOperationException("Number of cities is ambiguous between " + nameof(coordinates) + " and " + nameof(probabilities) + ".");
      Probabilities = probabilities.AsReadOnly();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MaximumPTSPData(this, cloner);
    }

    public double GetProbability(int city) => Probabilities[city];

    public new PTSPData Export() {
      return new PTSPData() {
        Name = name,
        Description = description,
        Coordinates = Coordinates.CloneAsMatrix(),
        Probabilities = Probabilities.CloneAsArray(),
        Dimension = Coordinates.Rows,
        DistanceMeasure = DistanceMeasure.Maximum,
      };
    }
  }

  internal static class PTSPDefaultInstance {
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
    internal static readonly PercentArray Probabilities = new PercentArray(new double[] {
0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.7, 0.6, 0.5, 0.4, 0.3, 0.2, 0.3, 0.4, 0.5
}, @readonly: true);
    internal static readonly string Name = "HL PTSP Default";
  }
}
