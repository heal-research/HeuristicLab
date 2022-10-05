# find ms build
$programFilesDirs = @($Env:ProgramFiles, ${Env:ProgramFiles(x86)})
$years = @("2022", "2019", "2017")
$editions = @("Enterprise", "Professional", "Community", "BuildTools")
$versions = @("Current", "15.0")

$msBuildPath = $undefined
$vstestPath = $undefined
:search Foreach ($dir in $programFilesDirs) {
  $vsDir = [System.IO.Path]::Combine($dir, "Microsoft Visual Studio")
  Foreach ($year in $years) {
    $loc = [System.IO.Path]::Combine($vsDir, $year)
    Foreach ($edition in $editions) {
      $edLoc = [System.IO.Path]::Combine($loc, $edition, "MSBuild")
      Foreach ($version in $versions) {
        $binLoc = [System.IO.Path]::Combine($edLoc, $version, "Bin")
        $loc64 = [System.IO.Path]::Combine($binLoc, "amd64", "MSBuild.exe")
        $loc32 = [System.IO.Path]::Combine($binLoc, "MSBuild.exe")

        If ([System.IO.File]::Exists($loc64)) {
          $msBuildPath = $loc64
          $vstestPath = [System.IO.Path]::Combine($loc, $edition, "Common7", "IDE", "CommonExtensions", "Microsoft", "TestWindow", "vstest.console.exe")
          Break search;
        }
        If ([System.IO.File]::Exists($loc32)) {
          $msBuildPath = $loc32
          $vstestPath = [System.IO.Path]::Combine($loc, $edition, "Common7", "IDE", "CommonExtensions", "Microsoft", "TestWindow", "vstest.console.exe")
          Break search;
        }
      }
    }
  }
}

If ($vstestPath -eq $undefined) {
  "Could not locate vstest.console.exe, ABORTING ..."
  Return
}

If(-not [System.IO.File]::Exists($vstestPath)) {
  "vstest.console.exe is not found at $vstestPath, ABORTING ..."
  Return
}

$testcontainer = "HeuristicLab.Tests.dll"
$input = Read-Host "Which container to test? [$($testcontainer)]"
$testcontainer = ($testcontainer, $input)[[bool]$input]

$testplatform = "x64"
$input = Read-Host "Which platform to run the tests? [$($testplatform)]"
$testplatform = ($testplatform, $input)[[bool]$input]

$testcategory = "Essential"
$input = Read-Host "Which category do you want to run? [$($testcategory)]"
$testcategory = ($testcategory, $input)[[bool]$input]

# query whether to build
$input = Read-Host "Should the tests be rebuilt y/N?"
$input = ([string]("n", $input)[[bool]$input]).ToLowerInvariant()
$dobuild = $input -eq "y"

If($dobuild) {
  $curPath = $MyInvocation.MyCommand.Path
  $curDir = Split-Path $curPath

  $slnFiles = Get-ChildItem $curDir -Filter *Tests.sln

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
}

& $vstestPath "bin\$testcontainer" /Framework:framework40 /Platform:$testplatform /TestCaseFilter:"TestCategory=$testcategory"

""

Write-Host -NoNewline "Press any key to continue ... "

[void][System.Console]::ReadKey($true)

