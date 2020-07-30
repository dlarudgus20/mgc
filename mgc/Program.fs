open Mgc
open Mgc.ScriptParser

let script = """

"""

let parser = createRecordTBlock typeof<System>
let result = parseString (byType parser) script

printfn "%A" parser
printfn "%A" result
