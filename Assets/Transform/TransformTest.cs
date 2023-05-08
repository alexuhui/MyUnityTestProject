using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTest : MonoBehaviour
{
    public RectTransform before;
    public RectTransform after;


    public VectorText beforePos;
    public VectorText beforeRot;
    public VectorText beforeScale;

    public VectorText afterPos;
    public VectorText afterRot;
    public VectorText afterScale;

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
    }
}
