namespace Mgc

type SceneObjectType =
    | Mesh of MeshData
    | Container

type SceneObjectScript = {
    Update: SceneObject -> float -> SceneObject
}
and SceneObject = {
    Type: SceneObjectType
    Transform: Transform
    Children: SceneObject list
    Script: SceneObjectScript
}

module SceneObject =
    let emptyScript = {
        Update = (fun x _ -> x)
    }

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
