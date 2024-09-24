using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class BruteForceSolver : MonoBehaviour
{
    private CityManager cityManager; // Reference to the CityManager
    private List<CityInfo> cities; // List of all cities in the scene

    // Start is called before the first frame update
    void Start()
    {
        cityManager = FindObjectOfType<CityManager>(); // Get the CityManager instance
    }

    // This method triggers the brute force algorithm to solve the TSP
    public void SolveTSP()
    {
        StartCoroutine(SolveTSPCoroutine());
    }
    //Using the Coroutine to update UI 
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

        List<int> cityIndices = Enumerable.Range(0, cities.Count).ToList();
        var allPermutations = GetPermutations(cityIndices, cities.Count);

        List<int> bestTour = null;
        float shortestDistance = float.MaxValue;

        int totalPermutations = allPermutations.Count(); // Total number of permutations
        int processedPermutations = 0;
        int progress = 0;

        foreach (var perm in allPermutations)
        {
            float currentDistance = CalculateTourDistance(perm.ToList());
            processedPermutations++;

            if (currentDistance < shortestDistance)
            {
                shortestDistance = currentDistance;
                bestTour = perm.ToList();
            }

            // Calculate and update progress percentage
            int newProgress = Mathf.RoundToInt(((float)processedPermutations / totalPermutations) * 100);
            if (newProgress > progress)
            {
                progress = newProgress;
                cityManager.GetComponent<UserConsole>().AddObjectInfo($"Progress: {progress}% complete");
            }

            // Yield to allow UI to update
            yield return null;
        }

        DisplayTour(bestTour, shortestDistance);
        cityManager.GetComponent<UserConsole>().AddObjectInfo("Algorithm completed: 100% complete");
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

    // Generate all permutations of a list 
    private IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
    {
        if (length == 1) return list.Select(t => new T[] { t });
        return GetPermutations(list, length - 1)
            .SelectMany(t => list.Where(e => !t.Contains(e)),
                        (t1, t2) => t1.Concat(new T[] { t2 }));
    }

    // Display the best tour and total distance in the console
    private void DisplayTour(List<int> bestTour, float totalDistance)
    {
        List<string> tourInfo = new List<string>();

        // Display the tour path
        string tourPath = "Best Brute Force Tour: ";
        for (int i = 0; i < bestTour.Count; i++)
        {
            tourPath += cities[bestTour[i]].GetCityData().GetCityName();
            if (i < bestTour.Count - 1)
            {
                tourPath += " -> ";
            }
        }
        tourPath += " -> " + cities[bestTour[0]].GetCityData().GetCityName(); 

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
