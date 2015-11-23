//To localize the descriptions see http://groups.google.be/group/microsoft.public.dotnet.languages.csharp/browse_thread/thread/3bb6895b49d7cbe/e3241b7fa085ba90?lnk=st&q=csharp+attribute+resource+file&rnum=4#e3241b7fa085ba90
namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// The document class represents the root of the controls' data 
  /// hierarchy. The document is the root of the serialization graph and 
  /// contains both the data of the diagram(s) and the metadata (or user 
  /// information).
  /// </summary>
  // ----------------------------------------------------------------------
  public partial class Document : IVersion {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// Document.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double documentVersion = 1.0;

    // ------------------------------------------------------------------
    /// <summary>
    /// Pointer to the model.
    /// </summary>       
    // ------------------------------------------------------------------
    protected IModel mModel;

    // ------------------------------------------------------------------
    /// <summary>
    /// The Information field.
    /// </summary>
    // ------------------------------------------------------------------
    protected DocumentInformation mInformation;

    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual double Version {
      get {
        return documentVersion;
      }
    }

    /// <summary>
    /// Gets or sets the Information
    /// </summary>
    public DocumentInformation Information {
      get {
        return mInformation;
      }
      set {
        mInformation = value;
      }
    }


    /// <summary>
    /// Gets the model.
    /// </summary>
    /// <value>The model.</value>
    public IModel Model {
      get {
        return mModel;
      }
    }

    #endregion

    #region Constructor
    ///<summary>
    ///Default constructor. Creates a new document and, hence, a new model with one default page and one default layer.
    ///</summary>
    public Document() {
      mModel = new Model();
      mInformation = new DocumentInformation();
    }


    #endregion

    #region Methods

    // ------------------------------------------------------------------
    /// <summary>
    /// Loops through all pages and returns ALL entities that belong to
    /// this Document.
    /// </summary>
    /// <returns>CollectionBase</returns>
    // ------------------------------------------------------------------
    public CollectionBase<IDiagramEntity> GetAllEntities() {
      CollectionBase<IDiagramEntity> entities =
              new CollectionBase<IDiagramEntity>();
      foreach (IPage page in this.mModel.Pages) {
        foreach (ILayer layer in page.Layers) {
          entities.AddRange(layer.Entities);
        }
      }
      return entities;
    }

    #endregion

  }
}
