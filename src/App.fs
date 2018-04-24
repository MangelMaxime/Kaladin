module App.View

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.FontAwesome
open Fable.PowerPack
open Fable.PowerPack.Fetch

type Model =
    { Value : string }

type Msg =
    | ChangeValue of string

let init _ = { Value = "" }, Cmd.none

let private update msg model =
    match msg with
    | ChangeValue newValue ->
        { model with Value = newValue }, Cmd.none

let private view model dispatch =
    Navbar.navbar [ ]
        [ ]

open Elmish.React
open Elmish.Debug
open Elmish.HMR

Program.mkProgram init update view
#if DEBUG
|> Program.withHMR
#endif
|> Program.withReactUnoptimized "kaladin"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run

type RouterMode =
    | Hash
    | History

type LandingConfig =
    { Title : string
      Description : string }

type PageConfig =
    { Title : string
      Path : string }

type DocConfig =
    { Title : string
      Children : PageConfig list }

type Config =
    { RouterMode : RouterMode
      Landing : LandingConfig
      Navbar : PageConfig list
      Docs : DocConfig list }

let kaladin (config : Config) =
    ()

kaladin
    { RouterMode = Hash
      Landing =
        { Title = "Kaladin"
          Description = "Opiniated documentation sites" }
      Navbar =
        [
          { Title = "Home"
            Path = "/" }
        ]
      Docs =
        [

        ] }
