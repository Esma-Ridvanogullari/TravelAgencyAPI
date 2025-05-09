using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Tutorial8.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Controllers
{


[Route("api/clients/{clientId}/[controller]")]
    [ApiController]
    public class RegistrationsController : ControllerBase
    {
        private readonly string _connectionString;

        public RegistrationsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //  GET /api/clients/{clientId}/trips 
        [HttpGet]
        public IActionResult GetClientTrips(int clientId)
        {
            var trips = new List<Trip>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    @"SELECT t.TripId, t.Name, t.Description, t.DateStart, t.DateEnd, t.MaxParticipants, t.CountryId 
                      FROM Trips t 
                      JOIN Client_Trip ct ON t.TripId = ct.TripId 
                      WHERE ct.ClientId = @ClientId", connection);

                command.Parameters.AddWithValue("@ClientId", clientId);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    trips.Add(new Trip
                    {
                        TripId = (int)reader["TripId"],
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        DateStart = (DateTime)reader["DateStart"],
                        DateEnd = (DateTime)reader["DateEnd"],
                        MaxParticipants = (int)reader["MaxParticipants"],
                        CountryId = (int)reader["CountryId"]
                    });
                }
            }

            return Ok(trips);
        }

        //  PUT /api/clients/{clientId}/trips/{tripId} 
        [HttpPut("{tripId}")]
        public IActionResult RegisterClientForTrip(int clientId, int tripId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var capacityCheck = new SqlCommand(
                        @"SELECT COUNT(*) FROM Client_Trip WHERE TripId = @TripId", connection);
                    capacityCheck.Parameters.AddWithValue("@TripId", tripId);
                    int count = (int)capacityCheck.ExecuteScalar();

                    if (count >= 20)  
                    {
                        return BadRequest("The trip is fully booked.");
                    }

                    var command = new SqlCommand(
                        @"INSERT INTO Client_Trip (ClientId, TripId, RegisteredAt) 
                          VALUES (@ClientId, @TripId, GETDATE());", connection);

                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@TripId", tripId);

                    int result = command.ExecuteNonQuery();
                    return result > 0 ? Ok("Client registered successfully.") : StatusCode(500, "Failed to register client.");
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

        // DELETE /api/clients/{clientId}/trips/{tripId} 
        [HttpDelete("{tripId}")]
        public IActionResult RemoveClientFromTrip(int clientId, int tripId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        @"DELETE FROM Client_Trip WHERE ClientId = @ClientId AND TripId = @TripId", connection);

                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@TripId", tripId);

                    int result = command.ExecuteNonQuery();
                    return result > 0 ? Ok("Client unregistered successfully.") : NotFound("Client not found for this trip.");
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
    }
}