using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization {
  public delegate void XAxisDescriptorChangedHandler(XAxisDescriptor sender);

  public class XAxisDescriptor : StorableBase {
    private string label = "";
    private Font font = new Font("Arial", 8);
    private Color color = Color.Blue;
    private bool showLabel = true;
    private bool showGrid = true;
    private Color gridColor = Color.LightBlue;
    private ILabelProvider labelProvider = new ContinuousLabelProvider("0.##");

    public event XAxisDescriptorChangedHandler XAxisDescriptorChanged;

    public string Label {
      get { return label; }
      set {
        label = value;
        FireXAxisDescriptorChanged();
      }
    }

    public Font Font {
      get { return font; }
      set {
        font = value;
        FireXAxisDescriptorChanged();
      }
    }

    public Color Color {
      get { return color; }
      set {
        color = value;
        FireXAxisDescriptorChanged();
      }
    }

    public bool ShowLabel {
      get { return showLabel; }
      set {
        showLabel = value;
        FireXAxisDescriptorChanged();
      }
    }

    public bool ShowGrid {
      get { return showGrid; }
      set {
        this.showGrid = value;
        FireXAxisDescriptorChanged();
      }
    }

    public Color GridColor {
      get { return this.gridColor; }
      set {
        this.gridColor = value;
        FireXAxisDescriptorChanged();
      }
    }

    public ILabelProvider LabelProvider {
      get { return labelProvider; }
      set {
        this.labelProvider = value;
        FireXAxisDescriptorChanged();
      }
    }

    private void FireXAxisDescriptorChanged() {
      if (XAxisDescriptorChanged != null)
        XAxisDescriptorChanged(this);
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);

      XmlSupport.SetAttribute("Color", this.Color.ToArgb().ToString(), node);
      XmlSupport.SetAttribute("GridColor", this.GridColor.ToArgb().ToString(), node);
      XmlSupport.SetAttribute("Label", this.Label, node);
      XmlSupport.SetAttribute("ShowGrid", this.ShowGrid ? "true" : "false", node);
      XmlSupport.SetAttribute("ShowLabel", this.ShowLabel ? "true" : "false", node);

      node.AppendChild(PersistenceManager.Persist("LabelProvider", this.LabelProvider, document, persistedObjects));

      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      this.Color = Color.FromArgb(int.Parse(XmlSupport.GetAttribute("Color", Color.Blue.ToArgb().ToString(), node)));
      this.GridColor = Color.FromArgb(int.Parse(XmlSupport.GetAttribute("GridColor", Color.LightBlue.ToArgb().ToString(), node)));
      this.Label = XmlSupport.GetAttribute("Label", "", node);
      this.ShowGrid = XmlSupport.GetAttribute("ShowGrid", "true", node) == "true";
      this.ShowLabel = XmlSupport.GetAttribute("ShowLabel", "true", node) == "true";

      XmlNode labelProviderNode = node.SelectSingleNode("XAxis");
      if (labelProviderNode != null)
        this.labelProvider = (ILabelProvider)PersistenceManager.Restore(labelProviderNode, restoredObjects);
    }
  }
}