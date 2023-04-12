module Tests

open Fable.Mocha
open Propositional_Logic
open Proof_Interface
open Language.Parser

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

let test_proof_parsing =
    testList "Basic Proof Parsing Tests" [
        let error_msg = "Incorrect parsing"
        test "Parse Empty Proof" {
            Expect.equal (parse_proof "proof {}") (Some({ Statements = [] }), "") error_msg
        }
        test "Parse Rules" {
            rules.Keys |> Seq.iter (fun rule -> Expect.equal
                                                    (parse_proof (sprintf "proof {have P by %s}" rule))
                                                    (Some({ Statements = [Intermediate(Variable("P"), rule)] }), "") error_msg)
        }
        test "Parse Simple Proof" {
            Expect.equal
                (parse_proof "proof {assume P & Q\n\thave P by Con_E1\n\thave Q by Con_E2\n\tshow Q & P by Con_I}")
                (Some({ Statements = [Assumption(Conjunction(Variable("P"), Variable("Q")))
                                      Intermediate(Variable("P"), "Con_E1")
                                      Intermediate(Variable("Q"), "Con_E2")
                                      Conclusion(Conjunction(Variable("Q"), Variable("P")), "Con_I")] }), "") error_msg
        }
    ]

Mocha.runTests test_proof_parsing |> ignore