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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using System.Drawing;
using System.IO;

namespace HeuristicLab.Problems.DataAnalysis.Regression {
  [Item("RegressionProblem", "Represents a regression problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public class RegressionProblem : ParameterizedNamedItem {
    private const string RegressionProblemDataParameterName = "RegressionProblemData";
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    #region Parameter Properties
    public ValueParameter<RegressionProblemData> RegressionProblemDataParameter {
      get { return (ValueParameter<RegressionProblemData>)Parameters[RegressionProblemDataParameterName]; }
    }
    #endregion
    #region properties
    public RegressionProblemData RegressionProblemData {
      get { return RegressionProblemDataParameter.Value; }
      set { RegressionProblemDataParameter.Value = value; }
    }
    #endregion

    public RegressionProblem()
      : base() {
      Parameters.Add(new ValueParameter<RegressionProblemData>(RegressionProblemDataParameterName, "The data set, target variable and input variables of the regression problem.", new RegressionProblemData()));
    }

    [StorableConstructor]
    private RegressionProblem(bool deserializing) : base() { }
  }
}
