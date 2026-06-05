using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CarCareTracker.Data;
using CarCareTracker.Models;

namespace CarCareTracker.Repositories
{
    public class LookupRepository
    {
        public List<VehicleType> GetVehicleTypes()
        {
            var list = new List<VehicleType>();
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT type_id, type_name FROM VEHICLE_TYPES ORDER BY type_name";
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read())
                            list.Add(new VehicleType
                            {
                                TypeId = Convert.ToInt32(rd["type_id"]),
                                TypeName = rd["type_name"].ToString()
                            });
                }
            }
            return list;
        }

        public List<ServiceType> GetServiceTypes()
        {
            var list = new List<ServiceType>();
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"SELECT service_type_id, service_name, default_interval_km, default_interval_months
                                        FROM SERVICE_TYPES ORDER BY service_name";
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read())
                            list.Add(new ServiceType
                            {
                                ServiceTypeId = Convert.ToInt32(rd["service_type_id"]),
                                ServiceName = rd["service_name"].ToString(),
                                DefaultIntervalKm = rd["default_interval_km"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["default_interval_km"]),
                                DefaultIntervalMonths = rd["default_interval_months"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["default_interval_months"])
                            });
                }
            }
            return list;
        }
    }
}
