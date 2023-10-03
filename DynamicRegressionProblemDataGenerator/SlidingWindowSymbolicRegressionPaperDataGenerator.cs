using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using Vector = MathNet.Numerics.LinearAlgebra.Double.DenseVector;

namespace DynamicRegressionProblemDataGenerator {
  public class SlidingWindowSymbolicRegressionPaperDataGenerator {

    private static readonly IReadOnlyList<double> EpochHiddenStates = Enumerable.Empty<double>()
      .Concat(Generate.Repeat(100, 1.0))
      .Concat(Generate.LinearSpaced(10, 1.0, 0.0))
      .Concat(Generate.Repeat(100, 0.0))
      .Concat(Generate.Repeat(100, 1.0))
      .Concat(Generate.LinearSpaced(50, 1.0, 0.0))
      .Concat(Generate.Repeat(100, 0.0))
      .Concat(Generate.LinearSpaced(20, 0.0, 1.0))
      .Concat(Generate.Repeat(50, 1.0))
      .Concat(Generate.Repeat(100, 0.0))
      .Concat(Generate.LinearSpaced(10, 0.0, 1.0))
      .Concat(Generate.Repeat(20, 1.0))
      .ToList();
    
    [Theory, Trait("Generate", "Benchmark")]
    [InlineData(@"C:\Users\P41107\Desktop\F1.csv", 100, 10, 42)]
    public void Generate_F1(string fileName, int trainingRowsPerEpoch, int testRowsPerEpoch, int seed) {
      var random = new Random(seed);

      var uniformDist = new ContinuousUniform(1, 10, random);
      var normalDist = new Normal(0, 1, random);

      var data = new Dictionary<string, List<double>>() {
        { "x1", new List<double>() },
        { "x2", new List<double>() },
        { "x3", new List<double>() },
        { "y", new List<double>() }
      };
      var partitions = new List<(int TrainStart, int TrainEnd, int TestStart, int TestEnd)>();
      
      for (int epoch = 0; epoch < EpochHiddenStates.Count; epoch++) {
        double h = EpochHiddenStates[epoch];
        var x1 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x2 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x3 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        
        var y = x1.PointwiseMultiply(h * x2 + (1 - h) * x3);

        data["x1"].AddRange(x1);
        data["x2"].AddRange(x2);
        data["x3"].AddRange(x3);
        data["y"].AddRange(y);
        
        int lastPartitionEnd = partitions.Count >= 1 ? partitions.Last().TestEnd : 0;
        partitions.Add((
          lastPartitionEnd, lastPartitionEnd + trainingRowsPerEpoch, 
          lastPartitionEnd + trainingRowsPerEpoch, lastPartitionEnd + trainingRowsPerEpoch + testRowsPerEpoch)
        );
      }

      WriteToFile(data, partitions, fileName, "x1", "x2", "x3", "y");
    }
    
    [Theory, Trait("Generate", "Benchmark")]
    [InlineData(@"C:\Users\P41107\Desktop\F2.csv", 100, 10, 43)]
    public void Generate_F2(string filename, int trainingRowsPerEpoch, int testRowsPerEpoch, int seed) {
      var random = new Random(seed);

      var uniformDist = new ContinuousUniform(1, 10, random);
      var normalDist = new Normal(0, 1, random);

      var data = new Dictionary<string, List<double>>() {
        { "x1", new List<double>() },
        { "x2", new List<double>() },
        { "x3", new List<double>() },
        { "x4", new List<double>() },
        { "x5", new List<double>() },
        { "x6", new List<double>() },
        { "x7", new List<double>() },
        { "x8", new List<double>() },
        { "y", new List<double>() }
      };
      var partitions = new List<(int TrainStart, int TrainEnd, int TestStart, int TestEnd)>();
     
      for (int epoch = 0; epoch < EpochHiddenStates.Count; epoch++) {
        double h = EpochHiddenStates[epoch];
        var x1 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x2 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x3 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x4 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x5 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x6 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x7 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x8 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());

        var left = x1.PointwiseMultiply(x2) + x3.PointwiseMultiply(x4);
        var right = h * x5.PointwiseMultiply(x6) + (1 - h) * x7.PointwiseMultiply(x8);

        var y = left / left.Variance() + right / right.Variance();
        
        data["x1"].AddRange(x1);
        data["x2"].AddRange(x2);
        data["x3"].AddRange(x3);
        data["x4"].AddRange(x4);
        data["x5"].AddRange(x5);
        data["x6"].AddRange(x6);
        data["x7"].AddRange(x7);
        data["x8"].AddRange(x8);
        data["y"].AddRange(y);
        
        int lastPartitionEnd = partitions.Count >= 1 ? partitions.Last().TestEnd : 0;
        partitions.Add((
          lastPartitionEnd, lastPartitionEnd + trainingRowsPerEpoch, 
          lastPartitionEnd + trainingRowsPerEpoch, lastPartitionEnd + trainingRowsPerEpoch + testRowsPerEpoch)
        );
      }

