using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ICSharpCode.AvalonEdit")]
[assembly: AssemblyDescription("WPF-based extensible text editor")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ic#code")]
[assembly: AssemblyProduct("SharpDevelop")]
[assembly: AssemblyCopyright("2000-2014 AlphaSierraPapa for the SharpDevelop Team")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: CLSCompliant(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5d2fbf08-c059-4fa7-98ef-a56916bd007b")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("5.0.1")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: NeutralResourcesLanguage("en-US")]

[assembly: ThemeInfo(
  ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
  //(used if a resource is not found in the page,
  // or application resource dictionaries)
  ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
  //(used if a resource is not found in the page,
  // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsPrefix("http://icsharpcode.net/sharpdevelop/avalonedit", "avalonedit")]

[assembly: XmlnsDefinition("http://icsharpcode.net/sharpdevelop/avalonedit", "ICSharpCode.AvalonEdit")]
[assembly: XmlnsDefinition("http://icsharpcode.net/sharpdevelop/avalonedit", "ICSharpCode.AvalonEdit.Editing")]
[assembly: XmlnsDefinition("http://icsharpcode.net/sharpdevelop/avalonedit", "ICSharpCode.AvalonEdit.Rendering")]
[assembly: XmlnsDefinition("http://icsharpcode.net/sharpdevelop/avalonedit", "ICSharpCode.AvalonEdit.Highlighting")]
[assembly: XmlnsDefinition("http://icsharpcode.net/sharpdevelop/avalonedit", "ICSharpCode.AvalonEdit.Search")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2243:AttributeStringLiteralsShouldParseCorrectly",
  Justification = "AssemblyInformationalVersion does not need to be a parsable version")]