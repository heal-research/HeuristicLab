using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman90 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman90() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman90(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman90(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "III.9.52 (p_d*Ef*t/h*sin((omega-omega_0)*t/2)**2/((omega-omega_0)*t/2)**2 | {0}",
           noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "prob" : "prob_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"p_d", "Ef", "t", "h", "omega", "omega_0", noiseRatio == null ? "prob" : "prob_noise"}; }
    }

    protected override string[] AllowedInputVariables {
      get { return new[] {"p_d", "Ef", "t", "h", "omega", "omega_0"}; }
    }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var p_d     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var Ef      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var t       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var h       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var omega   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var omega_0 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var prob = new List<double>();

      data.Add(p_d);
      data.Add(Ef);
      data.Add(t);
      data.Add(h);
      data.Add(omega);
      data.Add(omega_0);
      data.Add(prob);

      for (var i = 0; i < p_d.Count; i++) {
        var res = p_d[i] * Ef[i] * t[i] / h[i] *
                  Math.Pow(Math.Sin((omega[i] - omega_0[i]) * t[i] / 2), 2) /
                  Math.Pow((omega[i] - omega_0[i]) * t[i] / 2, 2);
        prob.Add(res);
      }

      if (noiseRatio != null) {
        var prob_noise  = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * prob.StandardDeviationPop();
        prob_noise.AddRange(prob.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(prob);
        data.Add(prob_noise);
      }

      return data;
    }
  }
}