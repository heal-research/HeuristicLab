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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.SequentialEngine {
  /// <summary>
  /// Visual representation of a <see cref="SequentialEngine"/>.
  /// </summary>
  [Content(typeof(SequentialEngine), true)]
  public partial class SequentialEngineView : EngineBaseView {
    /// <summary>
    /// Gets or set the engine to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="EngineBaseEditor.Engine"/> of base class 
    /// <see cref="EngineBaseEditor"/>. No own data storage present.</remarks>
    public SequentialEngine SequentialEngine {
      get { return (SequentialEngine)Engine; }
      set { base.Engine = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SequentialEngineEditor"/>.
    /// </summary>
    public SequentialEngineView() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="SequentialEngineEditor"/> with the given
    /// <paramref name="sequentialEngine"/>.
    /// </summary>
    /// <param name="sequentialEngine">The engine to display.</param>
    public SequentialEngineView(SequentialEngine sequentialEngine)
      : this() {
      SequentialEngine = sequentialEngine;
    }
  }
}
