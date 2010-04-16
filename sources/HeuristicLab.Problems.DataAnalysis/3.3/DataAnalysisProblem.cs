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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using HeuristicLab.Data;
using HeuristicLab.Problems.DataAnalysis;
using System.Drawing;
using System.IO;

namespace HeuristicLab.Problems.DataAnalysis {
  [Item("DataAnalysisProblem", "Represents a data analysis problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public class DataAnalysisProblem : ParameterizedNamedItem {
    private const string DataAnalysisProblemDataParameterName = "DataAnalysisProblemData";
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    #region Parameter Properties
    public ValueParameter<DataAnalysisProblemData> DataAnalysisProblemDataParameter {
      get { return (ValueParameter<DataAnalysisProblemData>)Parameters[DataAnalysisProblemDataParameterName]; }
    }
    #endregion
    #region properties
    public DataAnalysisProblemData DataAnalysisProblemData {
      get { return DataAnalysisProblemDataParameter.Value; }
      set { DataAnalysisProblemDataParameter.Value = value; }
    }
    #endregion

    public DataAnalysisProblem()
      : base() {
      Parameters.Add(new ValueParameter<DataAnalysisProblemData>(DataAnalysisProblemDataParameterName, "The data set, target variable and input variables of the data analysis problem.", new DataAnalysisProblemData()));
    }

    [StorableConstructor]
    private DataAnalysisProblem(bool deserializing) : base() { }
  }
}
