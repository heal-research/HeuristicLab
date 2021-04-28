using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus4 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus4() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus4(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus4(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Radiated gravitational wave power: -32/5*G**4/c**5*(m1*m2)**2*(m1+m2)/r**5 | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Pwr" : "Pwr_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"G", "c", "m1", "m2", "r", noiseRatio == null ? "Pwr" : "Pwr_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"G", "c", "m1", "m2", "r"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var G    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var c    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var m1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var m2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var r    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();

      var Pwr = new List<double>();

      data.Add(G);
      data.Add(c);
      data.Add(m1);
      data.Add(m2);
      data.Add(r);
      data.Add(Pwr);

      for (var i = 0; i < G.Count; i++) {
        var res = -32.0 / 5 * Math.Pow(G[i], 4) / Math.Pow(c[i], 5) * Math.Pow(m1[i] * m2[i], 2) * (m1[i] + m2[i]) /
                  Math.Pow(r[i], 5);
        Pwr.Add(res);
      }

      if (noiseRatio != null) {
        var Pwr_noise   = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * Pwr.StandardDeviationPop();
        Pwr_noise.AddRange(Pwr.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(Pwr);
        data.Add(Pwr_noise);
      }

      return data;
    }
  }
}