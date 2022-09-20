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

using System.Runtime.CompilerServices;

//[assembly: InternalsVisibleTo("HeuristicLab.PluginInfrastructure.Extension-3.3, PublicKey=0024000004800000940000000602000000240000525341310004000001000100e3d38bc66a0dd8\r\ndd36f57285e34632ec04b3049866ab1e64cf230e95ffcbfbb90c437b4d11dfe74ba981f7462742\r\n90bb03f3e636e139e685b501031dc6e0bc8409153f0c842721eb9e8e2a703c9e4d102283f3ddbd\r\nfab4078c08de51869715992a694d2f608d0fa865c9d17c06b8d6a9135004e982fd862cdb2277e4ad15a4a6")]
namespace HeuristicLab.PluginInfrastructure.Advanced {
  
  internal interface IStatusView {
    void ShowProgressIndicator(double percentProgress);
    void ShowProgressIndicator();
    void HideProgressIndicator();
    void ShowMessage(string message);
    void RemoveMessage(string message);
    void LockUI();
    void UnlockUI();
    void ShowError(string shortMessage, string description);
  }
}
