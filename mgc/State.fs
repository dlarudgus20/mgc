namespace Mgc

type State<'s, 'a> = State of ('s -> 'a * 's)

module State =
    let ofValue x = State (fun s -> x, s)

    let bind f (State p) =
        State (fun s ->
            let (r, s') = p s
            let (State f') = f r
            f' s')

    let put s = State (fun _ -> ((), s))
    let get = State (fun s -> (s, s))

    let run (State p) = p

type State<'s, 'a> with
    static member (>>=) (a, b) =
        State.bind b a

    static member (>>.) (a, b) =
        State.bind (fun _ -> b) a
