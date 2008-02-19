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
  public interface IObjectDataVisitor {
    void Visit(BoolData data);
    void Visit(IntData data);
    void Visit(DoubleData data);
    void Visit(StringData data);

    void Visit(BoolArrayData data);
    void Visit(IntArrayData data);
    void Visit(DoubleArrayData data);

    void Visit(BoolMatrixData data);
    void Visit(IntMatrixData data);
    void Visit(DoubleMatrixData data);

    void Visit(ConstrainedIntData data);
    void Visit(ConstrainedDoubleData data);

    void Visit(ConstrainedObjectData constrainedObjectData);
    void Visit(ObjectData objectData);
  }
}
