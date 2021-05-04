using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus7 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus7() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus7(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus7(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("Goldstein 3.16: sqrt(2/m*(E_n-U-L**2/(2*m*r**2))) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "v" : "v_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "m", "E_n", "U", "L", "r", "v" } : new[] { "m", "E_n", "U", "L", "r", "v", "v_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"m", "E_n", "U", "L", "r"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var m    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var E_n  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 8, 12).ToList();
      var U    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var L    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var r    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var v = new List<double>();

      data.Add(m);
      data.Add(E_n);
      data.Add(U);
      data.Add(L);
      data.Add(r);
      data.Add(v);

      for (var i = 0; i < m.Count; i++) {
        var res = Math.Sqrt(2 / m[i] * (E_n[i] - U[i] - Math.Pow(L[i], 2) / (2 * m[i] * Math.Pow(r[i], 2))));
        v.Add(res);
      }

      var targetNoise = GetNoisyTarget(v, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}