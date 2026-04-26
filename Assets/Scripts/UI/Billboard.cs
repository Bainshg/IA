using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hace que el Canvas del enemigo siempre mire a la cámara
public class Billboard : MonoBehaviour
{
    
    private Camera _mainCamera;

    void Start() => _mainCamera = Camera.main;

    void LateUpdate()
    {
    
        transform.forward = _mainCamera.transform.forward;
    }
}