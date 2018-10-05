# PowerShell zu updatealllbaseclients.cmd
# Erstellt: wian 20180417
# wozu    : wenn die lbase Setupdatei existiert, ist der lbase Build erfolgreich gewesen

<# Parameter:
    LBaseVersion=Main
    LBaseVersion=Release_7.2_DEV
    LBaseVersion=Release_7.1_DEV
    LBaseVersion=Release_6.4_DEV
#>

param
(
[string]$LBaseVersion="Main"
)

$title=echo lbaseClientUpdate $LBaseVersion do not close!
# Write-Host "hello"
$Host.UI.RawUI.WindowTitle = $title
<#
$h = (ls \\tfdbntfs02\builds\LBASE\Main\|sort -property lastwritetime |Select-Object -Last 1) -split ' '
robocopy \\tfdbntfs02\builds\LBASE\Main\$h\x86\Release c:\lbaseinternal\lbase7\ /mir /XF lbase.ini
#>

# suche das Verzeichnis, in dem die Setupdatei lbase.setup.msi vorhanden ist
$file = 'lbase.setup.msi'
# $lbaseV = $LBaseVersion
echo "Uebergebener Parameter: $LBaseVersion"
$s_path = '\\tfdbntfs02\builds\LBASE\'+$LBaseVersion+'\' <# SearchSourcePath #>
if ((echo $LBaseVersion) -match "Main") {
    # echo "MAIN"
    $t_path = 'c:\lbaseinternal\lbase7\' <# TargetPath #>
    echo "Target Path: $t_path"
}
else
{
    # echo "NICHT MAIN"
    $lv = (echo $LBaseVersion) -split '_'| Select-Object -Last 2|Select-Object -First 1 
    $t_path = 'C:\lbaseinternal\lbase-'+$lv <# TargetPath #>
    echo "Target Path: $t_path"

}

echo "Suche nach vorhandener Setup-Datei $file unter $LBaseVersion"
# Ergebnis der Suche in $copy_path
$copy_path = (Get-ChildItem -Path $s_path -Include $file -File -Recurse |sort -property lastwritetime |select DirectoryName|Select-Object -Last 1|Split-Path -Parent |Select-Object -Last 1 ) -split '='|Select-Object -Last 1
if (Test-Path "$copy_path`\release\") {$copy_path="$copy_path`\release\"}
if (Test-Path "$copy_path`\x86\Release\") {$copy_path="$copy_path`\x86\Release\"}
echo Verzeichnis: $copy_path
# ls $files
robocopy $copy_path $t_path /mir /XF lbase.ini $file
# sleep n Sec.
Start-Sleep -s 1