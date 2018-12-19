#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.IO;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("FileValue", "Represents the path to a file.")]
  [StorableClass]
  public class FileValue : PathValue {
    [Storable]
    private string fileDialogFilter;
    public string FileDialogFilter {
      get { return fileDialogFilter; }
      set {
        if (fileDialogFilter != value) {
          fileDialogFilter = value;
        }
      }
    }

    [Storable]
    public bool SaveFile { get; set; }

    [StorableConstructor]
    protected FileValue(bool deserializing) : base(deserializing) { }
    protected FileValue(FileValue original, Cloner cloner)
      : base(original, cloner) {
      fileDialogFilter = original.FileDialogFilter;
      SaveFile = original.SaveFile;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new FileValue(this, cloner);
    }

    public FileValue()
      : base() {
      fileDialogFilter = @"All files|*.*";
    }

    public override bool Exists() {
      if (string.IsNullOrEmpty(Value)) return false;
      if (string.IsNullOrEmpty(Path.GetDirectoryName(Value))) return false;
      return File.Exists(Value);
    }
  }
}
