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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Alba {
  [Item("AlbaTranslocationMoveHardTabuCriterion", "An operator which checks if translocation moves are tabu using a hard criterion.")]
  [StorableClass]
  public sealed class AlbaTranslocationMoveHardTabuCriterion : AlbaMoveOperator, IPermutationTranslocationMoveOperator, ITabuChecker {
    private TranslocationMoveHardTabuCriterion tabuChecker;
    protected override IPermutationMoveOperator PermutationMoveOperatorParameter {
      get { return tabuChecker; }
      set { tabuChecker = value as TranslocationMoveHardTabuCriterion; }
    }

    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return tabuChecker.TranslocationMoveParameter; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return tabuChecker.PermutationParameter; }
    }

    public ILookupParameter<BoolValue> MoveTabuParameter {
      get { return tabuChecker.MoveTabuParameter; }
    }

    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return tabuChecker.MoveQualityParameter; }
    }

    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return tabuChecker.MaximizationParameter; }
    }

    public AlbaTranslocationMoveHardTabuCriterion()
      : base() {
      tabuChecker = new TranslocationMoveHardTabuCriterion();
    }
  }
}
