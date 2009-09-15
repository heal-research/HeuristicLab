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

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.Sql;
using System.Text;

namespace HeuristicLab.Modeling.Database.SQLServerCompact {
  public class ModelingDataContext : DataContext{
    public ModelingDataContext(string connection)
      : base(connection) {
    }

    #region Tables
    public Table<Algorithm> Algorithms {
      get { return GetTable<Algorithm>(); }
    }

    public Table<Variable> Variables {
      get { return GetTable<Variable>(); }
    }

    public Table<Problem> Problems {
      get { return GetTable<Problem>(); }
    }

    public Table<Result> Results {
      get { return GetTable<Result>(); }
    }

    public Table<Model> Models {
      get { return GetTable<Model>(); }
    }

    public Table<ModelData> ModelData {
      get { return GetTable<ModelData>(); }
    }

    public Table<InputVariableResult> InputVariableResults {
      get { return GetTable<InputVariableResult>(); }
    }

    public Table<ModelResult> ModelResults {
      get { return GetTable<ModelResult>(); }
    }

    public Table<ModelMetaData> ModelMetaData {
      get { return GetTable<ModelMetaData>(); }
    }

    public Table<MetaData> MetaData {
      get { return GetTable<MetaData>(); }
    }

    public Table<InputVariable> InputVariables {
      get { return GetTable<InputVariable>(); }
    }
    #endregion
  }
}
