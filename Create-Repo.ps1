$URL = Read-Host "Enter the URL to clone"
echo "This is a mod repository where each branch is a different mod for the game. Its generated by the current branches scripts and template." >> README.md
git init
git add README.md
git commit -m "Init"
git branch -M Home
git remote add origin $URL
git push -u origin Home
### Delete README if it exists (both from disk and git)
##$readmeFiles = Get-ChildItem -Path $RootPath -Filter "readme*" -File

##foreach ($file in $readmeFiles) {
    ##Write-Host "Removing $($file.Name) from repo and disk..."
    ##git rm --cached $file.FullName 2>$null  # Untrack from git if already tracked
    ##Remove-Item $file.FullName -Force
##}

### Commit and push deletion
##if ($readmeFiles.Count -gt 0) {
    ##git commit -m "Init Complete"
    ##git push
##}