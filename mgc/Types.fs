namespace Mgc

open System

type SObject =
    | SOAtom of string
    | SOString of string
    | SONumber of float
    | SOAssign of string * SObject
    | SOBlock of SObject list

type SType =
    | TAny | TNone
    | TAtom of string
    | TString
    | TNumber of float * float
    | TInteger of int * int
    | TAssign of string * SType
    | TBlock of SType list

type ScriptPropAttribute() =
    inherit Attribute()
    member val Scripted = true with get, set

[<ScriptProp>]
type Planet = {
    Name: string
    Resources: Map<string, double>
}

[<ScriptProp>]
type Star = {
    Name: string
}

[<ScriptProp>]
type System = {
    Name: string
    Star: Star
    Planets: Planet list
}
