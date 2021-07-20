using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus13 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus13() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus13(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus13(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Goldstein 12.80: 1/(2*m)*(p**2+m**2*omega**2*x**2*(1+alpha*x/y)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "E_n" : "E_n_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "m", "omega", "p", "y", "x", "alpha", "E_n" } : new[] { "m", "omega", "p", "y", "x", "alpha", "E_n", "E_n_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"m", "omega", "p", "y", "x", "alpha"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var m     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var omega = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var p     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var y     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var x     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var alpha = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var E_n = new List<double>();

      data.Add(m);
      data.Add(omega);
      data.Add(p);
      data.Add(y);
      data.Add(x);
      data.Add(alpha);
      data.Add(E_n);

      for (var i = 0; i < m.Count; i++) {
        var res = 1.0 / (2 * m[i]) * (Math.Pow(p[i], 2) +
                                      Math.Pow(m[i], 2) * Math.Pow(omega[i], 2) * Math.Pow(x[i], 2) *
                                      (1 + alpha[i] * x[i] / y[i]));
        E_n.Add(res);
      }

      var targetNoise = GetNoisyTarget(E_n, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}