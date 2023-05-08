using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        Debug.Log("=========================================");

        beforePos.SetVector(position, "pos");
        beforeRot.SetVector(rotation, "rot");
        beforeScale.SetVector(scale, "scale");

        before.localEulerAngles = rotation;
        before.localScale = scale;

        var matrix = Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);
        matrixText.SetMatrix(matrix, "matrix");

        // 通过unity api 计算
        afterPos.SetVector(matrix.GetPosition(), "pos by unity api");
        afterRot.SetVector(matrix.rotation.eulerAngles, "rot by unity api");
        afterScale.SetVector(matrix.lossyScale, "scale by unity api");

        after.localEulerAngles = matrix.rotation.eulerAngles;
        after.localScale = matrix.lossyScale;


        // 自己计算
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

        //基向量
        Vector3 vecX = new Vector3(1, 0, 0);
        Vector3 vecY = new Vector3(0, 1, 0);
        //变换后基向量
        Vector3 vecTx = matrix2.MultiplyPoint(vecX);
        Vector3 vecTy = matrix2.MultiplyPoint(vecY);

        Debug.Log($"vecTx {vecTx.magnitude}   vecTy  {vecTy.magnitude}");
        // 缩放
        var cross1 = Vector3.Cross(vecX, vecY);
        var cross2 = Vector3.Cross(vecTx, vecTy);
        float scaleSign = cross1.z * cross2.z >= 0 ? 1 : -1;
        Debug.Log($"scaleSign {scaleSign}");

        //旋转
        var angle = Vector2.Angle(vecX, vecTx);
        var crossX = Vector3.Cross(vecX, vecTx);
        var crossY = Vector3.Cross(vecY, vecTy);
        float realAngle;
        if (crossY.z < 0)
            if (crossX.z > 0)
                realAngle = angle + 180;
            else
                realAngle = 360 - angle;
        else if (crossX.z < 0)
            realAngle = 180 - angle;
        else
            realAngle = angle;

        Debug.Log($"angle {angle}  crossX {crossX} crossY {crossY} realAngle  {realAngle}");

        var pos2 = new Vector3(tx, ty, 0);
        var scale2 = new Vector3(scaleSign * vecTx.magnitude, vecTy.magnitude, 0);
        var rot2 = new Vector3(0, 0, realAngle);

        v2dPos.SetVector(pos2, "pos");
        v2dRot.SetVector(rot2, "rot");
        v2dScale.SetVector(scale2, "scale");

        t2d.localEulerAngles = rot2;
        t2d.localScale = scale2;
    }


    //[Space(10)]
    //public Vector3 CrossV1;
    //public Vector3 CrossV2;
    //[ContextMenu("TestCross")]
    //public void TestCross()
    //{
    //    var cross = Vector3.Cross(CrossV1, CrossV2);
    //    var angle = Vector3.Angle(CrossV1, CrossV2);
    //    var realAngle = cross.z > 0 ? angle : 360 - angle;
    //    Debug.Log($"test : {CrossV1} cross {CrossV2}  = {cross}     angle = {realAngle}");
    //}
}
