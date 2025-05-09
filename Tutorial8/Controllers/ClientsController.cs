using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Tutorial8.Models;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ClientsController : ControllerBase
    {
        //private string _connectionString;
        private readonly string _connectionggString =
            "Server=localhost;Database=Tutorial8;Trusted_Connection=True;TrustServerCertificate=True;";


        public IActionResult GetClients()
        {
            var clients = new List<Client>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Clients", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clients.Add(new Client
                    {
                        ClientId = (int)reader["ClientId"],
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Telephone = reader["Telephone"].ToString(),
                        Pesel = reader["Pesel"].ToString()
                    });
                }
            }

            return Ok(clients);
        }
        
        [HttpPost]
        public IActionResult CreateClient([FromBody] Client client)
        {
            if (client == null)
            {
                return BadRequest("Client object is null.");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command =
                        new SqlCommand(
                            "INSERT INTO Clients (FirstName, LastName, Email, Telephone, Pesel) VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel);",
                            connection);

                    command.Parameters.AddWithValue("@FirstName", client.FirstName);
                    command.Parameters.AddWithValue("@LastName", client.LastName);
                    command.Parameters.AddWithValue("@Email", client.Email);
                    command.Parameters.AddWithValue("@Telephone", client.Telephone);
                    command.Parameters.AddWithValue("@Pesel", client.Pesel);

                    int result = command.ExecuteNonQuery();
                    return result > 0
                        ? Ok("Client added successfully.")
                        : StatusCode(500, "Failed to insert the client.");
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        [HttpGet("test-connection")]
        public IActionResult TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return Ok("Database connection successful!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database connection failed: {ex.Message}");
            }
            
        }
    }
}