namespace App

open Feliz
open Feliz.Router
open Fable.Core.JsInterop

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
    static member Editor(theme, value, ?setValue, ?readonly) =
        Monaco_Editor.Editor.create [
            Monaco_Editor.Props.value value
            Monaco_Editor.Props.language "fsharp"
            Monaco_Editor.Props.theme (if theme.Equals("dark") then !^Monaco_Editor.Dark else !^Monaco_Editor.Light)
            Monaco_Editor.Props.options
                (jsOptions<Monaco.Editor.IStandaloneEditorConstructionOptions>(fun o ->
                    o.minimap <- Some (jsOptions<Monaco.Editor.IEditorMinimapOptions>(fun oMinimap ->
                        oMinimap.enabled <- Some false
                    ))
                    o.readOnly <- defaultArg readonly (Some false)
                ))
            Monaco_Editor.Props.onChange (defaultArg setValue (fun _ -> ()))
        ]