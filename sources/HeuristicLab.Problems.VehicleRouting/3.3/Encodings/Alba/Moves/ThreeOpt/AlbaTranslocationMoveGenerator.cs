#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaTranslocationMoveGenerator", "An operator which generates translocation moves for the Alba representation.")]
  [StorableClass]
  public sealed class AlbaTranslocationMoveGenerator : PermutationMoveOperator, IAlbaTranslocationMoveOperator, IMultiMoveGenerator {
    public IValueLookupParameter<TranslocationMoveGenerator> TranslocationMoveGeneratorParameter {
      get { return (IValueLookupParameter<TranslocationMoveGenerator>)Parameters["TranslocationMoveGenerator"]; }
    }

    protected override IPermutationMoveOperator PermutationMoveOperatorParameter {
      get { return TranslocationMoveGeneratorParameter.Value; }
      set {
        TranslocationMoveGeneratorParameter.Value = value as TranslocationMoveGenerator;
        if (TranslocationMoveGeneratorParameter.Value is IMultiMoveGenerator) {
          ((IMultiMoveGenerator)TranslocationMoveGeneratorParameter.Value).SampleSizeParameter.ActualName = SampleSizeParameter.Name;
        }
      }
    }

    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get {
        if (TranslocationMoveGeneratorParameter.Value != null)
          return TranslocationMoveGeneratorParameter.Value.TranslocationMoveParameter;
        else
          return null;
      }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get {
        if (TranslocationMoveGeneratorParameter.Value != null)
          return TranslocationMoveGeneratorParameter.Value.PermutationParameter;
        else
          return null;
      }
    }

    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    [StorableConstructor]
    private AlbaTranslocationMoveGenerator(bool deserializing) : base(deserializing) { }

    public AlbaTranslocationMoveGenerator()
      : base() {
      Parameters.Add(new ValueLookupParameter<TranslocationMoveGenerator>("TranslocationMoveGenerator", "The move generator.",
        new StochasticTranslocationMultiMoveGenerator()));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));

      ((IMultiMoveGenerator)TranslocationMoveGeneratorParameter.Value).SampleSizeParameter.ActualName = SampleSizeParameter.Name;
    }

    public override IOperation Apply() {
      IOperation successor = base.Apply();

      Permutation permutation = VRPToursParameter.ActualValue as Permutation;
      string moveName = TranslocationMoveGeneratorParameter.ActualValue.TranslocationMoveParameter.Name;

      return successor;
    }
  }
}
