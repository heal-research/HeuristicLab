SET ZIPFILENAME=HeuristicLab 3.3 Plugin Template.zip
"%ProgramFiles%\7-Zip\7z" a "%ZIPFILENAME%" Properties HeuristicLab.Plugin.csproj HeuristicLab.snk HeuristicLabIcon.ico Plugin.cs Plugin.cs.frame PluginProjectTemplate.vstemplate
copy "%ZIPFILENAME%" "%UserProfile%\Documents\Visual Studio 2010\Templates\ProjectTemplates\Visual C#\"