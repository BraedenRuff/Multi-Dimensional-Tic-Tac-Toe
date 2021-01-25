using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_Rotation : MonoBehaviour {

    float _sensitivity;
    private bool _isRotating;
    private bool _isRotatingWeird;
    private bool _isRotatingWithKeys;
    private int directionVertical;
    private int directionHorizontal;
    private bool _isRotatingWeirdWithKeys;
    GameObject alpha;
    GameObject beta;
    GameObject gamma;


    void Start()
    {
        _sensitivity = 300f;
        alpha = GameObject.FindGameObjectWithTag("Alpha");
        beta = GameObject.FindGameObjectWithTag("Beta");
        gamma = GameObject.FindGameObjectWithTag("Gamma");
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 20))
        {
            
        }
        else
        {
            if (hit.collider == null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _isRotating = true;

                }

                
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            _isRotating = true;
        }
        if (Input.GetMouseButtonDown(2))
        {
            _isRotatingWeird = true;
        }
        if (_isRotating)
        {
            alpha.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0) * Time.deltaTime * _sensitivity, Space.World);
            beta.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0) * Time.deltaTime * _sensitivity, Space.World);
            gamma.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0) * Time.deltaTime * _sensitivity, Space.World);
        }
        if (_isRotatingWeird)
        {
            alpha.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, -Input.GetAxis("Mouse X")) * Time.deltaTime * _sensitivity, Space.World);
            beta.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, -Input.GetAxis("Mouse X")) * Time.deltaTime * _sensitivity, Space.World);
            gamma.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, -Input.GetAxis("Mouse X")) * Time.deltaTime * _sensitivity, Space.World);
        }
        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(0))
        {
            _isRotating = false;
            
        }
        if (Input.GetMouseButtonUp(2))
        {
            _isRotatingWeird = false;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            _isRotatingWithKeys = true;
            directionVertical = 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _isRotatingWithKeys = true;
            directionVertical = 2;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _isRotatingWithKeys = true;
            directionHorizontal = 3;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _isRotatingWithKeys = true;
            directionHorizontal = 4;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            directionVertical = 0;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            directionVertical = 0;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            directionHorizontal = 0;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            directionHorizontal = 0;
        }
        if (_isRotatingWithKeys && !_isRotatingWeirdWithKeys)
        {
            switch (directionVertical)
            {
                case 1:
                    alpha.transform.Rotate(new Vector3(1, 0, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    beta.transform.Rotate(new Vector3(1, 0, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    gamma.transform.Rotate(new Vector3(1, 0, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    break;
                case 2:
                    alpha.transform.Rotate(new Vector3(-1, 0, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    beta.transform.Rotate(new Vector3(-1, 0, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    gamma.transform.Rotate(new Vector3(-1, 0, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    break;
            }
            switch (directionHorizontal)
            {
                case 3:
                    alpha.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    beta.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    gamma.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    break;
                case 4:
                    alpha.transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    beta.transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    gamma.transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    break;
            }
        }
        if (_isRotatingWithKeys && _isRotatingWeirdWithKeys)
        {
            switch (directionVertical)
            {
                case 1:
                    alpha.transform.Rotate(new Vector3(1, 0, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    beta.transform.Rotate(new Vector3(1, 0, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    gamma.transform.Rotate(new Vector3(1, 0, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    break;
                case 2:
                    alpha.transform.Rotate(new Vector3(-1, 0, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    beta.transform.Rotate(new Vector3(-1, 0, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    gamma.transform.Rotate(new Vector3(-1, 0, 0) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    break;
            }
            switch (directionHorizontal)
            {
                case 3:
                    alpha.transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    beta.transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    gamma.transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    break;
                case 4:
                    alpha.transform.Rotate(new Vector3(0, 0, -1) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    beta.transform.Rotate(new Vector3(0, 0, -1) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    gamma.transform.Rotate(new Vector3(0, 0, -1) * Time.deltaTime * _sensitivity * 0.5f, Space.World);
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            _isRotatingWeirdWithKeys = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            _isRotatingWeirdWithKeys = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _sensitivity *= 0.2f;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _sensitivity *= 5f;
        }
    }

}
