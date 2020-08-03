namespace Mgc

open OpenTK

type Transform = {
    Position: Vector3
    Rotation: Quaternion
    Scale: Vector3
}

module Transform =
    let identity = { Position = Vector3.Zero; Rotation = Quaternion.Identity; Scale = Vector3.One }

    let rotateVector (q: Quaternion) (p: Vector3) =
        (q * Quaternion (p, 1.0f) * Quaternion.Conjugate q).Xyz

    let translate x tr =
        { tr with Position = x + tr.Position }
    let translate3 x y z = translate (Vector3 (x, y, z))
    let translateX x = translate3 x 0.0f 0.0f
    let translateY y = translate3 0.0f y 0.0f
    let translateZ z = translate3 0.0f 0.0f z

    let rotate q tr =
        let p = rotateVector q tr.Position
        let r = q * tr.Rotation
        { tr with Position = p; Rotation = r }
    let rotateAxisAngle axis angle = rotate (Quaternion.FromAxisAngle (axis, angle))
    let rotateEuler euler = rotate (Quaternion.FromEulerAngles euler)
    let rotateEuler3 pitch roll yaw = rotateEuler (Vector3 (pitch, roll, yaw))
    let rotateX angle = rotateAxisAngle (Vector3 (1.0f, 0.0f, 0.0f)) angle
    let rotateY angle = rotateAxisAngle (Vector3 (0.0f, 1.0f, 0.0f)) angle
    let rotateZ angle = rotateAxisAngle (Vector3 (0.0f, 0.0f, 1.0f)) angle


    let scale x tr =
        let p = x * tr.Position
        let s = (rotateVector tr.Rotation x) * tr.Scale
        { tr with Position = p; Scale = s }
    let scale3 x y z = scale (Vector3 (x, y, z))
    let scaleX x = scale3 x 0.0f 0.0f
    let scaleY y = scale3 0.0f y 0.0f
    let scaleZ z = scale3 0.0f 0.0f z

    let toMatrix tr =
        let t = Matrix4.CreateTranslation tr.Position
        let r = Matrix4.CreateFromQuaternion tr.Rotation
        let s = Matrix4.CreateScale tr.Scale
        // OpenTK matrix multiplication order is reversed by its historical reason
        // translation * rotation * scaling
        s * r * t

    let composite a b =
        translate a.Position (rotate a.Rotation (scale a.Scale b))

type Transform with
    static member (*) (a, b) = Transform.composite a b
