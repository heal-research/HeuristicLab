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
using System.Threading;
using System.Windows.Forms;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// Base class for views that can load and save data.
  /// </summary>
  public partial class EditorBase : ViewBase, IEditor {
    private string myFilename;
    /// <summary>
    /// Gets or sets the filename of the current editor.
    /// </summary>
    /// <remarks>Calls <see cref="OnFilenameChanged"/> in the setter if the filename is new.</remarks>
    public string Filename {
      get { return myFilename; }
      set {
        if (value != myFilename) {
          myFilename = value;
          OnFilenameChanged();
        }
      }
    }

    /// <summary>
    /// Gets or sets, if the contained item should be compressed.
    /// </summary>
    public bool Compressed { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="EditorBase"/> with the caption "Editor".
    /// </summary>
    public EditorBase() {
      InitializeComponent();
      Caption = "Editor";
    }

    /// <summary>
    /// Asynchronously saves the contained object to a file.
    /// </summary>
    /// <remarks>The filename to save the contained item to is given by <see cref="Filename"/>.</remarks>
    public void Save() {
      Enabled = false;
      ThreadPool.QueueUserWorkItem((o) => {
        try {
          if (Compressed)
            PersistenceManager.SaveCompressed(Item, Filename);
          else
            PersistenceManager.Save(Item, Filename);
        } catch (Exception e) {
          MessageBox.Show(String.Format(
            "Sorry couldn't save file \"{0}\".\n The following exception occurred: {1}",
            Filename, e.ToString()),
            "Reader Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        } finally {
          Invoke(new Action(() => Enabled = true));
          OnSaveFinished();
        }
      });
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (Item == null)
        Caption = "Editor";
      else
        Caption = "Editor (" + Item.GetType().Name + ")";
    }

    /// <summary>
    /// Occurs when the filename is changed.
    /// </summary>
    public event EventHandler FilenameChanged;
    /// <summary>
    /// Fires a new <c>FilenameChanged</c> event.
    /// </summary>
    protected virtual void OnFilenameChanged() {
      if (FilenameChanged != null)
        FilenameChanged(this, new EventArgs());
    }

    /// <summary>
    /// Occurs after a save operation is finished.
    /// </summary>
    public event EventHandler SaveFinished;

    /// <summary>
    /// Fires a new <c>SaveFinished</c> event.
    /// </summary>
    protected virtual void OnSaveFinished() {
      SaveFinished(this, new EventArgs());
    }
  }
}
