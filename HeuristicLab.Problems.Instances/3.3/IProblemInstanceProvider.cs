#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;

namespace HeuristicLab.Problems.Instances {
  public interface IProblemInstanceProvider : IContent {
    string Name { get; }
    string Description { get; }
    Uri WebLink { get; }
    string ReferencePublication { get; }
  }

  public interface IProblemInstanceProvider<TData> : IProblemInstanceProvider {
    IEnumerable<IDataDescriptor> GetDataDescriptors();
    TData LoadData(IDataDescriptor descriptor);

    bool CanImportData { get; }
    TData ImportData(string path);

    bool CanExportData { get; }
    void ExportData(TData instance, string path);
  }
}
