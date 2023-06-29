using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalksRepository : IWalksRepository
    {
        private readonly IConfiguration _config;

        public WalksRepository(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walks> GetAllWalks()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Date, Duration, WalkerId, DogId
                        FROM Walks
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walks> walks = new List<Walks>();
                    while (reader.Read())
                    {
                        Walks walk = new Walks()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId"))
                        };

                        walks.Add(walk);
                    }

                    reader.Close();

                    return walks;
                }
            }
        }

        public Walks GetWalksById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Date, Duration, WalkerId, DogId
                        FROM Walks
                        WHERE Id = @id
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Walks walk = new Walks()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId"))
                        };
                        reader.Close();
                        return walk;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }

        public List<Walks> GetWalksByWalkerId(int walkerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                              SELECT wlks.Id AS WalkId,
                                wlks.Date, wlks.Duration,
                                wlks.WalkerId, 
                                wlks.DogId,
                                o.[Name] AS OwnerName,
                                o.Id AS OId,
                                d.[Name] AS DogName,
                                d.Id AS DId
                              FROM Walks wlks
                              LEFT JOIN Dog d ON d.Id = wlks.DogId
                              LEFT JOIN Owner o ON d.OwnerId = o.Id
                              WHERE WalkerId = @walkerId
                              ";

                    cmd.Parameters.AddWithValue("@walkerId", walkerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walks> walks = new List<Walks>();

                    while (reader.Read())
                    {
                        Walks walk = new Walks()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("WalkId")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Dog = new Dog
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DId")),
                                Name = reader.GetString(reader.GetOrdinal("DogName")),
                            },
                            Owner = new Owner
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OId")),
                                Name = reader.GetString(reader.GetOrdinal("OwnerName"))
                            }
                        };

                        walks.Add(walk);
                    }
                    reader.Close();
                    return walks;
                }
            }
        }

        public void AddWalks(Walks walks)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Walks (Date, Duration, WalkerId, dogId)
                    OUTPUT INSERTED.ID
                    VALUES (@date, @duration, @walkerId, @dogId);
                ";

                    cmd.Parameters.AddWithValue("@date", walks.Date);
                    cmd.Parameters.AddWithValue("@duration", walks.Duration);
                    cmd.Parameters.AddWithValue("@walkerId", walks.WalkerId);
                    cmd.Parameters.AddWithValue("@dogId", walks.DogId);

                    int id = (int)cmd.ExecuteScalar();

                    walks.Id = id;
                }
            }
        }



    }
}
