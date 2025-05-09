using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;
using Tutorial8.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;


namespace Tutorial8.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly string _connectionString;

        // 🔄 Dependency Injection to get the connection string
        public TripsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ➡️ GET /api/trips - Retrieve all trips
        [HttpGet]
        public IActionResult GetTrips()
        {
            var trips = new List<Trip>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Trips", connection);
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

        // ➡️ POST /api/trips - Create a new trip
        [HttpPost]
        public IActionResult CreateTrip([FromBody] Trip trip)
        {
            if (trip == null)
            {
                return BadRequest("Trip object is null.");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        @"INSERT INTO Trips (Name, Description, DateStart, DateEnd, MaxParticipants, CountryId) 
                          VALUES (@Name, @Description, @DateStart, @DateEnd, @MaxParticipants, @CountryId);", 
                        connection
                    );

                    command.Parameters.AddWithValue("@Name", trip.Name);
                    command.Parameters.AddWithValue("@Description", trip.Description);
                    command.Parameters.AddWithValue("@DateStart", trip.DateStart);
                    command.Parameters.AddWithValue("@DateEnd", trip.DateEnd);
                    command.Parameters.AddWithValue("@MaxParticipants", trip.MaxParticipants);
                    command.Parameters.AddWithValue("@CountryId", trip.CountryId);

                    int result = command.ExecuteNonQuery();
                    return result > 0 ? Ok("Trip added successfully.") : StatusCode(500, "Failed to insert the trip.");
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
        // ➡️ PUT /api/trips/{id} - Update an existing trip
        [HttpPut("{id}")]
        public IActionResult UpdateTrip(int id, [FromBody] Trip trip)
        {
            if (trip == null)
            {
                return BadRequest("Trip object is null.");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        @"UPDATE Trips 
                  SET Name = @Name, 
                      Description = @Description, 
                      DateStart = @DateStart, 
                      DateEnd = @DateEnd, 
                      MaxParticipants = @MaxParticipants, 
                      CountryId = @CountryId 
                  WHERE TripId = @TripId", connection);

                    command.Parameters.AddWithValue("@TripId", id);
                    command.Parameters.AddWithValue("@Name", trip.Name);
                    command.Parameters.AddWithValue("@Description", trip.Description);
                    command.Parameters.AddWithValue("@DateStart", trip.DateStart);
                    command.Parameters.AddWithValue("@DateEnd", trip.DateEnd);
                    command.Parameters.AddWithValue("@MaxParticipants", trip.MaxParticipants);
                    command.Parameters.AddWithValue("@CountryId", trip.CountryId);

                    int result = command.ExecuteNonQuery();
                    return result > 0 ? Ok("Trip updated successfully.") : NotFound("Trip not found.");
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
        // ➡️ DELETE /api/trips/{id} - Delete a specific trip
        [HttpDelete("{id}")]
        public IActionResult DeleteTrip(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("DELETE FROM Trips WHERE TripId = @TripId", connection);
                    command.Parameters.AddWithValue("@TripId", id);

                    int result = command.ExecuteNonQuery();
                    return result > 0 ? Ok("Trip deleted successfully.") : NotFound("Trip not found.");
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

