namespace App

open Feliz
open Feliz.Router
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
    static member Editor() =
        let (value, setValue) = React.useState("")
        MonacoEditor.create [
            MonacoEditor.Value "let x = 10\nlet f x = x * x"
            MonacoEditor.Language "fsharp"
            MonacoEditor.Height "90vh"
            MonacoEditor.Theme Light
            MonacoEditor.Options {| minimap = {| enabled = false |}; |}
            MonacoEditor.OnChange setValue
        ]