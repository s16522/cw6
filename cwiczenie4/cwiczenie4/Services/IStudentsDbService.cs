using cwiczenie3.DTO;
using cwiczenie3.Models;

namespace cwiczenie3.Services
{
    public interface IStudentsDbService
    {
        public string PostStudent(PostStudentRequest student);

        public string PromoteStudents(PromoteStudentRequest request);
        
        public string GetStudent(string index);
    }
}