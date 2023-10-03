using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using Xunit;
using Xunit.Abstractions;
using Vector = MathNet.Numerics.LinearAlgebra.Double.DenseVector;

namespace DynamicRegressionProblemDataGenerator {
  public class SlidingWindowSymbolicRegressionPaperDataGenerator {
    private readonly ITestOutputHelper testOutputHelper;

    public SlidingWindowSymbolicRegressionPaperDataGenerator(ITestOutputHelper testOutputHelper) {
      this.testOutputHelper = testOutputHelper;
    }

    // columnnar 
    [Theory]
    [InlineData(100, 10, 42)]
    public void f1(int trainingRowsPerEpoch = 1000, int testRowsPerEpoch = 100, int seed = 42) {
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

      var epochHiddenStates = Enumerable.Empty<double>()
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

      for (int epoch = 0; epoch < epochHiddenStates.Count; epoch++) {
        double h = epochHiddenStates[epoch];
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

      var dataWriter = new StreamWriter(new FileStream(@"C:\Users\P41107\Desktop\F1.csv", FileMode.Create), Encoding.UTF8);
      dataWriter.WriteLine(string.Join(";", "x1", "x2", "x3", "y"));
      for (int i = 0; i < data["x1"].Count; i++) {
        dataWriter.WriteLine(string.Join(";", data["x1"][i], data["x2"][i], data["x3"][i], data["y"][i]));
      }
      dataWriter.Flush();
      var partitionsWriter = new StreamWriter(new FileStream(@"C:\Users\P41107\Desktop\F1.csv.indices", FileMode.Create), Encoding.UTF8);
      partitionsWriter.WriteLine(string.Join(";", "TrainingStart", "TrainingEnd", "TestStart", "TestEnd"));
      for (int i = 0; i < partitions.Count; i++) {
        partitionsWriter.WriteLine(string.Join(";", partitions[i].TrainStart, partitions[i].TrainEnd, partitions[i].TestStart, partitions[i].TestEnd));
      }
      partitionsWriter.Flush();
    }



    [Fact]
    public void Test1() {
      var h = Enumerable.Empty<double>()
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

      testOutputHelper.WriteLine(h.Count.ToString());
    }
  }
}
