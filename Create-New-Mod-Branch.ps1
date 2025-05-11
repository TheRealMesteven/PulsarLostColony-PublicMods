param(
    [string]$ModName,
    [string]$ModDescription = "Default mod description",
    [string]$ModAuthor = "Default author",
    [string]$RootPath = ".",
    [string]$TemplatePath = ".\Template"
)

# Ask for Mod Name if not provided
if (-not $ModName) {
    $ModName = Read-Host "Enter the mod name"
    $ModDescription = Read-Host "Enter the mod description"
    $ModAuthor = Read-Host "Enter the author name"
}
$ModGUID = [guid]::NewGuid()
Write-Host "Generated GUID: $ModGUID"

# Convert to absolute paths
$RootPath = [System.IO.Path]::GetFullPath($RootPath)
$TemplatePath = [System.IO.Path]::GetFullPath($TemplatePath)
$WorktreePath = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($RootPath, $ModName))

# Verify solution exists
if (-not (Get-ChildItem -Path $RootPath -Filter "*.sln" | Select-Object -First 1)) {
    Write-Host "No solution file found in $RootPath" -ForegroundColor Red
    exit 1
}

# Verify template exists
if (-not (Test-Path $TemplatePath)) {
    Write-Host "Template path '$TemplatePath' does not exist!" -ForegroundColor Red
    exit 1
}

# Verify base branch
try {
    $BaseBranch = git rev-parse --abbrev-ref HEAD
    if (-not $BaseBranch -or $BaseBranch -eq "HEAD") {
        Write-Host "No valid branch checked out. Commit something first!" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "Error determining current branch: $_" -ForegroundColor Red
    exit 1
}

# Check if branch exists
if (Test-Path $WorktreePath) {
    Write-Host "Branch folder '$WorktreePath' already exists!" -ForegroundColor Red
    exit 1
}

# Create orphan branch and worktree
try {
    $FirstCommit = git rev-list --max-parents=0 HEAD
    git worktree add -b $ModName $WorktreePath $FirstCommit
    Push-Location $WorktreePath
    git commit --allow-empty -m "Initial commit for $ModName"  # Needed for orphan branches
    Pop-Location
} catch {
    Write-Host "Error creating orphan branch or worktree: $_" -ForegroundColor Red
    exit 1
}

Write-Host "Created new orphan branch '$ModName' from '$BaseBranch' at '$WorktreePath'"

# Copy template files
# Copy the Template folder into the worktree
$TemplateCopyPath = Join-Path $WorktreePath "Template"
Write-Host "Copying template folder to '$TemplateCopyPath'"
Copy-Item -Path $TemplatePath -Destination $TemplateCopyPath -Recurse -Force

# Rename 'Template' folder to match ModName
$ModFolderPath = Join-Path $WorktreePath "src"
Rename-Item -Path $TemplateCopyPath -NewName "src" -ErrorAction Stop

# Process files to replace placeholders
$FilesToProcess = Get-ChildItem -Path $WorktreePath -Recurse -Include "*.cs", "*.csproj", "*.sln", "*.txt", "*.json"
Write-Host "Replacing $($FilesToProcess.Count) placeholders in files..."

foreach ($File in $FilesToProcess) {
    try {
        # Read file content
        $Content = [System.IO.File]::ReadAllText($File.FullName)
        
        # Replace placeholders
        $Content = $Content -replace '\[MOD_NAME\]', $ModName
        $Content = $Content -replace '\[MOD_DESCRIPTION\]', $ModDescription
        $Content = $Content -replace '\[MOD_AUTHOR\]', $ModAuthor
        $Content = $Content -replace '\[MOD_GUID\]', $ModGUID
        
        # Write back to file
        [System.IO.File]::WriteAllText($File.FullName, $Content)
        
        # Rename files containing [MOD_NAME]
        if ($File.Name -match '\[MOD_NAME\]') {
            $NewName = $File.Name -replace '\[MOD_NAME\]', $ModName
            $NewPath = Join-Path $File.Directory.FullName $NewName
            Rename-Item -LiteralPath $File.FullName -NewName $NewName -ErrorAction Stop
        }
    } catch {
        Write-Host "Error processing file $($File.FullName): $_" -ForegroundColor Yellow
    }
}

# Add project to .sln file
Write-Host "Adding project to solution file..."

# Find the solution file
$SolutionFile = Get-ChildItem -Path $RootPath -Filter "*.sln" | Select-Object -First 1
if (-not $SolutionFile) {
    Write-Host "Solution file not found in $RootPath" -ForegroundColor Red
    exit 1
}

# Find the .csproj file in the new worktree
$NewProjectFile = Get-ChildItem -Path $WorktreePath -Filter "*.csproj" -Recurse | Select-Object -First 1
if (-not $NewProjectFile) {
    Write-Host "Project file not found in $WorktreePath" -ForegroundColor Red
    exit 1
}

# Relative path from solution to project
$SolutionDir = (Resolve-Path $SolutionFile.Directory.FullName).Path
$ProjectFullPath = (Resolve-Path $NewProjectFile.FullName).Path

# Ensure both paths end with a separator
if (-not $SolutionDir.EndsWith('\')) { $SolutionDir += '\' }

# Compute relative path manually
$RelativePath = $ProjectFullPath -replace [regex]::Escape($SolutionDir), ''

# Escape backslashes for .sln format
$RelativePath = $RelativePath -replace '/', '\'

# Generate a project entry line
$ProjectEntry = @"
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = "$ModName", "$RelativePath", "{$ModGUID}"
EndProject
"@

# Inject the project into the solution file just before the "Global" section
$SlnContent = Get-Content $SolutionFile.FullName
$InsertIndex = $SlnContent.IndexOf("Global")
if ($InsertIndex -eq -1) {
    Write-Host "Could not find 'Global' section in .sln file." -ForegroundColor Red
    exit 1
}

# Insert project entry
$UpdatedSln = @()
$UpdatedSln += $SlnContent[0..($InsertIndex - 1)]
$UpdatedSln += $ProjectEntry
$UpdatedSln += $SlnContent[$InsertIndex..($SlnContent.Count - 1)]
Set-Content -Path $SolutionFile.FullName -Value $UpdatedSln -Encoding UTF8

Write-Host "Project added to solution: $RelativePath"

# Commit and push
Write-Host "Committing changes..."
try {
    Push-Location $WorktreePath
    git add .
    git commit -m "Initial commit for $ModName"
    git push -u origin $ModName
} catch {
    Write-Host "Error committing or pushing changes: $_" -ForegroundColor Red
} finally {
    Pop-Location
}

Write-Host "`nMod '$ModName' has been successfully created!`n"
Write-Host "Project location: $WorktreePath"
Write-Host "Description: $ModDescription"
Write-Host "Author: $ModAuthor"
Write-Host "Branch pushed to: origin/$ModName"
