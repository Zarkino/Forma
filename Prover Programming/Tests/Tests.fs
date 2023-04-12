module Tests

open Fable.Mocha
open Propositional_Logic
open Language.Parser

let arithmeticTests =
    testList "Arithmetic tests" [
        test "plus works" {
            Expect.equal (1 + 1) 2 "plus"
        }

        test "Test for falsehood" {
            Expect.isFalse (1 = 2) "false"
        }

        testAsync "Test async code" {
            let! x = async { return 21 }
            let answer = x * 2
            Expect.equal 42 answer "async"
        }
    ]

Mocha.runTests arithmeticTests |> ignore

let test_parsing =
    testList "Parsing tests" [
        test "Parse constant true" {
            Expect.equal (fst <| parse_formula "T") (Some(Constant(true))) "Incorrect parsing"
        }
        test "Parse constant false" {
            Expect.equal (fst <| parse_formula "F") (Some(Constant(false))) "Incorrect parsing"
        }
    ]

Mocha.runTests test_parsing |> ignore