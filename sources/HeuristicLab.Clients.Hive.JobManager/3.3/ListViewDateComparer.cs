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
using System.Collections;
using System.Windows.Forms;

namespace HeuristicLab.Clients.Hive.JobManager {
  /// <summary>
  /// Comparer for sorting items in a list view by date
  /// See: http://msdn.microsoft.com/en-us/library/ms996467.aspx
  /// </summary>
  public class ListViewDateComparer : IComparer {
    private int col;
    private SortOrder order;

    public ListViewDateComparer() {
      col = 0;
      order = SortOrder.Ascending;
    }

    public ListViewDateComparer(int column, SortOrder order) {
      col = column;
      this.order = order;
    }

    public int Compare(object x, object y) {
      int returnVal;

      if (!(x is ListViewItem) || !(y is ListViewItem)) {
        throw new InvalidCastException(string.Format("The ListViewDateComparer expects ListViewItems but received {0} and {1}.",
          x.GetType().ToString(), y.GetType().ToString()));
      }

      try {
        DateTime firstDate = DateTime.Parse(((ListViewItem)x).SubItems[col].Text);
        DateTime secondDate = DateTime.Parse(((ListViewItem)y).SubItems[col].Text);
        returnVal = DateTime.Compare(firstDate, secondDate);
      }
      // if neither compared object has a valid date format, compare as a string
      catch {
        returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
      }

      if (order == SortOrder.Descending) {
        // invert the value returned by Compare.
        returnVal *= -1;
      }
      return returnVal;
    }
  }
}
