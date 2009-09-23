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
  [Table(Name = "ModelData")]
  public class ModelData {

    public ModelData() {
      this.model = default(EntityRef<Model>);
    }

    public ModelData(Model model, byte[] data)
      : base() {
      this.modelId = model.Id;
      this.data = data;
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

    private byte[] data;
    [Column(Storage = "data", DbType = "image", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
    public byte[] Data {
      get { return this.data; }
      private set { this.data = value; }
    }
  }
}
