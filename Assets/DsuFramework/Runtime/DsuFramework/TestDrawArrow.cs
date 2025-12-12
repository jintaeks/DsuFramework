using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dsu.Framework;

public class TestDrawArrow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(transform.position, transform.forward * 3.0f);
        DrawArrow.ForDebug(transform.position, transform.forward * 3.0f);
        //Debug.LogFormat("{0} Update", transform.name);
    }

    private void OnDrawGizmos()
    {
        DrawArrow.ForGizmo(transform.position, transform.up * 3.0f, Color.magenta);
    }
}
