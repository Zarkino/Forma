module Main

open Feliz
open Feliz.Router
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

[<ReactComponent>]
let Main () =
    let (theme, setTheme) = React.useState(Browser.WebStorage.localStorage.getItem("theme") |> function null -> "light" | x -> x)
    let (accent, setAccent) = React.useState(Browser.WebStorage.localStorage.getItem("accent") |> function null -> "#5A0FEC" | x -> x)
    
    let (currentUrl, updateUrl) = React.useState(Router.currentUrl())
    
    React.useEffect(fun () ->
        Browser.WebStorage.localStorage.setItem("theme", theme)
    , [|box theme|])
    
    React.useEffect(fun () ->
        Browser.WebStorage.localStorage.setItem("accent", accent)
    , [|box accent|])
    
    React.contextProvider(Contexts.themeContext, theme,
        React.contextProvider(Contexts.accentContext, accent,
            React.fragment [
                Navigation_Bar.Navigation()
                React.router [
                    router.pathMode
                    router.onUrlChanged updateUrl
                    router.children [
                        match currentUrl with
                        | [ "settings" ]    -> Settings.Settings(setAccent, setTheme)
                        | [ "about" ]       -> About.About()
                        | _                 -> Home.Home()
                    ]
                ]
                Footer.Footer()
            ]
        )
    )

let root = ReactDOM.createRoot(Browser.Dom.document.getElementById "root")
root.render(Main())