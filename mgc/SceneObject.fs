namespace Mgc

open OpenTK.Graphics.OpenGL

type Mesh = {
    Shader: string
    Mode: PrimitiveType
    Vertices: single[]
}

type SceneObject =
    | Mesh of Mesh
    | Container of SceneObject list
