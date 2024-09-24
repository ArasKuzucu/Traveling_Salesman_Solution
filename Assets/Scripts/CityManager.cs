using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class CityManager : MonoBehaviour
{
    public GameObject CityPrefab; // Prefab for visual representation of a city
    public GameObject TextPrefab; // Prefab for the 3D text
    private int _cityNumber = 1; // Default Number of cities to generate
    public Vector2 PositionRange = new(-25f, 25f); // Range for random positions in the terrain

    public GameObject LineRendererPrefab; // Prefab for line renderer
    private readonly List<LineRenderer> _lineRenderers = new(); //Store LineRenderers into a list

    private readonly List<CityInfo> _cityInfos = new(); // Store references to all city objects
    private readonly List<GameObject> cityObjectList = new(); // Store objects we created in the scene 

    [SerializeField]
    private TextAsset cityNamesFile;  // Text file contains 81 cities in Turkiye plus some Canada cities :)

    public Terrain Terrain; // Reference to the terrain object to get height information
    public float CustomElevationValue;

    private readonly List<string> _cityNameList = new(); // Holding city names
    private readonly HashSet<string> _selectedCityNames = new();


    void Start()
    {
        // Reading CityName file and assign into a list for City object when app started    
        ReadTextFile();
    }

    // Randomly Generating non-repeating city names
    public string GenerateCityName(List<string> cityNameList)
    {

        if (_cityNameList?.Count == 0)
            ReadTextFile();

        //Debugging
        print(_cityNameList.Count);

        string selectedCity = _cityNameList[Random.Range(0, _cityNameList.Count)];

        while (!_selectedCityNames.Add(selectedCity))
            selectedCity = _cityNameList[Random.Range(0, _cityNameList.Count)];

        return selectedCity;
    }

    // Function to generate random cities
    public void GenerateCities(TMP_InputField userText)
    {
        // Check the user input is int if the input is invalid It shows an error message in the Conole Tab
        if (!int.TryParse(userText.text, out _cityNumber))
        {
            this.GetComponent<UserConsole>().AddObjectInfo("Invalid input please enter an integer");
            return;
        }

        for (int i = 0; i < _cityNumber; i++)
        {
            // Random position within range
            Vector3 randomPosition = new Vector3(Random.Range(-PositionRange.x, PositionRange.x), 0, Random.Range(-PositionRange.y, PositionRange.y));

            // Get the height of the terrain at the random X and Z position
            float terrainHeight = Terrain.SampleHeight(randomPosition);
            randomPosition.y = terrainHeight + Terrain.transform.position.y + CustomElevationValue; // Adjust the Y position based on terrain height

            // Add random city name from GenerateCityName method
            string newCityName = GenerateCityName(_cityNameList);
            City newCity = new City(newCityName, randomPosition); // Creating City object from the values we generated

            // Create city object (adding -90 degree rotation for city diagram to face camera) from the prefab
            GameObject cityObject = Instantiate(CityPrefab, randomPosition, Quaternion.Euler(0, -90, 0));
            cityObjectList.Add(cityObject);
            // Assign the random name we generated from the method to the GameObject
            cityObject.name = newCityName;

            // Add the CityInfo component to the city object
            CityInfo cityInfo = cityObject.AddComponent<CityInfo>();

            cityInfo.SetCityData(newCity, this); // Assigning both City object and GameObject into the CityInfo object

            // Add 3D text to display the city name
            CreateCityLabel(cityObject, newCityName);

            // Store the CityInfo reference
            _cityInfos.Add(cityInfo);
        }
    }

    // Method to create the 3D text label for a city
    void CreateCityLabel(GameObject cityObject, string cityName)
    {
        // Instantiate the text prefab above the city object
        Vector3 labelPosition = cityObject.transform.position + new Vector3(0, 4, 0); // Offset to place above the city object
        GameObject textObject = Instantiate(TextPrefab, labelPosition, Quaternion.identity);

        // Attach the text object to the city object as a child
        textObject.transform.SetParent(cityObject.transform);

        // Set the text value
        TextMeshPro textMesh = textObject.GetComponent<TextMeshPro>();
        textMesh.text = cityName;

        //Customize text properties 
        textMesh.fontSize = 24;
        textMesh.alignment = TextAlignmentOptions.Center;

    }

    // Draw a 3D Line on the scene taking the shortest path list from the selected algorithm
    public void DrawTour(List<CityInfo> tour)
    {
        ClearAllLines(); // Clear any existing lines before drawing new ones

        float lineYOffset = 1f; // Offset to raise the line above the terrain/city objects

        for (int i = 0; i < tour.Count; i++)
        {
            // Create a new LineRenderer object from the prefab
            GameObject lineObject = Instantiate(LineRendererPrefab);
            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

            _lineRenderers.Add(lineRenderer); // Store the line renderer for future reference

            // Set the positions for the LineRenderer
            Vector3 startPosition = tour[i].GetCityData().GetCityPositon();

            Vector3 endPosition = (i == tour.Count - 1) ? tour[0].GetCityData().GetCityPositon() : tour[i + 1].GetCityData().GetCityPositon();

            //Vector3 endPosition;
            //if (i == tour.Count - 1)
            //{
            //    // If we are at the last city, connect it to the first city
            //    endPosition = tour[0].GetCityData().GetCityPositon();
            //}
            //else
            //{
            //    // Otherwise, connect it to the next city
            //    endPosition = tour[i + 1].GetCityData().GetCityPositon();
            //}

            // Apply Y-axis offset to make the lines more visible above terrain
            startPosition.y += lineYOffset;
            endPosition.y += lineYOffset;

            lineRenderer.positionCount = 2; // Two points for the line
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);
        }
    }

    // Method to clear all existing lines from the scene
    public void ClearAllLines()
    {
        foreach (LineRenderer line in _lineRenderers)
        {
            Destroy(line.gameObject);
        }
        _lineRenderers.Clear();
    }

    private void ReadTextFile()
    {
        var content = cityNamesFile.text;
        var allNames = content.Split("\n"); //Environment.NewLine= "\n"?;
        _cityNameList.AddRange(allNames);
    }

    // Method to get a list of all cities (for distance and algorithm calculations)
    public List<CityInfo> GetAllCities() => _cityInfos;
    public void ClearAllCities()
    {
        // Destroying all cities from the scene
        foreach (GameObject cityObject in cityObjectList)
        {
            Destroy(cityObject);
        }

        // Clear any existing lines before drawing new ones
        ClearAllLines();
        // Clearing the cityinfo list
        _cityInfos.Clear();
        print(_cityNameList.Count);
        // Assigning the city names to the original name list      
        ReadTextFile();
    }
}
