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
        let ASP_THEME: obj = import "ASP_THEME" "./language/asp.ts"
        
        MonacoEditor.create [
            MonacoEditor.Value value
            MonacoEditor.Language "asp"
            MonacoEditor.Height "100%"
            MonacoEditor.Theme (if theme.Equals("dark") then Dark else Light)
            MonacoEditor.Options {| minimap = {| enabled = false |}; readOnly = readonly; |}
            MonacoEditor.BeforeMount
                (fun monaco ->
                    monaco?languages?register$({| id = "asp" |})
                    monaco?languages?setMonarchTokensProvider$("asp", ASP_FORMAT)
                    monaco?editor?defineTheme$("asp", ASP_THEME)
                    //Browser.Dom.console.log(monaco?languages?getLanguages())
                )
            MonacoEditor.OnChange setValue
        ]