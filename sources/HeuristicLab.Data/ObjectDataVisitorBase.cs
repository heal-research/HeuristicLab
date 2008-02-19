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

namespace HeuristicLab.Data {
  public class ObjectDataVisitorBase : IObjectDataVisitor{
    #region IObjectDataVisitor Members
    public virtual void Visit(BoolData data) {
    }

    public virtual void Visit(IntData data) {
    }

    public virtual void Visit(DoubleData data) {
    }

    public virtual void Visit(StringData data) {
    }

    public virtual void Visit(BoolArrayData data) {
    }

    public virtual void Visit(IntArrayData data) {
    }

    public virtual void Visit(DoubleArrayData data) {
    }

    public virtual void Visit(BoolMatrixData data) {
    }

    public virtual void Visit(IntMatrixData data) {
    }

    public virtual void Visit(DoubleMatrixData data) {
    }

    public virtual void Visit(ConstrainedIntData data) {
    }

    public virtual void Visit(ConstrainedDoubleData data) {
    }
    public void Visit(ConstrainedObjectData constrainedObjectData) {
      throw new NotImplementedException();
    }

    public void Visit(ObjectData objectData) {
      throw new NotImplementedException();
    }
    #endregion
  }
}
