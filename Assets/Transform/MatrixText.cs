using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class MatrixText : Text
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void SetMatrix(Matrix4x4 matrix,string header = "")
    {
        StringBuilder sb = new StringBuilder();
        if(!string.IsNullOrEmpty(header) )
        {
            sb.Append($"{header}:\n");
        }
        sb.Append($"{matrix.m00.ToString("f3"),-6}    {matrix.m01.ToString("f3"),-6}    {matrix.m02.ToString("f3"),-6}    {matrix.m03.ToString("f3"),-6}\n");
        sb.Append($"{matrix.m10.ToString("f3"),-6}    {matrix.m11.ToString("f3"),-6}    {matrix.m12.ToString("f3"),-6}    {matrix.m13.ToString("f3"),-6}\n");
        sb.Append($"{matrix.m20.ToString("f3"),-6}    {matrix.m21.ToString("f3"),-6}    {matrix.m22.ToString("f3"),-6}    {matrix.m23.ToString("f3"),-6}\n");
        sb.Append($"{matrix.m30.ToString("f3"),-6}    {matrix.m31.ToString("f3"),-6}    {matrix.m32.ToString("f3"),-6}    {matrix.m33.ToString("f3"),-6}");

        text = sb.ToString();
    }
}
