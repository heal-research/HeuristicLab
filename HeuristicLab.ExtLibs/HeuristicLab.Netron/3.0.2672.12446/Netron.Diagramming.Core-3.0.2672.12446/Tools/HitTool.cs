using System;
using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// This tool implement the action of hitting an entity on the canvas. 
  /// </summary>
  // ----------------------------------------------------------------------
  public class HitTool : AbstractTool, IMouseListener, IKeyboardListener {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Specifies if one of the multi-select keys are pressed.  If this
    /// is false, then the current Selection is cleared before adding
    /// any selected entities to the Selection.
    /// </summary>
    // ------------------------------------------------------------------
    bool isMultiSelectKeyPressed = false;

    // ------------------------------------------------------------------
    /// <summary>
    /// The keyboard keys that enable/disable multi-selection.
    /// </summary>
    // ------------------------------------------------------------------
    Keys[] myMultiSelectKeys = new Keys[]
            {
                Keys.ShiftKey,
                Keys.ControlKey
            };

    #endregion

    #region Constructor

    // ------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="T:HitTool"/> class.
    /// </summary>
    /// <param name="name">The name of the tool.</param>
    // ------------------------------------------------------------------
    public HitTool(string name)
      : base(name) {
    }
    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Called when the tool is activated.
    /// </summary>
    // ------------------------------------------------------------------
    protected override void OnActivateTool() {
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the mouse down event
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public virtual bool MouseDown(MouseEventArgs e) {
      if (e == null) {
        throw new ArgumentNullException("The argument object is 'null'");
      }

      //if(e.Button == MouseButtons.Left  && Enabled && !IsSuspended)
      if (Enabled && !IsSuspended) {
        // Only if one of the multi-select keys are pressed do we NOT
        // clear the selection before adding to it the selected entity.
        bool clearSelectionFirst = true;
        if (this.isMultiSelectKeyPressed == true) {
          clearSelectionFirst = false;
        }

        // Also don't clear the selection if a group is currently
        // selected so we can drill-down into it.
        if (this.Controller.Model.Selection.SelectedItems.Count > 0) {
          foreach (IDiagramEntity entity in this.Controller.Model.Selection.SelectedItems) {
            if ((entity is IGroup) &&
                (entity.Hit(e.Location) == true)) {
              clearSelectionFirst = false;
            }
          }
        }
        this.Controller.Model.Selection.CollectEntitiesAt(e.Location, clearSelectionFirst);

        if (this.Controller.Model.Selection.SelectedItems.Count > 0) {
          IMouseListener listener =
             this.Controller.Model.Selection.SelectedItems[0].GetService(
              typeof(IMouseListener)) as IMouseListener;

          if (listener != null) {
            if (listener.MouseDown(e))
              return true;
          }
        }

        if ((this.Controller.Model.Selection.SelectedItems.Count > 0) &&
            (this.Controller.Model.Selection.SelectedItems[0] is ITextProvider)) {
          //ActivateTool();
          ITextProvider textProvider =
             this.Controller.Model.Selection.SelectedItems[0] as ITextProvider;

          if ((e.Clicks == textProvider.EditTextClicks) &&
              (textProvider.AllowTextEditing)) {
            return Controller.ActivateTextEditor(textProvider);
          }
        }
      }
      return false;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the mouse move event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public virtual void MouseMove(MouseEventArgs e) {

    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the mouse up event.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public virtual void MouseUp(MouseEventArgs e) {
      if (IsActive) {
        DeactivateTool();
      }
    }
    #endregion

    #region IKeyboardListener Members

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the key down event.  If the key pressed is one of our
    /// keys defined as a multi-select key, then selected items are
    /// appended to the Selection (i.e. the Selection is not cleared
    /// first).
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public void KeyDown(KeyEventArgs e) {
      foreach (Keys k in this.myMultiSelectKeys) {
        if (e.KeyCode == k) {
          this.isMultiSelectKeyPressed = true;
          return;
        }
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the key up event.  If the key pressed is one of our
    /// keys defined as a multi-select key, then multi-select mode is
    /// disabled.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public void KeyUp(KeyEventArgs e) {
      foreach (Keys k in this.myMultiSelectKeys) {
        if (e.KeyCode == k) {
          this.isMultiSelectKeyPressed = false;
          return;
        }
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the key press event.  Nothing is performed here.
    /// </summary>
    /// <param name="e">The 
    /// <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance 
    /// containing the event data.</param>
    // ------------------------------------------------------------------
    public void KeyPress(KeyPressEventArgs e) { }

    #endregion
  }

}
