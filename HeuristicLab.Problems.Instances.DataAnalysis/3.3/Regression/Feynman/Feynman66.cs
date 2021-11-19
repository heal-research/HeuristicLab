using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman66 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman66() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman66(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman66(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.13.17 1/(4*pi*epsilon*c**2)*2*I/r | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "B" : "B_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "epsilon", "c", "I", "r", "B" } : new[] { "epsilon", "c", "I", "r", "B", "B_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"epsilon", "c", "I", "r"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var epsilon = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var c       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var I       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var r       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var B = new List<double>();

      data.Add(epsilon);
      data.Add(c);
      data.Add(I);
      data.Add(r);
      data.Add(B);

      for (var i = 0; i < epsilon.Count; i++) {
        var res = 1.0 / (4 * Math.PI * epsilon[i] * Math.Pow(c[i], 2)) * 2 * I[i] / r[i];
        B.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(B, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}