namespace Mgc

open System
open OpenTK
open OpenTK.Graphics
open OpenTK.Graphics.OpenGL

open Mgc

type RenderContext = {
    Camera: SceneObject
    CameraData: CameraData
}

type MainWindow(scene: SceneObject) =
    inherit GameWindow(800, 600,
        GraphicsMode(ColorFormat(32), 24, 0, 8),
        "LearnOpenTK")

    let defaultCameraData =
        CameraData.ortho -1.0f 1.0f -1.0f 1.0f -1.0f 1.0f
    let defaultCamera =
        SceneObject.camera defaultCameraData SceneObject.emptyScript Transform.identity []

    let mutable currentScene = scene
    let mutable currentContext = {
        Camera = defaultCamera; CameraData = defaultCameraData
    }

    let loadOne ctx obj =
        match obj.Type with
        | Mesh m -> m.Bake (); ctx
        | Camera c -> { ctx with Camera = obj; CameraData = c }
        | Container -> ctx
    let rec loadRenderer ctx obj =
        let ctx' = loadOne ctx obj
        List.fold loadRenderer ctx' obj.Children

    let renderContextApply ctx =
        let c = ctx.CameraData
        c.Apply (Shader.ByName "") ctx.Camera.Transform

    let rec renderRecursive obj =
        if obj.Enabled then
            match obj.Type with
            | Mesh m -> m.Render obj.Transform
            | Camera c -> ()
            | Container -> ()
        List.iter renderRecursive obj.Children

    let rec unloadRenderer renderer =
        match renderer.Type with
        | Mesh m -> m.Unbake ()
        | _ -> ()
        List.iter unloadRenderer renderer.Children

    let rec updateRenderer deltaTime ctx obj =
        let ctx', obj' =
            if obj.Enabled then
                let obj' = obj.Script.Update obj deltaTime
                let ctx' = loadOne ctx obj'
                ctx', obj'
            else
                ctx, obj
        let children, ctx'' = List.mapFold (updateRenderer deltaTime) ctx' obj'.Children
        { obj' with Children = children }, ctx''

    override this.OnLoad e =
        GL.ClearColor (0.0f, 0.0f, 0.0f, 1.0f)
        List.iter GL.Enable [
            EnableCap.Multisample;
            EnableCap.AlphaTest;
            EnableCap.Blend;
            EnableCap.CullFace;
            EnableCap.DepthTest
        ]
        loadRenderer currentContext currentScene |> ignore
        let s, c = updateRenderer 0.0 currentContext currentScene
        currentScene <- s
        currentContext <- c
        base.OnLoad e

    override this.OnUnload e =
        Shader.DisposeShaders ()
        unloadRenderer currentScene
        base.OnUnload e

    override this.OnResize e =
        GL.Viewport (0, 0, this.Width, this.Height)
        base.OnResize e

    override this.OnUpdateFrame e =
        let s, c = updateRenderer e.Time currentContext currentScene
        currentScene <- s
        currentContext <- c
        base.OnUpdateFrame e

    override this.OnRenderFrame e =
        GL.Clear (ClearBufferMask.ColorBufferBit ||| ClearBufferMask.DepthBufferBit)
        renderContextApply currentContext
        renderRecursive currentScene
        this.Context.SwapBuffers ()
        base.OnRenderFrame e
