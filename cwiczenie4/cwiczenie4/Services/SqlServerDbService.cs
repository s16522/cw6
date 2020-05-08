using System;
using System.Data;
using System.Data.SqlClient;
using cwiczenie3.DAL;
using cwiczenie3.DTO;
using cwiczenie3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cwiczenie3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        private static string _connStr = $@"
            Server=127.0.0.1,1433;
            Database=Master;
            User Id=SA;
            Password=<YourNewStrong@Passw0rd>
            ";
        
        public string PostStudent(PostStudentRequest student)
        {
            Enrollment enrollment = new Enrollment();

            using (var connection = new SqlConnection(_connStr))
            using (var query = new SqlCommand())
            {
                query.Connection = connection;
                connection.Open();

                var transaction = connection.BeginTransaction();

                try
                {
                    query.CommandText = "SELECT IdStudy FROM Studies where Name=@name";
                    query.Parameters.AddWithValue("name", student.Studies);
                    query.Transaction = transaction;

                    var studs = query.ExecuteReader();
                    if (!studs.Read())
                    {
                        return "Not Found";
                    }

                    int idstudies = (int) studs["IdStudy"];
                    studs.Close();

                    query.CommandText =
                        "SELECT IdEnrollment FROM Enrollment WHERE IdEnrollment >= (SELECT MAX(IdEnrollment) FROM Enrollment)";
                    var idenr = query.ExecuteReader();
                    
                    int idEnroll = (int) idenr["IdEnrollment"] + 10;
                    idenr.Close();

                    query.CommandText =
                        "SELECT idEnrollment, StartDate from Enrollment WHERE idStudy=@idStudy AND Semester=1 ORDER BY StartDate";
                    query.Parameters.AddWithValue("idStudy", idstudies);

                    DateTime enrollDate;

                    var enrol = query.ExecuteReader();
                    if (!enrol.Read())
                    {
                        enrollDate = DateTime.Now;
                        query.CommandText = "INSERT INTO Enrollment VALUES(@id, @Semester, @IdStud, @StartDate)";
                        query.Parameters.AddWithValue("id", idEnroll);
                        query.Parameters.AddWithValue("Semester", 1);
                        query.Parameters.AddWithValue("IdStud", idstudies);
                        query.Parameters.AddWithValue("StartDate", enrollDate);
                        enrol.Close();
                        query.ExecuteNonQuery();
                    }
                    else
                    {
                        idEnroll = (int) enrol["IdEnrollment"];
                        enrollDate = (DateTime) enrol["StartDate"];
                        enrol.Close();
                    }

                    enrollment.IdEnrollment = idEnroll;
                    enrollment.Semester = 1;
                    enrollment.IdStudy = idstudies;
                    enrollment.StartDate = enrollDate;

                    query.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber=@indexNum";
                    query.Parameters.AddWithValue("indexNum", student.IndexNumber);

                    DateTime bDate = Convert.ToDateTime(student.BirthDate);
                    string formattedDate = bDate.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    try
                    {
                        query.CommandText =
                            "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@index, @fName, @lName, @birthDate, @idEnrollment)";
                        query.Parameters.AddWithValue("index", student.IndexNumber);
                        query.Parameters.AddWithValue("fName", student.FirstName);
                        query.Parameters.AddWithValue("lName", student.LastName);
                        query.Parameters.AddWithValue("birthDate", formattedDate);
                        query.Parameters.AddWithValue("idEnrollment", idEnroll);

                        query.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        return "BadRequest";
                    }
                }
                catch (SqlException exc)
                {
                    transaction.Rollback();
                    return "BadRequest";
                }

                return enrollment.ToString();
            }
        }
        
        public string PromoteStudents(PromoteStudentRequest request)
        {
            using (var connection = new SqlConnection(_connStr))
            
            {
             
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                connection.Open();

                int newSem = request.Semester + 1;

                command.CommandText = "SELECT IdStudy FROM Studies where Name=@name";
                command.Parameters.AddWithValue("name", request.Studies);

                var studs = command.ExecuteReader();
                if (!studs.Read())
                {
                      
                      return "Not found";
                }
                int idstudies = (int)studs["IdStudy"];
                studs.Close();

                Enrollment enrollment = new Enrollment();

                command.CommandText = "SELECT idEnrollment,idStudy,Semester,StartDate FROM Enrollment WHERE idStudy=@idStudy AND Semester=@Semester";
                command.Parameters.AddWithValue("idStudy", idstudies);
                command.Parameters.AddWithValue("Semester", newSem);

                var enr = command.ExecuteReader();
                if (enr.Read())
                {
                    enrollment.IdEnrollment = (int)enr["IdEnrollment"];
                    enrollment.IdStudy = (int)enr["IdStudy"];
                    enrollment.Semester = (int)enr["Semester"];
                    enrollment.StartDate = (DateTime)enr["StartDate"];
                }
                enr.Close();
                

                return enrollment.ToString();
            }
        }
        public string GetStudent(string index)
        {
            using (var connection = new SqlConnection(_connStr))
            using (var query = new SqlCommand())
            {
                query.Connection = connection;

                query.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber=@index";
                query.Parameters.AddWithValue("index", index);
                connection.Open();

                var dr = query.ExecuteReader();
                if (dr.Read())
                {
                    return index;
                }
                return null;
            } 
        }
    }
}