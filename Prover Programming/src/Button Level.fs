module Button_Level

open Feliz
open Feliz.Bulma

let examples = [|
    "lemma (P & Q) --> (Q & P)\nproof (rule Imp_I) {\n\tassume P & Q\n\tfrom P & Q have P by (rule Con_E1)\n\tfrom P & Q have Q by (rule Con_E2)\n\tfrom P and Q show Q & P by (rule Con_I)\n}"
    "lemma P --> ~~P\nproof (rule Imp_I) {\n\tassume P\n\tshow ~~P\n\tproof (rule Neg_I) {\n\t\tassume ~P\n\t\tfrom ~P and P show F by (rule Neg_E)\n\t}\n}"
    "lemma ((P --> Q) & ~Q) --> ~P\nproof (rule Imp_I) {\n\tassume (P --> Q) & ~Q\n\tshow ~P\n\tproof (rule Neg_I) {\n\t\tassume P\n\t\tfrom (P --> Q) & ~Q have P --> Q by (rule Con_E1)\n\t\tfrom (P --> Q) & ~Q have ~Q by (rule Con_E2)\n\t\tfrom P --> Q and P have Q by (rule Imp_E)\n\t\tfrom ~Q and Q show F by (rule Neg_E)\n\t}\n}"
    "lemma (P <--> Q) <--> (Q <--> P)\nproof (rule Iff_I) {\n\tassume P <--> Q\n\tshow Q <--> P\n\tproof (rule Iff_I) {\n\t\tassume Q\n\t\tshow P by (rule Iff_E2)\n\tnext\n\t\tassume P\n\t\tshow Q by (rule Iff_E1)\n\t}\nnext\n\tassume Q <--> P\n\tshow P <--> Q\n\tproof (rule Iff_I) {\n\t\tassume P\n\t\tshow Q by (rule Iff_E2)\n\tnext\n\t\tassume Q\n\t\tshow P by (rule Iff_E1)\n\t}\n}"
    "lemma classical: (~P ==> P) ==> P\nproof none {\n\tassume ~P ==> P\n\thave P ==> P by this\n\thave P | ~P by (rule LEM)\n\tshow P by (rule Dis_E)\n}\n\nlemma ccontr: (~P ==> F) ==> P\nproof none {\n\tassume ~P ==> F\n\thave ~P ==> P\n\tproof none {\n\t\tassume ~P\n\t\thave ~P --> F by (rule Imp_I)\n\t\thave F by (rule Imp_E)\n\t\tshow P by (rule Falsity_E)\n\t}\n\tshow P by (rule classical)\n}"
|]

