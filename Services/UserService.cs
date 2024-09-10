using DelegacjePracownicze.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using System.Transactions;

namespace DelegacjePracownicze.Services;

public class UserService
{
    private readonly string connectionString = "Server=10.0.2.2;Database=DelegacjePracownicze;User Id=Zuzanna;Password=1234;TrustServerCertificate=True;";


    public UserService()
    {
        
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        User? user = null;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            string query = @"SELECT IDpracownika, AdresEmail, Haslo, Imie, Nazwisko, Stanowisko, 
                                 StatusZatrudnienia, Adres, Miasto, Kraj, Telefon, KodPocztowy 
                                 FROM Pracownicy WHERE AdresEmail = @Email";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new User()
                        {
                            Email = reader.GetString(1),
                            Password = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            FirstName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            LastName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            EmployeeId = reader.GetInt32(0),
                            Position = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            EmploymentStatus = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            Address = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            City = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                            Country = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                            PhoneNumber = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                            PostalCode = reader.IsDBNull(11) ? string.Empty : reader.GetString(11)
                        };
                    }
                }
            }
        }

        return user;
    }

    public async Task<List<Settlement>> GetSettlementsAsync(string email)
    {
        List<Settlement> settlements = new List<Settlement>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            string query = @"SELECT IDrozliczenia, DataWystawienia, KrajDelegacji FROM Rozliczenia r JOIN Delegacje d ON r.IDdelegacji=d.IDdelegacji JOIN Pracownicy p ON r.IDpracownika=p.IDpracownika WHERE p.AdresEmail=@Email";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var settlement = new Settlement
                        {
                            SettlementId = reader.GetInt32(0),
                            IssueDate = reader.GetDateTime(1),
                            Delegation = new Delegation
                            {
                                DelegationCountry = reader.GetString(2)
                            }
                        };
                        settlements.Add(settlement);
                    }
                }
            }
        }

        return settlements;
    }

    public async Task<List<Bill>> GetBillsBySettlementIdAsync(int settlementId)
    {
        List<Bill> bills = new List<Bill>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            string query = @"SELECT roz.IDrachunku, kat.NazwaKategorii, rach.WartoscBrutto, z.StatusZwrotu  FROM Rachunki rach JOIN Rozliczenia roz ON rach.IDrachunku=roz.IDrachunku JOIN Kategorie kat ON kat.IDkategorii=rach.IDkategorii JOIN Zwroty z ON z.IDrachunku=rach.IDrachunku WHERE IdRozliczenia = @SettlementId";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SettlementId", settlementId);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var bill = new Bill
                        {
                            BillId = reader.GetInt32(0),
                            Category = new Category
                            {
                                CategoryName = reader.GetString(1)
                            },
                            GrossValue = reader.GetSqlMoney(2),
                            Return = new Return
                            {
                                ReturnStatus = reader.GetString(3)
                            }

                        };

                        Console.WriteLine($"IdBill: {reader.GetInt32(0)}, CategoryName: {reader.GetString(1)}, GrossValue: {reader.GetSqlMoney(2)}, ReturnStatus: {reader.GetString(3)}");
                        bills.Add(bill);
                    }
                }
            }
        }

        return bills;
    }

    public async Task<IEnumerable<Delegation>> GetDelegationsAsync()
    {
        List<Delegation> delegations = new List<Delegation>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            string query = @"SELECT IDdelegacji, KrajDelegacji, DataWyjazdu, DataPowrotu 
                         FROM Delegacje 
                         WHERE DATEDIFF(DAY, GETDATE(), DataPowrotu) <= 14 AND YEAR(DataPowrotu)=YEAR(GETDATE())";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Debug.WriteLine($"Found delegation: {reader.GetString(1)}");
                        var delegation = new Delegation
                        {
                            DelegationId = reader.GetInt32(0),
                            DelegationCountry = reader.GetString(1),
                            DepartureDate = reader.GetDateTime(2),
                            ReturnDate = reader.GetDateTime(3)
                        };
                        delegations.Add(delegation);
                    }
                }
            }
        }

        return delegations;
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {

        List<Category> categories = new List<Category>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            string query = @"SELECT IDkategorii, NazwaKategorii, KrajDiety
                         FROM Kategorie";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Debug.WriteLine($"Found category: {reader.GetString(1)}");
                        var category = new Category
                        {
                            CategoryId = reader.GetInt32(0),
                            CategoryName = reader.GetString(1),
                            AllowanceCountry = reader.GetString(2)
                        };
                        categories.Add(category);
                    }
                }
            }
        }

        return categories;

    }


    public async Task<bool> CheckIfSettlementExistsAsync(string settlementId)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            string query = @"SELECT COUNT(1) FROM Rozliczenia WHERE IDrozliczenia = @SettlementId";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SettlementId", settlementId);

                var result = await command.ExecuteScalarAsync();

                int count = (result as int?) ?? 0;

                return count > 0;
            }
        }
    }

    public async Task<bool> CheckIfBillExistsAsync(int? billId)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            string query = @"SELECT COUNT(1) FROM Rachunki WHERE IDrachunku = @BillId";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BillId", billId);

                var result = await command.ExecuteScalarAsync();

                int count = (result as int?) ?? 0;

                return count > 0;
            }
        }
    }

    public async Task SaveSettlementWithBillsAsync(Settlement settlement, int employeeId)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            Console.WriteLine("Połączenie z bazą danych otwarte.");
            using (var transaction = (SqlTransaction)await connection.BeginTransactionAsync())
            {
                Console.WriteLine("Transakcja rozpoczęta.");
                try
                {

                    // Save Bills
                    foreach (var bill in settlement.Bills)
                    {
                        Console.WriteLine($"Przygotowuję do zapisu rachunek o ID: {bill.BillId}");

                        string billQuery = @"
                        INSERT INTO Rachunki 
                        (IDrachunku,IDdelegacji, IDkategorii, NazwaSprzedawcy, AdresSprzedawcy, KrajSprzedawcy, MiastoSprzedawcy, KodPocztowySprzedawcy, NIP, StawkaVAT, WartoscNetto, VAT, WartoscBrutto) 
                        VALUES 
                        (@BillId, @DelegationId, @CategoryId, @SellerName, @SellerAddress, @SellerCountry, @SellerCity, @SellerPostalCode, @SellerTaxId, @VATRate, @NetValue, @VAT, @GrossValue)";

                        using (SqlCommand billCommand = new SqlCommand(billQuery, connection))
                        {
                            billCommand.Transaction = transaction;

                            billCommand.Parameters.AddWithValue("@BillId", bill.BillId);
                            billCommand.Parameters.AddWithValue("@SellerName", bill.SellerName);
                            billCommand.Parameters.AddWithValue("@SellerAddress", bill.SellerAddress);
                            billCommand.Parameters.AddWithValue("@SellerCountry", bill.SellerCountry);
                            billCommand.Parameters.AddWithValue("@SellerCity", bill.SellerCity);
                            billCommand.Parameters.AddWithValue("@SellerPostalCode", bill.SellerPostalCode);
                            billCommand.Parameters.AddWithValue("@SellerTaxId", bill.SellerTaxId);
                            billCommand.Parameters.AddWithValue("@VATRate", bill.VATRate);
                            billCommand.Parameters.AddWithValue("@NetValue", bill.NetValue.IsNull ? DBNull.Value : bill.NetValue.Value);
                            billCommand.Parameters.AddWithValue("@VAT", bill.VAT.IsNull ? DBNull.Value : bill.VAT.Value);
                            billCommand.Parameters.AddWithValue("@GrossValue", bill.GrossValue.IsNull ? DBNull.Value : bill.GrossValue.Value);
                            billCommand.Parameters.AddWithValue("@DelegationId", settlement.DelegationId);
                            billCommand.Parameters.AddWithValue("@CategoryId", bill.CategoryId);

                            Console.WriteLine($"Zapisuję rachunek o ID: {bill.BillId}");
                            await billCommand.ExecuteNonQueryAsync();
                            Console.WriteLine($"Rachunek o ID: {bill.BillId} został zapisany.");
                        }
                    }

                    // Save Settlements
                    foreach (var bill in settlement.Bills)
                    {
                        Console.WriteLine($"Przygotowuję do zapisu rozliczenie dla rachunku o ID: {bill.BillId}");

                        string settlementQuery = @"
                        INSERT INTO Rozliczenia 
                        (IDrozliczenia, IDrachunku, IDpracownika, IDdelegacji,  WartoscBrutto, DataWystawienia) 
                        VALUES 
                        (@SettlementId, @BillId, @EmployeeId, @DelegationId, @GrossValue, @IssueDate)";

                        using (SqlCommand settlementCommand = new SqlCommand(settlementQuery, connection))
                        {
                            Console.WriteLine($"Stan transakcji przed zapisaniem rozliczenia: {transaction?.Connection != null}");
                            settlementCommand.Transaction = transaction;

                            settlementCommand.Parameters.AddWithValue("@SettlementId", settlement.SettlementId);
                            settlementCommand.Parameters.AddWithValue("@DelegationId", settlement.DelegationId);
                            settlementCommand.Parameters.AddWithValue("@EmployeeId", employeeId);
                            settlementCommand.Parameters.AddWithValue("@BillId", bill.BillId);
                            settlementCommand.Parameters.AddWithValue("@GrossValue", bill.GrossValue.IsNull ? DBNull.Value : bill.GrossValue.Value);
                            settlementCommand.Parameters.AddWithValue("@IssueDate", settlement.IssueDate);

                            Console.WriteLine($"Zapisuję rozliczenie o ID: {settlement.SettlementId} dla rachunku o ID: {bill.BillId}");
                            await settlementCommand.ExecuteNonQueryAsync();
                            Console.WriteLine($"Rozliczenie o ID: {settlement.SettlementId} zostało zapisane.");
                        }
                    }

                    Console.WriteLine("Zatwierdzam transakcję.");
                    await transaction.CommitAsync();
                    Console.WriteLine("Transakcja zatwierdzona pomyślnie.");
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine("Błąd SQL podczas zapisywania. Wycofuję transakcję.");
                    Console.WriteLine($"Kod błędu SQL: {sqlEx.Number}");
                    Console.WriteLine($"Komunikat błędu SQL: {sqlEx.Message}");
                    Console.WriteLine($"Szczegóły błędu SQL: {sqlEx.StackTrace}");

                    await transaction.RollbackAsync();
                    Console.WriteLine("Transakcja została wycofana.");
                    throw new Exception("Błąd SQL przy zapisie: " + sqlEx.Message, sqlEx);
                }
                catch (Exception ex)
                {
                    // Wycofanie transakcji w przypadku błędu
                    Console.WriteLine("Błąd podczas zapisywania. Wycofuję transakcję.");
                    await transaction.RollbackAsync();
                    Console.WriteLine("Transakcja została wycofana.");
                    throw new Exception("Błąd przy zapisie: " + ex.Message, ex);
                }
            }
        }
    }



    public async Task TestConnectionAsync()
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine("Połączenie z bazą danych nawiązane pomyślnie.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd połączenia: {ex.Message}");
        }
    }
}
