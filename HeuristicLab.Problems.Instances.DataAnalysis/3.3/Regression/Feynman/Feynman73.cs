using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman73 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman73() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman73(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman73(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.27.16 epsilon*c*Ef**2 | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "flux" : "flux_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "epsilon", "c", "Ef", "flux" } : new[] { "epsilon", "c", "Ef", "flux", "flux_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"epsilon", "c", "Ef"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var epsilon = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var c       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var Ef      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var flux = new List<double>();

      data.Add(epsilon);
      data.Add(c);
      data.Add(Ef);
      data.Add(flux);

      for (var i = 0; i < epsilon.Count; i++) {
        var res = epsilon[i] * c[i] * Math.Pow(Ef[i], 2);
        flux.Add(res);
      }

      var targetNoise = GetNoisyTarget(flux, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}