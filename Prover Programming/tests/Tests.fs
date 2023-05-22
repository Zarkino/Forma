module Tests

open Fable.Mocha

open Tests.Parsing
open Tests.Evaluation

// Tests parsing
Mocha.runTests parsing_PL |> ignore
Mocha.runTests parsing_Proofs |> ignore
Mocha.runTests parsing_ML |> ignore

// Tests evaluation
Mocha.runTests evaluating_Proofs |> ignore