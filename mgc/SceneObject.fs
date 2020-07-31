namespace Mgc

open OpenTK
open OpenTK.Graphics.OpenGL

type MeshData = {
    Shader: string
    Mode: PrimitiveType
    Vertices: single[]
}

type SObjectData = {
    Position: Vector3
    Rotation: Quaternion
    Scale: Vector3
}

type SceneObject =
    | Mesh of mesh: MeshData * data: SObjectData * children: SceneObject list
    | Container of data: SObjectData * children: SceneObject list

module SObject =
    let rotate (q: Quaternion) (p: Vector3) =
        (q * Quaternion (p, 1.0f) * Quaternion.Conjugate q).Xyz

    let compositeData (a: SObjectData) (b: SObjectData) =
        let p = a.Position + rotate a.Rotation (a.Scale * b.Position)
        let r = a.Rotation * b.Rotation
        let s = a.Scale * b.Scale // TODO: Rotation
        { Position = p; Rotation = r; Scale = s }
