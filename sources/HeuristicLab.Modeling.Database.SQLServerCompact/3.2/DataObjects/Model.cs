using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;

namespace HeuristicLab.Modeling.Database.SQLServerCompact {
  [Table(Name = "Model")]
  public class Model : IModel {
    public Model() {
      targetVariable = default(EntityRef<Variable>);
      algorithm = default(EntityRef<Algorithm>);
    }

    public Model(Variable targetVariable, Algorithm algorithm)
      : this() {
      this.targetVariableId = targetVariable.Id;
      this.algorithmId = algorithm.Id;
    }

    private int id;
    [Column(Storage = "id", IsPrimaryKey = true, IsDbGenerated = true)]
    public int Id {
      get { return this.id; }
      private set { this.id = value; }
    }

    private int algorithmId;
    [Column(Storage = "algorithmId", CanBeNull = false)]
    public int AlgorithmId {
      get { return this.algorithmId; }
      private set {
        if (algorithmId != value) {
          if (algorithm.HasLoadedOrAssignedValue)
            throw new ForeignKeyReferenceAlreadyHasValueException();
          algorithmId = value;
        }
      }
    }

    private EntityRef<Algorithm> algorithm;
    [Association(Storage = "algorithm", ThisKey = "AlgorithmId", OtherKey = "Id", IsForeignKey = true)]
    public Algorithm Algorithm {
      get { return this.algorithm.Entity; }
      //private set {
      //  Algorithm previousValue = algorithm.Entity;
      //  if (previousValue != value || (!algorithm.HasLoadedOrAssignedValue)) {
      //    if (previousValue != null) {
      //      algorithm.Entity = null;
      //    }
      //    algorithm.Entity = value;
      //    if (value != null) {
      //      algorithmId = value.Id;
      //    }
      //  }
      //}
    }

    IAlgorithm IModel.Algorithm {
      get { return this.Algorithm; }
    }

    private int targetVariableId;
    [Column(Storage = "targetVariableId", CanBeNull = false)]
    public int TargetVariableId {
      get { return this.targetVariableId; }
      private set {
        if (targetVariableId != value) {
          if (targetVariable.HasLoadedOrAssignedValue)
            throw new ForeignKeyReferenceAlreadyHasValueException();
          targetVariableId = value;
        }
      }
    }

    private EntityRef<Variable> targetVariable;
    [Association(Storage = "targetVariable", ThisKey = "TargetVariableId", OtherKey = "Id", IsForeignKey = true)]
    public Variable TargetVariable {
      get { return this.targetVariable.Entity; }
      //private set {
      //  Variable previousValue = targetVariable.Entity;
      //  if (previousValue != value || (!targetVariable.HasLoadedOrAssignedValue)) {
      //    if (previousValue != null) {
      //      targetVariable.Entity = null;
      //    }
      //    targetVariable.Entity = value;
      //    if (value != null) {
      //      targetVariableId = value.Id;
      //    }
      //  }
      //}
    }

    IVariable IModel.TargetVariable {
      get { return this.TargetVariable; }
    }

    private int trainingSamplesStart;
    [Column(Storage = "trainingSamplesStart", CanBeNull = false)]
    public int TrainingSamplesStart {
      get { return this.trainingSamplesStart; }
      set { this.trainingSamplesStart = value; }
    }

    private int trainingSamplesEnd;
    [Column(Storage = "trainingSamplesEnd", CanBeNull = false)]
    public int TrainingSamplesEnd {
      get { return this.trainingSamplesEnd; }
      set { this.trainingSamplesEnd = value; }
    }

    private int validationSamplesStart;
    [Column(Storage = "validationSamplesStart", CanBeNull = false)]
    public int ValidationSamplesStart {
      get { return this.validationSamplesStart; }
      set { this.validationSamplesStart = value; }
    }

    private int validationSamplesEnd;
    [Column(Storage = "validationSamplesEnd", CanBeNull = false)]
    public int ValidationSamplesEnd {
      get { return this.validationSamplesEnd; }
      set { this.validationSamplesEnd = value; }
    }

    private int testSamplesStart;
    [Column(Storage = "testSamplesStart", CanBeNull = false)]
    public int TestSamplesStart {
      get { return this.testSamplesStart; }
      set { this.testSamplesStart = value; }
    }

    private int testSamplesEnd;
    [Column(Storage = "testSamplesEnd", CanBeNull = false)]
    public int TestSamplesEnd {
      get { return this.testSamplesEnd; }
      set { this.testSamplesEnd = value; }
    }
  }
}
