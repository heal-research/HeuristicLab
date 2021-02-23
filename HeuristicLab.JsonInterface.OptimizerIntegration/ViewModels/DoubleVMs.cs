namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class DoubleRangeVM : RangeVM<double, DoubleRangeJsonItem> {
    protected override double MinTypeValue => double.MinValue;
    protected override double MaxTypeValue => double.MaxValue;
  }

  public class DoubleArrayValueVM : ArrayValueVM<double, DoubleArrayJsonItem> {
    protected override double MinTypeValue => double.MinValue;
    protected override double MaxTypeValue => double.MaxValue;
  }

  public class DoubleMatrixValueVM : MatrixValueVM<double, DoubleMatrixJsonItem> {
    protected override double MinTypeValue => double.MinValue;
    protected override double MaxTypeValue => double.MaxValue;
  }

  public class DoubleValueVM : SingleValueVM<double, DoubleJsonItem> {
    protected override double MinTypeValue => double.MinValue;
    protected override double MaxTypeValue => double.MaxValue;
  }
}
