using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using cwiczenie3.DAL;
using cwiczenie3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cwiczenie3.Controllers
{
    [ApiController]
    [Route("api/students")]
    
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        private static string _connStr = $@"
            Server=127.0.0.1,1433;
            Database=Master;
            User Id=SA;
            Password=<YourNewStrong@Passw0rd>
            ";

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        
        [HttpGet]
        public IActionResult GetStudent(string orderBy)
        {
            List<Student> _students = new List<Student>();
            using (var client = new SqlConnection(_connStr))
            {
                using (var query = new SqlCommand())
                {
                    query.Connection = client;
                    query.CommandText = "Select FirstName, LastName, BirthDate, Semester, Name from Student JOIN Enrollment ON (Student.IdEnrollment = Enrollment.IdEnrollment) JOIN Studies ON (Enrollment.IdStudy = Studies.IdStudy)";
        
                    client.Open();
                    var dr = query.ExecuteReader();
                    while (dr.Read())
                    {
                        var st = new Student();
                        st.FirstName = dr["FirstName"].ToString();
                        st.LastName = dr["LastName"].ToString();
                        st.BirthDate = DateTime.Parse(dr["BirthDate"].ToString());
                        //st.Name = dr["Name"].ToString();
                        st.Semester = Int32.Parse(dr["Semester"].ToString());
                        _students.Add(st);
                    }
                }
            }

            return Ok(_students);
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentById(string id)
        {
            String result = null;
            using (var client = new SqlConnection(_connStr))
            {
                using (var query = new SqlCommand())
                {
                    query.Connection = client;
                    query.CommandText = "Select IndexNumber, Semester, StartDate, Name from Student JOIN Enrollment ON (Student.IdEnrollment = Enrollment.IdEnrollment) JOIN Studies ON (Enrollment.IdStudy = Studies.IdStudy) WHERE Student.IndexNumber = @id";
                    query.Parameters.AddWithValue("id", id);
                    client.Open();
                    var dr = query.ExecuteReader();
                    while (dr.Read())
                    {
                        
                        result = "Data rozpoczęcia: " + dr["StartDate"].ToString() + ", nazwa: " + dr["Name"].ToString() + ", semestr: " + dr["Semester"].ToString();
                    }
                }
            }

            return Ok(result);
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
           // student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent() {
            return Ok("Aktualizacja dokończona");
        }
        
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent() {
            return Ok("Usuwanie ukończone");
        }
        
    }
}