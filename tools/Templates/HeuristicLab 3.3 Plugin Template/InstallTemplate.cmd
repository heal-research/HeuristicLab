SET ZIPFILENAME=HeuristicLab 3.3 Plugin Template.zip
"%ProgramFiles%\7-Zip\7z" a "%ZIPFILENAME%" 3.3 HeuristicLabIcon.ico PluginProjectCollectionTemplate.vstemplate
copy "%ZIPFILENAME%" "%UserProfile%\Documents\Visual Studio 2010\Templates\ProjectTemplates\Visual C#\"