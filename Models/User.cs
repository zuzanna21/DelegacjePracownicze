using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegacjePracownicze.Models;

public class User
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string EmploymentStatus { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    private string _password = string.Empty;


    public bool VerifyPassword(string password)
    {
        return _password == password;
    }

    public string Password
    {
        get => _password;
        set => _password = value;
    }

}
