namespace Mgc

open System
open System.IO
open System.Text
open OpenTK
open OpenTK.Graphics.OpenGL

type Shader(name: string, layout: int list) =
    static let shaderDirectory = "shaders"
    static let defaultShader = "shader"
    static let shaderList = [
        defaultShader, [ 3; 3 ]
    ]

    static let shaderMap =
        let mp = Map.ofList (List.map (fun (n, l) -> n, new Shader(n, l)) shaderList)
        Map.add "" (Map.find defaultShader mp) mp

    static let mutable shaderInUsing = None

    let vertPath = Path.Combine (shaderDirectory, name + ".vert")
    let fragPath = Path.Combine (shaderDirectory, name + ".frag")

    let vertCode = using (new StreamReader (vertPath, Encoding.UTF8)) (fun stream ->
        stream.ReadToEnd ())
    let fragCode = using (new StreamReader (fragPath, Encoding.UTF8)) (fun stream ->
        stream.ReadToEnd ())

    let vertShader = GL.CreateShader ShaderType.VertexShader
    let fragShader = GL.CreateShader ShaderType.FragmentShader

    let program = GL.CreateProgram ()

    let mutable disposed = false
    let dispose this disposing =
        if not disposed then
            disposed <- true
            if shaderInUsing = Some this then shaderInUsing <- None
            GL.DetachShader (program, vertShader)
            GL.DetachShader (program, fragShader)
            GL.DeleteShader vertShader
            GL.DeleteShader fragShader

    do
        GL.ShaderSource (vertShader, vertCode)
        GL.ShaderSource (fragShader, fragCode)

        GL.CompileShader vertShader
        match GL.GetShaderInfoLog vertShader with
        | "" -> ()
        | err -> failwith (sprintf "[%s] vertex shader compile error: %s" name err)
        GL.CompileShader fragShader

        match GL.GetShaderInfoLog fragShader with
        | "" -> ()
        | err -> failwith (sprintf "[%s] fragment shader compile error: %s" name err)

        GL.AttachShader (program, vertShader)
        GL.AttachShader (program, fragShader)
        GL.LinkProgram program

        match GL.GetProgramInfoLog program with
        | "" -> ()
        | err -> failwith (sprintf "[%s] shader link error: %s" name err)

    static member ByName name = Map.find name shaderMap

    static member DisposeShaders () =
        shaderMap |> Map.iter (fun _ shader -> shader.Dispose ())

    static member ClearProgram () =
        shaderInUsing <- None
        GL.UseProgram 0
    static member UseByName name = (Shader.ByName name).Use ()

    member this.SetUniform (name: string, value: single) =
        GL.Uniform1 (GL.GetUniformLocation (program, name), value)
    member this.SetUniform (name: string, value: Vector2) =
        GL.Uniform2 (GL.GetUniformLocation (program, name), value)
    member this.SetUniform (name: string, value: Vector3) =
        GL.Uniform3 (GL.GetUniformLocation (program, name), value)
    member this.SetUniform (name: string, value: Vector4) =
        GL.Uniform4 (GL.GetUniformLocation (program, name), value)
    member this.SetUniform (name: string, value: Matrix2) =
        let mutable v = value in GL.UniformMatrix2 (GL.GetUniformLocation (program, name), false, &v)
    member this.SetUniform (name: string, value: Matrix3) =
        let mutable v = value in GL.UniformMatrix3 (GL.GetUniformLocation (program, name), false, &v)
    member this.SetUniform (name: string, value: Matrix4) =
        let mutable v = value in GL.UniformMatrix4 (GL.GetUniformLocation (program, name), false, &v)

    member this.Use () =
        if shaderInUsing <> Some this then
            shaderInUsing |> Option.iter (fun _ -> GL.UseProgram 0)
            GL.UseProgram program
            shaderInUsing <- Some this

    member this.VerticesLayout = layout

    override this.Finalize () = dispose this false
    member this.Dispose () = dispose this true; GC.SuppressFinalize true
    interface IDisposable with member this.Dispose () = this.Dispose ()
