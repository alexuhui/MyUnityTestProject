using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefTest : MonoBehaviour
{
    [RefSearch]
    public GameObject go;
    [RefSearch]
    public Transform tran;
}
