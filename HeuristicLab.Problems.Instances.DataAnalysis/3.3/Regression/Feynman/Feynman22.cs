using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman22 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman22() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman22(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman22(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.18.12 r*F*sin(theta) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "tau" : "tau_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"r", "F", "theta", noiseRatio == null ? "tau" : "tau_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"r", "F", "theta"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var r     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var F     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var theta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0, 5).ToList();

      var tau = new List<double>();

      data.Add(r);
      data.Add(F);
      data.Add(theta);
      data.Add(tau);

      for (var i = 0; i < r.Count; i++) {
        var res = r[i] * F[i] * Math.Sin(theta[i]);
        tau.Add(res);
      }

      if (noiseRatio != null) {
        var tau_noise   = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * tau.StandardDeviationPop();
        tau_noise.AddRange(tau.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(tau);
        data.Add(tau_noise);
      }

      return data;
    }
  }
}