#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
using HeuristicLab.Parameters;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.BinPacking3D.Encoding {

  /// <summary>
  /// This class creates a solution
  /// </summary>
  [Item("Extreme-point Permutation Decoder (3d) Base", "Base class for 3d decoders")]
  [StorableClass]
  public class ExtremePointPermutationDecoder : ParameterizedNamedItem, IDecoder<Permutation> {

    [Storable]
    private IValueParameter<EnumValue<FittingMethod>> fittingMethodParameter;
    public IValueParameter<EnumValue<FittingMethod>> FittingMethodParameter {
      get { return fittingMethodParameter; }
    }

    public FittingMethod FittingMethod {
      get { return fittingMethodParameter.Value.Value; }
      set { fittingMethodParameter.Value.Value = value; }
    }

    [Storable]
    private readonly IValueParameter<EnumValue<ExtremePointCreationMethod>> extremePointCreationMethodParameter;
    public IValueParameter<EnumValue<ExtremePointCreationMethod>> ExtremePointCreationMethodParameter {
      get { return extremePointCreationMethodParameter; }
    }

    public ExtremePointCreationMethod ExtremePointCreationMethod {
      get { return extremePointCreationMethodParameter.Value.Value; }
      set { extremePointCreationMethodParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected ExtremePointPermutationDecoder(bool deserializing) : base(deserializing) { }
    protected ExtremePointPermutationDecoder(ExtremePointPermutationDecoder original, Cloner cloner)
      : base(original, cloner) {
      fittingMethodParameter = cloner.Clone(original.fittingMethodParameter);
      //_binPacker = cloner.Clone(original._binPacker);
    }
    public ExtremePointPermutationDecoder() {
      Parameters.Add(fittingMethodParameter = 
            new ValueParameter<EnumValue<FittingMethod>>("Fitting Method", 
                                                         "The fitting method that the decoder uses.", 
                                                         new EnumValue<FittingMethod>(FittingMethod.FirstFit)));

      Parameters.Add(extremePointCreationMethodParameter =
            new ValueParameter<EnumValue<ExtremePointCreationMethod>>("Extreme Point Generation Method",
                                                         "The Extreme point generation method that the decoder uses.",
                                                         new EnumValue<ExtremePointCreationMethod>(ExtremePointCreationMethod.PointProjection)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExtremePointPermutationDecoder(this, cloner);
    }

    /*[Storable]
    BinPacker _binPacker;
    */
    /// <summary>
    /// Creates a solution for displaying it on the HEAL gui
    /// </summary>
    /// <param name="permutation">Permutation of items sorted by a sorting method. The value of each permutation index references to the index of the items list</param>
    /// <param name="binShape">Bin for storing the items</param>
    /// <param name="items">A list of packing items which should be assigned to a bin</param>
    /// <param name="useStackingConstraints"></param>
    /// <returns></returns>
    public Solution Decode(Permutation permutation, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints) {
      var binPacker = BinPackerFactory.CreateBinPacker(FittingMethod);
      var pruningMethod = ExtremePointPruningMethod.None;
      Solution solution = new Solution(binShape, useExtremePoints: true, stackingConstraints: useStackingConstraints);
      foreach (var packedBin in binPacker.PackItems(permutation, binShape, items, ExtremePointCreationMethod, pruningMethod, useStackingConstraints)) {
        solution.Bins.Add(packedBin);
      }
      return solution;
    }
  }
}

