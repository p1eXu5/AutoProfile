#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target //"
#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.DotNet.NuGet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.initEnvironment ()

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ "packages"
    |> Shell.cleanDirs 
)


Target.create "Build" (fun _ ->
    let buildConfiguration (defaults: DotNet.BuildOptions) =
        { defaults with
            Configuration = DotNet.Release
        }

    !! "src/**/*.*proj"
    |> Seq.iter (DotNet.build buildConfiguration)
)



Target.create "Test" (fun _ ->
    let testConfiguration (defaults: DotNet.TestOptions) =
        { defaults with
            Configuration = DotNet.Release
        }

    !! "test/**/*.*proj"
    |> Seq.iter (DotNet.test testConfiguration)
)


Target.create "Pack" (fun _ ->
    let packConfiguration (defaults: DotNet.PackOptions) =
        { defaults with
              Configuration = DotNet.Release
              OutputPath = Some "./packages"
              IncludeSymbols = true 
        }

    !! "src/p1eXu5.AutoProfile/*.*proj"
    |> Seq.iter (DotNet.pack packConfiguration)
)

Target.create "Push" (fun _ ->
    let setNugetPushParams (defaults: NuGet.NuGet.NuGetPushParams) =
        { defaults with
            DisableBuffering = true
            ApiKey = Some "oy2duoockoilw77emehaxmvhdptuu2funex4on3j7jefx4"
            Source = Some "https://api.nuget.org/v3/index.json"
         }

    let setParams (defaults:DotNet.NuGetPushOptions) =
            { defaults with
                PushParams = setNugetPushParams defaults.PushParams
            }

    !! "./packages/*.nupkg"
    |> Seq.iter (DotNet.nugetPush setParams)
)


Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "Test"
  ==> "Pack"
  ==> "Push"
  ==> "All"

Target.runOrDefault "All"
