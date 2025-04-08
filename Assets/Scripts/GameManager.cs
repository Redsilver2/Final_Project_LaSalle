using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player Inputs")]
    [SerializeField] private float cameraMovementSpeed      = 10f;
    [SerializeField] private float cameraSizeIncrementSpeed = 5f;
    [SerializeField] private float spacingIncrementSpeed    = 5f;

    [Space]
    [Header("Prefab")]
    [SerializeField] private DataVisualizer visualizerPrefab;

    [Space]
    [Header("List Datas")]
    [SerializeField] private List<string> names;


    private float currentHorizontalSpacing = 10f;
    private const float MIN_HORIZONTAL_SPACING = 3f;
    private const float MAX_HORIZONTAL_SPACING = 10f;

    private       float currentOrthographicSize;
    private const float MIN_ORTHOGRAPHIC_SIZE = 1.5f;
    private const float MAX_ORTHOGRAPHIC_SIZE = 5f;

    private Vector2 movementDirection;
    private DataVisualizationMode currentVisualizationMode;

    private List<DataVisualizer> dataVisualizers;

    private void Awake()
    {
        dataVisualizers          = new List<DataVisualizer>();
        currentOrthographicSize  = MAX_ORTHOGRAPHIC_SIZE;
        currentHorizontalSpacing = MAX_HORIZONTAL_SPACING;

        if(visualizerPrefab != null) 
            visualizerPrefab.gameObject.SetActive(false);   

        DisplayListData();
    }

    private void Update()
    {
        ListInput();
        UpdateHorizontalSpacing();
        UpdateOrthographicSize();

        UpdateMovementDirection();
        UpdateDataVisualizers();
    }

    private void LateUpdate()
    {
        Move();
        Zoom();
    }

    private void ListInput()
    {
        // Checks Input And Deletes Last Index
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (names.Count > 0)
                names.RemoveAt(0);
        }

        //Checks Input And Adds Random Number
        if (Input.GetKeyUp(KeyCode.T))
            names.Add(((int)Random.Range(0, 1000)).ToString());
    }

    private void UpdateDataVisualizers()
    {
        // Check If Data Visualizers Is Not Null
        if(dataVisualizers != null)
            // Check The Current Visualization Mode And Updates Values
            switch (currentVisualizationMode)
            {
                case DataVisualizationMode.List:
                    DisplayListData();
                break;
            }
    }

    private void UpdateMovementDirection()
    {
        // Updates Movement Direction
        movementDirection = Vector3.up    * cameraMovementSpeed * Input.GetAxis("Vertical") +
                            Vector3.right * cameraMovementSpeed * Input.GetAxis("Horizontal");
    }

    private void UpdateOrthographicSize()
    {
        // Zooms Out Camera
        if (Input.GetKey(KeyCode.E))
            currentOrthographicSize += Time.deltaTime * cameraSizeIncrementSpeed;

        // Zooms In Camera
        if (Input.GetKey(KeyCode.Q))
            currentOrthographicSize -= Time.deltaTime * cameraSizeIncrementSpeed;

        // Clamp Camera's Orthographic Size
        currentOrthographicSize = Mathf.Clamp(currentOrthographicSize, MIN_ORTHOGRAPHIC_SIZE, MAX_ORTHOGRAPHIC_SIZE);
    }


    private void UpdateHorizontalSpacing()
    {
        // Puts Data Visualizers Further Horizontaly From One And Another
        if (Input.GetKey(KeyCode.Z))
            currentHorizontalSpacing += Time.deltaTime * spacingIncrementSpeed;

        // Puts Data Visualizers Closer Horizontaly Frpm One And Another
        if (Input.GetKey(KeyCode.X))
            currentHorizontalSpacing -= Time.deltaTime * spacingIncrementSpeed;

        // Clamps Horizontal Spacing
        currentHorizontalSpacing = Mathf.Clamp(currentHorizontalSpacing, MIN_HORIZONTAL_SPACING, MAX_HORIZONTAL_SPACING);
    }

    private void Zoom()
    {
        // Grabs The Reference For The Main Camera
        Camera camera = Camera.main;

        // Check If Camera Is Not Null
        if (camera != null)
            camera.orthographicSize = currentOrthographicSize;
    }
    private void Move()
    {
        // Move The Current Tranform's Position
        transform.position += Time.deltaTime * (Vector3.up * movementDirection.y +
                                              Vector3.right * movementDirection.x);
    }


    private void DisplayListData()
    {
        // Checks If Names List Is Valid
        if (names == null) return;

        for (uint i = 0; i < names.Count; i++)
        {
            // Grabs The Visualizer By Index In Data Visualizer List
            DataVisualizer visualizer = GetDataVisualizerByIndex(i);

            // Sets Data Visualizer Position 
            visualizer.transform.position = Vector3.right * i * currentHorizontalSpacing;

            // Updates Index Text And Data Text
            visualizer.UpdateDisplayers(i + 1, names[(int)i]);

            // Check if The Index Is More Than 0 And Sets The Previous Visualizer's LineRenderer Positions
            if (i > 0)
                GetDataVisualizerByIndex(i - 1).UpdateLineRendererPositions(visualizer);
        }

        // Check If Name Array Is More Than 0
        if (names.Count > 0)
        {
            // Gets Last Visualizer And Hides LineRenderer
            GetDataVisualizerByIndex((uint)names.Count - 1).SetLineRendererEnabledState(false);
          
            // Sets Visualizers Visibility State
            SetVisualizersActiveState((uint)names.Count);
        }
        else
        {
            SetVisualizersActiveState(0);
        }
    }

    private void SetVisualizersActiveState(uint visualizersActif)
    {
        // Loops Through Data Visualizers List
        for (uint i = 0; i < dataVisualizers.Count; i++)
        {
            // Grabs Data Visaulizer In Data Visualizer List By Index.
            DataVisualizer visualizer = dataVisualizers[(int)i];
            
            // Checks If Data Visualiser Is Null
            if (visualizer == null) continue;

            // Check If Index Is More Than Visualizers Actif (> = Disable Visualizer | <= Enable Visualizer) 
            if (i >= visualizersActif) 
                visualizer.gameObject.SetActive(false);
            else
                visualizer.gameObject.SetActive(true);
        }
    }

    private DataVisualizer AddAndGetDataVisualizer()
    {
        // Instanciate New Data Visualizer
        DataVisualizer dataVisualizer = Instantiate(visualizerPrefab);
       
        // Sets Visualizer's Line Renderer
        dataVisualizer.SetLineRenderer();

        // Add Visualizer To The List
        dataVisualizers.Add(dataVisualizer);

        // Return The Newely Instanciated Visualizer
        return dataVisualizer;
    }

    private DataVisualizer GetDataVisualizerByIndex(uint index)
    {
        // Check If Index Is Bigger Than Data Visualizer Array Count
        if (index >= dataVisualizers.Count) return AddAndGetDataVisualizer();

        // Return The Visualizer From The List
        return dataVisualizers[(int)index];
    }

    private enum DataVisualizationMode
    {
        List
    }
}
