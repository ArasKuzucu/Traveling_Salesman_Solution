using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CityInfo : MonoBehaviour
{
    private City _cityData;
    private CityManager _cityManager;
    private Color _cityColor; // Holding the original city color
    private List<string> _outputTextList = new List<string>();
    public void Start()
    {
        _cityColor = this.gameObject.GetComponent<Renderer>().material.color;
    }
    public void SetCityData(City city, CityManager manager)
    {
        _cityData = city;
        _cityManager = manager;

    }
    public City GetCityData() => _cityData;
   
    //Getting dataa from the user selected city with mouse click
    void OnMouseDown()
    {
        //Debugging
        Debug.Log("Clicked City: " + _cityData.GetCityName());

        //Assigning the name value to assign in list in this way only using the method one time
        string cityInfoName = _cityData.GetCityName();
        _outputTextList.Add(cityInfoName);

        // Calculate and log the distance to all other cities
        foreach (CityInfo otherCity in _cityManager.GetAllCities())
        {
            // Skip the clicked city itself
            if (otherCity != this)
            {
                float distance = Vector3.Distance(_cityData.GetCityPositon(), otherCity._cityData.GetCityPositon());
                //Debug.Log("Distance to " + otherCity.cityData.GetCityName() + ": " + distance);

                _outputTextList.Add($"Distance to {otherCity._cityData.GetCityName()} : {distance:F2}");

            }
        }

        _cityManager.GetComponent<UserConsole>().AddObjectInfo(_outputTextList);
    }

    //an indicator that shows which city the user has hovered the mouse
    private void OnMouseEnter()
    {
        this.gameObject.GetComponent<Renderer>().material.color = new Color(0.9f, 0.9f, 0.9f);
    }
    // Revert to original color from hovered color
    private void OnMouseExit()
    {
        this.gameObject.GetComponent<Renderer>().material.color = _cityColor;
    }
}
