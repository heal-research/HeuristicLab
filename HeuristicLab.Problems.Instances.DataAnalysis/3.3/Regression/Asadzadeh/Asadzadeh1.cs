using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public sealed class Asadzadeh1 : ArtificialRegressionDataDescriptor {
    public override string Name => "Asadzadeh1";

    public override string Description => "Asadzadeh1";

    protected override string TargetVariable => "F";
    protected override string[] VariableNames => new string[] { "h", "sigma_y", "t", "Rt", "wR", "F" };
    protected override string[] AllowedInputVariables => new string[] { "h", "sigma_y", "t", "Rt", "wR" };

    protected override int TrainingPartitionStart => 0;
    protected override int TrainingPartitionEnd => 500;
    protected override int TestPartitionStart => 500;
    protected override int TestPartitionEnd => 1000;

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint)DateTime.Now.Ticks);

      List<double> h = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.1, 5).ToList();
      List<double> sigmaY = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 250, 1000).ToList();
      List<double> t = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.5, 5).ToList();
      List<double> Rt = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 100).ToList();
      List<double> wR = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0.5, 1.5).ToList();
      List<double> F = new List<double>();

      for (int i = TrainingPartitionStart; i < TestPartitionEnd; ++i) {
        F.Add(Calc(h[i], sigmaY[i], t[i], Rt[i], wR[i]));
      }

      return new List<List<double>> { h, sigmaY, t, Rt, wR, F };
    }

    private static double Calc(double h, double sigmaY, double t, double Rt, double wR) {
      var E = 210000;
      var R = Rt * t;
      var w = wR * R;
      var e = E / (E + h);
      var yieldStrength = (sigmaY * (t * t)) / w;
      var plasticHardening = (h * t * t * t) / (3 * R * w);
      var elasticity = (4 * sigmaY * sigmaY * sigmaY * R * R) / (3 * w * E * E);
      var C = 1 + 4 * (t / w);
      return e * (yieldStrength + plasticHardening - elasticity) * C;
    }
  }
}
