using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Contains a collection of shapes and handles loading shapes from
  /// assemblies.  This can also be serialized for later use.
  /// </summary>
  // ----------------------------------------------------------------------
  public class Library {
    // ------------------------------------------------------------------
    /// <summary>
    /// The collection of shapes for this library.
    /// </summary>
    // ------------------------------------------------------------------
    CollectionBase<IShape> myShapes;

    // ------------------------------------------------------------------
    /// <summary>
    /// The name of this library.
    /// </summary>
    // ------------------------------------------------------------------
    string myName = "Library";

    // ------------------------------------------------------------------
    /// <summary>
    /// The complete path to the assembly to load.
    /// </summary>
    // ------------------------------------------------------------------
    string myPath = String.Empty;

    // ------------------------------------------------------------------
    /// <summary>
    /// Our assembly.
    /// </summary>
    // ------------------------------------------------------------------
    Assembly assembly;

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the name of this library.
    /// </summary>
    // ------------------------------------------------------------------
    public string Name {
      get {
        return myName;
      }
      set {
        myName = value;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the shapes in the library.
    /// </summary>
    // ------------------------------------------------------------------
    public CollectionBase<IShape> Shapes {
      get {
        return myShapes;
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    // ------------------------------------------------------------------
    public Library() {
      this.myShapes = new CollectionBase<IShape>();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Returns if this library contains a shape with the GUID specified.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    // ------------------------------------------------------------------
    public bool ContainsShape(string guid) {
      foreach (IShape shape in myShapes) {
        if (shape.Uid.ToString() == guid) {
          return true;
        }
      }
      return false;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the shape with the GUID specified.
    /// </summary>
    /// <param name="guid">string</param>
    /// <returns>IShape</returns>
    // ------------------------------------------------------------------
    public IShape CreateNewInstance(string guid) {
      if ((assembly == null) ||
          (guid == "") ||
          (guid == String.Empty)) {
        return null;
      }

      string typeName = "";
      foreach (IShape shape in myShapes) {
        if (shape.Uid.ToString() == guid) {
          typeName = shape.GetType().FullName;
          IShape newInstance =
              (IShape)assembly.CreateInstance(typeName);

          return newInstance;
        }
      }
      return null;
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Imports all shapes that have the "ShapeAttribute" attribute
    /// from the assembly (i.e. "dll") specified.
    /// </summary>
    /// <param name="path">string: The complete path to the assembly to
    /// load.</param>
    // ------------------------------------------------------------------
    public void Load(string path) {
      try {
        myPath = path;
        Directory.SetCurrentDirectory(
            Path.GetDirectoryName(Application.ExecutablePath));

        // Use the "LoadFile" option so the assembly is also loaded
        // into the current domain.  This is important!!!
        assembly = Assembly.LoadFile(path);
        if (assembly == null) {
          return;
        }

        // The assembly types.
        Type[] types = assembly.GetTypes();

        if (types == null) {
          return;
        }

        IShape shapeInstance = null;
        object[] objs;

        // Loop over all modules in the assembly and get all shapes
        // that are marked with the attribute that indicates they're
        // to be loaded into the library.
        for (int k = 0; k < types.Length; k++) {
          // It has to be a class to be a shape!
          if (!types[k].IsClass) {
            continue;
          }

          objs = types[k].GetCustomAttributes(
              typeof(ShapeAttribute), false);

          if (objs.Length < 1) {
            // This module isn't a shape so go to the next one.
            continue;
          }

          // Now, we are sure to have a shape object.					

          try {
            // Normally you'd need the constructor passing the 
            // Model, but this instance will not actually live 
            // on the canvas and hence cause no problem.
            // However, you do need a ctor with no parameters!
            shapeInstance = (IShape)assembly.CreateInstance(
                types[k].FullName);

            ShapeAttribute shapeAtts = objs[0] as ShapeAttribute;
            this.myShapes.Add(shapeInstance);
            //summary = new ShapeSummary(path, shapeAtts.Key,shapeAtts.Name, shapeAtts.ShapeCategory, shapeAtts.ReflectionName, shapeAtts.Description);
            //library.ShapeSummaries.Add(summary);
          }
          catch (Exception exc) {
            Trace.WriteLine(exc.Message, "An error occurred " +
                "while creating a shape from type " +
                types[k].FullName);
            continue;
          }
        }

        // Now, set the name of this library to the assembly title.
        foreach (Attribute attr in
            Attribute.GetCustomAttributes(assembly)) {
          // Check for the AssemblyTitle attribute.
          if (attr.GetType() == typeof(AssemblyTitleAttribute)) {
            this.myName = ((AssemblyTitleAttribute)attr).Title;
            break;
          }
        }

      }
      catch (Exception e) {
        Trace.WriteLine(e.Message, "An error occurred while " +
            "loading library from:\n" + path);
        return;
      }
    }
  }
}
