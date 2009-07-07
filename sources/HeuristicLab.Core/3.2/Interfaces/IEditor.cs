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

namespace HeuristicLab.Core {
  /// <summary>
  /// Interface to represent an editor.
  /// </summary>
  public interface IEditor : IView {

    /// <summary>
    /// Gets or sets the filename.
    /// </summary>
    string Filename { get; set; }

    /// <summary>
    /// Saves the contained item to a file.
    /// </summary>
    /// <remarks>
    ///   The filename to save the contained item to is given by <see cref="Filename"/>.
    ///   Save is an asynchronous method. After saving the contained item is finished, the
    ///   <see cref="SaveFinished"/> event is fired.
    /// </remarks>
    void Save();

    /// <summary>
    /// Occurs when the filename was changed.
    /// </summary>
    event EventHandler FilenameChanged;
    /// <summary>
    /// Occurs after saving the contained object is finished.
    /// </summary>
    event EventHandler SaveFinished;
  }
}
