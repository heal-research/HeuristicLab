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
  public static class Auxiliary {
    #region Cloning
    public static object Clone(IStorable obj, IDictionary<Guid, object> clonedObjects) {
      object clone;
      if (clonedObjects.TryGetValue(obj.Guid, out clone))
        return clone;
      else
        return obj.Clone(clonedObjects);
    }
    #endregion

    #region Error Messages
    public static void ShowErrorMessageBox(string message) {
      MessageBox.Show(message,
                      "Error",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
    }
    public static void ShowErrorMessageBox(Exception ex) {
      MessageBox.Show(BuildErrorMessage(ex),
                      "Error - " + ex.GetType().Name,
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
    }
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
    public static void ShowConstraintViolationMessageBox(ICollection<IConstraint> violatedConstraints) {
      string message = BuildConstraintViolationMessage(violatedConstraints);
      MessageBox.Show("The following constraints are violated. The operation could not be completed.\n\n" + message,
                      "Constraint Violation",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);
    }
    public static DialogResult ShowIgnoreConstraintViolationMessageBox(ICollection<IConstraint> violatedConstraints) {
      string message = BuildConstraintViolationMessage(violatedConstraints);
      return MessageBox.Show("The following constraints are violated. Do you want to complete the operation anyhow?\n\n" + message,
                             "Constraint Violation",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question);
    }
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
