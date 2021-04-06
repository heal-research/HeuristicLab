using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman21 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman21() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman21(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman21(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.18.4 (m1*r1 + m2*r2)/(m1 + m2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "r" : "r_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"m1", "m2", "r1", "r2", noiseRatio == null ? "r" : "r_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"m1", "m2", "r1", "r2"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var m1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var m2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var r1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var r2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var r = new List<double>();

      data.Add(m1);
      data.Add(m2);
      data.Add(r1);
      data.Add(r2);
      data.Add(r);

      for (var i = 0; i < m1.Count; i++) {
        var res = (m1[i] * r1[i] + m2[i] * r2[i]) / (m1[i] + m2[i]);
        r.Add(res);
      }

      if (noiseRatio != null) {
        var r_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * r.StandardDeviationPop();
        r_noise.AddRange(r.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(r);
        data.Add(r_noise);
      }

      return data;
    }
  }
}