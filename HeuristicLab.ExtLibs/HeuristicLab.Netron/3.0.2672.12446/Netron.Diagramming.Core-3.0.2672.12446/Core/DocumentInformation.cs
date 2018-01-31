using System;

namespace Netron.Diagramming.Core {
  /// <summary>
  /// The meta-data (author, date, description) related to a diagram is collected in this class
  /// </summary>
  public partial class DocumentInformation : IVersion {
    #region Fields

    // ------------------------------------------------------------------
    /// <summary>
    /// Implementation of IVersion - the current version of 
    /// DocumentInformation.
    /// </summary>
    // ------------------------------------------------------------------
    protected const double documentInformationVersion = 1.0;

    /// <summary>
    /// the CreationDate field
    /// </summary>
    private string mCreationDate = DateTime.Now.ToString();
    /// <summary>
    /// the field
    /// </summary>
    private string mAuthor = string.Empty;
    /// <summary>
    /// the Description field
    /// </summary>
    private string mDescription = string.Empty;
    /// <summary>
    /// the Title field
    /// </summary>
    private string mTitle = string.Empty;
    #endregion

    #region Properties

    // ------------------------------------------------------------------
    /// <summary>
    /// Gets the current version.
    /// </summary>
    // ------------------------------------------------------------------
    public virtual double Version {
      get {
        return documentInformationVersion;
      }
    }

    /// <summary>
    /// Gets or sets the 
    /// </summary>
    public string Author {
      get { return mAuthor; }
      set { mAuthor = value; }
    }
    /// <summary>
    /// Gets or sets the Description
    /// </summary>
    public string Description {
      get { return mDescription; }
      set { mDescription = value; }
    }
    /// <summary>
    /// Gets or sets the Title
    /// </summary>
    public string Title {
      get { return mTitle; }
      set { mTitle = value; }
    }
    /// <summary>
    /// Gets or sets the CreationDate
    /// </summary>
    public string CreationDate {
      get { return mCreationDate; }
      set { mCreationDate = value; }
    }
    #endregion

    #region Constructor
    ///<summary>
    ///Default constructor
    ///</summary>
    public DocumentInformation() {

    }
    #endregion

  }
}
