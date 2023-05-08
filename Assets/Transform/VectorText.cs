using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class VectorText : Text
{

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetVector(Vector2 v2, string header = "")
    {
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(header))
        {
            sb.Append($"{header}:\n");
        }
        sb.Append($"{v2.x.ToString("f3"),-6}    {v2.y.ToString("f3"),-6}");

        text = sb.ToString();
    }

    public void SetVector(Vector3 v3, string header = "")
    {
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(header))
        {
            sb.Append($"{header}:\n");
        }
        sb.Append($"{v3.x.ToString("f3"),-6}    {v3.y.ToString("f3"),-6}    {v3.z.ToString("f3"), - 6}");

        text = sb.ToString();
    }
}
