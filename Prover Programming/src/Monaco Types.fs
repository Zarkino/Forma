module rec Monaco_Types

open Fable.Core
open Fable.Core.JsInterop
open Feliz

[<StringEnum>]
type Theme =
    | [<CompiledName("vs-dark")>] Dark
    | [<CompiledName("light")>] Light

type EditorProps =
    /// Default value of the current model
    static member inline defaultValue (defaultValue: string) = Interop.mkAttr "defaultValue" defaultValue
    /// Default language of the current model
    static member inline defaultLanguage (defaultLanguage: string) = Interop.mkAttr "defaultLanguage" defaultLanguage
    /// <summary>
    /// Default path of the current model
    /// Will be passed as the third argument to <c>.createModel</c> method
    /// <c>monaco.editor.createModel(..., ..., monaco.Uri.parse(defaultPath))</c>
    /// </summary>
    static member inline defaultPath (defaultPath: string) = Interop.mkAttr "defaultPath" defaultPath
    /// Value of the current model
    static member inline value (value: string) = Interop.mkAttr "value" value
    /// Language of the current model
    static member inline language (language: string) = Interop.mkAttr "language" language
    /// <summary>
    /// Path of the current model
    /// Will be passed as the third argument to <c>.createModel</c> method
    /// <c>monaco.editor.createModel(..., ..., monaco.Uri.parse(defaultPath))</c>
    /// </summary>
    static member inline path (path: string) = Interop.mkAttr "path" path
    /// <summary>
    /// The theme for the monaco
    /// Available options "vs-dark" | "light"
    /// Define new themes by <c>monaco.editor.defineTheme</c>
    /// Defaults to "light"
    /// </summary>
    static member inline theme (theme: U2<Theme, string>) = Interop.mkAttr "theme" theme
    /// The line to jump on it
    static member inline line (line: float) = Interop.mkAttr "line" line
    /// The loading screen before the editor will be mounted
    /// Defaults to 'loading...'
    static member inline loading (loading: ReactElement) = Interop.mkAttr "loading" loading
    /// IStandaloneEditorConstructionOptions
    static member inline options (options: Monaco.Editor.IStandaloneEditorConstructionOptions) = Interop.mkAttr "options" options
    /// IEditorOverrideServices
    static member inline overrideServices (overrideServices: Monaco.Editor.IEditorOverrideServices) = Interop.mkAttr "overrideServices" overrideServices
    /// Indicator whether to save the models' view states between model changes or not
    /// Defaults to true
    static member inline saveViewState (saveViewState: bool) = Interop.mkAttr "saveViewState" saveViewState
    /// Indicator whether to dispose the current model when the Editor is unmounted or not
    /// Defaults to false
    static member inline keepCurrentModel (keepCurrentModel: bool) = Interop.mkAttr "keepCurrentModel" keepCurrentModel
    /// Width of the editor wrapper
    /// Defaults to 100%
    static member inline width (width: U2<float, string>) = Interop.mkAttr "width" width
    /// Height of the editor wrapper
    /// Defaults to 100%
    static member inline height (height: U2<float, string>) = Interop.mkAttr "height" height
    /// Class name for the editor container
    static member inline className (className: string) = Interop.mkAttr "className" className
    /// Props applied to the wrapper element
    static member inline wrapperProps (wrapperProps: obj) = Interop.mkAttr "wrapperProps" wrapperProps
    /// Signature: function(monaco: Monaco) => void
    /// An event is emitted before the editor is mounted
    /// It gets the monaco instance as a first argument
    /// Defaults to "noop"
    static member inline beforeMount (beforeMount: obj -> unit) = Interop.mkAttr "beforeMount" beforeMount
    /// Signature: function(editor: monaco.editor.IStandaloneCodeEditor, monaco: Monaco) => void
    /// An event is emitted when the editor is mounted
    /// It gets the editor instance as a first argument and the monaco instance as a second
    /// Defaults to "noop"
    static member inline onMount (onMount: Monaco.Editor.IStandaloneCodeEditor * obj -> unit) = Interop.mkAttr "onMount" onMount
    /// Signature: function(value: string | undefined, ev: monaco.editor.IModelContentChangedEvent) => void
    /// An event is emitted when the content of the current model is changed
    static member inline onChange (onChange: string -> unit) = Interop.mkAttr "onChange" onChange // string option * Monaco.Editor.IModelContentChangedEvent
    /// Signature: function(markers: monaco.editor.IMarker[]) => void
    /// An event is emitted when the content of the current model is changed
    /// and tthe current model markers are ready
    /// Defaults to "noop"
    static member inline onValidate (onValidate: ResizeArray<Monaco.Editor.IMarker> -> unit) = Interop.mkAttr "onValidate" onValidate

type ReactEditor =
    static member inline editor props = Interop.reactApi.createElement (import "default" "@monaco-editor/react", createObj !!props)