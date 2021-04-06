using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman60 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman60() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman60(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman60(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.10.9 sigma_den/epsilon*1/(1+chi) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Ef" : "Ef_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"sigma_den", "epsilon", "chi", noiseRatio == null ? "Ef" : "Ef_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"sigma_den", "epsilon", "chi"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data      = new List<List<double>>();
      var sigma_den = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var epsilon   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var chi       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var Ef = new List<double>();

      data.Add(sigma_den);
      data.Add(epsilon);
      data.Add(chi);
      data.Add(Ef);

      for (var i = 0; i < sigma_den.Count; i++) {
        var res = sigma_den[i] / epsilon[i] * 1 / (1 + chi[i]);
        Ef.Add(res);
      }

      if (noiseRatio != null) {
        var Ef_noise    = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * Ef.StandardDeviationPop();
        Ef_noise.AddRange(Ef.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(Ef);
        data.Add(Ef_noise);
      }

      return data;
    }
  }
}