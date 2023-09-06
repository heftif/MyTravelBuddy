using System;
namespace MyTravelBuddy.Models;

public class Address
{
    public string Street { get; set; }
    public string Country { get; set; }
    public string City { get; set; }

    public string CountryCode { get; set; }
    public string AdminArea { get; set; }
    public string Zip { get; set; }

    public Address(string street, string city, string country, string countryCode, string admineArea, string zip)
    {
        Street = street;
        Country = country;
        City = city;
        CountryCode = countryCode;
        AdminArea = admineArea;
        Zip = zip;
    }

    public string GetAddressString()
    {
        string[] str = new string[] { Street, City, Country };

        return string.Join(", ", str);
    }

}

