module Tests.Parsing

open Fable.Mocha

open Parsec
open Logic.PL

let parsing_PL =
    let parse input =
        match runString Language.PL.formula () input with
        | Ok(v, r, _)   -> Some v, StringSegment.toString r
        | Error(msg)    -> None, $"Error: %A{msg}"
    
    testList "Basic Propositional Logic Parsing Tests" [
        let error_msg = "Incorrect parsing"
        test "Parse Constant true" {
            Expect.equal (parse "T") (Some(Constant(true)), "") error_msg
        }
        test "Parse Constant false" {
            Expect.equal (parse "F") (Some(Constant(false)), "") error_msg
        }
        test "Parse Variables (Lowercase)" {
            ['a'..'z'] |> List.map string |> List.iter (fun x -> Expect.equal (parse x) (Some(Variable(x)), "") error_msg)
        }
        test "Parse Variables (Uppercase)" {
            ['A'..'E']@['G'..'S']@['U'..'Z'] |> List.map string |> List.iter (fun x -> Expect.equal (parse x) (Some(Variable(x)), "") error_msg)
        }
        test "Parse Negation (Single)" {
            Expect.equal (parse "~P") (Some(Negation(Variable("P"))), "") error_msg
        }
        test "Parse Negation (Multiple)" {
            Expect.equal (parse "~~~P") (Some(Negation(Negation(Negation(Variable("P"))))), "") error_msg
        }
        test "Parse Conjunction" {
            Expect.equal (parse "P & Q") (Some(Conjunction(Variable("P"), Variable("Q"))), "") error_msg
        }
        test "Parse Disjunction" {
            Expect.equal (parse "P | Q") (Some(Disjunction(Variable("P"), Variable("Q"))), "") error_msg
        }
        test "Parse Implication" {
            Expect.equal (parse "P -> Q") (Some(Implication(Variable("P"), Variable("Q"))), "") error_msg
        }
        test "Parse Bi-implication" {
            Expect.equal (parse "P <-> Q") (Some(Equivalence(Variable("P"), Variable("Q"))), "") error_msg
        }
    ]

open Proof_Interface

let parsing_Proofs =
    let parse input =
        match runString Language.Proof.proof () input with
        | Ok(v, r, _)   -> Some v, StringSegment.toString r
        | Error(msg)    -> None, $"Error: %A{msg}"
    
    testList "Basic Proof Parsing Tests" [
        let error_msg = "Incorrect parsing"
        test "Parse Empty Proof" {
            Expect.equal (parse "proof (rule Empty) {}") (Some(Proof("Empty", [])), "") error_msg
        }
        test "Parse Rules" {
            rules.Keys |> Seq.iter (fun rule -> Expect.equal
                                                    (parse (sprintf "proof (rule %s) {}" rule))
                                                    (Some(Proof(rule, [])), "") error_msg)
        }
        test "Parse Simple Proof" {
            Expect.equal
                (parse "proof (rule Imp_I) {\n\tassume P & Q\n\tfrom P & Q have P by Con_E1\n\tfrom P & Q have Q by Con_E2\n\tfrom P and Q show Q & P by Con_I\n}")
                (Some(Proof("Imp_I", [Assumption(Conjunction(Variable("P"), Variable("Q")))
                                      Instant(Some [Conjunction(Variable("P"), Variable("Q"))], Variable("P"), "Con_E1")
                                      Instant(Some [Conjunction(Variable("P"), Variable("Q"))], Variable("Q"), "Con_E2")
                                      Instant(Some [Variable("P"); Variable("Q")], Conjunction(Variable("Q"), Variable("P")), "Con_I")])), "") error_msg
        }
    ]

open Logic.ML

let parsing_ML =
    let parse input =
        match runString Language.ML.meta () input with
        | Ok(v, r, _)   -> Some v, StringSegment.toString r
        | Error(e)      -> None, $"Error: %A{e}"
    
    testList "Basic Meta-logic Parsing Tests" [
        let error_msg = "Incorrect parsing"
        test "Parse Implication" {
            Expect.equal (parse "p ==> p") (Some(Implication(Formula(Variable("p")), Formula(Variable("p")))), "") error_msg
        }
        test "Parse Equality" {
            Expect.equal (parse "p == p") (Some(Equality(Formula(Variable("p")), Formula(Variable("p")))), "") error_msg
        }
        test "Parse Universal Quantifier" {
            Expect.equal (parse "!!x. p") (Some(Universal("x", Formula(Variable("p")))), "") error_msg
        }
    ]