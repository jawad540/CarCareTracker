using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CarCareTracker.Data;
using CarCareTracker.Models;

namespace CarCareTracker.Repositories
{
    public class VehicleRepository
    {
        public List<Vehicle> GetByUser(int userId)
        {
            var list = new List<Vehicle>();
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"SELECT v.vehicle_id, v.user_id, v.type_id, t.type_name,
                                               v.plate_number, v.brand, v.model, v.year_made,
                                               v.color, v.current_odometer, v.created_at
                                        FROM VEHICLES v JOIN VEHICLE_TYPES t ON v.type_id = t.type_id
                                        WHERE v.user_id = @uid
                                        ORDER BY v.created_at DESC";
                    cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read()) list.Add(Map(rd));
                }
            }
            return list;
        }

        public List<Vehicle> GetAll()
        {
            var list = new List<Vehicle>();
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"SELECT v.vehicle_id, v.user_id, v.type_id, t.type_name,
                                               v.plate_number, v.brand, v.model, v.year_made,
                                               v.color, v.current_odometer, v.created_at
                                        FROM VEHICLES v JOIN VEHICLE_TYPES t ON v.type_id = t.type_id
                                        ORDER BY v.created_at DESC";
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read()) list.Add(Map(rd));
                }
            }
            return list;
        }

        public Vehicle GetById(int id)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"SELECT v.vehicle_id, v.user_id, v.type_id, t.type_name,
                                               v.plate_number, v.brand, v.model, v.year_made,
                                               v.color, v.current_odometer, v.created_at
                                        FROM VEHICLES v JOIN VEHICLE_TYPES t ON v.type_id = t.type_id
                                        WHERE v.vehicle_id = @id";
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    using (var rd = cmd.ExecuteReader())
                        if (rd.Read()) return Map(rd);
                }
            }
            return null;
        }

        public int Create(Vehicle v)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO VEHICLES (user_id, type_id, plate_number, brand, model, year_made, color, current_odometer)
                                        VALUES (@uid, @tid, @plate, @brand, @model, @yr, @color, @odo);
                                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    cmd.Parameters.Add("@uid", SqlDbType.Int).Value = v.UserId;
                    cmd.Parameters.Add("@tid", SqlDbType.Int).Value = v.TypeId;
                    cmd.Parameters.Add("@plate", SqlDbType.NVarChar).Value = v.PlateNumber;
                    cmd.Parameters.Add("@brand", SqlDbType.NVarChar).Value = v.Brand;
                    cmd.Parameters.Add("@model", SqlDbType.NVarChar).Value = v.Model;
                    cmd.Parameters.Add("@yr", SqlDbType.Int).Value = v.YearMade;
                    cmd.Parameters.Add("@color", SqlDbType.NVarChar).Value = (object)v.Color ?? DBNull.Value;
                    cmd.Parameters.Add("@odo", SqlDbType.BigInt).Value = v.CurrentOdometer;
                    var result = cmd.ExecuteScalar();
                    return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
                }
            }
        }

        public void Update(Vehicle v)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE VEHICLES SET type_id=@tid, plate_number=@plate, brand=@brand,
                                               model=@model, year_made=@yr, color=@color, current_odometer=@odo
                                        WHERE vehicle_id=@id";
                    cmd.Parameters.Add("@tid", SqlDbType.Int).Value = v.TypeId;
                    cmd.Parameters.Add("@plate", SqlDbType.NVarChar).Value = v.PlateNumber;
                    cmd.Parameters.Add("@brand", SqlDbType.NVarChar).Value = v.Brand;
                    cmd.Parameters.Add("@model", SqlDbType.NVarChar).Value = v.Model;
                    cmd.Parameters.Add("@yr", SqlDbType.Int).Value = v.YearMade;
                    cmd.Parameters.Add("@color", SqlDbType.NVarChar).Value = (object)v.Color ?? DBNull.Value;
                    cmd.Parameters.Add("@odo", SqlDbType.BigInt).Value = v.CurrentOdometer;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = v.VehicleId;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM VEHICLES WHERE vehicle_id=@id";
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int CountByUser(int userId)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM VEHICLES WHERE user_id=@uid";
                    cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        private Vehicle Map(IDataRecord rd)
        {
            return new Vehicle
            {
                VehicleId = Convert.ToInt32(rd["vehicle_id"]),
                UserId = Convert.ToInt32(rd["user_id"]),
                TypeId = Convert.ToInt32(rd["type_id"]),
                TypeName = rd["type_name"].ToString(),
                PlateNumber = rd["plate_number"].ToString(),
                Brand = rd["brand"].ToString(),
                Model = rd["model"].ToString(),
                YearMade = Convert.ToInt32(rd["year_made"]),
                Color = rd["color"] == DBNull.Value ? null : rd["color"].ToString(),
                CurrentOdometer = rd["current_odometer"] == DBNull.Value ? 0 : Convert.ToInt64(rd["current_odometer"]),
                IsActive = true,
                CreatedAt = Convert.ToDateTime(rd["created_at"])
            };
        }
    }
}
