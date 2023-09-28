<#
    .SYNOPSIS
    Script to convert xml documentation to markdown.

    .DESCRIPTION
    This relies on dotnet2md in order to convert our xml files and metadata into markdown files.

    .PARAMETER xmlPath
    The path to the .xml files root directory. If none, this will assume a default relative path.

    .PARAMETER outPath
    Path to the output directory. If none, this will assume a default relative path.

    .PARAMETER targets
    Target assemblies to build the documentation for.  If none, this will assume a set of default assemblies.

    .PARAMETER version
    Target version of the dotnet2md tool.

    .EXAMPLE
    PS> ./dotnet2md.ps1 -xmlPath c:\my-own-path\root

    .LINK
    https://github.com/isadorasophia/dotnet2md
#>
param([System.String]$xmlPath,
      [System.String]$outPath,
      [System.String]$targets="bang murder",
      [System.String]$version="v0.2.6")

if ($args[0] -cmatch "^-h" -or $xmlPath -eq "help")
{
    Get-Help $PSCommandPath
    exit
}

if (-not $xmlPath)
{
    $xmlPath = Join-Path -Path $PSScriptRoot -ChildPath "../src/Murder/bin/Debug/net7.0/publish"
}

if (-not $outPath)
{
    $outPath = Join-Path -Path $PSScriptRoot -ChildPath "../docs/src"
}

# Check if the path is valid!
if (!(Test-Path -Path $xmlPath))
{
    $message += "'$xmlPath' is not a valid path! Please specify a valid path to the markdown root folder."

    Write-Error -Message $message -Category InvalidArgument
    exit
}

$bin = Join-Path -Path $PSScriptRoot -ChildPath "bin"
$parser = Join-Path -Path $bin -ChildPath "parser.exe"

function Download-And-Unzip
{
    param(
        [Parameter (Mandatory = $true)] [String]$url,
        [Parameter (Mandatory = $true)] [String]$name
    )

    Invoke-WebRequest $url -OutFile $name
    Expand-Archive $name -DestinationPath $bin
    Remove-Item $name
    Write-Output "Finish downloading $name!"
}

if (!(Test-Path -Path $parser))
{
    Write-Output "Downloading parser from source..."

    if (!(Test-Path -Path $bin))
    {
        mkdir $bin
    }

    Write-Output "Installing dependencies..."

    $parserZip = Join-Path -Path $bin -ChildPath "parser.zip"
    Download-And-Unzip "https://github.com/isadorasophia/dotnet2md/releases/download/$version/dotnet2md-parser-$version-win-x64.zip" $parserZip
}

Write-Output "Building xml documentation from '$xmlPath' to '$outPath' targeting '$targets'"
$output = &$parser $xmlPath $outPath $targets
Write-Output $output