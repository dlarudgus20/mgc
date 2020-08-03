open Mgc
open Mgc.StateBuilder
open Mgc.Transform
open Mgc.SceneObject

open OpenTK
open OpenTK.Graphics.OpenGL

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
            let transform = identity |> rotateZ (single state) |> translate3 0.3f 0.3f 0.0f
            return { x with Transform = transform }
        })
    }

let scene = container emptyScript identity [
    mesh rect emptyScript (identity |> scale3 0.01f 0.01f 0.01f |> translate3 0.3f 0.3f -0.3f) [];
    mesh triangle (script triangleScript) identity []
]

let wnd = new MainWindow(scene)
wnd.Run 60.0
