using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("CopyCrossover", "This operator creates an offspring by creating a clone of a randomly chosen parent. It can be used in situations where no crossover should occur after selection.")]
  [StorableType("4F639A12-9D80-457C-B098-19970BC37560")]
  public class CopyCrossover : RealVectorCrossover {
    [StorableConstructor]
    protected CopyCrossover(StorableConstructorFlag _) : base(_) { }
    protected CopyCrossover(CopyCrossover original, Cloner cloner) : base(original, cloner) { }
    public CopyCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CopyCrossover(this, cloner);
    }

    /// <summary>
    /// Performs the copy crossover.
    /// </summary>
    /// <remarks>
    /// There can be more than two parents.
    /// </remarks>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The list of parents.</param>
    /// <returns>The child vector, a copy of one of the parents.</returns>
    public static RealVector Apply(IRandom random, ItemArray<RealVector> parents) {
      int index = random.Next(parents.Length);
      var selectedParent = parents[index];
      var result = (RealVector)selectedParent.Clone();

      return result;
    }

    /// <summary>
    /// Forwards the call to <see cref="Apply(IRandom, ItemArray<RealVector>)"/>.
    /// </summary>
    /// <param name="random">The random number generator.</param>
    /// <param name="parents">The list of parents.</param>
    /// <returns>The child vector, a copy of one of the parents.</returns>
    protected override RealVector Cross(IRandom random, ItemArray<RealVector> parents) {
      return Apply(random, parents);
    }
  }
}

