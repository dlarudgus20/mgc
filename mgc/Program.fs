open Mgc
open Mgc.Transform
open Mgc.SceneObject

open OpenTK
open OpenTK.Graphics.OpenGL

let rect = new MeshData ("", PrimitiveType.Triangles, [|
        -1.0f; -1.0f; 1.0f; 1.0f; 0.0f; 0.0f;
        1.0f; -1.0f; 1.0f; 0.0f; 0.0f; 1.0f;
        -1.0f; 1.0f; 1.0f; 1.0f; 0.0f; 0.0f;
        -1.0f; 1.0f; 1.0f; 1.0f; 0.0f; 0.0f;
        1.0f; -1.0f; 1.0f; 0.0f; 0.0f; 1.0f;
        1.0f; 1.0f; 1.0f; 0.0f; 0.0f; 1.0f;
    |])

let triangle = new MeshData ("", PrimitiveType.Triangles, [|
        -0.5f; -0.43f; 0.0f; 1.0f; 1.0f; 1.0f;
        0.5f; -0.43f; 0.0f; 1.0f; 1.0f; 1.0f;
        0.0f; 0.5f; 0.0f; 1.0f; 1.0f; 1.0f;
    |])

let triangleScript =
    { emptyScript with
        Update = (fun x delta ->
            let transform = x.Transform |> rotateZ (single delta * 0.3f)
            { x with Transform = transform })
    }

let scene = container emptyScript identity [
    mesh rect emptyScript identity [];
    mesh triangle triangleScript identity []
]

let wnd = new MainWindow(scene)
wnd.Run 60.0
