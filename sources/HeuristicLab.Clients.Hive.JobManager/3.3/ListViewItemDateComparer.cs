#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Windows.Forms;

namespace HeuristicLab.Clients.Hive.JobManager {
  /// <summary>
  /// Comparer for sorting items in a list view by date
  /// See: http://msdn.microsoft.com/en-us/library/ms996467.aspx
  /// </summary>
  public class ListViewItemDateComparer : IComparer {
    private int col;
    public SortOrder Order { get; set; }

    public ListViewItemDateComparer() {
      col = 0;
      Order = SortOrder.Ascending;
    }

    public ListViewItemDateComparer(int column, SortOrder order) {
      col = column;
      Order = order;
    }

    public int Compare(object x, object y) {
      int returnVal;
      bool result;
      DateTime firstDate, secondDate;
      ListViewItem listViewItemX, listViewItemY;
      listViewItemX = x as ListViewItem;
      listViewItemY = y as ListViewItem;

      if (listViewItemX == null || listViewItemY == null) {
        throw new ArgumentException(string.Format("The ListViewItemDateComparer expects ListViewItems but received {0} and {1}.",
          x.GetType().ToString(), y.GetType().ToString()));
      }

      result = DateTime.TryParse(listViewItemX.SubItems[col].Text, out firstDate);
      result = DateTime.TryParse(listViewItemY.SubItems[col].Text, out secondDate) && result;

      if (result) {
        returnVal = DateTime.Compare(firstDate, secondDate);
      } else {
        throw new ArgumentException(string.Format("The ListViewItemDateComparer expects DateTimes. Can't parse {0} and {1}.",
           listViewItemX.SubItems[col].Text, listViewItemY.SubItems[col].Text));
      }

      if (Order == SortOrder.Descending) {
        // invert the value returned by Compare.
        returnVal *= -1;
      }
      return returnVal;
    }
  }
}
