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
using System.Text;

namespace HeuristicLab.Modeling.Database.SQLServerCompact {
  [Table(Name = "ModelResult")]
  public class ModelResult : IModelResult {
    public ModelResult() {
      this.model = default(EntityRef<Model>);
      this.result = default(EntityRef<Result>);
    }

    public ModelResult(Model model, Result result, double value)
      : this() {
      this.modelId = model.Id;
      this.resultId = result.Id;
      this.value = value;
    }

    private double value;
    [Column(Storage = "value", CanBeNull = false)]
    public double Value {
      get { return this.value; }
      set { this.value = value; }
    }

    private int modelId;
    [Column(Storage = "modelId",IsPrimaryKey = true, CanBeNull = false)]
    public int ModelId {
      get { return this.modelId; }
      private set {
        if (modelId != value) {
          if (model.HasLoadedOrAssignedValue)
            throw new ForeignKeyReferenceAlreadyHasValueException();
        }
        modelId = value;
      }
    }

    private EntityRef<Model> model;
    [Association(Storage = "model", ThisKey = "ModelId", OtherKey = "Id", IsForeignKey = true)]
    public Model Model {
      get { return this.model.Entity; }
      //set {
      //  Model previousValue = model.Entity;
      //  if (previousValue != value || (!model.HasLoadedOrAssignedValue)) {
      //    if (previousValue != null) {
      //      model.Entity = null;
      //    }
      //    model.Entity = value;
      //    if (value != null) {
      //      modelId = value.Id;
      //    } 
      //  }
      //}
    }

    IModel IModelResult.Model {
      get { return this.Model; }
    }

    private int resultId;
    [Column(Storage = "resultId",IsPrimaryKey = true, CanBeNull = false)]
    public int ResultId {
      get { return this.resultId; }
      private set {
        if (resultId != value) {
          if (result.HasLoadedOrAssignedValue)
            throw new ForeignKeyReferenceAlreadyHasValueException();
        }
        modelId = value;
      }
    }

    private EntityRef<Result> result;
    [Association(Storage = "result", ThisKey = "ResultId", OtherKey = "Id", IsForeignKey = true)]
    public Result Result {
      get { return this.result.Entity; }
      //set {
      //  Result previousValue = result.Entity;
      //  if (previousValue != value || (!model.HasLoadedOrAssignedValue)) {
      //    if (previousValue != null) {
      //      result.Entity = null;
      //    }
      //    result.Entity = value;
      //    if (value != null) {
      //      resultId = value.Id;
      //    }
      //  }
      //}
    }

    IResult IModelResult.Result {
      get { return this.Result; }
    }

  }
}
