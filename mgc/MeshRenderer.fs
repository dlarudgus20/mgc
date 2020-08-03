namespace Mgc

open System
open OpenTK.Graphics.OpenGL

type MeshRenderData = {
    VAO: int
    VBO: int
    Shader: Shader
    Count: int
    Mode: PrimitiveType
    Vertices: single[]
}

type MeshData (shader: string, mode: PrimitiveType, vertices: single[]) =
    let mutable data = None

    let bake () =
        let vao = GL.GenVertexArray ()
        let vbo = GL.GenBuffer ()
        let shader = Shader.ByName shader
        let layout = shader.VerticesLayout

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

        {
            VAO = vao
            VBO = vbo
            Shader = shader
            Count = count
            Mode = mode
            Vertices = vertices
        }

    let unbake () =
        data |> Option.iter (fun d ->
            GL.BindBuffer (BufferTarget.ArrayBuffer, 0)
            GL.DeleteBuffer d.VBO
            data <- None)

    let mutable disposed = false
    let dispose disposing =
        if not disposed then
            disposed <- true
            unbake ()

    member this.Bake () =
        match data with
        | Some d -> ()
        | None -> data <- Some (bake ()); ()

    member this.Unbake () = unbake ()

    member this.IsBaked = data.IsSome

    member this.Render (transform: Transform) =
        match data with
        | Some d ->
            d.Shader.Use ()
            d.Shader.SetUniform ("vmMatrix", Transform.toMatrix transform)
            GL.BindVertexArray d.VAO
            GL.DrawArrays (d.Mode, 0, d.Count)
        | None -> failwith "MeshData is not baked"

    override this.Finalize () = dispose false
    interface IDisposable with member this.Dispose () = dispose true
