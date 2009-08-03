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

    public Table<InputVariable> InputVariables {
      get { return GetTable<InputVariable>(); }
    }
    #endregion
  }
}
