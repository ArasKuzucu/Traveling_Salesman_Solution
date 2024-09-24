using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class SimulatedAnnealingSolver : MonoBehaviour
{
    private CityManager cityManager; // Reference to the CityManager
    private List<CityInfo> cities; // List of all cities in the scene
    private float initialTemperature = 1000.0f; // Initial temperature
    private float coolingRate = 0.003f; // Cooling rate
    private int maxIterations = 10000; // Maximum number of iterations

    // Start is called before the first frame update
    void Start()
    {
        cityManager = FindObjectOfType<CityManager>(); // Get the CityManager instance
    }

    // This method triggers the Simulated Annealing algorithm to solve the TSP
    public void SolveTSP()
    {
        StartCoroutine(SolveTSPCoroutine());
    }

    private IEnumerator SolveTSPCoroutine()
    {
        cities = cityManager.GetAllCities(); // Get the list of all cities
        if (cities == null || cities.Count == 0)
        {
            Debug.LogError("No cities available!");
            cityManager.GetComponent<UserConsole>().AddObjectInfo("No cities available!");
            yield break;
        }

        cityManager.GetComponent<UserConsole>().AddObjectInfo("Algorithm started: 0% complete");

        // Generate an initial random solution (tour)
        List<int> currentTour = GenerateInitialTour();
        float currentDistance = CalculateTourDistance(currentTour);

        List<int> bestTour = new List<int>(currentTour);
        float bestDistance = currentDistance;

        float temperature = initialTemperature;
        int progress = 0;

        // Simulated Annealing loop
        for (int i = 0; i < maxIterations; i++)
        {
            // Generate a new neighboring solution by swapping two cities
            List<int> newTour = GenerateNeighbor(currentTour);
            float newDistance = CalculateTourDistance(newTour);

            // Check if the new solution is better, or accept it with a probability
            if (newDistance < currentDistance || ShouldAcceptWorseSolution(currentDistance, newDistance, temperature))
            {
                currentTour = new List<int>(newTour);
                currentDistance = newDistance;
            }

            // Track the best solution
            if (currentDistance < bestDistance)
            {
                bestTour = new List<int>(currentTour);
                bestDistance = currentDistance;
            }

            // Cool down the temperature
            temperature *= (1 - coolingRate);

            // Calculate and update progress percentage
            int newProgress = Mathf.RoundToInt(((float)i / maxIterations) * 100);
            if (newProgress > progress)
            {
                progress = newProgress;
                cityManager.GetComponent<UserConsole>().AddObjectInfo($"Progress: {progress}% complete");
            }

            // Yield to allow UI to update
            yield return null;

            // Stop if temperature is low
            if (temperature < 1)
                break;
        }

        DisplayTour(bestTour, bestDistance);
        cityManager.GetComponent<UserConsole>().AddObjectInfo("Algorithm completed: 100% complete");
    }

    // Generate an initial random tour
    private List<int> GenerateInitialTour()
    {
        List<int> tour = new List<int>();
        for (int i = 0; i < cities.Count; i++)
        {
            tour.Add(i);
        }
        ShuffleList(tour); // Shuffle to create a random tour
        return tour;
    }

    // Generate a neighboring solution by swapping two random cities in the tour
    private List<int> GenerateNeighbor(List<int> tour)
    {
        List<int> newTour = new List<int>(tour);
        int cityIndex1 = Random.Range(0, cities.Count);
        int cityIndex2 = Random.Range(0, cities.Count);

        // Swap two cities
        int temp = newTour[cityIndex1];
        newTour[cityIndex1] = newTour[cityIndex2];
        newTour[cityIndex2] = temp;

        return newTour;
    }

    // Calculate the total distance of the given tour (using a list of city indices)
    private float CalculateTourDistance(List<int> tour)
    {
        float totalDistance = 0;
        for (int i = 0; i < tour.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(cities[tour[i]].GetCityData().GetCityPositon(), cities[tour[i + 1]].GetCityData().GetCityPositon());
        }

        // Return to the starting city to complete the loop
        totalDistance += Vector3.Distance(cities[tour[tour.Count - 1]].GetCityData().GetCityPositon(), cities[tour[0]].GetCityData().GetCityPositon());

        return totalDistance;
    }

    // Determine whether to accept a worse solution
    private bool ShouldAcceptWorseSolution(float currentDistance, float newDistance, float temperature)
    {
        float acceptanceProbability = Mathf.Exp((currentDistance - newDistance) / temperature);
        return Random.Range(0f, 1f) < acceptanceProbability;
    }

    // Shuffle the list to generate a random tour
    private void ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Display the best tour and total distance in the console
    private void DisplayTour(List<int> bestTour, float totalDistance)
    {
        List<string> tourInfo = new List<string>();

        // Display the tour path
        string tourPath = "Best Simulated Annealing Tour: ";
        for (int i = 0; i < bestTour.Count; i++)
        {
            tourPath += cities[bestTour[i]].GetCityData().GetCityName();
            if (i < bestTour.Count - 1)
            {
                tourPath += " -> ";
            }
        }
        tourPath += " -> " + cities[bestTour[0]].GetCityData().GetCityName(); // Complete the loop

        tourInfo.Add(tourPath);
        tourInfo.Add($"Total Distance: {totalDistance:F2}");

        // Send to UserConsole for display
        cityManager.GetComponent<UserConsole>().AddObjectInfo(tourInfo);

        // Convert bestTour indices to CityInfo list for visualization
        List<CityInfo> bestTourCities = bestTour.Select(index => cities[index]).ToList();

        // Draw the best tour path using lines
        cityManager.DrawTour(bestTourCities);
    }
}
