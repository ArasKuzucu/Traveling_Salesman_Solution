using UnityEngine;


public class City
{
    private readonly string _cityName;
    private readonly Vector3 _position;


    public City(string name, Vector3 position)
    {
        _cityName = name;
        _position = position;
    }
    public Vector3 GetCityPositon() => _position;
    public string GetCityName() => _cityName;
    public string GetCityInfo() => $"Name of the city is: {_cityName} \n The position of the city is: {_position}";




}
