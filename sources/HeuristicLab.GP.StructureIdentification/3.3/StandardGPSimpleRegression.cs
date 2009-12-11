#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Selection;
using HeuristicLab.Logging;
using HeuristicLab.Data;
using HeuristicLab.GP.Operators;
using HeuristicLab.Modeling;
using System.Collections.Generic;
using System;
using System.Linq;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.GP.Algorithms;

namespace HeuristicLab.GP.StructureIdentification {
  public class StandardGPSimpleRegression : HeuristicLab.GP.StructureIdentification.StandardGPRegression, IStochasticAlgorithm {

    public override string Name { get { return "StandardGP - SimpleStructureIdentification"; } }

    public StandardGPSimpleRegression()
      : base() {
    }

    protected override IOperator CreateFunctionLibraryInjector() {
      return DefaultStructureIdentificationOperators.CreateSimpleFunctionLibraryInjector();
    }
  }
}
