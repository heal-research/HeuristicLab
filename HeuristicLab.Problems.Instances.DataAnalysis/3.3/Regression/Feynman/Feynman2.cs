using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman2 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman2() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman2(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman2(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.6.20 exp(-(theta/sigma)**2/2)/(sqrt(2*pi)*sigma) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "f" : "f_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"sigma", "theta", noiseRatio == null ? "f" : "f_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"sigma", "theta"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var sigma = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var theta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var f = new List<double>();

      data.Add(sigma);
      data.Add(theta);
      data.Add(f);

      for (var i = 0; i < sigma.Count; i++) {
        var res = Math.Exp(-Math.Pow(theta[i] / sigma[i], 2) / 2) / (Math.Sqrt(2 * Math.PI) * sigma[i]);
        f.Add(res);
      }

      if (noiseRatio != null) {
        var f_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * f.StandardDeviationPop();
        f_noise.AddRange(f.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(f);
        data.Add(f_noise);
      }

      return data;
    }
  }
}