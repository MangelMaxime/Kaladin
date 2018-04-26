#r "paket: groupref netcorebuild //"
#load ".fake/build.fsx/intellisense.fsx"

#nowarn "52"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.JavaScript

Target.create "Clean" (fun _ ->
    !! "src/bin"
    ++ "src/obj"
    ++ "src/build"
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

/// Provides a stereotypical JavaScript-like "debounce" service for events.
/// Set initialBounce to true to cause a inject a bounce when first the debouncer is first constructed.
type Debounce(timeout, initialBounce, fn) as self =
    let debounce fn timeout = MailboxProcessor.Start(fun agent ->
        let rec loop ida idb = async {
            let! r = agent.TryReceive(timeout)
            match r with
            | Some _ -> return! loop ida (idb + 1)
            | None when ida <> idb -> fn (); return! loop idb idb
            | None -> return! loop ida idb
        }
        loop 0 0)

    let mailbox = debounce fn timeout
    do if initialBounce then self.Bounce()

    /// Calls the function, after debouncing has been applied.
    member __.Bounce() = mailbox.Post(null)

let generateHtml _ =
    let timeout = System.TimeSpan.FromMinutes 1.

    let result =
        Process.execSimple
            (fun p ->
                { p with FileName = "node"
                         Arguments = "src/build/App.js" })
            timeout

    if result <> 0 then
        Trace.traceError "HTML generation: Failed"
    else
        Trace.trace "HTML generation: Succeed"

Target.create "Watch" (fun _ ->

    let debouncer =
        Debounce(800, false, generateHtml)

    // Make sure the directory exist for the watcher
    Directory.ensure "src/build/"

    use watcher =
        !! "src/build/**/*.js"
        |> ChangeWatcher.run
            (fun _ -> debouncer.Bounce())

    [
        async {
            let result =
                DotNet.exec
                    (DotNet.Options.withWorkingDirectory __SOURCE_DIRECTORY__)
                    "fable"
                    "yarn-run fable-splitter --port free -- -c src/splitter.config.js -w"

            if not result.OK then failwithf "dotnet fable failed with code %i" result.ExitCode
        }

        async {
            Yarn.exec "node-sass --output-style compressed --watch --output docs/ src/scss/style.scss" id
        }
        async {
            Yarn.exec
                "live-server --port=8000 --watch=docs"
                (fun o ->
                    { o with WorkingDirectory = __SOURCE_DIRECTORY__ </> "docs" })
        }
    ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore

    watcher.Dispose()
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
