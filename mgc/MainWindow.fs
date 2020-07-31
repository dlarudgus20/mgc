namespace Mgc

open System
open OpenTK
open OpenTK.Graphics
open OpenTK.Graphics.OpenGL

open Mgc

// SceneObject with renderer
type SceneObjWR =
    | MeshWR of mesh: Mesh * renderer: MeshRenderer
    | ContainerWR of SceneObjWR list

type MainWindow(scene: SceneObject) =
    inherit GameWindow(800, 600, GraphicsMode.Default, "LearnOpenTK")

    let rec loadRenderer = function
    | Mesh m -> MeshWR (m, new MeshRenderer(m))
    | Container lst -> ContainerWR (List.map loadRenderer lst)

    let rec render = function
    | MeshWR (_, r) -> r.Render ()
    | ContainerWR lst -> List.iter render lst

    let rec unloadRenderer = function
    | MeshWR (_, r) -> using r ignore
    | ContainerWR lst -> List.iter unloadRenderer lst

    let mutable sceneRenderer = None

    override this.OnLoad e =
        GL.ClearColor (0.0f, 0.0f, 0.0f, 1.0f)
        List.iter GL.Enable [
            EnableCap.AlphaTest;
            EnableCap.Blend;
            EnableCap.CullFace;
            EnableCap.DepthTest
        ]
        sceneRenderer <- Some (loadRenderer scene)
        base.OnLoad e

    override this.OnUnload e =
        Shader.DisposeShaders ()
        sceneRenderer |> Option.iter unloadRenderer
        sceneRenderer <- None
        base.OnUnload e

    override this.OnResize e =
        GL.Viewport (0, 0, this.Width, this.Height)
        base.OnResize e

    override this.OnUpdateFrame e =
        base.OnUpdateFrame e

    override this.OnRenderFrame e =
        GL.Clear (ClearBufferMask.ColorBufferBit ||| ClearBufferMask.DepthBufferBit)

        sceneRenderer |> Option.iter render

        this.Context.SwapBuffers ()
        base.OnRenderFrame e
