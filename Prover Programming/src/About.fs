module About

open Feliz
open Feliz.Bulma

[<ReactComponent>]
let About() =
    let theme = React.useContext(Contexts.themeContext)
    
    Html.div [
        prop.className [ theme; "about" ]
        prop.children [
            Bulma.container [
                prop.className [ "pt-3"; "px-3" ]
                prop.style [
                    style.display.flex
                    style.flexDirection.column
                    style.alignItems.center
                ]
                prop.children [
                    Bulma.title [
                        title.is3
                        prop.text "What is Forma?"
                    ]
                    Bulma.block [
                        prop.className "has-text-centered"
                        prop.style [
                            style.display.flex
                            style.flexDirection.column
                            style.rowGap 10
                            style.justifyContent.spaceBetween
                        ]
                        prop.children [
                            Html.p "Forma provides an interactive environment for developing proofs. Users can work in a step-by-step manner, interactively constructing proofs and exploring different approaches."
                            Html.p "The purpose of developing Forma is to contribute to logic education. Forma is created to fill an educational niche that exists as an unintended side effect of the complexity of Isabelle by being as expansive and feature-rich as it is. In essence, we aim to solve the problem of a steep learning curve by building a step around the halfway point, ideally splitting the curve in half and thereby making each part easier to overcome."
                        ]
                    ]
                    
                    Bulma.title [
                        title.is3
                        prop.className "pt-2"
                        prop.text "Why use Forma?"
                    ]
                    Bulma.block [
                        prop.className "has-text-centered"
                        prop.style [
                            style.display.flex
                            style.flexDirection.column
                            style.rowGap 10
                            style.justifyContent.spaceBetween
                        ]
                        prop.children [
                            Html.p "This tool is a great starting point as it is much simpler to use. One of the obvious examples is simplified syntax. We only use symbols present on the keyboard and avoid syntax menus that the user needs to tab through to find a correct Unicode character. Another is a streamlined, modern and minimal user interface."
                            Html.p "By removing clutter we are able to reduce the uncertainty that new users feel while shortening the amount of time they need to familiarise themselves with the user interface. This ties in with being minimal, as a smaller UI is naturally faster to familiarise oneself with."
                            Html.p "Finally, Forma is more accessible since it can simply be hosted and accessed from any modern browser, with no need to download and install a heavy piece of software, potentially with dependencies."
                        ]
                    ]
                    
                    Bulma.title [
                        title.is3
                        prop.className "pt-2"
                        prop.text "Credits"
                    ]
                    Bulma.block [
                        prop.className "has-text-centered"
                        prop.style [
                            style.display.flex
                            style.flexDirection.column
                            style.rowGap 10
                            style.justifyContent.spaceBetween
                        ]
                        prop.children [
                            Html.p "Forma is developed by Fredrik Haarklou Veileborg and Jónas Mittún Peltonen at the Technical University of Denmark."
                            Html.p "It is a product of the Bachelor Project \"Forma - A Web-Based Proof Assistant (Prover Programming)\"."
                            Html.p "Supervised by Assoc. Prof. Jørgen Villadsen and Phd. Student Frederik Krogsdal Jacobsen."
                        ]
                    ]
                ]
            ]
        ]
    ]