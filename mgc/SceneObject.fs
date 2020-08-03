namespace Mgc

type SceneObjectType =
    | Mesh of MeshData
    | Container

type IScriptObjectExecutor =
    abstract member Update: SceneObject -> float -> SceneObject
and 's SceneObjectScript = {
    Load: unit -> 's
    Update: SceneObject -> float -> State<'s, SceneObject>
}
and SceneObject = {
    Type: SceneObjectType
    Transform: Transform
    Children: SceneObject list
    Script: IScriptObjectExecutor
}

type 's ScriptObjectExecutor(script: 's SceneObjectScript) =
    let mutable state = script.Load ()
    interface IScriptObjectExecutor with
        member this.Update o x =
            let (o', s) = State.run (script.Update o x) state
            state <- s
            o'

module SceneObject =
    let emptyWith initial = {
        Load = fun () -> initial
        Update = fun x _ -> State.ofValue x
    }

    let script script = ScriptObjectExecutor script :> IScriptObjectExecutor

    let emptyScript = script (emptyWith ())

    let container script transform children = {
        Type = Container
        Transform = transform
        Children = children
        Script = script
    }

    let mesh data script transform children = {
        Type = Mesh data
        Transform = transform
        Children = children
        Script = script
    }
