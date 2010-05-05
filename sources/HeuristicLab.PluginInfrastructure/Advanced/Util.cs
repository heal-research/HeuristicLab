#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.PluginInfrastructure.Manager;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using System.ServiceModel;
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal static class Util {
    internal static void ResizeColumn(ColumnHeader columnHeader) {
      columnHeader.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
      int headerSize = columnHeader.Width;
      columnHeader.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      int contentSize = columnHeader.Width;
      columnHeader.Width = Math.Max(headerSize, contentSize);      
    }

    internal static void ResizeColumns(IEnumerable<ColumnHeader> columnHeaders) {
      foreach (var columnHeader in columnHeaders)
        ResizeColumn(columnHeader);
    }
  }
}
