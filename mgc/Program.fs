open Mgc

open OpenTK.Graphics.OpenGL

let rect = {
    Shader = ""; Mode = PrimitiveType.Triangles; Vertices = [|
        -1.0f; -1.0f; 0.0f; 1.0f; 0.0f; 0.0f;
        1.0f; -1.0f; 0.0f; 0.0f; 0.0f; 1.0f;
        -1.0f; 1.0f; 0.0f; 1.0f; 0.0f; 0.0f;
        -1.0f; 1.0f; 0.0f; 1.0f; 0.0f; 0.0f;
        1.0f; -1.0f; 0.0f; 0.0f; 0.0f; 1.0f;
        1.0f; 1.0f; 0.0f; 0.0f; 0.0f; 1.0f;
    |]
}
let triangle = {
    Shader = ""; Mode = PrimitiveType.Triangles; Vertices = [|
        -0.5f; -0.5f; -1.0f; 1.0f; 1.0f; 1.0f;
        0.5f; -0.5f; -1.0f; 1.0f; 1.0f; 1.0f;
        0.0f; 0.5f; -1.0f; 1.0f; 1.0f; 1.0f;
    |]
}
let scene = SceneObject.container [
    Mesh (rect, Transform.identity, []);
    Mesh (triangle, Transform.identity, [])
]

let wnd = new MainWindow(scene)
wnd.Run 60.0
