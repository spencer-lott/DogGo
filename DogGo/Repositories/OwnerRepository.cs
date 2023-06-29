using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NuGet.Protocol.Plugins;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public OwnerRepository(IConfiguration config)
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
        //This Method gets all the owners using a sql query. We also got the neightborhood at this time so that we could display for future use the name of the neighborhood.
        public List<Owner> GetAllOwners()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT o.Id AS OwnerId,
                        o.Email,
                        o.[Name] AS OwnerName,
                        o.Address,
                        o.NeighborhoodId, 
                        o.Phone,
                        n.Id AS HoodId, 
                        n.[Name] AS HoodName
                        FROM Owner o
                        LEFT JOIN Neighborhood n On o.NeighborhoodId = n.Id"; 

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Owner> owners = new List<Owner>();
                    while (reader.Read())
                    {
                          Owner  owner = new Owner
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Name = reader.GetString(reader.GetOrdinal("OwnerName")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                Neighborhood = new Neighborhood
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("HoodId")),
                                    Name = reader.GetString(reader.GetOrdinal("HoodName"))
                                },
                            };                        

                        owners.Add(owner);
                    }

                    reader.Close();

                    return owners;
                }
            }
        }
        //Here we get the Owner by its Id. Owners have lots of connections to other data tables. I joined the tables on this method because we would needed to match the ids of other tables and access their information
        public Owner GetOwnerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT o.Id AS OwnerId,
                        o.Email,
                        o.[Name] AS OwnerName,
                        o.Address,
                        o.NeighborhoodId, 
                        o.Phone,
                        n.Id AS HoodId, 
                        n.[Name] AS HoodName,
                        d.Id AS DogId,
                        d.[Name] AS DogName,
                        d.Breed,
                        d.ImageUrl,
                        d.OwnerId AS DogOwnerId
                        FROM Owner o
                        LEFT JOIN Neighborhood n On o.NeighborhoodId = n.Id  
                        LEFT JOIN Dog d ON o.Id = d.OwnerId
                        WHERE o.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    Owner owner = null;

                    while (reader.Read())
                    {
                        if (owner == null)
                        {

                            owner = new Owner
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Name = reader.GetString(reader.GetOrdinal("OwnerName")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                Neighborhood = new Neighborhood
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("HoodId")),
                                    Name = reader.GetString(reader.GetOrdinal("HoodName"))
                                },
                                Dogs = new List<Dog>()
                            };
                        }
                        //Dogs has possible nullable values (ie ImageUrl and Notes), and that's why we have a conditional here. 
                        if (!reader.IsDBNull(reader.GetOrdinal("DogId")))
                        {
                            Dog dog = new Dog
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                                Name = reader.GetString(reader.GetOrdinal("DogName")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("DogOwnerId")),
                                //IsDBNull is essential here because we still need to upload a value to the database. It will either upload null or the user input
                                ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl"))
                            };

                            owner.Dogs.Add(dog);
                        }
                            }
                            reader.Close();
                            return owner;
                        }
                    }
                }
        //This was code given to us, but we actually didn't use it in the project for anything.
        public Owner GetOwnerByEmail(string email)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], Email, Address, Phone, NeighborhoodId
                        FROM Owner
                        WHERE Email = @email";

                    cmd.Parameters.AddWithValue("@email", email);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Owner owner = new Owner()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        reader.Close();
                        return owner;
                    }

                    reader.Close();
                    return null;
                }
            }
        }
        //Here we add a new owner to the database. You need every single value in the table in order for this to work
        public void AddOwner(Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Owner ([Name], Email, Phone, Address, NeighborhoodId)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @email, @phoneNumber, @address, @neighborhoodId);
                ";

                    cmd.Parameters.AddWithValue("@name", owner.Name);
                    cmd.Parameters.AddWithValue("@email", owner.Email);
                    cmd.Parameters.AddWithValue("@phoneNumber", owner.Phone);
                    cmd.Parameters.AddWithValue("@address", owner.Address);
                    cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);

                    int id = (int)cmd.ExecuteScalar();

                    owner.Id = id;
                }
            }
        }
        //This is the method for editing an owners information
        public void UpdateOwner(Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Owner
                            SET 
                                [Name] = @name, 
                                Email = @email, 
                                Address = @address, 
                                Phone = @phone, 
                                NeighborhoodId = @neighborhoodId
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", owner.Name);
                    cmd.Parameters.AddWithValue("@email", owner.Email);
                    cmd.Parameters.AddWithValue("@address", owner.Address);
                    cmd.Parameters.AddWithValue("@phone", owner.Phone);
                    cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);
                    cmd.Parameters.AddWithValue("@id", owner.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        //This is the DELETE method. The id is the only thing that needs targeted here.
        public void DeleteOwner(int ownerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            DELETE FROM Owner
                            WHERE Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", ownerId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}