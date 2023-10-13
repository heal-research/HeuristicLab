using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Random;
using MathNet.Numerics.Statistics;
using Xunit;

namespace DynamicRegressionProblemDataGenerator.DataGenerators {
  public class Friedman {
    
    [Theory, Trait("Generate", "Benchmark")]
    [InlineData(@"C:\Users\P41107\Desktop\Friedman_Test_{0}_{1}.csv", 1000, 100, 1, 0, 142, false)]
    [InlineData(@"C:\Users\P41107\Desktop\Friedman_Test_{0}_{1}.csv", 1000, 100, 2, 0, 143, false)]
    [InlineData(@"C:\Users\P41107\Desktop\Friedman_Test_{0}_{1}.csv", 1000, 100, 3, 0, 144, false)]
    void Generate(string fileName, int trainingRowsPerEpoch, int testRowsPerEpoch, int numberOfFeaturesPerState, double noiseRatio, uint seed, bool randomizeEpochs) {
      var random = new MersenneTwister(seed);
      
      var friedmanGenerator = new FriedmanRandomFunction(trainingRowsPerEpoch, testRowsPerEpoch, numberOfFeaturesPerState, noiseRatio, random);


      var hiddenEpochStates = new IReadOnlyList<double>[] {
        new double[]{ 1, 0, 0 },
        new double[]{ 0, 1, 0 },
        new double[]{ 0, 0, 1 },
        new double[]{ 0, 1, 1 },
        new double[]{ 1, 0, 1 },
        new double[]{ 1, 0, 0 },
        new double[]{ 1, 1, 1 },
      };
      fileName = string.Format(fileName, numberOfFeaturesPerState * hiddenEpochStates[0].Count, noiseRatio.ToString("0.00"));

      var data = new Dictionary<string, List<double>>();
      var partitions = new List<(int TrainStart, int TrainEnd, int TestStart, int TestEnd)>();

      var statesData = new[] {
        friedmanGenerator.GenerateRegressionData(),
        friedmanGenerator.GenerateRegressionData(),
        friedmanGenerator.GenerateRegressionData()
      };
      
      var inputNames = new List<string>();
      string targetName = null;
     
      foreach (IReadOnlyList<double> epochStates in hiddenEpochStates) {

        var epochInputs = new Dictionary<string, double[]>();
        var epochTargetsPerState = new List<double[]>();
        
        
        foreach ((double state, int i) in epochStates.Select((s, i) => (s, i))) {
          char statePrefix = (char)('A' + i);

          if (randomizeEpochs) {
            statesData[i] = friedmanGenerator.GenerateRegressionData();
          }

          var stateData = statesData[i];

          foreach (string originalVariableName in stateData.AllowedInputVariables) {
            string newVariableName = $"{statePrefix}_{originalVariableName}";
            epochInputs.Add(newVariableName, stateData.Dataset.GetDoubleValues(originalVariableName).ToArray());
            if (!inputNames.Contains(newVariableName)) inputNames.Add(newVariableName);
          }
          epochTargetsPerState.Add(stateData.Dataset.GetDoubleValues(stateData.TargetVariable).Select(t => t * state).ToArray());
          if (targetName == null) targetName = stateData.TargetVariable;
        }

        foreach (var input in epochInputs) {
          if (!data.ContainsKey(input.Key)) data.Add(input.Key, new List<double>());
          data[input.Key].AddRange(input.Value);
        }
        if (!data.ContainsKey(targetName)) data.Add(targetName, new List<double>());
        double[] summedTargets = new double[epochTargetsPerState[0].Length];
        foreach (double[] t in epochTargetsPerState) {
          double tMean = t.Mean();
          double tStandardDeviation = Statistics.StandardDeviation(t);
          if (tStandardDeviation.IsAlmost(0.0) || double.IsNaN(tStandardDeviation)) tStandardDeviation = 1.0;
          for (int r = 0; r < t.Length; r++) {
            summedTargets[r] += (t[r] - tMean) / tStandardDeviation;
          }
        }
        data[targetName].AddRange(summedTargets);
        
        int lastPartitionEnd = partitions.Count >= 1 ? partitions.Last().TestEnd : 0;
        partitions.Add((
            lastPartitionEnd, lastPartitionEnd + trainingRowsPerEpoch, 
            lastPartitionEnd + trainingRowsPerEpoch, lastPartitionEnd + trainingRowsPerEpoch + testRowsPerEpoch)
        );
      }

      WriteToFile(data, partitions, fileName, inputNames.ToArray().Concat(new[] { targetName }).ToArray());

    }
    
    private static void WriteToFile(
      Dictionary<string, List<double>> data, List<(int TrainStart, int TrainEnd, int TestStart, int TestEnd)> partitions,
      string fileName, params string[] columnNames) {
      var dataWriter = new StreamWriter(new FileStream(fileName, FileMode.Create), Encoding.UTF8);
      dataWriter.WriteLine(string.Join(";", columnNames));
      for (int i = 0; i < data.Values.First().Count; i++) {
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
