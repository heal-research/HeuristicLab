export AIFile=$ProjectDir/Properties/AssemblyInfo.cs.frame
export PluginFile=$ProjectDir/Plugin.cs.frame

command_exists () {
    command -v "$1" 
}

if command_exists svnwcrev ; then   
	if [ -f $AIFile ];
	then
	  svnwcrev $ProjectDir $AIFile $ProjectDir/Properties/AssemblyInfo.cs
	fi

	if [ -f $PluginFile ];
	then
	  svnwcrev $ProjectDir $PluginFile $ProjectDir/Plugin.cs
	fi
else
	if [ -f $AIFile ];
	then
	  sed 's/\$WCREV\$/'0'/g' $AIFile > $ProjectDir/Properties/AssemblyInfo.cs 
	fi

	if [ -f $PluginFile ];
	then
	 sed 's/\$WCREV\$/'0'/g' $PluginFile > $ProjectDir/Plugin.cs
	fi
fi

