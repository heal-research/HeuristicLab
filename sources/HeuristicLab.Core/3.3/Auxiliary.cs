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
using System.Windows.Forms;

namespace HeuristicLab.Core {
  /// <summary>
  /// Static helper class.
  /// </summary>
  public static class Auxiliary {
    #region Cloning
    /// <summary>
    /// Clones the given <paramref name="obj"/> (deep clone).
    /// </summary>
    /// <remarks>Checks before clone if object has not already been cloned.</remarks>
    /// <param name="obj">The object to clone.</param>
    /// <param name="clonedObjects">A dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object.</returns>
    public static object Clone(IStorable obj, IDictionary<Guid, object> clonedObjects) {
      object clone;
      if (clonedObjects.TryGetValue(obj.Guid, out clone))
        return clone;
      else
        return obj.Clone(clonedObjects);
    }
    #endregion

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

    #region Constraint Violation Messages
    /// <summary>
    /// Shows a warning message box with an OK-Button, indicating that the given constraints were violated and so 
    /// the operation could not be completed. 
    /// </summary>
    /// <param name="violatedConstraints">The constraints that could not be fulfilled.</param>
    public static void ShowConstraintViolationMessageBox(ICollection<IConstraint> violatedConstraints) {
      string message = BuildConstraintViolationMessage(violatedConstraints);
      MessageBox.Show("The following constraints are violated. The operation could not be completed.\n\n" + message,
                      "Constraint Violation",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);
    }
    /// <summary>
    /// Shows a question message box with a yes-no option, where to choose whether to ignore 
    /// the given violated constraints and to complete the operation or not.
    /// </summary>
    /// <param name="violatedConstraints">The constraints that could not be fulfilled.</param>
    /// <returns>The result of the choice ("Yes" = 6, "No" = 7).</returns>
    public static DialogResult ShowIgnoreConstraintViolationMessageBox(ICollection<IConstraint> violatedConstraints) {
      string message = BuildConstraintViolationMessage(violatedConstraints);
      return MessageBox.Show("The following constraints are violated. Do you want to complete the operation anyhow?\n\n" + message,
                             "Constraint Violation",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question);
    }
    /// <summary>
    /// Builds a message out of a given collection of violated constraints, 
    /// including the constraints type and description.
    /// </summary>
    /// <param name="violatedConstraints">The constraints that could not be fulfilled.</param>
    /// <returns>The message to display.</returns>
    private static string BuildConstraintViolationMessage(ICollection<IConstraint> violatedConstraints) {
      StringBuilder sb = new StringBuilder();
      foreach (IConstraint constraint in violatedConstraints) {
        sb.AppendLine(constraint.GetType().Name);
        sb.AppendLine(constraint.Description);
        sb.AppendLine();
      }
      return sb.ToString();
    }
    #endregion
  }
}
