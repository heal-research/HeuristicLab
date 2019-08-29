#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class ListViewItemComparer : IComparer {
    private int[] cols;
    public SortOrder[] Orders { get; set; }
    private const string dateFormat = "dd.MM.yyyy HH:mm";

    public ListViewItemComparer() {
      cols = new []{ 0 };
      Orders = new[] { SortOrder.Ascending };
    }

    public ListViewItemComparer(int[] columns, SortOrder[] orders) {
      cols = columns;
      Orders = orders;
    }

    public int Compare(object x, object y) {
      int returnVal;
      bool result;
      ListViewItem listViewItemX, listViewItemY;
      listViewItemX = x as ListViewItem;
      listViewItemY = y as ListViewItem;

      if (listViewItemX == null || listViewItemY == null) {
        throw new ArgumentException(string.Format("The ListViewItemComparer expects ListViewItems but received {0} and {1}.",
          x.GetType().ToString(), y.GetType().ToString()));
      }

      int cmpres = 0;
      for (int i = 0; i < cols.Length && cmpres == 0; i++) {
        string textX = listViewItemX.SubItems[cols[i]].Text;
        string textY = listViewItemY.SubItems[cols[i]].Text;

        DateTime dateX, dateY;
        int intX, intY;
        double doubleX, doubleY;

        if(DateTime.TryParse(textX, out dateX) && DateTime.TryParse(textY, out dateY)) {
          cmpres = DateTime.Compare(dateX, dateY);
        } else if(Int32.TryParse(textX, out intX) && Int32.TryParse(textY, out intY)) {
          cmpres = (intX == intY) ? 0 : (intX > intY) ? 1 : -1;
        } else if(Double.TryParse(textX, out doubleX) && Double.TryParse(textY, out doubleY)) {
          cmpres = (doubleX == doubleY) ? 0 : (doubleX > doubleY) ? 1 : -1;
        } else {
          cmpres = String.Compare(textX, textY);
        }
        if(Orders[i] == SortOrder.Descending) {
          cmpres *= -1;
        }
      }
      return cmpres;
    }
  }
}
