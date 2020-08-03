namespace Mgc

open System
open OpenTK
open OpenTK.Graphics
open OpenTK.Graphics.OpenGL

open Mgc

type MainWindow(scene: SceneObject) =
    inherit GameWindow(800, 600,
        GraphicsMode(ColorFormat(32), 24, 0, 8),
        "LearnOpenTK")

    let mutable currentScene = scene

    let loadOne obj =
        match obj.Type with
        | Mesh m -> m.Bake ()
        | _ -> ()
    let rec loadRenderer obj =
        loadOne obj
        List.iter loadRenderer obj.Children

    let rec renderRecursive obj =
        match obj.Type with
        | Mesh m -> m.Render obj.Transform
        | Camera c -> c.Apply (Shader.ByName "") obj.Transform
        | Container -> ()
        List.iter renderRecursive obj.Children

    let rec unloadRenderer renderer =
        match renderer.Type with
        | Mesh m -> m.Unbake ()
        | _ -> ()
        List.iter unloadRenderer renderer.Children

    let rec updateRenderer deltaTime obj =
        let updated = obj.Script.Update obj deltaTime
        loadOne updated
        let children = List.map (updateRenderer deltaTime) updated.Children
        { updated with Children = children }

    override this.OnLoad e =
        GL.ClearColor (0.0f, 0.0f, 0.0f, 1.0f)
        List.iter GL.Enable [
            EnableCap.Multisample;
            EnableCap.AlphaTest;
            EnableCap.Blend;
            EnableCap.CullFace;
            EnableCap.DepthTest
        ]
        loadRenderer currentScene
        base.OnLoad e

    override this.OnUnload e =
        Shader.DisposeShaders ()
        unloadRenderer currentScene
        base.OnUnload e

    override this.OnResize e =
        GL.Viewport (0, 0, this.Width, this.Height)
        base.OnResize e

    override this.OnUpdateFrame e =
        currentScene <- updateRenderer e.Time currentScene
        base.OnUpdateFrame e

    override this.OnRenderFrame e =
        GL.Clear (ClearBufferMask.ColorBufferBit ||| ClearBufferMask.DepthBufferBit)
        renderRecursive currentScene
        this.Context.SwapBuffers ()
        base.OnRenderFrame e
