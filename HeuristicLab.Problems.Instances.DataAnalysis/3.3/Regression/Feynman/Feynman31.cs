using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman31 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman31() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman31(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman31(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.30.5 arcsin(lambd/(n*d)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "theta" : "theta_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"lambd", "d", "n", noiseRatio == null ? "theta" : "theta_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"lambd", "d", "n"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var lambd = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var d     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 2, 5).ToList();
      var n     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var theta = new List<double>();

      data.Add(lambd);
      data.Add(d);
      data.Add(n);
      data.Add(theta);

      for (var i = 0; i < lambd.Count; i++) {
        var res = Math.Asin(lambd[i] / (n[i] * d[i]));
        theta.Add(res);
      }

      if (noiseRatio != null) {
        var theta_noise = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * theta.StandardDeviationPop();
        theta_noise.AddRange(theta.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(theta);
        data.Add(theta_noise);
      }

      return data;
    }
  }
}