namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public struct BatchInstruction {
    public byte opcode;
    public ushort narg;
    public int childIndex;

    public double value; // for constants
    public double weight; // for variables
    public double[] buf;
    public double[] data; // to hold dataset data
  }
}
