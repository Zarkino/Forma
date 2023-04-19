module Tests.Parsing

open Fable.Mocha
open Language.Parser

open Propositional_Logic

let parsing_PL =
    testList "Basic Propositional Logic Parsing Tests" [
        let error_msg = "Incorrect parsing"
        test "Parse Constant true" {
            Expect.equal (parse_formula "T") (Some(Constant(true)), "") error_msg
        }
        test "Parse Constant false" {
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

open Proof_Interface

let parsing_Proofs =
    testList "Basic Proof Parsing Tests" [
        let error_msg = "Incorrect parsing"
        test "Parse Empty Proof" {
            Expect.equal (parse_proof "proof {}") (Some([]), "") error_msg
        }
        test "Parse Rules" {
            rules.Keys |> Seq.iter (fun rule -> Expect.equal
                                                    (parse_proof (sprintf "proof {have P by %s}" rule))
                                                    (Some([Intermediate(Variable("P"), rule)]), "") error_msg)
        }
        test "Parse Simple Proof" {
            Expect.equal
                (parse_proof "proof {assume P & Q\n\thave P by Con_E1\n\thave Q by Con_E2\n\tshow Q & P by Con_I}")
                (Some([Assumption(Conjunction(Variable("P"), Variable("Q")))
                       Intermediate(Variable("P"), "Con_E1")
                       Intermediate(Variable("Q"), "Con_E2")
                       Conclusion(Conjunction(Variable("Q"), Variable("P")), "Con_I")]), "") error_msg
        }
    ]

open Meta_Logic

let parsing_ML =
    testList "Basic Meta-logic Parsing Tests" [
        let error_msg = "Incorrect parsing"
        test "Parse Implication" {
            Expect.equal (parse_meta "p ==> p") (Some(Implication(Formula(Variable("p")), Formula(Variable("p")))), "") error_msg
        }
        test "Parse Equality" {
            Expect.equal (parse_meta "p == p") (Some(Equality(Formula(Variable("p")), Formula(Variable("p")))), "") error_msg
        }
        test "Parse Universal Quantifier" {
            Expect.equal (parse_meta "!!x. p") (Some(Universal("x", Formula(Variable("p")))), "") error_msg
        }
    ]