using System;
using System.Threading.Tasks;
using cw5.Models;
using Microsoft.Data.SqlClient;

namespace cw5.Services
{
    public class SQLServerDvService : IStudentsDbService
    {
        private readonly string _conn = "Data Source=db-mssql;Initial Catalog=s18579;Integrated Security=True";
        public bool checkStudentIndex(string index)
        {
            using (var con = new SqlConnection(_conn))
            {
                using (var com = new SqlCommand())
                {
                    com.Connection = con;
                    con.Open();
                    com.CommandText = "SELECT 1 FROM student s WHERE s.indexnumber = @index";
                    com.Parameters.AddWithValue("index", index);
                    var dr = com.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        dr.Close();
                        return true;
                    }
                    dr.Close();
                    return false;
                }
            }
        }
        public async Task<Study> GetStudy(string name)
        {
            using (var conn = new SqlConnection(_conn))
            {
                using (var com = new SqlCommand())
                {
                    com.Connection = conn;
                    com.CommandText = @"SELECT idstudy, name
                                        FROM studies WHERE name = @name";
                    com.Parameters.AddWithValue("name", name);
                    conn.Open();
                    try
                    {
                        var reader = await com.ExecuteReaderAsync();
                        await reader.ReadAsync();
                        var study = new Study
                        {
                            IdStudy = int.Parse(reader["idstudy"].ToString()),
                            Name = reader["name"].ToString()
                        };
                        return study;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
        }
        public async Task<Enroll> GetEnroll(string study, int semester)
        {
            using (var conn = new SqlConnection(_conn))
            {
                using (var com = new SqlCommand())
                {
                    com.Connection = conn;
                    com.CommandText =@"SELECT e.idenrollment, e.semester, e.startdate, s.idstudy, s.name
                                    FROM Enrollment e
                                    INNER JOIN studies s ON s.idstudy = e.idstudy
                                    WHERE s.name = @studyname
                                    AND e.semester = @semester";
                    com.Parameters.AddWithValue("studyname", study);
                    com.Parameters.AddWithValue("semester", semester);
                    conn.Open();
                    using (var reader = await com.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        return new Enroll
                        {
                            IdEnrollment = int.Parse(reader["idenrollment"].ToString()),
                            Semester = int.Parse(reader["semester"].ToString()),
                            StartDate = DateTime.Parse(reader["startdate"].ToString()),
                            Study = new Study
                            {
                                IdStudy = int.Parse(reader["idstudy"].ToString()),
                                Name = reader["name"].ToString(),
                            }
                        };
                    }
                }
            }
        }
        public async Task Register(string index, string firstName, string lastName, DateTime birthDate, int studyId)
        {
            using (var conn = new SqlConnection(_conn))
            {
                using (var com = new SqlCommand())
                {
                    conn.Open();
                    var transaction = conn.BeginTransaction();

                    com.Connection = conn;
                    com.Transaction = transaction;
                    com.CommandText =
                        @"DECLARE @_enrollment int
                          SELECT @_enrollment = e.idenrollment
                          FROM enrollment e
                          INNER JOIN studies s ON e.idstudy = s.idstudy
                          WHERE e.idstudy = @_study AND e.semester = 1;
                          IF @_enrollment IS NULL
                          BEGIN
                            SELECT @_enrollment = max(idenrollment) + 1 
                            FROM enrollment;
                            INSERT INTO enrollment 
                            VALUES (@_enrollment, 1, @_study, getdate());
                          END
                          INSERT INTO Student values (@_index, @_firstname, @_lastname, @_birthdate, @_enrollment)";
                    com.Parameters.AddWithValue("_study", studyId);
                    com.Parameters.AddWithValue("_index", index);
                    com.Parameters.AddWithValue("_firstName", firstName);
                    com.Parameters.AddWithValue("_lastName", lastName);
                    com.Parameters.AddWithValue("_birthDate", birthDate);
                    await com.ExecuteNonQueryAsync();
                    try
                    {
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                    }
                }
            }
        }
        public async Task Promote(string studies, int semester)
        {
            using (var conn = new SqlConnection(_conn))
            {
                using (var com = new SqlCommand())
                {
                    com.Connection = conn;
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "PromoteStudents";
                    /*Create PROCEDURE PromoteStudents @studies varchar(255), @semester int
                    AS
                    DECLARE @tmp int
                    BEGIN

                        IF EXISTS(SELECT 1 FROM Studies WHERE name = @studies)
	                    BEGIN
                            INSERT INTO Studies VALUES((SELECT max(idstudy) + 1 FROM Studies),@studies);
                                        RETURN
                                    END;
                                        DECLARE cur CURSOR FOR SELECT IdEnrollment FROM enrollment e
                                        INNER JOIN studies s on e.idstudy = s.idstudy
                        WHERE s.name = @studies and e.semester = @semester;
                                        OPEN cur

                        FETCH FROM cur INTO @tmp
                            UPDATE Enrollment
                            SET Semester = Semester + 1

                            WHERE IdEnrollment = @tmp

                            FETCH FROM cur INTO @tmp
                        CLOSE cur;
                                        END;
                    */
                    com.Parameters.AddWithValue("studies", studies);
                    com.Parameters.AddWithValue("semester", semester);
                    conn.Open();
                    await com.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
