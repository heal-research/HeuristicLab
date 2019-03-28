#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances {
  public abstract class ProblemInstanceProvider<TData> : IProblemInstanceProvider<TData> {
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract Uri WebLink { get; }
    public abstract string ReferencePublication { get; }

    public abstract IEnumerable<IDataDescriptor> GetDataDescriptors();

    public abstract TData LoadData(IDataDescriptor descriptor);

    public virtual bool CanImportData {
      get { return false; }
    }
    public virtual TData ImportData(string path) {
      throw new NotSupportedException();
    }

    public virtual bool CanExportData {
      get { return false; }
    }
    public virtual void ExportData(TData instance, string path) {
      throw new NotSupportedException();
    }
  }
}
