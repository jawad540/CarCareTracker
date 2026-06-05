using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CarCareTracker.Data;
using CarCareTracker.Models;

namespace CarCareTracker.Repositories
{
    public class FuelRepository
    {
        public List<FuelRecord> GetByUser(int userId)
        {
            var list = new List<FuelRecord>();
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"SELECT f.fuel_id, f.vehicle_id, f.fuel_date, f.liters, f.cost,
                                               f.odometer_reading, f.station_name, f.notes, f.created_at,
                                               v.brand + ' ' + v.model + ' (' + v.plate_number + ')' AS vlabel
                                        FROM FUEL_RECORDS f JOIN VEHICLES v ON f.vehicle_id = v.vehicle_id
                                        WHERE v.user_id = @uid
                                        ORDER BY f.fuel_date DESC";
                    cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read()) list.Add(Map(rd));
                }
            }
            return list;
        }

        public int Create(FuelRecord f)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO FUEL_RECORDS (vehicle_id, fuel_date, liters, cost, odometer_reading, station_name, notes)
                                        VALUES (@vid, @fdate, @lit, @cost, @odo, @station, @notes);
                                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    cmd.Parameters.Add("@vid", SqlDbType.Int).Value = f.VehicleId;
                    cmd.Parameters.Add("@fdate", SqlDbType.Date).Value = f.FuelDate;
                    cmd.Parameters.Add("@lit", SqlDbType.Decimal).Value = f.Liters;
                    cmd.Parameters.Add("@cost", SqlDbType.Decimal).Value = f.Cost;
                    cmd.Parameters.Add("@odo", SqlDbType.BigInt).Value = f.OdometerReading;
                    cmd.Parameters.Add("@station", SqlDbType.NVarChar).Value = (object)f.StationName ?? DBNull.Value;
                    cmd.Parameters.Add("@notes", SqlDbType.NVarChar).Value = (object)f.Notes ?? DBNull.Value;
                    return Convert.ToInt32(cmd.ExecuteScalar());
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
                    cmd.CommandText = "DELETE FROM FUEL_RECORDS WHERE fuel_id=@id";
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private FuelRecord Map(IDataRecord rd)
        {
            return new FuelRecord
            {
                FuelId = Convert.ToInt32(rd["fuel_id"]),
                VehicleId = Convert.ToInt32(rd["vehicle_id"]),
                VehicleLabel = rd["vlabel"].ToString(),
                FuelDate = Convert.ToDateTime(rd["fuel_date"]),
                Liters = Convert.ToDecimal(rd["liters"]),
                Cost = Convert.ToDecimal(rd["cost"]),
                OdometerReading = Convert.ToInt64(rd["odometer_reading"]),
                StationName = rd["station_name"] == DBNull.Value ? null : rd["station_name"].ToString(),
                Notes = rd["notes"] == DBNull.Value ? null : rd["notes"].ToString(),
                CreatedAt = Convert.ToDateTime(rd["created_at"])
            };
        }
    }

    public class MaintenanceRepository
    {
        public List<MaintenanceRecord> GetByUser(int userId)
        {
            var list = new List<MaintenanceRecord>();
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"SELECT m.maintenance_id, m.vehicle_id, m.service_type_id, s.service_name,
                                               m.maintenance_date, m.cost, m.odometer_reading, m.description,
                                               m.garage_name, m.created_at,
                                               v.brand + ' ' + v.model + ' (' + v.plate_number + ')' AS vlabel
                                        FROM MAINTENANCE_RECORDS m
                                        JOIN VEHICLES v ON m.vehicle_id = v.vehicle_id
                                        JOIN SERVICE_TYPES s ON m.service_type_id = s.service_type_id
                                        WHERE v.user_id = @uid
                                        ORDER BY m.maintenance_date DESC";
                    cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read()) list.Add(Map(rd));
                }
            }
            return list;
        }

        public int Create(MaintenanceRecord m)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO MAINTENANCE_RECORDS (vehicle_id, service_type_id, maintenance_date, cost, odometer_reading, description, garage_name)
                                        VALUES (@vid, @sid, @mdate, @cost, @odo, @descr, @garage);
                                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    cmd.Parameters.Add("@vid", SqlDbType.Int).Value = m.VehicleId;
                    cmd.Parameters.Add("@sid", SqlDbType.Int).Value = m.ServiceTypeId;
                    cmd.Parameters.Add("@mdate", SqlDbType.Date).Value = m.MaintenanceDate;
                    cmd.Parameters.Add("@cost", SqlDbType.Decimal).Value = m.Cost;
                    cmd.Parameters.Add("@odo", SqlDbType.BigInt).Value = (object)m.OdometerReading ?? DBNull.Value;
                    cmd.Parameters.Add("@descr", SqlDbType.NVarChar).Value = (object)m.Description ?? DBNull.Value;
                    cmd.Parameters.Add("@garage", SqlDbType.NVarChar).Value = (object)m.GarageName ?? DBNull.Value;
                    return Convert.ToInt32(cmd.ExecuteScalar());
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
                    cmd.CommandText = "DELETE FROM MAINTENANCE_RECORDS WHERE maintenance_id=@id";
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private MaintenanceRecord Map(IDataRecord rd)
        {
            return new MaintenanceRecord
            {
                MaintenanceId = Convert.ToInt32(rd["maintenance_id"]),
                VehicleId = Convert.ToInt32(rd["vehicle_id"]),
                VehicleLabel = rd["vlabel"].ToString(),
                ServiceTypeId = Convert.ToInt32(rd["service_type_id"]),
                ServiceName = rd["service_name"].ToString(),
                MaintenanceDate = Convert.ToDateTime(rd["maintenance_date"]),
                Cost = Convert.ToDecimal(rd["cost"]),
                OdometerReading = rd["odometer_reading"] == DBNull.Value ? (long?)null : Convert.ToInt64(rd["odometer_reading"]),
                Description = rd["description"] == DBNull.Value ? null : rd["description"].ToString(),
                GarageName = rd["garage_name"] == DBNull.Value ? null : rd["garage_name"].ToString(),
                CreatedAt = Convert.ToDateTime(rd["created_at"])
            };
        }
    }

    public class ReminderRepository
    {
        public List<Reminder> GetByUser(int userId)
        {
            var list = new List<Reminder>();
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"SELECT r.reminder_id, r.vehicle_id, r.service_type_id, s.service_name,
                                               r.reminder_date, r.title, r.status, r.notes, r.created_at,
                                               v.brand + ' ' + v.model + ' (' + v.plate_number + ')' AS vlabel
                                        FROM REMINDERS r
                                        JOIN VEHICLES v ON r.vehicle_id = v.vehicle_id
                                        LEFT JOIN SERVICE_TYPES s ON r.service_type_id = s.service_type_id
                                        WHERE v.user_id = @uid
                                        ORDER BY r.reminder_date ASC";
                    cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read()) list.Add(Map(rd));
                }
            }
            return list;
        }

        public List<Reminder> GetUpcoming(int userId, int days)
        {
            var list = new List<Reminder>();
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"SELECT r.reminder_id, r.vehicle_id, r.service_type_id, s.service_name,
                                               r.reminder_date, r.title, r.status, r.notes, r.created_at,
                                               v.brand + ' ' + v.model + ' (' + v.plate_number + ')' AS vlabel
                                        FROM REMINDERS r
                                        JOIN VEHICLES v ON r.vehicle_id = v.vehicle_id
                                        LEFT JOIN SERVICE_TYPES s ON r.service_type_id = s.service_type_id
                                        WHERE v.user_id = @uid AND r.status = 'Pending'
                                          AND r.reminder_date BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY, @days, CAST(GETDATE() AS DATE))
                                        ORDER BY r.reminder_date ASC";
                    cmd.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@days", SqlDbType.Int).Value = days;
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read()) list.Add(Map(rd));
                }
            }
            return list;
        }

        public int Create(Reminder r)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO REMINDERS (vehicle_id, service_type_id, reminder_date, title, status, notes)
                                        VALUES (@vid, @sid, @rdate, @title, @status, @notes);
                                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    cmd.Parameters.Add("@vid", SqlDbType.Int).Value = r.VehicleId;
                    cmd.Parameters.Add("@sid", SqlDbType.Int).Value = (object)r.ServiceTypeId ?? DBNull.Value;
                    cmd.Parameters.Add("@rdate", SqlDbType.Date).Value = r.ReminderDate;
                    cmd.Parameters.Add("@title", SqlDbType.NVarChar).Value = r.Title;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(r.Status) ? "Pending" : r.Status;
                    cmd.Parameters.Add("@notes", SqlDbType.NVarChar).Value = (object)r.Notes ?? DBNull.Value;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public void UpdateStatus(int id, string status)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE REMINDERS SET status=@s WHERE reminder_id=@id";
                    cmd.Parameters.Add("@s", SqlDbType.NVarChar).Value = status;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
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
                    cmd.CommandText = "DELETE FROM REMINDERS WHERE reminder_id=@id";
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Reminder Map(IDataRecord rd)
        {
            return new Reminder
            {
                ReminderId = Convert.ToInt32(rd["reminder_id"]),
                VehicleId = Convert.ToInt32(rd["vehicle_id"]),
                VehicleLabel = rd["vlabel"].ToString(),
                ServiceTypeId = rd["service_type_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["service_type_id"]),
                ServiceName = rd["service_name"] == DBNull.Value ? null : rd["service_name"].ToString(),
                ReminderDate = Convert.ToDateTime(rd["reminder_date"]),
                Title = rd["title"].ToString(),
                Status = rd["status"].ToString(),
                Notes = rd["notes"] == DBNull.Value ? null : rd["notes"].ToString(),
                CreatedAt = Convert.ToDateTime(rd["created_at"])
            };
        }
    }
}
