using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.Modeling.Database.SQLServerCompact {
  public class PredictorPersister : OperatorBase {

    public PredictorPersister()
      : base() {
      AddVariableInfo(new VariableInfo("DatabaseFile", "Database file", typeof(StringData), VariableKind.In));      
      AddVariableInfo(new VariableInfo("ModelType", "The ModelType", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "The input dataset", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("AlgorithmName", "", typeof(StringData), VariableKind.In));
    }

    public override string Description {
      get { return "Saves a predictor to a given database"; }
    }

    public override IOperation Apply(IScope scope) {
      string database = GetVariableValue<StringData>("DatabaseFile", scope, true).Data;    
      Dataset ds = GetVariableValue<Dataset>("Dataset", scope, true);
      string mt = GetVariableValue<StringData>("ModelType",scope,true).Data;
      string algorithm = GetVariableValue<StringData>("AlgorithmName",scope,true).Data;

      ModelType modelType = ModelType.Regression;
      if (Enum.IsDefined(typeof(ModelType), mt))
        modelType = (ModelType)Enum.Parse(typeof(ModelType), mt);
      else
        throw new ArgumentException("Passed model type " + mt + " is not defined.\n Possible Values are: " + 
          string.Join(",",Enum.GetNames(typeof(ModelType))));

      DatabaseService db = new DatabaseService(database);
      db.Connect();
      Dataset temp = db.GetDataset();
      if (temp == null) {
        db.PersistProblem(ds);
        db.Commit();
      }

      IAnalyzerModel model = new AnalyzerModel();                 
      DefaultModelAnalyzerOperators.PopulateAnalyzerModel(scope, model, modelType);
      db.Persist(model, algorithm, algorithm);
      db.Commit();
      db.Disconnect();
      

      return null;
    }
  }
}