type Modal =
    [<ReactComponent>]
    static member Download (id: string, active: bool, setActive: bool -> unit, input: string) =
        let theme = React.useContext(Contexts.themeContext)
        let accent = React.useContext(Contexts.accentContext)
        
        let (filename, setFilename) = React.useState("File1")
        
        Bulma.modal [
            prop.id id
            if active then modal.isActive
            prop.children [
                Bulma.modalBackground [ prop.onClick (fun _ -> setActive(false)) ]
                Bulma.modalClose [ prop.onClick (fun _ -> setActive(false)) ]
                Bulma.modalContent [
                    Bulma.box [
                        prop.className theme
                        prop.children [
                            Html.h1 [
                                prop.className [ "title"; "is-4" ]
                                prop.text "Save this file"
                            ]
                            Bulma.field.div [
                                Bulma.label "File Name"
                                Html.div [
                                    prop.className [ "control"; "input-container" ]
                                    prop.children [
                                        Bulma.input.text [
                                            prop.defaultValue filename
                                            prop.onChange setFilename
                                        ]
                                    ]
                                ]
                            ]
                            Bulma.field.div [
                                field.isGrouped
                                prop.style [ style.justifyContent.right ]
                                prop.children [
                                    Bulma.control.p [
                                        Bulma.button.button [
                                            prop.style [ style.color "white"; style.backgroundColor accent ]
                                            prop.onClick
                                                (fun _  ->
                                                    if filename.Length > 0 then
                                                        let anchor = Browser.Dom.document.createElement("a")
                                                        anchor.setAttribute("href", input |> sprintf "data:text/plain;charset=utf-8,%s" |> Fable.Core.JS.encodeURI)
                                                        anchor.setAttribute("download", filename + ".txt")
                                                        anchor.click()
                                                        setActive(false)
                                                )
                                            prop.children [ Html.span [ Html.text "Save" ] ]
                                        ]
                                    ]
                                    Bulma.control.p [
                                        Bulma.button.button [
                                            prop.className theme
                                            prop.onClick (fun _ -> setActive(false))
                                            prop.children [ Html.span [ Html.text "Cancel" ] ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]

    [<ReactComponent>]
    static member Help (id: string, help: bool, setHelp: bool -> unit) =
        let theme = React.useContext(Contexts.themeContext)
        
        let (active, setActive) = React.useState(0)
        
        Bulma.modal [
            prop.id id
            if help then modal.isActive
            prop.children [
                Bulma.modalBackground [ prop.onClick (fun _ -> setHelp(false)) ]
                Bulma.modalClose [ prop.onClick (fun _ -> setHelp(false)) ]
                Bulma.modalContent [
                    Bulma.box [
                        prop.className theme
                        prop.children [
                            Bulma.tabs [
                                prop.children [
                                    Html.ul [
                                        Bulma.tab [
                                            if active = 0 then tab.isActive
                                            prop.onClick (fun _ -> setActive(0))
                                            prop.children [
                                                Html.a [
                                                    Html.span [ prop.className "icon"; prop.children [ Html.i [ prop.className "fa-solid fa-book" ] ] ]
                                                    Html.span [ Html.text "Rules" ]
                                                ]
                                            ]
                                        ]
                                        Bulma.tab [
                                            if active = 1 then tab.isActive
                                            prop.onClick (fun _ -> setActive(1))
                                            prop.children [
                                                Html.a [
                                                    Html.span [ prop.className "icon"; prop.children [ Html.i [ prop.className "fa-solid fa-code" ] ] ]
                                                    Html.span [ Html.text "Language Reference" ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                            match active with
                            | 1 -> Language_Reference.Language_Reference()
                            | _ -> Rules.Rules()
                        ]
                    ]
                ]
            ]
        ]

[<ReactComponent>]
let Button_Level(input, setInput, output) =
    let theme = React.useContext(Contexts.themeContext)
    let accent = React.useContext(Contexts.accentContext)
    
    let (format, setFormat) = React.useState(false)
    let (download, setDownload) = React.useState(false)
    let (help, setHelp) = React.useState(false)
    
    let reader = Browser.Dom.FileReader.Create()
    
    Bulma.level [
        prop.className [ theme; "py-2" ]
        prop.style [style.margin 0]
        prop.children [
            Bulma.levelLeft [
                Bulma.button.button [
                    prop.className "mx-1"
                    prop.style [ style.color "white"; style.backgroundColor accent ]
                    prop.onClick (fun _ ->
                        match output with
                        | Some _, _     ->
                            setFormat(true)
                            async {
                                do! Async.Sleep(3000)
                                setFormat(false)
                            } |> Async.StartImmediate
                        | None, list    ->
                            list
                            |> List.map Proof_Interface.toPlaintext
                            |> String.concat "\n\n"
                            |> setInput
                    )
                    prop.children [
                        Html.span [ prop.className "icon"; prop.children [ Html.i [ prop.className "fa-solid fa-pencil" ] ] ]
                        Html.span [ Html.text "Format" ]
                    ]
                ]
                Bulma.notification [
                    Bulma.color.isDanger
                    prop.style [
                        if not format then style.display.none
                        style.position.absolute
                        style.right 10
                        style.bottom 10
                        style.zIndex 1
                    ]
                    prop.children [
                        Bulma.delete [ prop.onClick (fun _ -> setFormat(false)) ]
                        Html.span [ prop.className "icon"; prop.children [ Html.i [ prop.className "fa-solid fa-triangle-exclamation" ] ] ]
                        Html.span [ Html.text "Cannot format due to parsing error!" ]
                    ]
                ]
                Bulma.button.button [
                    prop.className "mx-1"
                    prop.style [ style.color "white"; style.backgroundColor accent ]
                    prop.target "modal-download"
                    prop.onClick (fun _ -> setDownload(true))
                    prop.onKeyDown (key.escape, fun _ -> setDownload(false))
                    prop.children [
                        Html.span [ prop.className "icon"; prop.children [ Html.i [ prop.className "fa-solid fa-download" ] ] ]
                        Html.span [ Html.text "Download" ]
                    ]
                ]
                Modal.Download("modal-download", download, setDownload, input)
                Bulma.file [
                    prop.className "mx-1"
                    prop.children [
                        Bulma.fileLabel.label [
                            prop.children [
                                Bulma.fileInput [
                                    prop.accept "text/*"
                                    prop.onChange
                                        (fun (e: Browser.Types.Event) ->
                                            let target = e.target :?> Browser.Types.HTMLInputElement
                                            let file = target.files.Item(0)
                                            
                                            reader.readAsText(file)
                                            reader.onload <- (fun _ -> reader.result |> function :? string as str -> setInput(str) | _ -> ()))
                                ]
                                Bulma.fileCta [
                                    prop.className "button"
                                    prop.style [
                                        style.backgroundColor accent
                                        style.color "white"
                                    ]
                                    prop.children [
                                        Bulma.fileIcon [ Html.i [ prop.className "fa-solid fa-upload" ] ]
                                        Bulma.fileLabel.span [ Html.text "Upload" ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
                Bulma.button.button [
                    prop.className "mx-1"
                    prop.style [ style.color "white"; style.backgroundColor accent ]
                    prop.target "modal-help"
                    prop.onClick (fun _ -> setHelp(true))
                    prop.onKeyDown (key.escape, fun _ -> setHelp(false))
                    prop.children [
                        Html.span [ prop.className "icon"; prop.children [ Html.i [ prop.className "fa-solid fa-circle-info" ] ] ]
                        Html.span [ Html.text "Help" ]
                    ]
                ]
                Modal.Help("modal-help", help, setHelp)
                Bulma.dropdown [
                    dropdown.isHoverable
                    prop.children [
                        Bulma.dropdownTrigger [
                            prop.children [
                                Bulma.button.button [
                                    prop.className "mx-1"
                                    prop.style [ style.color "white"; style.backgroundColor accent ]
                                    prop.ariaHasPopup true
                                    prop.ariaControls "dropdown-menu"
                                    prop.children [
                                        Html.span [ Html.text "Examples" ]
                                        Html.span [ prop.className "icon"; prop.children [ Html.i [ prop.className "fa-solid fa-angle-down" ] ] ]
                                    ]
                                ]
                            ]
                        ]
                        Bulma.dropdownMenu [
                            prop.id "dropdown-menu"
                            prop.role "menu"
                            prop.children [
                                Bulma.dropdownContent [
                                    prop.className theme
                                    prop.children
                                        (examples |> Array.mapi (fun i ex ->
                                        Bulma.dropdownItem.a [
                                            prop.onClick (fun _ -> setInput(ex))
                                            prop.text $"Example %i{i+1}"
                                        ]))
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]