using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman52 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman52() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman52(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman52(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.2.42 kappa*(T2-T1)*A/d | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Pwr" : "Pwr_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"kappa", "T1", "T2", "A", "d", noiseRatio == null ? "Pwr" : "Pwr_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"kappa", "T1", "T2", "A", "d"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var kappa = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var T1    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var T2    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var A     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var d     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var Pwr = new List<double>();

      data.Add(kappa);
      data.Add(T1);
      data.Add(T2);
      data.Add(A);
      data.Add(d);
      data.Add(Pwr);

      for (var i = 0; i < kappa.Count; i++) {
        var res = kappa[i] * (T2[i] - T1[i]) * A[i] / d[i];
        Pwr.Add(res);
      }

      if (noiseRatio != null) {
        var Pwr_noise   = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * Pwr.StandardDeviationPop();
        Pwr_noise.AddRange(Pwr.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(Pwr);
        data.Add(Pwr_noise);
      }

      return data;
    }
  }
}