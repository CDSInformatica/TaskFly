 $rootDir = $env:APPVEYOR_BUILD_FOLDER
 $buildNumber = $env:APPVEYOR_BUILD_NUMBER
 $solutionFile = "$rootDir\TaskFly.Integra\TaskFly.Integra.csproj"
 $srcDir = "$rootDir\TaskFly.Integra"
 $slns = ls "$rootDir\*.csproj"
 $packagesDir = "$rootDir\packages"
 $nuspecPath = "$rootDir\TaskFly.Integra\TaskFly.Integra.nuspec"
 $nugetExe = "$packagesDir\NuGet.CommandLine.2.8.5\tools\NuGet.exe"
 $nupkgPath = "$rootDir\TaskFly.Integra\NuGet\TaskFly.Integra.{0}.nupkg"

foreach($sln in $slns) {
   nuget restore $sln
}

[xml]$xml = cat $nuspecPath
$xml.package.metadata.version+=".$buildNumber"
$xml.Save($nuspecPath)

[xml]$xml = cat $nuspecPath
$nupkgPath = $nupkgPath -f $xml.package.metadata.version

nuget pack $nuspecPath -properties "Configuration=$env:configuration;Platform=AnyCPU;Version=$($env:appveyor_build_version)" -OutputDirectory $srcDir 
appveyor PushArtifact $nupkgPath
