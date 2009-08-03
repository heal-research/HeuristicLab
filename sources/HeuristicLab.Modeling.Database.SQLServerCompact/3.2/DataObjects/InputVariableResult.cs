using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;

namespace HeuristicLab.Modeling.Database.SQLServerCompact {
  [Table(Name="InputVariableResult")]
  public class InputVariableResult : IInputVariableResult {
    public InputVariableResult() {
      this.model = default(EntityRef<Model>);
      this.variable = default(EntityRef<Variable>);
      this.result = default(EntityRef<Result>);
    }

    public InputVariableResult(InputVariable inputVariable, Result result, double value)
      : this() {
      this.variableId = inputVariable.VariableId;
      this.modelId = inputVariable.ModelId;
      this.resultId = result.Id;
      this.value = value;
    }

    private int variableId;
    [Column(Storage = "variableId", IsPrimaryKey = true)]
    public int VariableId {
      get { return this.variableId; }
      private set {
        if (variableId != value) {
          if (variable.HasLoadedOrAssignedValue)
            throw new ForeignKeyReferenceAlreadyHasValueException();
          variableId = value;
        }
      }
    }

    private EntityRef<Variable> variable;
    [Association(Storage = "variable", ThisKey = "VariableId", OtherKey = "Id", IsForeignKey = true)]
    public Variable Variable {
      get { return variable.Entity; }
    }

    IVariable IInputVariableResult.Variable {
      get { return this.Variable; }
    }

    private int modelId;
    [Column(Storage = "modelId", IsPrimaryKey = true)]
    public int ModelId {
      get { return this.modelId; }
      private set {
        if (modelId != value) {
          if (model.HasLoadedOrAssignedValue)
            throw new ForeignKeyReferenceAlreadyHasValueException();
          modelId = value;
        }
      }
    }

    private EntityRef<Model> model;
    [Association(Storage = "model", ThisKey = "ModelId", OtherKey = "Id", IsForeignKey = true)]
    public Model Model {
      get { return model.Entity; }
    }

    IModel IInputVariableResult.Model {
      get { return this.Model; }
    }

    private int resultId;
    [Column(Storage = "resultId", IsPrimaryKey = true)]
    public int ResultId {
      get { return this.resultId; }
      private set {
        if (resultId != value) {
          if (result.HasLoadedOrAssignedValue)
            throw new ForeignKeyReferenceAlreadyHasValueException();
          resultId = value;
        }
      }
    }

    private EntityRef<Result> result;
    [Association(Storage = "result", ThisKey = "ResultId", OtherKey = "Id", IsForeignKey = true)]
    public Result Result {
      get { return result.Entity; }
    }

    IResult IInputVariableResult.Result {
      get { return this.Result; }
    }

    private double value;
    [Column(Storage = "value", CanBeNull = false)]
    public double Value {
      get { return this.value; }
      set { this.value = value; }
    }
  }
}
