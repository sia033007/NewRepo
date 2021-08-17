using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SampleCommand : MonoBehaviour
{
    public static SampleCommand instance;
    public delegate void ExampleDelegate();
    public static ExampleDelegate exampleDelegate;
    public static event ExampleDelegate exampleDelegate1;

    public static event Action updatescore;
    // Start is called before the first frame update
    private void Awake() {
        instance = this;
    }
    void Start()
    {
        exampleDelegate += MyFunction;
        
    }

    // Update is called once per frame
    void Update()
    {
        exampleDelegate?.Invoke();
        if(Input.GetButton("Fire1"))
        {
            updatescore?.Invoke();
        }
        
    }
    public void MyFunction()
    {
        Debug.Log("Access Denied!");
    }
}
