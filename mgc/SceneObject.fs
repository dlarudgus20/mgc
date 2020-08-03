namespace Mgc

type SceneObjectType =
    | Mesh of MeshData
    | Camera of CameraData
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

    let object t script transform children = {
        Type = t
        Transform = transform
        Children = children
        Script = script
    }

    let container script = object Container script
    let mesh data = object (Mesh data)
    let camera data = object (Camera data)
