using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player Inputs")]
    [SerializeField] private float cameraMovementSpeed      = 10f;
    [SerializeField] private float cameraSizeIncrementSpeed = 5f;

    [Space]
    [Header("Datas")]
    [SerializeField] private List<string> names;

    private float minOrthographicSize = 1.5f;
    private float maxOrthographicSize = 5f;
    private float currentOrthographicSize;

    private DataVisualizationMode currentVisualizationMode;
    private Vector2 movementDirection;

    private void Awake()
    {
        currentOrthographicSize = maxOrthographicSize; 
    }

    private void Update()
    {
        UpdateOrthographicSize();
        UpdateMovementDirection();
    }

    private void LateUpdate()
    {
        Move();
        Zoom();
    }

    private void UpdateMovementDirection()
    {
        movementDirection = Vector3.up    * cameraMovementSpeed * Input.GetAxis("Vertical") +
                            Vector3.right * cameraMovementSpeed * Input.GetAxis("Horizontal");
    }
    private void UpdateOrthographicSize()
    {
        if (Input.GetKey(KeyCode.E))
        {
            currentOrthographicSize += Time.deltaTime * cameraSizeIncrementSpeed;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            currentOrthographicSize -= Time.deltaTime * cameraSizeIncrementSpeed;
        }

        currentOrthographicSize = Mathf.Clamp(currentOrthographicSize, minOrthographicSize, maxOrthographicSize);

    }

    private void Zoom()
    {
        Camera camera = Camera.main;

        if (camera != null)
            camera.orthographicSize = currentOrthographicSize;
    }
    private void Move()
    {
        transform.position += Time.deltaTime * (Vector3.up * movementDirection.y +
                                              Vector3.right * movementDirection.x);
    }


    private void SetTableVisualization()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && TrySetSelectedVisualizationMode(DataVisualizationMode.Table))
        {
            
        }
    }

    private bool TrySetSelectedVisualizationMode(DataVisualizationMode visualizationMode)
    {
        if(visualizationMode != currentVisualizationMode)
        {
            currentVisualizationMode = visualizationMode;
            return true;
        }

        return false;
    }

    private enum DataVisualizationMode
    {
        Table
    }
}