# SIG # Begin signature block
# MIIRPwYJKoZIhvcNAQcCoIIRMDCCESwCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCBtCktnwnVbhfUW
# rrODW7qXn4ckTNXv9s/As4v50Ft51aCCDo4wggbaMIIEwqADAgECAhNoAAE6ILAh
# JeBjaP8pAAEAATogMA0GCSqGSIb3DQEBCwUAMD4xEjAQBgoJkiaJk/IsZAEZFgJh
# dDEVMBMGCgmSJomT8ixkARkWBWZob29lMREwDwYDVQQDEwhGSE9PZUNBMTAeFw0x
# OTExMDYwOTI1NTJaFw0yNDExMDQwOTI1NTJaMD4xFjAUBgNVBAsTDUZ1RSBIYWdl
# bmJlcmcxJDAiBgNVBAMTG0ZIIE9PRSBTdHVkaWVuYmV0cmllYnMgR21iSDCCASIw
# DQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBANCook/YMdRq1PdL6DPB2ktDaZ5U
# 5+fP9EwD+fjS5CgpsSy8U54tFwKwAohaBl4SAsobOEDwTHFqn0g2SWrj/Kct+pAB
# a+1lFOFV6YsV5rj+ldPDpQde8bXO+XWTN/6+9zd//Xrxem6Zk1ObeDOrsOUr9PLT
# JIYAWN5Iwp2ziKmsgxBxJHx5FU7pTgB/RhZ2gqu+w5uPZEg8rtUQqwvaNREmIEGg
# XStODc1yLzcEx1VyWA1DUmdB6e4LLIh/NwYiOHrObdazS9IsI1DOD1jcTScBL5je
# CAZF+smlJGw6GJXztgRoU5d2IcpVwpzCOBbNfkuxwbJRrjGyp0kPFxWm5zECAwEA
# AaOCAs8wggLLMD0GCSsGAQQBgjcVBwQwMC4GJisGAQQBgjcVCIXvr0qBqfofgtGT
# A93ie4P1wUaBMoebr3KF1uNaAgFkAgEDMBMGA1UdJQQMMAoGCCsGAQUFBwMDMAsG
# A1UdDwQEAwIHgDAMBgNVHRMBAf8EAjAAMBsGCSsGAQQBgjcVCgQOMAwwCgYIKwYB
# BQUHAwMwHQYDVR0OBBYEFMi6j7BVQzhTjLBM6ivt63QrHvPuMB8GA1UdIwQYMBaA
# FOGlT0Cc05JYLnOIQcSeAsDFJDXFMIH1BgNVHR8Ege0wgeowgeeggeSggeGGgbBs
# ZGFwOi8vL0NOPUZIT09lQ0ExKDEpLENOPWZob29lY2ExLENOPUNEUCxDTj1QdWJs
# aWMlMjBLZXklMjBTZXJ2aWNlcyxDTj1TZXJ2aWNlcyxDTj1Db25maWd1cmF0aW9u
# LERDPWZob29lLERDPWF0P2NlcnRpZmljYXRlUmV2b2NhdGlvbkxpc3Q/YmFzZT9v
# YmplY3RDbGFzcz1jUkxEaXN0cmlidXRpb25Qb2ludIYsaHR0cDovL3BraS5zcnYu
# Zmgtb29lLmF0L3BraS9GSE9PZUNBMSgxKS5jcmwwggEDBggrBgEFBQcBAQSB9jCB
# 8zCBpAYIKwYBBQUHMAKGgZdsZGFwOi8vL0NOPUZIT09lQ0ExLENOPUFJQSxDTj1Q
# dWJsaWMlMjBLZXklMjBTZXJ2aWNlcyxDTj1TZXJ2aWNlcyxDTj1Db25maWd1cmF0
# aW9uLERDPWZob29lLERDPWF0P2NBQ2VydGlmaWNhdGU/YmFzZT9vYmplY3RDbGFz
# cz1jZXJ0aWZpY2F0aW9uQXV0aG9yaXR5MEoGCCsGAQUFBzAChj5odHRwOi8vcGtp
# LnNydi5maC1vb2UuYXQvcGtpL2Zob29lY2ExLmZob29lLmF0X0ZIT09lQ0ExKDEp
# LmNydDANBgkqhkiG9w0BAQsFAAOCAgEAO1g1XWnosz2Qrm2Qt6wruKNn/Cd1f/a2
# QZjtzCmTPLGWWFw/kX7hPwrjsfojZoMWT+17ogBB7LVLQG90DwzPVFtAmyPdIJ8/
# tbu1nu+kRR6qyC3iPW8RZetjT5FAyxHSPNvyI1INXenHmShwkG1hjvjVOGiBECh3
# bfrY95qbZNdBpYee9xqEGlOj/LkRokM+hTlj65MgkFHGwPkiR6AZhnC6tVg3j2wX
# Y+0yaYiXc+iNWOLmQDVwxBmD5bEzToH7EoTsCNcxkatvNzwapdyyz8ycENqyVAGQ
# DxcEf2xNKZQU6PV1xJC8ltfpL4fL2MVQQ35VazMKGm+FeY5Jw+z7tph7Yxseb7KP
# wy4iKY9GAt7bEIJSSUNX0ni/MoWULsdU3EQDPedMS5v3I5C+7GfDJiThq+b2xSm3
# UIImrG+saST3hmC0fM3iffoujzn1tFm2POnNtIwL1X165quLCRhs4YHm57CxRcei
# LfETrOL3Qd08w1nJVnsEvppqe4NauzdO4jPbxsSFCse10xO/2TW1C/ugraXv1fSU
# HlYpUtt+wGhFV75QspOgUgWGc3KBMYm1XwjTWRwBZUd3l7S3UP6IqHaRu3I7eBsF
# OUArTCtXSK+szMW+1G1gIoT61ZyxemUjg9GuF7qIRwlt+Bax4Jy1aZ1TsmV0z1SS
# nBxuiKeBO1MwggesMIIFlKADAgECAhMeAAAABpSzaQpcq/lwAAEAAAAGMA0GCSqG
# SIb3DQEBCwUAMBcxFTATBgNVBAMTDEZIT09lUm9vdENBMTAeFw0xNTA1MDcxMjIy
# MzFaFw0yNzA1MDcxMjMyMzFaMD4xEjAQBgoJkiaJk/IsZAEZFgJhdDEVMBMGCgmS
# JomT8ixkARkWBWZob29lMREwDwYDVQQDEwhGSE9PZUNBMTCCAiIwDQYJKoZIhvcN
# AQEBBQADggIPADCCAgoCggIBAMA7PbbPxLUYOLUThQt0UWtwpT5rT1ZIRinD/wXG
# asjsvxTEsWDG6kf0tLyLlf64zNOhtXG5pbidu+rcmdnlut58h9So8QuZELdb8xbS
# 9rM3oTC9wKGW57oRydxIo7WkQwX/IZHefQf2X8AVp0Sb+kCvedUee1/GLIpfef1Y
# SvIxYURxI5xM7wcnRPPrWDWF+qvKe5miC8DiJ4+xAWqwbXwUUdIq9DUk9JL+2waq
# GfRodEpMZu/2KlYb6X4z9ZHFgq5OmW+g/WsvyZVQmuXuW0049yP6y79upgFitSrA
# VeJhEumheU58NC6VBAFvhyCwiR0R1brkpW5s5UP4A3nv1KsVTP12uU7kdWkqwhSa
# 6OMrga5H4ixHYLIpVqQ4j3K1qHNZfxBtVs5x6VhBhPOxT4uFTzFXiX1VZZK1bH2k
# K6jV0Hvyr/DzUNlwED2mZ8K7PwPbiDJyemvyvSeEvV/Nk12q8rSM308fVXy2Pkm5
# LuAxgNejOBNSmH828IR3Zd2TGgRYfGFklfNpAEOBNgz287/saMkNr/BqTrWFkuD8
# sevpa93bCo7OhWUgVNiVWjSdL/J3aj9cX3lu/M0nv06Mi1WdexUcstgxS8nY3ypP
# +WRbbmmll7+u/8udk+07Wm+DbQMQ9kS3ElnYgSCu93N/yVoR/lpGAdPkuns0lPHM
# ju3bAgMBAAGjggLIMIICxDASBgkrBgEEAYI3FQEEBQIDAQABMCMGCSsGAQQBgjcV
# AgQWBBR6W9jjcxJzbDG3JxXfKJnYNFE5gjAdBgNVHQ4EFgQU4aVPQJzTklguc4hB
# xJ4CwMUkNcUwGQYJKwYBBAGCNxQCBAweCgBTAHUAYgBDAEEwCwYDVR0PBAQDAgGG
# MBIGA1UdEwEB/wQIMAYBAf8CAQAwHwYDVR0jBBgwFoAUUam+2+0DBpS2t/5JT7W9
# I1iYA9YwggEBBgNVHR8EgfkwgfYwgfOggfCgge2GgbhsZGFwOi8vL0NOPUZIT09l
# Um9vdENBMSgxKSxDTj1maG9vZXJvb3RjYTEsQ049Q0RQLENOPVB1YmxpYyUyMEtl
# eSUyMFNlcnZpY2VzLENOPVNlcnZpY2VzLENOPUNvbmZpZ3VyYXRpb24sREM9Zmhv
# b2UsREM9YXQ/Y2VydGlmaWNhdGVSZXZvY2F0aW9uTGlzdD9iYXNlP29iamVjdENs
# YXNzPWNSTERpc3RyaWJ1dGlvblBvaW50hjBodHRwOi8vcGtpLnNydi5maC1vb2Uu
# YXQvcGtpL0ZIT09lUm9vdENBMSgxKS5jcmwwggEGBggrBgEFBQcBAQSB+TCB9jCB
# qAYIKwYBBQUHMAKGgZtsZGFwOi8vL0NOPUZIT09lUm9vdENBMSxDTj1BSUEsQ049
# UHVibGljJTIwS2V5JTIwU2VydmljZXMsQ049U2VydmljZXMsQ049Q29uZmlndXJh
# dGlvbixEQz1maG9vZSxEQz1hdD9jQUNlcnRpZmljYXRlP2Jhc2U/b2JqZWN0Q2xh
# c3M9Y2VydGlmaWNhdGlvbkF1dGhvcml0eTBJBggrBgEFBQcwAoY9aHR0cDovL3Br
# aS5zcnYuZmgtb29lLmF0L3BraS9maG9vZXJvb3RjYTFfRkhPT2VSb290Q0ExKDEp
# LmNydDANBgkqhkiG9w0BAQsFAAOCAgEAY3YxOVWln3QrOR5q01Uv7YHi0EI3dIIi
# qk0f1TW1694O/Q6Rm2vpBeP+jrZXLtyyivgBdx0Vc2UPjvjj5v/uImqjVvnUUCjU
# PYcV00UYT6pEzwXHu82rRwbaF8KreNzUIliJlrwG9sJ0TvnpNObsY0SwFElUhq/g
# cFRDJ38yVNPBOEDtKm+VlYF09LWs9xVw8JZk8K3xYSSlXy9sZQqTklFlL7dsb75h
# MYJEzohQgbr0mfwINEi52DcwU4D6IqPv6HABC3L5hcr9g1eSQ6HLPb0NjIWYaSHg
# ESWlRrhrM6nEygoAfIJMtUj0h7IUvV/FhkxpDHpcy4Y4mSOvc4F8uMJBSrn605IT
# rH1bdTXd4U3F89MOkWuJQD+JAiISzWeRCZNl9ID0fOM4Cn7bbO9wFZEpHgdoOaCM
# kcwi9SMQ2pzceTiyGshmMfT+Qd1haqUk2KX9ZlDms46rfGZj+RyB1IZg5nkYJ/hC
# /lgU+M4fWa1vrGJuYtSf0UwBd9qdP6MbWEtFTj0d0LCPOA5Pw7p2m1p3HEMI6mcX
# 4Fi+KO6CIvlX+SKrZwIattLO3VQZqV68/W3fmyC2FYwfVUU/InwIDGx3hXR3i+zr
# t41t11bnp8B4Bhq7PXKgUEHuX0gcNE0o62HstYOf/U5HkCM+Ag43gFrR/RDwMq6L
# 7VxKws52ao8xggIHMIICAwIBATBVMD4xEjAQBgoJkiaJk/IsZAEZFgJhdDEVMBMG
# CgmSJomT8ixkARkWBWZob29lMREwDwYDVQQDEwhGSE9PZUNBMQITaAABOiCwISXg
# Y2j/KQABAAE6IDANBglghkgBZQMEAgEFAKCBhDAYBgorBgEEAYI3AgEMMQowCKAC
# gAChAoAAMBkGCSqGSIb3DQEJAzEMBgorBgEEAYI3AgEEMBwGCisGAQQBgjcCAQsx
# DjAMBgorBgEEAYI3AgEVMC8GCSqGSIb3DQEJBDEiBCDDzy6RhRdWhlEvoZ9lSBH8
# 1bgdAp3kSHSn3I9JzPC1gzANBgkqhkiG9w0BAQEFAASCAQCKOmizxpz/TOCEhRPY
# f1KVbo9UBYP3YT7g/TNdUCcI0ITkyQkXI2dowJACBzAyb50EqARONPobf81koPhh
# /AW0jvak1bXrLRHE6DwitNCBLwTLNkZ2Zn3yUBjB4OwIGyUoX+4af9hC9+G6eoTt
# aE5vbVnZOZBsultoeKpso5QgL5CPEnqkMXsV1p3MCF2ryD5lHKLEHXvF4A3bfRkz
# y8sVKscEuRjOMhvCx2qjNLpRFGf5Ppd6hzCIw/e67c/nGUCh6GflK9UfZanW4Gn7
# NQ9KUd9NZXJ8Bo3bEU9Co51zGXQY18atrTX0AaYizZ/MBCtYOU7x15BtQzraFAiL
# PHJ6
# SIG # End signature block
