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

let test_formula_parsing =
    testList "Basic Formula Parsing Tests" [
        let error_msg = "Incorrect parsing"
        test "Parse constant true" {
            Expect.equal (parse_formula "T") (Some(Constant(true)), "") error_msg
        }
        test "Parse constant false" {
            Expect.equal (parse_formula "F") (Some(Constant(false)), "") error_msg
        }
        test "Parse Variables (Lowercase)" {
            ['a'..'z'] |> List.map string |> List.iter (fun x -> Expect.equal (parse_formula x) (Some(Variable(x)), "") error_msg)
        }
        test "Parse Variables (Uppercase)" {
            ['A'..'E']@['G'..'S']@['U'..'Z'] |> List.map string |> List.iter (fun x -> Expect.equal (parse_formula x) (Some(Variable(x)), "") error_msg)
        }
        test "Parse Negation (Single)" {
            Expect.equal (parse_formula "~P") (Some(Negation(Variable("P"))), "") error_msg
        }
        test "Parse Negation (Multiple)" {
            Expect.equal (parse_formula "~~~P") (Some(Negation(Negation(Negation(Variable("P"))))), "") error_msg
        }
        test "Parse Conjunction" {
            Expect.equal (parse_formula "P & Q") (Some(Conjunction(Variable("P"), Variable("Q"))), "") error_msg
        }
        test "Parse Disjunction" {
            Expect.equal (parse_formula "P | Q") (Some(Disjunction(Variable("P"), Variable("Q"))), "") error_msg
        }
        test "Parse Implication" {
            Expect.equal (parse_formula "P -> Q") (Some(Implication(Variable("P"), Variable("Q"))), "") error_msg
        }
        test "Parse Bi-implication" {
            Expect.equal (parse_formula "P <-> Q") (Some(Equivalence(Variable("P"), Variable("Q"))), "") error_msg
        }
    ]

Mocha.runTests test_formula_parsing |> ignore