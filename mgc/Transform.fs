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

type Transform with
    static member (*) (a, b) =
        let p = a.Position + (Transform.rotateVector a.Rotation (a.Scale * b.Position))
        let r = a.Rotation * b.Rotation
        let s = (Transform.rotateVector b.Rotation a.Scale) * b.Scale
        { Position = p; Rotation = r; Scale = s }
