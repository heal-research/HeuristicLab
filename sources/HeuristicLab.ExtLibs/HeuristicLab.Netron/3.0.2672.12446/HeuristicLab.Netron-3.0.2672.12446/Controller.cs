using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Netron.Diagramming.Core;

namespace HeuristicLab.Netron {
  public class Controller : ControllerBase {

    public const string TextToolName = "Text Tool";
    public const string TextEditorToolName = "Text Editor Tool";

    public Controller(IDiagramControl surface)
      : base(surface) {
    }

    public override bool ActivateTextEditor(ITextProvider textProvider) {
      TextEditor.GetEditor(textProvider);
      TextEditor.Show();
      return true;
    }

    public ILayout StandardTreeLayout {
      get {
        return (ILayout)this.FindActivity("Standard TreeLayout");
      }
    }
  }
}
