namespace Mgc

open System
open System.Text
open System.Reflection
open FParsec

module ScriptParser =
    let wc = skipMany (anyOf " \t")
    let wcl = spaces

    let failIf p f msg =
        p >>= (fun x -> if f x then preturn x else fail msg)

    let atom =
        let first c = isLetter c || c = '_'
        let remain c = isLetter c || isDigit c || c = '_'
        many1Satisfy2 first remain

    let atomFor id = failIf atom (fun x -> id = "" || x = id) (sprintf "atom '%s' is expected" id)

    let string =
        let normalChar = satisfy (fun c -> c <> '\\' && c <> '"')
        let escapedChar =
            skipString "\\" >>. (anyOf "\"\\bfnrt" |>> function
                | 'b' -> '\b'
                | 'f' -> '\u000c'
                | 'n' -> '\n'
                | 'r' -> '\r'
                | 't' -> '\t'
                | c -> c)
        between (skipString "\"") (skipString "\"")
            (manyChars (normalChar <|> escapedChar))

    let number = pfloat
    let numberIn b e = number >>= (fun x ->
        if b <= x && x < e then preturn x
        else fail (sprintf "number must be in [%f, %f)" b e))
    let integer = number >>= (fun x ->
        if x = (floor x) then preturn (int x)
        else fail "number must be an integer")
    let integerIn b e = integer >>= (fun x ->
        if b <= x && x < e then preturn x
        else fail (sprintf "integer must be in [%d, %d)" b e))

    let rec assignFor id vtype x =
        let sep = wc >>. skipString ":" >>. wcl
        let p = ((atomFor id) .>> sep) .>>. byType vtype
        p x
    and blockFor ptypes x =
        let first = wc .>> skipString "{" .>> wcl
        let last = skipString "}"
        let middleFolder state ptype = state <|> attempt (byType ptype)
        let middle = ptypes |> List.fold middleFolder (fail "folder dummy")
        let middleSeq = sepEndBy middle wcl
        (between first last middleSeq) x
    and sobject x =
        ((atom |>> SOAtom)
        <|> (string |>> SOString)
        <|> (number |>> SONumber)
        <|> (assignFor "" TAny |>> SOAssign)
        <|> (blockFor [TAny] |>> SOBlock)) x
    and byType t x =
        (match t with
        | TAny -> sobject
        | TNone -> fail "type check failed"
        | TAtom id -> atomFor id |>> SOAtom
        | TString -> string |>> SOString
        | TNumber (b, e) -> numberIn b e |>> SONumber
        | TInteger (b, e) -> integerIn b e |>> (float >> SONumber)
        | TAssign (key, vtype) -> assignFor key vtype |>> SOAssign
        | TBlock ptypes -> blockFor ptypes |>> SOBlock) x

    let findAttr<'a when 'a :> Attribute> (attrs: seq<Attribute>) =
        attrs |> Seq.tryPick (fun x ->
            if x :? 'a then Some (x :?> 'a) else None)

    let pascal2camel (str: string) =
        str.ToCharArray() |> List.ofArray |> List.collect (fun c ->
            if isUpper c then ['_'; Char.ToLower c] else [c]
        ) |> List.tail |> String.Concat

    let rec createRecordTBlock (recordType: Type) =
        let recordAttr = findAttr<ScriptPropAttribute> (recordType.GetCustomAttributes ())
        let defaultAttr = ScriptPropAttribute()
        defaultAttr.Scripted <- match recordAttr with Some x -> x.Scripted | None -> false
        let props = recordType.GetProperties() |> List.ofArray |> List.choose (fun propInfo ->
            let propAttr =
                findAttr<ScriptPropAttribute> (propInfo.GetCustomAttributes ())
                |> Option.defaultValue defaultAttr
            if propAttr.Scripted then
                let rec getSOType = function
                | x when x = typeof<string> -> TString
                | x when x = typeof<float> -> TNumber (-infinity, +infinity)
                | x when x = typeof<int> -> TInteger (Int32.MinValue, Int32.MaxValue)
                | x when x.IsGenericType && x.GetGenericTypeDefinition() = typeof<list<unit>>.GetGenericTypeDefinition() ->
                    TBlock [ getSOType x.GenericTypeArguments.[0] ]
                | x when x.IsGenericType && x.GetGenericTypeDefinition() = typeof<Map<string, unit>>.GetGenericTypeDefinition() ->
                    TBlock [ TAssign ("", getSOType x.GenericTypeArguments.[1]) ]
                | x ->
                    match findAttr<ScriptPropAttribute> (x.GetCustomAttributes ()) with
                    | Some att when att.Scripted -> createRecordTBlock x
                    | _ -> failwith ""
                Some (TAssign (pascal2camel propInfo.Name, getSOType propInfo.PropertyType))
            else None
        )
        TBlock props

    let parseSObject str =
        run sobject str

    let parseString parser str =
        run parser str
