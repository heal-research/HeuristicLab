# find ms build
$programFilesX86Dir = ($Env:ProgramFiles, ${Env:ProgramFiles(x86)})[[bool]${Env:ProgramFiles(x86)}]
$locations = @(
  [System.IO.Path]::Combine($programFilesX86Dir, "Microsoft Visual Studio", "2017", "Enterprise", "MSBuild", "15.0", "Bin", "amd64")
  [System.IO.Path]::Combine($programFilesX86Dir, "Microsoft Visual Studio", "2017", "Enterprise", "MSBuild", "15.0", "Bin")
  [System.IO.Path]::Combine($programFilesX86Dir, "Microsoft Visual Studio", "2017", "Community", "MSBuild", "15.0", "Bin", "amd64")
  [System.IO.Path]::Combine($programFilesX86Dir, "Microsoft Visual Studio", "2017", "Community", "MSBuild", "15.0", "Bin")
  [System.IO.Path]::Combine($programFilesX86Dir, "Microsoft Visual Studio", "2017", "BuildTools", "MSBuild", "15.0", "Bin", "amd64"),
  [System.IO.Path]::Combine($programFilesX86Dir, "Microsoft Visual Studio", "2017", "BuildTools", "MSBuild", "15.0", "Bin")
)

$msBuildPath = $undefined
Foreach ($loc in $locations) {
  $loc = [System.IO.Path]::Combine($loc, "MSBuild.exe")
  If ([System.IO.File]::Exists($loc)) {
    $msBuildPath = $loc
    Break;
  }
}

Try {
  If ($msBuildPath -eq $undefined) {
    "Could not locate MSBuild, ABORTING ..."
    Return
  }

  $curPath = $MyInvocation.MyCommand.Path
  $curDir = Split-Path $curPath

  $slnFiles = Get-ChildItem $curDir -Filter *.sln

  If ($slnFiles.Count -le 0) {
    "No solutions found, ABORTING ..."
    Return
  }

  $slnIndices = @()

  If ($slnFiles.Count -eq 1) {
    "Selecting the only solution found: `"{0}`"" -f $slnFiles[0].Name
    $slnIndices += 0
  } Else {
    "Found the following solutions:"

    ""

    $slnFiles | % { $i = 0 } { ("  {0}. `"{1}`"" -f ($i + 1), $_.Name); $i++ }

    ""

    $success = $false

    # query solution to build
    $slnIndex = -1
    Do {
        $input = Read-Host "Which solution(s) to build? (e.g.: 1 2 3) { 1..$($slnFiles.Count) }"
        $inputParts = $input -Split " "

        Foreach ($part in $inputParts) {
          If ($part -eq "") { Continue }

          $success = [int]::TryParse($part, [ref]$slnIndex) -and ($slnIndex -gt 0) -and ($slnIndex -le $slnFiles.Count)

          If ($success) {
            $slnIndices += $slnIndex - 1
          } Else {
            $slnIndices = @()
            Break
          }
        }
    } While (-Not $success)

    $slnIndices = $slnIndices | Select-Object -Unique
  }


  # query configuration to build
  $config = "Release"
  $input = Read-Host "Which configuration to build? [$($config)]"
  $config = ($config, $input)[[bool]$input]

  # query platform to build
  $platform = "Any CPU"
  $input = Read-Host "Which platform to build? [$($platform)]"
  $platform = ($platform, $input)[[bool]$input]

  # query clean desire
  $clean = $false
  Do {
      $input = Read-Host "Would you like to clean before building? [y/N]"
      $input = ([string]("n", $input)[[bool]$input]).ToLowerInvariant()
      $success = $input -eq "n" -or ($clean = $input -eq "y")
  } While (-Not $success)

  ""

  If ($clean) {
    Foreach ($slnIndex in $slnIndices) {
      $solution = $slnFiles[$slnIndex]
      "Cleaning `"$($solution.Name)`" ..."
      $args = @(
        $solution.FullName,
        "/t:Clean",
        "/p:Configuration=`"$config`",Platform=`"$platform`"",
        "/m", "/nologo", "/verbosity:q", "/clp:ErrorsOnly"
      )
      & $msBuildPath $args
      "===== CLEAN FINISHED ====="
    }
  }

  Foreach ($slnIndex in $slnIndices) {
    $solution = $slnFiles[$slnIndex]
    "Building `"$($solution.Name)`" ($config|$platform) ..."
    $args = @(
      $solution.FullName,
      "/t:Restore,Build",
      "/p:Configuration=`"$config`",Platform=`"$platform`"",
      "/m", "/nologo", "/verbosity:q", "/clp:ErrorsOnly"
    )
    & $msBuildPath $args
    "===== BUILD FINISHED ====="
  }
} Finally {
  ""

  Write-Host -NoNewline "Press any key to continue ... "

  [void][System.Console]::ReadKey($true)
}
