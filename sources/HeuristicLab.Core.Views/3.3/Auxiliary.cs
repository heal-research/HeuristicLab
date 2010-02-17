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
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// Static helper class.
  /// </summary>
  public static class Auxiliary {
    #region Error Messages
    /// <summary>
    /// Shows an error message box with a given error <paramref name="message"/> and an OK-Button.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    public static void ShowErrorMessageBox(string message) {
      MessageBox.Show(message,
                      "Error",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
    }
    /// <summary>
    /// Shows an error message box with a given exception <paramref name="ex"/> and an OK-Button.
    /// </summary>
    /// <param name="ex">The exception to display.</param>
    public static void ShowErrorMessageBox(Exception ex) {
      MessageBox.Show(BuildErrorMessage(ex),
                      "Error - " + ex.GetType().Name,
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
    }
    /// <summary>
    /// Builds an error message out of an exception and formats it accordingly.
    /// </summary>
    /// <param name="ex">The exception to format.</param>
    /// <returns>The formated message.</returns>
    private static string BuildErrorMessage(Exception ex) {
      StringBuilder sb = new StringBuilder();
      sb.Append("Sorry, but something went wrong!\n\n" + ex.Message + "\n\n" + ex.StackTrace);

      while (ex.InnerException != null) {
        ex = ex.InnerException;
        sb.Append("\n\n-----\n\n" + ex.Message + "\n\n" + ex.StackTrace);
      }
      return sb.ToString();
    }
    #endregion
  }
}
