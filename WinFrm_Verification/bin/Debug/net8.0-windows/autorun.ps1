# Define dependencies
$dotnetRuntimeUrl = "https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-7.0.0-windows-x64-installer"
$dependencyInstallerPath = "$PSScriptRoot\Dependencies"

# Ensure the dependencies directory exists
if (-not (Test-Path -Path $dependencyInstallerPath)) {
    New-Item -ItemType Directory -Path $dependencyInstallerPath
}

# Check if .NET Runtime is installed
$dotnetInstalled = Get-Command "dotnet" -ErrorAction SilentlyContinue
if (-not $dotnetInstalled) {
    Write-Host "Downloading and installing .NET Runtime..."
    $runtimeInstaller = Join-Path -Path $dependencyInstallerPath -ChildPath "dotnet-runtime-installer.exe"
    Invoke-WebRequest -Uri $dotnetRuntimeUrl -OutFile $runtimeInstaller
    Start-Process -FilePath $runtimeInstaller -ArgumentList "/quiet /norestart" -Wait
    Write-Host ".NET Runtime installed."
} else {
    Write-Host ".NET Runtime is already installed."
}

# Add other dependencies here
Write-Host "All dependencies are installed."

# Get the directory where the script resides
$scriptDirectory = $PSScriptRoot

# Build the path to the .exe file
$programPath = Join-Path -Path $scriptDirectory -ChildPath "WinFrm_Verification.exe"

# Check if the .exe file exists
if (Test-Path $programPath) {
    # Start the program
    Start-Process -FilePath $programPath
    Write-Host "Program started successfully."
} else {
    Write-Host "Error: Program not found at $programPath"
}
