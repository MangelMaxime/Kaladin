module App.View

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.FontAwesome
open Fable.PowerPack
open Fable.PowerPack.Fetch

type LandingColumn =
    { Title : string
      Description : string }

type LandingConfig =
    { ProjectDescription : string
      Columns : LandingColumn list }

type PageConfig =
    { Title : string
      Path : string }

type DocConfig =
    { Title : string
      Children : PageConfig list }


type Page =
    | Landing of LandingConfig
    | OtherPage

type Config =
    { ProjectName : string
      Pages : Page list }

let tryFindLanding (pages : Page list) =
    pages
    |> List.tryFind (fun page ->
        match page with
        | Landing _ -> false
        | _ -> true
    )

let renderLanding (projectName : string) (config : LandingConfig) =
    Fable.Import.JS.console.log config

let kaladin (siteConfig: Config) =
    for page in siteConfig.Pages do
        match page with
        | Landing pageConfig -> renderLanding siteConfig.ProjectName pageConfig
        | OtherPage -> Fable.Import.JS.console.log "other page"



kaladin
    { ProjectName = "Kaladin"
      Pages =
        [
            Landing
                { ProjectDescription = "Opiniated documentation sites"
                  Columns = [ ] }
        ]
    }
