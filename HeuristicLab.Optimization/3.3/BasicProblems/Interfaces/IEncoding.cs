#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  [StorableType("d70b2675-246c-489c-a91b-b2e19a1616a3")]
  public interface IEncoding : IParameterizedNamedItem {
    IValueParameter SolutionCreatorParameter { get; }
    ISolutionCreator SolutionCreator { get; }

    IEnumerable<IOperator> Operators { get; set; }

    void ConfigureOperator(IItem @operator);
    void ConfigureOperators(IEnumerable<IItem> operators);

    event EventHandler OperatorsChanged;
    event EventHandler SolutionCreatorChanged;
  }

  [StorableType("DB23907F-BE6E-44E4-9596-3D3BF1532631")]
  public interface IEncoding<TEncodedSolution> : IEncoding
      where TEncodedSolution : class, IEncodedSolution {
    //new ISolutionCreator<TEncodedSolution> SolutionCreator { get; }
  }
}
