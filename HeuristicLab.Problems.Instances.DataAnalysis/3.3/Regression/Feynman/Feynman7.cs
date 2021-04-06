using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman7 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman7() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman7(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman7(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.11.19 x1*y1+x2*y2+x3*y3 | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "A" : "A_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"x1", "x2", "x3", "y1", "y2", "y3", noiseRatio == null ? "A" : "A_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"x1", "x2", "x3", "y1", "y2", "y3"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var x1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var x2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var x3   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var y1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var y2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var y3   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var A = new List<double>();

      data.Add(x1);
      data.Add(x2);
      data.Add(x3);
      data.Add(y1);
      data.Add(y2);
      data.Add(y3);
      data.Add(A);

      for (var i = 0; i < x1.Count; i++) {
        var res = x1[i] * y1[i] + x2[i] * y2[i] + x3[i] * y3[i];
        A.Add(res);
      }

      if (noiseRatio != null) {
        var A_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * A.StandardDeviationPop();
        A_noise.AddRange(A.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(A);
        data.Add(A_noise);
      }

      return data;
    }
  }
}