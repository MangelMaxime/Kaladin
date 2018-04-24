module Utils

open Fable.PowerPack
open Fable.PowerPack.Fetch


let fetchFile path =
    promise {
        let! res = fetch path []
        return! res.text()
    }
