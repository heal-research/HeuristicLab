using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman56 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman56() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman56(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman56(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.6.15a 3/(4*pi*epsilon)*p_d*z/r**5*sqrt(x**2+y**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Ef" : "Ef_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "epsilon", "p_d", "r", "x", "y", "z", "Ef" } : new[] { "epsilon", "p_d", "r", "x", "y", "z", "Ef", "Ef_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"epsilon", "p_d", "r", "x", "y", "z"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var epsilon = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var p_d     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var r       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var x       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var y       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var z       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var Ef = new List<double>();

      data.Add(epsilon);
      data.Add(p_d);
      data.Add(r);
      data.Add(x);
      data.Add(y);
      data.Add(z);
      data.Add(Ef);

      for (var i = 0; i < epsilon.Count; i++) {
        var res = 3.0 / (4 * Math.PI * epsilon[i]) * p_d[i] * z[i] / Math.Pow(r[i], 5) *
                  Math.Sqrt(Math.Pow(x[i], 2) + Math.Pow(y[i], 2));
        Ef.Add(res);
      }

      var targetNoise = GetNoisyTarget(Ef, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}