      WriteToFile(data, partitions, filename, "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "y");
    }
    
    [Theory, Trait("Generate", "Benchmark")]
    [InlineData(@"C:\Users\P41107\Desktop\F3.csv", 100, 10, 44)]
    public void Generate_F3(string filename, int trainingRowsPerEpoch, int testRowsPerEpoch, int seed) {
      var random = new Random(seed);

      var uniformDist = new ContinuousUniform(1, 10, random);
      var normalDist = new Normal(0, 1, random);

      var data = new Dictionary<string, List<double>>() {
        { "x1", new List<double>() },
        { "x2", new List<double>() },
        { "x3", new List<double>() },
        { "x4", new List<double>() },
        { "x5", new List<double>() },
        { "x6", new List<double>() },
        { "x7", new List<double>() },
        { "x8", new List<double>() },
        { "x9", new List<double>() },
        { "x10", new List<double>() },
        { "y", new List<double>() }
      };
      var partitions = new List<(int TrainStart, int TrainEnd, int TestStart, int TestEnd)>();
     
      for (int epoch = 0; epoch < EpochHiddenStates.Count; epoch++) {
        double h = EpochHiddenStates[epoch];
        var x1 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x2 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x3 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x4 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x5 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x6 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x7 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x8 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x9 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        var x10 = new Vector(uniformDist.Samples().Take(trainingRowsPerEpoch + testRowsPerEpoch).ToArray());
        
        var left = x1.PointwiseMultiply(x2).PointwiseMultiply(x3) + x4.PointwiseMultiply(x5).PointwiseMultiply(x6);
        var right = h * x7.PointwiseMultiply(x8) + (1 - h) * x9.PointwiseMultiply(x10);

        var y = left / left.Variance() + right / right.Variance();
        
        data["x1"].AddRange(x1);
        data["x2"].AddRange(x2);
        data["x3"].AddRange(x3);
        data["x4"].AddRange(x4);
        data["x5"].AddRange(x5);
        data["x6"].AddRange(x6);
        data["x7"].AddRange(x7);
        data["x8"].AddRange(x8);
        data["x9"].AddRange(x9);
        data["x10"].AddRange(x10);
        data["y"].AddRange(y);
        
        int lastPartitionEnd = partitions.Count >= 1 ? partitions.Last().TestEnd : 0;
        partitions.Add((
          lastPartitionEnd, lastPartitionEnd + trainingRowsPerEpoch, 
          lastPartitionEnd + trainingRowsPerEpoch, lastPartitionEnd + trainingRowsPerEpoch + testRowsPerEpoch)
        );
      }

      WriteToFile(data, partitions, filename, "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "x9", "x10", "y");
    }

    private static void WriteToFile(
      Dictionary<string, List<double>> data, List<(int TrainStart, int TrainEnd, int TestStart, int TestEnd)> partitions,
      string fileName, params string[] columnNames) {
      var dataWriter = new StreamWriter(new FileStream(fileName, FileMode.Create), Encoding.UTF8);
      dataWriter.WriteLine(string.Join(";", columnNames));
      for (int i = 0; i < data["x1"].Count; i++) {
        var rowValues = columnNames.Select(c => data[c][i]).ToList();
        dataWriter.WriteLine(string.Join(";", rowValues));
      }
      dataWriter.Flush();
      var partitionsWriter = new StreamWriter(new FileStream($"{fileName}.indices", FileMode.Create), Encoding.UTF8);
      partitionsWriter.WriteLine(string.Join(";", "TrainingStart", "TrainingEnd", "TestStart", "TestEnd"));
      for (int i = 0; i < partitions.Count; i++) {
        partitionsWriter.WriteLine(string.Join(";", partitions[i].TrainStart, partitions[i].TrainEnd, partitions[i].TestStart, partitions[i].TestEnd));
      }
      partitionsWriter.Flush();
    }
  }
}
