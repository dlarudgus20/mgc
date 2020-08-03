namespace Mgc

type State<'s, 'a> = State of ('s -> 'a * 's)

module State =
    let ofValue x = State (fun s -> x, s)

    let bind f (State p) =
        State (fun s ->
            let (r, s') = p s
            let (State f') = f r
            f' s')

    let combine (State a) (State b) =
        State (fun s -> let (r, s') = a s in b s)

    let put s = State (fun _ -> ((), s))
    let get = State (fun s -> (s, s))

    let update f = State (fun s -> let r = f s in (r, r))

    let run (State p) = p

type State<'s, 'a> with
    static member (>>=) (a, b) = State.bind b a
    static member (>>.) (a, b) = State.combine a b

type StateBuilder() =
    member this.Zero () = State.ofValue ()
    member this.Return x = State.ofValue x
    member this.ReturnFrom (x: State<'s, 'a>) = x
    member this.Bind (x, f) = State.bind f x
    member this.Combine (a, b) = State.combine a b
    member this.Delay f : State<'s, 'a> = f ()
    member this.For (s, f): State<'s, 'b> =
        s |> Seq.map f |> Seq.reduceBack State.combine
    member this.While (f, x) =
        if f () then State.combine x (this.While (f, x))
        else State.ofValue ()

module StateBuilder =
    let state = StateBuilder ()
