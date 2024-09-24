using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearestNeighborSolver : MonoBehaviour
{
    private CityManager _cityManager;
    private List<CityInfo> _cities;
    
    void Start()
    {
        _cityManager = FindObjectOfType<CityManager>(); // Get the CityManager instance
    }

    // Triggers the algorithm to solve the TSP
    public void SolveTSP()
    {
        StartCoroutine(SolveTSPCoroutine());
    }

    //Using the Coroutine to update UI 
    private IEnumerator SolveTSPCoroutine()
    {
        _cities = _cityManager.GetAllCities(); // Get the list of all cities
        if (_cities == null || _cities.Count == 0)
        {
            Debug.LogError("No cities available!");
            _cityManager.GetComponent<UserConsole>().AddObjectInfo("No cities available!");
            yield break;
        }

        _cityManager.GetComponent<UserConsole>().AddObjectInfo("Algorithm started: 0% complete"); // Initial message

        List<CityInfo> bestTour = null;
        float shortestDistance = float.MaxValue;
        int totalIterations = _cities.Count;
        int progress = 0;

        // Loop through all cities as possible starting points
        for (int i = 0; i < totalIterations; i++)
        {
            CityInfo startCity = _cities[i];
            List<CityInfo> currentTour = NearestNeighbor(startCity);
            float currentDistance = CalculateTourDistance(currentTour);

            // Check if this tour is the shortest one so far
            if (currentDistance < shortestDistance)
            {
                shortestDistance = currentDistance;
                bestTour = currentTour;
            }

            // Calculate and update progress percentage
            int newProgress = Mathf.RoundToInt(((float)(i + 1) / totalIterations) * 100);
            if (newProgress > progress)
            {
                progress = newProgress;
                _cityManager.GetComponent<UserConsole>().AddObjectInfo($"Progress: {progress}% complete");
            }

            // Yield to allow UI to update
            yield return null;
        }

        DisplayTour(bestTour, shortestDistance);
        _cityManager.GetComponent<UserConsole>().AddObjectInfo("Algorithm completed: 100% complete"); // Final message
    }

    
    private List<CityInfo> NearestNeighbor(CityInfo startCity)
    {
        List<CityInfo> unvisitedCities = new List<CityInfo>(_cities);
        List<CityInfo> tour = new List<CityInfo>();

        CityInfo currentCity = startCity;
        tour.Add(currentCity);
        unvisitedCities.Remove(currentCity);

        // Keep visiting the nearest unvisited city until all cities are visited. Checking all possible starting cities
        while (unvisitedCities.Count > 0)
        {
            CityInfo nearestCity = null;
            float nearestDistance = float.MaxValue;

            foreach (var city in unvisitedCities)
            {
                float distance = Vector3.Distance(currentCity.GetCityData().GetCityPositon(), city.GetCityData().GetCityPositon());
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestCity = city;
                }
            }

            tour.Add(nearestCity);
            unvisitedCities.Remove(nearestCity);
            currentCity = nearestCity;
        }

        return tour;
    }

    // Calculate the total distance of the tour
    private float CalculateTourDistance(List<CityInfo> tour)
    {
        float totalDistance = 0;
        for (int i = 0; i < tour.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(tour[i].GetCityData().GetCityPositon(), tour[i + 1].GetCityData().GetCityPositon());
        }

        // Return to the starting city to complete the loop
        totalDistance += Vector3.Distance(tour[tour.Count - 1].GetCityData().GetCityPositon(), tour[0].GetCityData().GetCityPositon());
        return totalDistance;
    }

    // Display the tour and total distance in the console
    private void DisplayTour(List<CityInfo> tour, float totalDistance)
    {
        List<string> tourInfo = new List<string>();

        // Display the tour path
        string tourPath = "Best Nearest Neighbor Tour: ";
        for (int i = 0; i < tour.Count; i++)
        {
            tourPath += tour[i].GetCityData().GetCityName();
            if (i < tour.Count - 1)
            {
                tourPath += " -> ";
            }
        }
        tourPath += " -> " + tour[0].GetCityData().GetCityName(); // Complete the loop

        tourInfo.Add(tourPath);
        tourInfo.Add($"Total Distance: {totalDistance:F2}");

        // Send to UserConsole for display
        _cityManager.GetComponent<UserConsole>().AddObjectInfo(tourInfo);

        // Draw the best tour path using lines
        _cityManager.DrawTour(tour);
    }
}
