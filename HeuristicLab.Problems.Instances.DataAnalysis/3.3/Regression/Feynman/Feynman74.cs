using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman74 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman74() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman74(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman74(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.27.18 epsilon*Ef**2 | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "E_den" : "E_den_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "epsilon", "Ef", "E_den" } : new[] { "epsilon", "Ef", "E_den", "E_den_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"epsilon", "Ef"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var epsilon = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var Ef      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var E_den = new List<double>();

      data.Add(epsilon);
      data.Add(Ef);
      data.Add(E_den);

      for (var i = 0; i < epsilon.Count; i++) {
        var res = epsilon[i] * Math.Pow(Ef[i], 2);
        E_den.Add(res);
      }

      var targetNoise = GetNoisyTarget(E_den, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}