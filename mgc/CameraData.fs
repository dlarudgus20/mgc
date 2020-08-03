namespace Mgc

open OpenTK
open OpenTK.Graphics.OpenGL

type CameraType =
    | Orthogonal of bounding: Box2
    | Perspective of fovy: single * aspect: single

type CameraData(ctype: CameraType, near: single, far: single) =
    let projMatrix =
        match ctype with
        | Orthogonal b ->
            Matrix4.CreateOrthographicOffCenter (b.Left, b.Right, b.Bottom, b.Top, near, far)
        | Perspective (fovy, aspect) ->
            Matrix4.CreatePerspectiveFieldOfView (fovy, aspect, near, far)

    member this.Apply (shader: Shader) (tr: Transform) =
        shader.Use ()
        shader.SetUniform ("viewMatrix", Transform.toMatrix tr)
        shader.SetUniform ("projMatrix", projMatrix)

module CameraData =
    let ortho left right bottom top near far =
        CameraData (Orthogonal (Box2.FromTLRB (top, left, right, bottom)), near, far)

    let perspective fovy aspect near far =
        CameraData (Perspective (fovy, aspect), near, far)
