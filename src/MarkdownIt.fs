module MarkdownIt

open Fable.Core
open Fable.Core.JsInterop



type MarkdownIt =
    abstract render : string -> string

type MarkdownItStatic =
    [<Emit("new $0()")>] abstract Create: unit -> MarkdownIt

let markdownIt : MarkdownItStatic = importAll "markdown-it"
