using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkerRepository : IWalkerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkerRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walker> GetAllWalkers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT w.Id AS WalkerId, 
                        w.[Name] AS WalkerName, 
                        w.NeighborhoodId, 
                        w.ImageUrl, 
                        n.Id AS HoodId, 
                        n.[Name] AS HoodName
                    FROM Walker w
                    JOIN Neighborhood n On w.NeighborhoodId = n.Id
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            Name = reader.GetString(reader.GetOrdinal("WalkerName")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Neighborhood = new Neighborhood
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("HoodId")),
                                Name = reader.GetString(reader.GetOrdinal("HoodName"))

                            }
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }

        public Walker GetWalkerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT w.Id AS WalkerId, 
                        w.[Name] AS WalkerName, 
                        w.NeighborhoodId, 
                        w.ImageUrl, 
                        n.Id AS HoodId, 
                        n.[Name] AS HoodName,
                        wlks.Date,
                        wlks.Duration,
                        o.[Name] AS Client
                    FROM Walker w
                    LEFT JOIN Neighborhood n On w.NeighborhoodId = n.Id
                    LEFT JOIN Walks wlks On w.Id = wlks.WalkerId
                    LEFT JOIN Owner o On wlks.WalkerId = o.Id                    
                    WHERE w.id = @id
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            Name = reader.GetString(reader.GetOrdinal("WalkerName")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                            Neighborhood = new Neighborhood
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("HoodId")),
                                Name = reader.GetString(reader.GetOrdinal("HoodName"))
                            },
                            Owner = new Owner
                            {
                                Name = reader.IsDBNull(reader.GetOrdinal("Client")) ? null : reader.GetString(reader.GetOrdinal("Client"))
                            },
                            Walks = new List<Walks>()

                        };
                        



                        reader.Close();
                        return walker;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }

        public List<Walker> GetWalkersInNeighborhood(int neighborhoodId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT Id, [Name], ImageUrl, NeighborhoodId
                FROM Walker
                WHERE NeighborhoodId = @neighborhoodId
            ";

                    cmd.Parameters.AddWithValue("@neighborhoodId", neighborhoodId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }
    }
}