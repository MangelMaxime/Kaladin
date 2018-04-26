#r "paket: groupref netcorebuild //"
#load ".fake/build.fsx/intellisense.fsx"

#nowarn "52"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.JavaScript

module Fable =

    open System

    let exec ((buildOptions: DotNet.Options -> DotNet.Options)) command args (event : Event<string>) =
        let results = new System.Collections.Generic.List<Fake.Core.ConsoleMessage>()
        let timeout = TimeSpan.MaxValue

        let mutable serverStarted = false

        let errorF msg =
            Trace.traceError msg
            results.Add (ConsoleMessage.CreateError msg)

        let messageF (msg : string) =
            if not serverStarted && msg.Contains("daemon started on port") then
                serverStarted <- true
                event.Trigger(msg.Substring(msg.IndexOf("port") + 5))

            Trace.trace msg
            results.Add (ConsoleMessage.CreateOut msg)

        let options = buildOptions (DotNet.Options.Create())
        let cmdArgs = sprintf "%s %s" command args

        let result =
            let f (info:ProcStartInfo) =
                let dir = System.IO.Path.GetDirectoryName options.DotNetCliPath
                let oldPath =
                    match options.Environment |> Map.tryFind "PATH" with
                    | None -> ""
                    | Some s -> s
                { info with
                    FileName = options.DotNetCliPath
                    WorkingDirectory = options.WorkingDirectory
                    Arguments = cmdArgs }
                |> Process.setEnvironment options.Environment
                |> Process.setEnvironmentVariable "PATH" (sprintf "%s%c%s" dir System.IO.Path.PathSeparator oldPath)

            // if options.RedirectOutput then
            Process.execRaw f timeout true errorF messageF
            // else Process.execSimple f timeout
        ProcessResult.New result (results |> List.ofSeq)

Target.create "Clean" (fun _ ->
    !! "src/bin"
    ++ "src/obj"
    ++ "output"
    |> Seq.iter Shell.cleanDir
)

Target.create "Install" (fun _ ->
    DotNet.restore
        (DotNet.Options.withWorkingDirectory __SOURCE_DIRECTORY__)
        "Kaladin.sln"
)

Target.create "YarnInstall" (fun _ ->
    Yarn.install id
)

Target.create "Build" (fun _ ->
    let result =
        DotNet.exec
            (DotNet.Options.withWorkingDirectory __SOURCE_DIRECTORY__)
            "fable"
            "webpack --port free -- -p"

    if not result.OK then failwithf "dotnet fable failed with code %i" result.ExitCode
)

Target.create "Watch" (fun _ ->
    let fableStarted = Event<string>()

    // [

    //     async {
    //         Yarn.exec "fable-splitter -c src/splitter.config.js" id
    //     }
    // ]

    [
        async {
            let! fablePort = Async.AwaitEvent fableStarted.Publish
            Yarn.exec ("fable-splitter -c src/splitter.config.js -w --port " + fablePort) id
        }

        async {
            let result =
                Fable.exec
                    (DotNet.Options.withWorkingDirectory __SOURCE_DIRECTORY__)
                    "fable"
                    "start"
                    fableStarted

            if not result.OK then failwithf "dotnet fable failed with code %i" result.ExitCode
        }
    ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore

)

// Build order
"Clean"
    ==> "Install"
    ==> "YarnInstall"
    ==> "Build"

// "Watch"
//     <== [ "YarnInstall" ]

// start build
Target.runOrDefault "Build"
