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


namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// Abstract base implementation for the IApplication interface.
  /// </summary>
  public abstract class ApplicationBase : IApplication {
    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationBase"/>.
    /// </summary>
    protected ApplicationBase() { }

    private ApplicationAttribute ApplicationAttribute {
      get {
        object[] appAttributes = this.GetType().GetCustomAttributes(typeof(ApplicationAttribute), false);

        // exactly one attribute of the type ClassInfoAttribute must be given
        if (appAttributes.Length == 0) {
          throw new InvalidPluginException("ApplicationAttribute on type " + this.GetType() + " is missing.");
        } else if (appAttributes.Length > 1) {
          throw new InvalidPluginException("Found multiple ApplicationAttributes on type " + this.GetType());
        }

        return (ApplicationAttribute)appAttributes[0];
      }
    }

    #region IApplication Members

    /// <summary>
    /// Gets the name of the application.
    /// </summary>
    public string Name {
      get { return ApplicationAttribute.Name; }
    }

    /// <summary>
    /// Gets the description of the application.
    /// </summary>
    public string Description {
      get {
        return ApplicationAttribute.Description;
      }
    }

    /// <summary>
    /// Runs the application.
    /// </summary>
    public abstract void Run(ICommandLineArgument[] args);

    #endregion
  }
}
