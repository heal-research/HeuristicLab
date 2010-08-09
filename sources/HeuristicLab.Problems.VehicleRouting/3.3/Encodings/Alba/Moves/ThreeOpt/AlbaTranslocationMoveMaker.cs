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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaTranslocationMoveMaker", "An operator which makes translocation moves for the Alba representation.")]
  [StorableClass]
  public sealed class AlbaTranslocationMoveMaker : AlbaMoveMaker, IAlbaTranslocationMoveOperator, IVRPMoveMaker {
    private TranslocationMoveMaker moveMaker;
    protected override IPermutationMoveOperator PermutationMoveOperatorParameter {
      get { return moveMaker; }
      set { moveMaker = value as TranslocationMoveMaker; }
    }

    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return moveMaker.TranslocationMoveParameter; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return moveMaker.QualityParameter; }
    }

    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return moveMaker.MoveQualityParameter; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return moveMaker.PermutationParameter; }
    }

    public AlbaTranslocationMoveMaker()
      : base() {
      moveMaker = new TranslocationMoveMaker();
    }
  }
}
