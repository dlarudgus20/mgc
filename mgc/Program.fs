open Mgc
open Mgc.StateBuilder
open Mgc.Transform
open Mgc.SceneObject

open OpenTK
open OpenTK.Graphics.OpenGL

//let mycam = CameraData.ortho -1.0f 1.0f -1.0f 1.0f -1.0f 1.0f
//let campos = identity |> translate3 0.0f 0.0f 0.0f
let mycam = CameraData.perspective 2.0f 1.0f 0.1f 10.0f
let campos = identity |> translate3 0.0f 0.0f -2.0f

let rect = new MeshData ("", PrimitiveType.Triangles, [|
        -1.0f; -1.0f; 0.0f; 1.0f; 0.0f; 0.0f;
        1.0f; -1.0f; 0.0f; 0.0f; 0.0f; 1.0f;
        -1.0f; 1.0f; 0.0f; 1.0f; 0.0f; 0.0f;
        -1.0f; 1.0f; 0.0f; 1.0f; 0.0f; 0.0f;
        1.0f; -1.0f; 0.0f; 0.0f; 0.0f; 1.0f;
        1.0f; 1.0f; 0.0f; 0.0f; 0.0f; 1.0f;
    |])

let triangle = new MeshData ("", PrimitiveType.Triangles, [|
        -0.43f; -0.25f; 0.0f; 1.0f; 1.0f; 1.0f;
        0.43f; -0.25f; 0.0f; 1.0f; 1.0f; 1.0f;
        0.0f; 0.5f; 0.0f; 1.0f; 1.0f; 1.0f;
    |])

let triangleScript: float SceneObjectScript =
    { emptyWith 0.0 with
        Update = (fun x delta -> state {
            let! state = State.update (fun s -> s + delta * 4.0)
            let transform = identity |> rotateZ (single state) |> translate3 0.3f 0.3f -0.5f
            return { x with Transform = transform }
        })
    }

let scene = container emptyScript identity [
    camera mycam emptyScript campos [];
    mesh rect emptyScript (identity |> scale3 0.05f 0.05f 0.0f |> translate3 0.3f 0.3f 0.5f) [];
    mesh triangle (script triangleScript) identity []
]

let wnd = new MainWindow(scene)
wnd.Run 60.0
