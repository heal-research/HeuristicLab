using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman44 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman44() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman44(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman44(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "I.41.16 h*omega**3/(pi**2 * c**2 * (exp(h*omega/(kb*T))-1)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "L_rad" : "L_rad_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "omega", "T", "h", "kb", "c", "L_rad" } : new[] { "omega", "T", "h", "kb", "c", "L_rad", "L_rad_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"omega", "T", "h", "kb", "c"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var omega = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var T     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var h     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var kb    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var c     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var L_rad = new List<double>();

      data.Add(omega);
      data.Add(T);
      data.Add(h);
      data.Add(kb);
      data.Add(c);
      data.Add(L_rad);

      for (var i = 0; i < omega.Count; i++) {
        var res = h[i] * Math.Pow(omega[i], 3) /
                  (Math.Pow(Math.PI, 2) * Math.Pow(c[i], 2) *
                   (Math.Exp(h[i] * omega[i] / (kb[i] * T[i])) - 1));
        L_rad.Add(res);
      }

      var targetNoise = GetNoisyTarget(L_rad, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}