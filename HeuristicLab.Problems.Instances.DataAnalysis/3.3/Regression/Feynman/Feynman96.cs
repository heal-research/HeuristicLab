using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman96 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman96() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman96(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman96(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("III.15.14 h**2/(2*E_n*d**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "m" : "m_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "h", "E_n", "d", "m" } : new[] { "h", "E_n", "d", "m", "m_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"h", "E_n", "d"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var h    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var E_n  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var d    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var m = new List<double>();

      data.Add(h);
      data.Add(E_n);
      data.Add(d);
      data.Add(m);

      for (var i = 0; i < h.Count; i++) {
        var res = Math.Pow(h[i], 2) / (2 * E_n[i] * Math.Pow(d[i], 2));
        m.Add(res);
      }

      var targetNoise = GetNoisyTarget(m, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}