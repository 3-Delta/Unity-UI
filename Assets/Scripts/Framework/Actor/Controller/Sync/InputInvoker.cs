using UnityEngine;

public struct Test {
    uint value;
    private string s;
    private long l;
}

// 其实是ai,执行opcmd
// 这为了不引入行为树，简单对待
[DisallowMultipleComponent]
public class InputInvoker : MonoBehaviour {
    public float moveSpeed = 4;

    public void Exec(ref OpCmd cmd, Transform target) {
        if (cmd.HasMoveInput) {
            Vector3 movingDir = Vector3.zero;
            if ((cmd.input.move & EMoveKey.Forward) != 0) {
                movingDir.z += 1;
            }

            if ((cmd.input.move & EMoveKey.Backward) != 0) {
                movingDir.z -= 1;
            }

            if ((cmd.input.move & EMoveKey.Left) != 0) {
                movingDir.x += 1;
            }

            if ((cmd.input.move & EMoveKey.Right) != 0) {
                movingDir.x -= 1;
            }

            Vector3 velocity = movingDir * moveSpeed;
            var position = target.position = transform.position + velocity * Time.fixedDeltaTime;
            cmd.output.position = position;
        }
    }
}
