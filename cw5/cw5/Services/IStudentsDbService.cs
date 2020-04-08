using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cw5.Models;
namespace cw5.Services
{
    public interface IStudentsDbService
    {
        Task<Study> GetStudy(string name);
        Task<Enroll> GetEnroll(string study, int semester);

        Task Register(string index, string firstName, string lastName, DateTime birthDate, int studyId);
        Task Promote(string studies, int semester);
        bool checkStudentIndex(string index);
    }
}
