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
            Expect.equal (parse "P --> Q") (Some(Implication(Variable("P"), Variable("Q"))), "") error_msg
        }
        test "Parse Bi-implication" {
            Expect.equal (parse "P <--> Q") (Some(Equivalence(Variable("P"), Variable("Q"))), "") error_msg
        }
    ]

open Proof_Interface
open Logic.ML

let parsing_Proofs =
    let parse input =
        match runString Language.Proof.proof () input with
        | Ok(v, r, _)   -> Some v, StringSegment.toString r
        | Error(msg)    -> None, $"Error: %A{msg}"
    
    testList "Basic Proof Parsing Tests" [
        let error_msg = "Incorrect parsing"
        test "Parse Trivial Proof" {
            ["-"; "none"] |> List.iter
                (fun x ->
                    Expect.equal
                        (parse ("proof " + x + " {\n\tshow P ==> P by this\n}"))
                        (Some(Proof(Trivial, [Conclusion(Instant(None, Implication(Entity(Variable("P")), Entity(Variable("P"))), This))])), "") error_msg)
        }
        test "Parse Simple Proof" {
            Expect.equal
                (parse "proof (rule Imp_I) {\n\tassume P & Q\n\tfrom P & Q have P by (rule Con_E1)\n\tfrom P & Q have Q by (rule Con_E2)\n\tfrom P and Q show Q & P by (rule Con_I)\n}")
                (Some(Proof(Rule("Imp_I"),
                    [Assumption([Entity(Conjunction(Variable("P"), Variable("Q")))])
                     Intermediate(Instant(Some [Entity(Conjunction(Variable("P"), Variable("Q")))], Entity(Variable("P")), Rule("Con_E1")))
                     Intermediate(Instant(Some [Entity(Conjunction(Variable("P"), Variable("Q")))], Entity(Variable("Q")), Rule("Con_E2")))
                     Conclusion(Instant(Some [Entity(Variable("P")); Entity(Variable("Q"))], Entity(Conjunction(Variable("Q"), Variable("P"))), Rule("Con_I")))])), "") error_msg
        }
    ]

let parsing_ML =
    let parse input =
        match runString Language.ML.meta () input with
        | Ok(v, r, _)   -> Some v, StringSegment.toString r
        | Error(e)      -> None, $"Error: %A{e}"
    
    testList "Basic Meta-logic Parsing Tests" [
        let error_msg = "Incorrect parsing"
        test "Parse Implication" {
            Expect.equal (parse "p ==> p") (Some(Implication(Entity(Variable("p")), Entity(Variable("p")))), "") error_msg
        }
        test "Parse Equality" {
            Expect.equal (parse "p == p") (Some(Equality(Entity(Variable("p")), Entity(Variable("p")))), "") error_msg
        }
        test "Parse Universal Quantifier" {
            Expect.equal (parse "!!x. p") (Some(Universal(["x"], Entity(Variable("p")))), "") error_msg
        }
    ]