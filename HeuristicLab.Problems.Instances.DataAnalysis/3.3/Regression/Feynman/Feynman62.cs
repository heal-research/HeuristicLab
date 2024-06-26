using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman62 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman62() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman62(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman62(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.11.17 n_0*(1 + p_d*Ef*cos(theta)/(kb*T)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "n" : "n_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "n_0", "kb", "T", "theta", "p_d", "Ef", "n" } : new[] { "n_0", "kb", "T", "theta", "p_d", "Ef", "n", "n_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"n_0", "kb", "T", "theta", "p_d", "Ef"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var n_0   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var kb    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var T     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var theta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var p_d   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var Ef    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var n = new List<double>();

      data.Add(n_0);
      data.Add(kb);
      data.Add(T);
      data.Add(theta);
      data.Add(p_d);
      data.Add(Ef);
      data.Add(n);

      for (var i = 0; i < n_0.Count; i++) {
        var res = n_0[i] * (1 + p_d[i] * Ef[i] * Math.Cos(theta[i]) / (kb[i] * T[i]));
        n.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(n, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}