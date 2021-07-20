using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman13 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman13() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman13(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman13(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.13.4 1/2*m*(v**2+u**2+w**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "K" : "K_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "m", "v", "u", "w", "K" } : new[] { "m", "v", "u", "w", "K", "K_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"m", "v", "u", "w"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var m    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var v    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var u    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var w    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var K = new List<double>();

      data.Add(m);
      data.Add(v);
      data.Add(u);
      data.Add(w);
      data.Add(K);

      for (var i = 0; i < m.Count; i++) {
        var res = 1.0 / 2 * m[i] * (Math.Pow(v[i], 2) + Math.Pow(u[i], 2) + Math.Pow(w[i], 2));
        K.Add(res);
      }

      var targetNoise = GetNoisyTarget(K, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}