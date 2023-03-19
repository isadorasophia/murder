<#
    .SYNOPSIS
    Script to automate the engine documentation.

    .DESCRIPTION
    This relies on mdBook in order to build the contents from a .xml output of the project file.
    The .xml file will be parsed to .md, which will be converted into the website format.
    Complicated, right? But it's pretty!

    .PARAMETER mdPath
    The path to the .md files root directory. If none, this will assume a default relative path.

    .PARAMETER version
    mdBook version to pull from the release website.

    .PARAMETER action
    The desired action. The actions available are 'build' and 'deploy'.

    .EXAMPLE
    PS> ./publish.ps1 -mdPath c:\my-own-path\root

    .EXAMPLE
    PS> ./publish.ps1 -action deploy

    .LINK
    https://rust-lang.github.io/mdBook/index.html
#>
param([System.String]$mdPath,
      [System.String]$mdBookVersion="v0.4.28",
      [System.String]$action="build")

if ($args[0] -cmatch "^-h" -or $mdPath -eq "help")
{
    Get-Help $PSCommandPath
    exit
}

if (-not $mdPath)
{
    $mdPath = Join-Path -Path $PSScriptRoot -ChildPath "../docs"
}

# Check if the path is valid!
if (!(Test-Path -Path $mdPath))
{
    $message += "'$mdPath' is not a valid path! Please specify a valid path to the markdown root folder."

    Write-Error -Message $message -Category InvalidArgument
    exit
}

$bin = Join-Path -Path $PSScriptRoot -ChildPath "bin"
$mdBook = Join-Path -Path $bin -ChildPath "mdbook.exe"

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

if (!(Test-Path -Path $mdBook))
{
    Write-Output "Downloading mdBook from source..."

    if (!(Test-Path -Path $bin))
    {
        mkdir $bin
    }

    # Invoke-WebRequest https://github.com/rust-lang/mdBook/releases/download/$version/mdbook-$version-x86_64-pc-windows-msvc.zip -OutFile $zip
    # Expand-Archive $zip -DestinationPath $bin
    # Remove-Item $zip
    Write-Output "Installing dependencies..."

    $mdBookZip = Join-Path -Path $bin -ChildPath "mdbook.zip"
    Download-And-Unzip "https://github.com/rust-lang/mdBook/releases/download/$mdBookVersion/mdbook-$mdBookVersion-x86_64-pc-windows-msvc.zip" $mdBookZip
}

switch ($action)
{
    "build"
    {
        Write-Output "Building from $mdPath"
        &$mdBook build $mdPath
    }
    "deploy"
    {
        Write-Output "Deploying website!"
        &$mdBook serve --open $mdPath
    }
}
