using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman47 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman47() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman47(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman47(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.43.43 1/(gamma-1)*kb*v/A | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "kappa" : "kappa_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"gamma", "kb", "A", "v", noiseRatio == null ? "kappa" : "kappa_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"gamma", "kb", "A", "v"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var gamma = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 2, 5).ToList();
      var kb    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var A     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var v     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var kappa = new List<double>();

      data.Add(gamma);
      data.Add(kb);
      data.Add(A);
      data.Add(v);
      data.Add(kappa);

      for (var i = 0; i < gamma.Count; i++) {
        var res = 1.0 / (gamma[i] - 1) * kb[i] * v[i] / A[i];
        kappa.Add(res);
      }

      if (noiseRatio != null) {
        var kappa_noise = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * kappa.StandardDeviationPop();
        kappa_noise.AddRange(kappa.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(kappa);
        data.Add(kappa_noise);
      }

      return data;
    }
  }
}