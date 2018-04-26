module Main

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
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

type NavItem =
    { Title : string
      Path : string }

type MarkdownConfig =
    { Title : string
      Source : string }

type Nav =
    | NavItem of NavItem

type Page =
    | Markdown of MarkdownConfig

type Landing =
    | Landing of LandingConfig
    | NoLanding

type Section =
    { Title : string
      Children : Page list }

type Config =
    { ProjectName : string
      Landing : Landing
      Docs : Section list
      Nav : Nav list
      Error404 : React.ReactElement }

// let tryFindLanding (pages : Page list) =
//     pages
//     |> List.tryFind (fun page ->
//         match page with
//         | Landing _ -> false
//         | _ -> true
//     )

let renderNavbarMenu (items : Nav list) =
    items
    |> List.map (
        function
        | NavItem navItem ->
            Navbar.Item.a [ Navbar.Item.Props [ Href navItem.Path ] ]
                [ str navItem.Title ]
    )
    |> Navbar.menu [ ]

let navbar (config : Config) =
    Navbar.navbar [ Navbar.IsFixedTop
                    Navbar.Color IsPrimary ]
        [ Container.container [ ]
            [ Navbar.Brand.div [ ]
                [ Navbar.Item.a [ ]
                    [ Heading.p [ Heading.Is4 ]
                        [ str config.ProjectName ] ] ]
              renderNavbarMenu config.Nav ] ]

let htmlPage (config : Config) content =
    html [ Class "has-navbar-fixed-top" ]
        [ head [ ]
            [ link [ Rel "stylesheet"
                     Type "text/css"
                     Href "/style.css" ]
              title [ ]
                [ str config.ProjectName ] ]
          body [ ]
            [ navbar config
              content ] ]

let renderLanding (info: Landing) =
    match info with
    | Landing config ->
        Hero.hero [ Hero.IsFullHeight ]
            [ Hero.body [ ]
                [ str config.ProjectDescription ] ]

let renderMarkdownPage (siteConfig : Config) (config : MarkdownConfig) =
    let output = Node.Exports.path.join("docs", config.Source.Replace(".md", ".html"))
    let entry = Node.Exports.path.join("content", config.Source)
    let markdown = MarkdownIt.markdownIt.Create()

    Utils.readFile entry
    |> markdown.render
    |> (fun html ->
        Container.container [ ]
            [ Section.section [ ]
                [ Content.content [ Content.Props [ DangerouslySetInnerHTML { __html = html } ] ] [ ] ] ])
    |> htmlPage siteConfig
    |> ReactDomServer.renderToStaticMarkup
    |> Utils.writeFile output


let kaladin (siteConfig: Config) =
    // Generate 404 page
    siteConfig.Error404
    |> htmlPage siteConfig
    |> ReactDomServer.renderToStaticMarkup
    |> Utils.writeFile "docs/404.html"

    siteConfig.Landing
    |> renderLanding
    |> htmlPage siteConfig
    |> ReactDomServer.renderToStaticMarkup
    |> Utils.writeFile "docs/index.html"

    for section in siteConfig.Docs do
        for page in section.Children do
            match page with
            | Markdown pageConfig ->
                renderMarkdownPage siteConfig pageConfig


let error404 =
    Hero.hero [ Hero.IsFullHeight
                Hero.Color IsDanger ]
        [ Hero.body [ ]
            [ str "404" ] ]

kaladin
    { ProjectName = "Kaladin"
      Landing =
        Landing
            { ProjectDescription = "Opiniated documentation sites"
              Columns = [ ] }
      Docs =
        [
            { Title = "Docs"
              Children =
                [
                    Markdown
                        { Title = "Docs"
                          Source = "/docs/installation.md" }
                ]
            }
        ]
      Nav =
        [
            NavItem
                { Title = "Home"
                  Path = "/" }
            NavItem
                { Title = "Docs"
                  Path = "/docs/installation.html" }
        ]
      Error404 = error404
    }
