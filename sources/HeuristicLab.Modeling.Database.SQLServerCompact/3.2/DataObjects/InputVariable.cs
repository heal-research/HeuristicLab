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
  [Table(Name = "InputVariable")]
  public class InputVariable : IInputVariable {
    public InputVariable() {
      this.model = default(EntityRef<Model>);
      this.variable = default(EntityRef<Variable>);
    }

    public InputVariable(Model model, Variable variable)
      : base() {
      this.modelId = model.Id;
      this.variableId = variable.Id;
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

    IVariable IInputVariable.Variable {
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

    IModel IInputVariable.Model {
      get { return this.Model;}
    }
  }
}
