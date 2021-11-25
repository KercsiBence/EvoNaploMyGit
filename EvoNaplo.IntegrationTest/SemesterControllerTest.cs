using EvoNaplo.Controllers;
using EvoNaplo.DataAccessLayer;
using EvoNaplo.Models;
using EvoNaplo.Models.DTO;
using EvoNaplo.Services;
using EvoNaplo.TestHelper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNaplo.IntegrationTest
{
    [TestFixture]
    public class SemesterControllerTest
    {
        private SemesterController _semesterController;
        private EvoNaploContext _evoNaploContext;

        [SetUp]
        public void SetUp()
        {
            _evoNaploContext = EvoNaploContextHelper.CreateInMemoryDatabaseContext();
            SemesterService semesterService = new SemesterService(_evoNaploContext);

            _semesterController = new SemesterController(semesterService);
        }

        [TearDown]
        public void TearDown()
        {
            _semesterController = null;
            _evoNaploContext = null;
        }
        [Test]
        public async Task AddSemester()
        {
            //Arrange
            List<Semester> semesters = _evoNaploContext.Semesters.ToList();
            int expectedValue = semesters.Count+1;
            Semester semester = new()
            {
                Id = 2,
                StartDate = new DateTime(2016, 8, 21),
                EndDate = new DateTime(2022, 9, 13),
                IsAppliable = true
            };

            //Act
            await _semesterController.AddSemester(semester);
            _evoNaploContext.SaveChanges();

            //Assert
            Assert.AreEqual(expectedValue, _evoNaploContext.Semesters.ToList().Count);
        }
        [Test]
        public void GetSemesters()
        {
            //Arrange
            int expectedValue=_evoNaploContext.Semesters.ToList().Count()+1;
            Semester semester = TestSemester();

            //Act
            _evoNaploContext.Semesters.Add(semester);
            _evoNaploContext.SaveChanges();
            int actualValue = _semesterController.GetSemesters().Count();

            //Assert
            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void GetSemestersByID()
        {
            //Arrange
            Semester semester = TestSemester();
            SemesterDTO semesterDTO = new(semester);

            _evoNaploContext.Semesters.Add(semester);
            _evoNaploContext.SaveChanges();

            //Act
            SemesterDTO actual = _semesterController.GetSemesterById(semester.Id);

            //Assert
            Assert.AreEqual(semesterDTO.Id, actual.Id);
            Assert.AreEqual(semesterDTO.IsAppliable, actual.IsAppliable);
            Assert.AreEqual(semesterDTO.StartDate, actual.StartDate);
            Assert.AreEqual(semesterDTO.EndDate, actual.EndDate);
        }
        [Test]
        public void GetSemesterToEditById()
        {
            //Arrange
            Semester semester = TestSemester();

            _evoNaploContext.Semesters.Add(semester);
            _evoNaploContext.SaveChanges();

            //Act
            Semester actual = _semesterController.GetSemesterToEditById(semester.Id);

            //Assert
            Assert.AreEqual(semester.Id, actual.Id);
            Assert.AreEqual(semester.IsAppliable, actual.IsAppliable);
            Assert.AreEqual(semester.StartDate, actual.StartDate);
            Assert.AreEqual(semester.EndDate, actual.EndDate);
        }
        [Test]
        public async Task EditSemester()
        {
            //Arrange

            Semester semester = TestSemester();
            _evoNaploContext.Semesters.Add(semester);
            _evoNaploContext.SaveChanges();
            semester.IsAppliable = false;

            //Act
            await _semesterController.EditSemester(semester);
            _evoNaploContext.SaveChanges();

            //Assert
            Assert.AreEqual(false, _evoNaploContext.Semesters.Last().IsAppliable);

        }
        [Test]
        public async Task DeleteSemester()
        {
            //Arrange
            _evoNaploContext.Semesters.Add(TestSemester());
            //ACt
            await _semesterController.DeleteSemester(TestSemester().Id);
            _evoNaploContext.SaveChanges();

            //Assert
            Assert.IsFalse(_evoNaploContext.Semesters.Contains(TestSemester()));

        }
        [Test]
        public void GetSemesterProjects()
        {
            //Arrange
            Semester semester = TestSemester();
            Project project = new()
            {
                Id = 1,
                ProjectName = "EvoNaplo",
                Description = "Egy naplo",
                SourceLink = "asd",
                Technologies = "C#",
                SemesterId = 2
            };
            _evoNaploContext.Projects.Add(project);
            _evoNaploContext.SaveChanges();

            //Act
            List<ProjectDTO> projects = _semesterController.GetSemesterProjects(semester.Id).ToList();

            //Assert
            Assert.AreEqual(1, projects.Count);
        }
        [Test]
        public async Task JoinSemester()
        {
            //Arrange
            User user = UserHelper.CreateDefaultUser(User.RoleTypes.Student);
            _evoNaploContext.Users.Add(user);
            Semester semester = TestSemester();
            _evoNaploContext.Semesters.Add(semester);
            _evoNaploContext.SaveChanges();



            //Act
            await _semesterController.JoinSemester(user.Id);
            _evoNaploContext.SaveChanges();

            //Assert
            Assert.IsTrue(_evoNaploContext.UsersOnSemester.Last().UserId == user.Id);
            Assert.IsTrue(_evoNaploContext.UsersOnSemester.Last().SemesterId==semester.Id);

        }



        private Semester TestSemester()
        {
            Semester semester = new()
            {
                Id = 2,
                StartDate = new DateTime(2016, 8, 21),
                EndDate = new DateTime(2022, 9, 13),
                IsAppliable = true
            };
            return semester;
        }
    }
}
