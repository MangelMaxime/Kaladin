module App.View

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.FontAwesome
open Fable.Import

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

let navbar (config : Config) =
    Navbar.navbar [ Navbar.IsFixedTop ]
        [ Container.container [ ]
            [ Navbar.Brand.div [ ]
                [ Navbar.Item.a [ ]
                    [ Heading.p [ Heading.Is4 ]
                        [ str config.ProjectName ] ] ] ] ]

let htmlPage (config : Config) content =
    html [ Class "has-navbar-fixed-top" ]
        [ head [ ]
            [ link [ Rel "stylesheet"
                     Type "text/css"
                     Href "style.css" ] ]
          body [ ]
            [ navbar config
              content ] ]

let renderLanding (projectName : string) (config : LandingConfig) =
    Hero.hero [ Hero.IsFullHeight ]
        [ Hero.body [ ]
            [ str config.ProjectDescription ] ]

let kaladin (siteConfig: Config) =
    for page in siteConfig.Pages do
        match page with
        | Landing pageConfig ->
            renderLanding siteConfig.ProjectName pageConfig
            |> htmlPage siteConfig
            |> ReactDomServer.renderToStaticMarkup
            |> Utils.writeFile "docs/index.html"

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
