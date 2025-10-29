$projectPath = "$(Split-Path -Parent "$PSScriptRoot")"

$process = Start-UnityEditor `
    -Project "$projectPath" `
    -BuildTarget WebGL `
    -ExecuteMethod Pianola.Editor.Builder.BuildWebGL `
    -LogFile "${projectPath}/build.log" `
    -BatchMode `
    -PassThru `
    -Quit

echo "$($process.Id)"
