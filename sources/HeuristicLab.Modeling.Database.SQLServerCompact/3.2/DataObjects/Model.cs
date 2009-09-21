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

    public Model(Variable targetVariable, Algorithm algorithm, ModelType modelType)
      : this() {
      this.targetVariableId = targetVariable.Id;
      this.algorithmId = algorithm.Id;
      ModelType = modelType.ToString();
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
    }

    IVariable IModel.TargetVariable {
      get { return this.TargetVariable; }
    }

    private string modelType;
    [Column(Storage = "modelType", CanBeNull = false)]
    public string ModelType {
      get { return this.modelType; }
      private set { this.modelType = value; }
    }

    ModelType IModel.ModelType {
      get {
        if (!Enum.IsDefined(typeof(ModelType), this.modelType))
          throw new ArgumentException("ModelType " + modelType + " not declared.");
        return (ModelType)Enum.Parse(typeof(ModelType), this.modelType);
      }
    }

    private string name;
    [Column(Storage = "name", CanBeNull = true)]
    public string Name {
      get { return this.name; }
      set { this.name = value; }
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
