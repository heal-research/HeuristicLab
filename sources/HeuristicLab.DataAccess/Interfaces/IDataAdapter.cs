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
using System.Text;

namespace HeuristicLab.DataAccess.Interfaces {
  public interface IDataAdapter<ObjT>
    where ObjT: IPersistableObject
  {
    /// <summary>
    /// Save or update the object
    /// </summary>
    /// <param name="user"></param>
    void Update(ObjT obj);

    /// <summary>
    /// Get the object with the specified ID
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    ObjT GetById(Guid id);

    /// <summary>
    /// Get all objects
    /// </summary>
    /// <returns></returns>
    ICollection<ObjT> GetAll();

    /// <summary>
    /// Deletes the object
    /// </summary>
    /// <param name="user"></param>
    bool Delete(ObjT obj);

    /// <summary>
    /// sets the session
    /// </summary>
    ISession Session { set; }
  }
}
