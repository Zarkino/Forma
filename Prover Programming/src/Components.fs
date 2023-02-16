namespace App

open Feliz
open Feliz.Router
open Fable.Core.JsInterop
open MonacoEditor

type Components =
    /// <summary>
    /// The simplest possible React component.
    /// Shows a header with the text Hello World
    /// </summary>
    [<ReactComponent>]
    static member HelloWorld() = Html.h1 "Hello World"

    /// <summary>
    /// A stateful React component that maintains a counter
    /// </summary>
    [<ReactComponent>]
    static member Counter() =
        let (count, setCount) = React.useState(0)
        Html.div [
            Html.h1 count
            Html.button [
                prop.onClick (fun _ -> setCount(count + 1))
                prop.text "Increment"
            ]
        ]

    /// <summary>
    /// A React component that uses Feliz.Router
    /// to determine what to show based on the current URL
    /// </summary>
    [<ReactComponent>]
    static member Router() =
        let (currentUrl, updateUrl) = React.useState(Router.currentUrl())
        React.router [
            router.onUrlChanged updateUrl
            router.children [
                match currentUrl with
                | [ ] -> Html.h1 "Index"
                | [ "hello" ] -> Components.HelloWorld()
                | [ "counter" ] -> Components.Counter()
                | otherwise -> Html.h1 "Not found"
            ]
        ]
    
    [<ReactComponent>]
    static member Editor(value, setValue, readonly, theme) =
        let ASP_FORMAT: obj = import "ASP_FORMAT" "./language/asp.ts"
        let ASP_THEME_LIGHT: obj = import "ASP_THEME_LIGHT" "./language/asp.ts"
        let ASP_THEME_DARK: obj = import "ASP_THEME_DARK" "./language/asp.ts"
        
        MonacoEditor.create [
            MonacoEditor.Value value
            MonacoEditor.Language "asp-lang"
            MonacoEditor.Height "100%"
            MonacoEditor.Theme (if theme.Equals("dark") then "asp-theme-dark" else "asp-theme-light")
            MonacoEditor.Options {| minimap = {| enabled = false |}; readOnly = readonly; |}
            MonacoEditor.BeforeMount
                (fun monaco ->
                    monaco?languages?register$({| id = "asp-lang" |})
                    monaco?languages?setMonarchTokensProvider$("asp-lang", ASP_FORMAT)
                    monaco?editor?defineTheme$("asp-theme-light", ASP_THEME_LIGHT)
                    monaco?editor?defineTheme$("asp-theme-dark", ASP_THEME_DARK)
                )
            MonacoEditor.OnChange setValue
        ]