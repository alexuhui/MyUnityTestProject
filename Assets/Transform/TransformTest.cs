using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTest : MonoBehaviour
{
    public RectTransform before;
    public RectTransform after;
    public RectTransform t2d;


    public VectorText beforePos;
    public VectorText beforeRot;
    public VectorText beforeScale;

    public VectorText afterPos;
    public VectorText afterRot;
    public VectorText afterScale;

    public VectorText v2dPos;
    public VectorText v2dRot;
    public VectorText v2dScale;

    public MatrixText matrixText;

    public Vector3 position = new Vector3(2 , 2, 0);
    public Vector3 rotation = new Vector3(0, 0, 0);
    public Vector3 scale = new Vector3(-1, 1, 1);

    // Start is called before the first frame update
    void Start()
    {
        TransTest();
    }


    [ContextMenu("TransTest")]
    public void TransTest()
    {
        beforePos.SetVector(position, "pos");
        beforeRot.SetVector(rotation, "rot");
        beforeScale.SetVector(scale, "scale");

        before.localEulerAngles = rotation;
        before.localScale = scale;

        var matrix = Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);
        matrixText.SetMatrix(matrix, "matrix");

        afterPos.SetVector(matrix.GetPosition(), "pos from mat");
        afterRot.SetVector(matrix.rotation.eulerAngles, "rot from mat");
        afterScale.SetVector(matrix.lossyScale, "scale from mat");

        after.localEulerAngles = matrix.rotation.eulerAngles;
        after.localScale = matrix.lossyScale;


        // 2d 方式计算
        float a = matrix.m00, c = matrix.m01;
        float b = matrix.m10, d = matrix.m11;
        float tx = position.x;
        float ty = position.y;

        var matrix2 = new Matrix4x4(
                     new Vector4(0, 0, 0, 0),
                     new Vector4(0, 0, 0, 0),
                     new Vector4(0, 0, 0, 0),
                     new Vector4(0, 0, 0, 1)
                     );
        matrix2.m00 = a; matrix2.m01 = c;
        matrix2.m10 = b; matrix2.m11 = d;

        var _2dpos = new Vector2(tx, ty);
        var _2drot = matrix2.rotation;
        var _2dscale = matrix2.lossyScale;

        v2dPos.SetVector(_2dpos, "2d pos");
        v2dRot.SetVector(_2drot.eulerAngles, "2d rot");
        v2dScale.SetVector(_2dscale, "2d scale");

        t2d.localEulerAngles = _2drot.eulerAngles;
        t2d.localScale = _2dscale;

    }
}
