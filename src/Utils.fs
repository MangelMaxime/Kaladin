module Utils

open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Node
open Fable.Import.Node.Globals

let rec private ensureDirExists (dir: string) (cont: (unit->unit) option) =
    if Exports.fs.existsSync !^dir then
        match cont with Some c -> c() | None -> ()
    else
        ensureDirExists (Exports.path.dirname dir) (Some (fun () ->
            if not(Exports.fs.existsSync !^dir) then
                Exports.fs?mkdirSync(dir) |> ignore
            match cont with Some c -> c() | None -> ()
        ))

let writeFile (path: string) (content: string) =
    ensureDirExists (Exports.path.dirname path) None
    Exports.fs.writeFileSync(path, content)

let readFile (path: string) =
    Exports.fs.readFileSync(path).toString()
