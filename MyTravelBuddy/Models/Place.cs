using System;
namespace MyTravelBuddy.Models;

public class Place
{
    public Location Location { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }

    /*public string GetStreet()
    {
        var array = Address.Split(',');

        var street = "";
        //return all elements except second to last and last element, comma separated
        if(array.Length > 2)
        {
            for(int i = 0; i < array.Length-2; i++)
            {
                street += array[i] + ", ";

            }

            //Remove unnecessary comma
            street = street[..^2];
        }

        return street;
    }

    public string GetCity()
    {
        var array = Address.Split(',');

        //city is always second to last element of address
        return array[array.Length - 2];
    }

    public string GetCountry()
    {
        var array = Address.Split(',');

        //country is always last element of address
        return array.Last();

    }*/
}


