using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParent : MonoBehaviour
{
    public GameObject Child;

    // Start is called before the first frame update
    void Start()
    {
        var child1 = Instantiate(Child);
        child1.name = "worldPositionStays_false";
        child1.transform.SetParent(transform, false);

        var child2 = Instantiate(Child);
        child2.name = "worldPositionStays_true";
        child2.transform.SetParent(transform, true);

    }

}
