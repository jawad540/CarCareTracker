using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CarCareTracker.Data;
using CarCareTracker.Models;

namespace CarCareTracker.Repositories
{
    public class UserRepository
    {
        public User GetByEmail(string email)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"SELECT u.user_id, u.full_name, u.email, u.password_hash,
                                               u.phone, u.role_id, r.role_name, u.is_active, u.created_at
                                        FROM USERS u JOIN ROLES r ON u.role_id = r.role_id
                                        WHERE LOWER(u.email) = LOWER(@email)";
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read()) return Map(rd);
                    }
                }
            }
            return null;
        }

        public User GetById(int id)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"SELECT u.user_id, u.full_name, u.email, u.password_hash,
                                               u.phone, u.role_id, r.role_name, u.is_active, u.created_at
                                        FROM USERS u JOIN ROLES r ON u.role_id = r.role_id
                                        WHERE u.user_id = @id";
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read()) return Map(rd);
                    }
                }
            }
            return null;
        }

        public bool EmailExists(string email)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM USERS WHERE LOWER(email)=LOWER(@email)";
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public int Create(User u)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO USERS (full_name, email, password_hash, phone, role_id, is_active)
                                        VALUES (@fn, @em, @ph, @phone, @role, 1);
                                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    cmd.Parameters.Add("@fn", SqlDbType.NVarChar).Value = u.FullName;
                    cmd.Parameters.Add("@em", SqlDbType.NVarChar).Value = u.Email;
                    cmd.Parameters.Add("@ph", SqlDbType.NVarChar).Value = u.PasswordHash;
                    cmd.Parameters.Add("@phone", SqlDbType.NVarChar).Value = (object)u.Phone ?? DBNull.Value;
                    cmd.Parameters.Add("@role", SqlDbType.Int).Value = u.RoleId;
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public List<User> GetAll()
        {
            var list = new List<User>();
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"SELECT u.user_id, u.full_name, u.email, u.password_hash,
                                               u.phone, u.role_id, r.role_name, u.is_active, u.created_at
                                        FROM USERS u JOIN ROLES r ON u.role_id = r.role_id
                                        ORDER BY u.created_at DESC";
                    using (var rd = cmd.ExecuteReader())
                        while (rd.Read()) list.Add(Map(rd));
                }
            }
            return list;
        }

        public void SetActive(int userId, bool active)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE USERS SET is_active=@a WHERE user_id=@id";
                    cmd.Parameters.Add("@a", SqlDbType.Bit).Value = active ? 1 : 0;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int userId)
        {
            using (var con = AppDb.GetConnection())
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM USERS WHERE user_id=@id";
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private User Map(IDataRecord rd)
        {
            return new User
            {
                UserId = Convert.ToInt32(rd["user_id"]),
                FullName = rd["full_name"].ToString(),
                Email = rd["email"].ToString(),
                PasswordHash = rd["password_hash"].ToString(),
                Phone = rd["phone"] == DBNull.Value ? null : rd["phone"].ToString(),
                RoleId = Convert.ToInt32(rd["role_id"]),
                RoleName = rd["role_name"].ToString(),
                IsActive = Convert.ToBoolean(rd["is_active"]),
                CreatedAt = Convert.ToDateTime(rd["created_at"])
            };
        }
    }
}
