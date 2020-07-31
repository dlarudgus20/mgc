namespace Mgc

open System
open OpenTK.Graphics.OpenGL

type MeshRenderer private (vao: int, vbo: int, shader: Shader, count: int, mode: PrimitiveType, vertices: single[]) =
    let mutable disposed = false
    let dispose disposing =
        if not disposed then
            disposed <- true
            GL.BindBuffer (BufferTarget.ArrayBuffer, 0)
            GL.DeleteBuffer vbo

    new (mesh: Mesh) =
        let vao = GL.GenVertexArray ()
        let vbo = GL.GenBuffer ()
        let shader = Shader.ByName mesh.Shader
        let layout = shader.VerticesLayout
        let vertices = mesh.Vertices

        let stride = List.sum layout
        let count = (Array.length vertices) / stride
        if (Array.length vertices) % stride <> 0 then
            failwith "Mesh vertices does not fit in the layout of the input of shader."

        GL.BindVertexArray vao
        GL.BindBuffer (BufferTarget.ArrayBuffer, vbo)
        GL.BufferData (BufferTarget.ArrayBuffer, (Array.length vertices) * sizeof<single>, vertices, BufferUsageHint.StaticDraw)

        let rec attrib (index: int) (offset: int) = function
        | dim :: xs ->
            GL.VertexAttribPointer (index, dim, VertexAttribPointerType.Float, false, stride * sizeof<single>, offset)
            GL.EnableVertexAttribArray index
            attrib (index + 1) (offset + dim * sizeof<single>) xs
        | [] -> ()
        attrib 0 0 layout
        GL.BindVertexArray 0

        new MeshRenderer(vao, vbo, shader, count, mesh.Mode, vertices)

    member this.Render () =
        shader.Use ()
        GL.BindVertexArray vao
        GL.DrawArrays (mode, 0, count)

    override this.Finalize () = dispose false
    interface IDisposable with member this.Dispose () = dispose true
