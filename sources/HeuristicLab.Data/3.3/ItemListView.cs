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
using HeuristicLab.MainForm;

namespace HeuristicLab.Data {
  /// <summary>
  /// The visual representation of the class <see cref="ItemList"/>.
  /// </summary>
  [Content(typeof(ItemList<IItem>), true)]
  public class ItemListView : ItemListView<IItem> {
    /// <summary>
    /// Initializes a new instance of the class <see cref="ItemListView"/>.
    /// </summary>
    public ItemListView() { }
    /// <summary>
    /// Initializes a new instnace of the class <see cref="ItemListView"/> with the given 
    /// <paramref name="itemList"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="itemList"/> is not copied!</note>
    /// </summary>
    /// <param name="itemList">The list of items to represent.</param>
    public ItemListView(ItemList itemList)
      : this() {
      ItemList = itemList;
    }
  }
}
