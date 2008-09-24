using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public partial class ProtocolInjectorView : ViewBase {
    public ProtocolInjector ProtocolInjector {
      get { return (ProtocolInjector)Item; }
      set {
        Item = value;
      }
    }

    public ProtocolInjectorView() {
      InitializeComponent(); // killed self included code regarding ItemDictionaryView
      dictionaryTabPage.SuspendLayout();
      SuspendLayout();
      itemDictionaryView = new ItemDictionaryView<StringData, StringData>();
      itemDictionaryView.Location = new Point(6, 6);
      itemDictionaryView.Dock = DockStyle.Fill;
      itemDictionaryView.Name = "itemDictionaryView";
      dictionaryTabPage.Controls.Add(itemDictionaryView);
      dictionaryTabPage.ResumeLayout(false);
      ResumeLayout(false);
    }

    public ProtocolInjectorView(ProtocolInjector protocolInjector)
      : this() {
      ProtocolInjector = protocolInjector;
    }

    protected override void RemoveItemEvents() {
      operatorBaseVariableInfosView.Operator = null;
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      operatorBaseVariableInfosView.Operator = ProtocolInjector;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (ProtocolInjector == null) {
        loadProtocolButton.Enabled = false;
        protocolEditor.Enabled = false;
        protocolEditor.Protocol = null;
        itemDictionaryView.Enabled = false;
        itemDictionaryView.ItemDictionary = null;
      } else {
        loadProtocolButton.Enabled = true;
        protocolEditor.Protocol = (Protocol)ProtocolInjector.GetVariable("Protocol").Value;
        protocolEditor.Enabled = true;
        IVariable dict = ProtocolInjector.GetVariable("Dictionary");
        if (dict == null) { // BACKWARDS COMPATIBLE CODE for versions prior to 3.0.0.40
          dict = new Variable("Dictionary", new ItemDictionary<StringData, StringData>());
          ProtocolInjector.AddVariable(dict);
          ProtocolInjector.AddVariableInfo(new VariableInfo("Dictionary", "The received value translation dictionary", typeof(Dictionary<StringData, StringData>), VariableKind.New));
        }
        itemDictionaryView.ItemDictionary = (ItemDictionary<StringData, StringData>)dict.Value;
        itemDictionaryView.Enabled = true;
      }
    }

    private void loadProtocolButton_Click(object sender, EventArgs e) {
      if (protocolOpenFileDialog.ShowDialog() == DialogResult.OK) {
        Protocol protocol = null;
        try {
          protocol = (Protocol)PersistenceManager.Load(protocolOpenFileDialog.FileName);
        } catch (InvalidCastException) {
          throw new InvalidOperationException("Document is not a protocol");
        } catch (Exception ex) {
          throw new InvalidOperationException("An exception has occured:\n"+ex.Message.ToString());
        }
        ProtocolInjector.GetVariable("Protocol").Value = protocol;
        UpdateControls();
      }
    }
  }
}
