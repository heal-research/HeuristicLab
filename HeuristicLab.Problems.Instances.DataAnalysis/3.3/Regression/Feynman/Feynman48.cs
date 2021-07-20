using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman48 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman48() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman48(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman48(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.44.4 n*kb*T*ln(V2/V1) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "E_n" : "E_n_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "n", "kb", "T", "V1", "V2", "E_n" } : new[] { "n", "kb", "T", "V1", "V2", "E_n", "E_n_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"n", "kb", "T", "V1", "V2"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var n    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var kb   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var T    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var V1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var V2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var E_n = new List<double>();

      data.Add(n);
      data.Add(kb);
      data.Add(T);
      data.Add(V1);
      data.Add(V2);
      data.Add(E_n);

      for (var i = 0; i < n.Count; i++) {
        var res = n[i] * kb[i] * T[i] * Math.Log(V2[i] / V1[i]);
        E_n.Add(res);
      }

      var targetNoise = GetNoisyTarget(E_n, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}