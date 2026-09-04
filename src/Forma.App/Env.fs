namespace Env

open Fable.Core

[<RequireQualifiedAccess>]
module Vite =
    [<Emit("import.meta.env.MODE")>]
    let MODE: string = jsNative

    [<Emit("import.meta.env.BASE_URL")>]
    let BASE_URL: string = jsNative
    
    [<Emit("import.meta.env.PROD")>]
    let PROD: bool = jsNative

    [<Emit("import.meta.env.DEV")>]
    let DEV: bool = jsNative

    [<Emit("import.meta.env.SSR")>]
    let SSR: bool = jsNative