using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Problems.BinPacking3D.Packer;

namespace HeuristicLab.Problems.BinPacking3D.Decoder {

  /// <summary>
  /// This class creates a solution
  /// </summary>
  [Item("Extreme-point Permutation Decoder (3d) Base", "Base class for 3d decoders")]
  [StorableClass]
  public class ExtremePointPermutationDecoder : Item, IDecoder<Permutation> {

    [StorableConstructor]
    protected ExtremePointPermutationDecoder(bool deserializing) : base(deserializing) { }
    protected ExtremePointPermutationDecoder(ExtremePointPermutationDecoderBase original, Cloner cloner)
      : base(original, cloner) {
    }

    /// <summary>
    /// Creates an extrem point based permutation decoder
    /// </summary>
    /// <param name="binPacker">Contains the packing algorithm for packing the items</param>
    public ExtremePointPermutationDecoder(BinPacker binPacker) : base() {
      _binPacker = binPacker;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      throw new NotImplementedException();
    }

    BinPacker _binPacker;

    /// <summary>
    /// Creates a solution for displaying it on the HEAL gui
    /// </summary>
    /// <param name="permutation">Permutation of items sorted by a sorting method. The value of each permutation index references to the index of the items list</param>
    /// <param name="binShape">Bin for storing the items</param>
    /// <param name="items">A list of packing items which should be assigned to a bin</param>
    /// <param name="useStackingConstraints"></param>
    /// <returns></returns>
    public Solution Decode(Permutation permutation, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      Solution solution = new Solution(binShape, useExtremePoints: true, stackingConstraints: useStackingConstraints);
      
      foreach (var packedBin in _binPacker.PackItems()) {
        solution.Bins.Add(packedBin);
      }
      return solution;
    }
  }
}

