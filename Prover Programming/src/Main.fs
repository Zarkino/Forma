module Main

open Feliz
open Feliz.Router
open Fable.Core.JsInterop

importSideEffects "./styles/global.scss"

[<ReactComponent>]
let Main () =
    let (theme, setTheme) = React.useState(Browser.WebStorage.localStorage.getItem("theme") |> function null -> "light" | x -> x)
    let (currentUrl, updateUrl) = React.useState(Router.currentUrl())
    
    React.contextProvider(Contexts.themeContext, theme,
        React.fragment [
            Navigation_Bar.Navigation(setTheme)
            React.router [
                router.pathMode
                router.onUrlChanged updateUrl
                router.children [
                    match currentUrl with
                    | _ -> Home.Home()
                ]
            ]
            Footer.Footer()
        ]
    )

let root = ReactDOM.createRoot(Browser.Dom.document.getElementById "root")
root.render(Main())