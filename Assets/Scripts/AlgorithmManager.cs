using UnityEngine;

public class AlgorithmManager : MonoBehaviour
{
    private NearestNeighborSolver nearestNeighborSolver;
    private BruteForceSolver bruteForceSolver;
    private SimulatedAnnealingSolver simulatedAnnealingSolver;

    void Start()
    {
        nearestNeighborSolver = FindObjectOfType<NearestNeighborSolver>();
        bruteForceSolver = FindObjectOfType<BruteForceSolver>();
        simulatedAnnealingSolver = FindObjectOfType<SimulatedAnnealingSolver>();
    }

    public void RunNearestNeighbor()
    {
        nearestNeighborSolver.SolveTSP();
    }

    public void RunBruteForce()
    {
        bruteForceSolver.SolveTSP();
    }

    public void RunSimulatedAnnealing()
    {
        simulatedAnnealingSolver.SolveTSP();
    }
}
