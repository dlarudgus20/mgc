namespace Mgc

type SceneObject =
    | Mesh of mesh: MeshData * transform: Transform * children: SceneObject list
    | Container of transform: Transform * children: SceneObject list

module SceneObject =
    let container children = Container (Transform.identity, children)
