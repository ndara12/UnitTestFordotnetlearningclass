using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using dotnetlearningclass.Controllers;
using dotnetlearningclass.Entities;
using dotnetlearningclass;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace UnitTestForLearningClass
{
    public class TestForStudentControllers
    {
        [Fact]
        public async Task CreateStudent_ValidStudent_ReturnsCreatedResult()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<Students>>();
            var controller = new StudentsController(mockRepository.Object);

            var validStudent = new Students
            {
                StudentId = 99999999,  // Make sure this ID doesn't exist in the original DB
                StudentName = "John Doe",
                StudentClassId = 1
            };

            // Set up the repository to return the entity when adding
            mockRepository.Setup(repo => repo.AddAsync(validStudent)).Callback(() =>
            {
                // Simulate the behavior of adding the entity to the database
                validStudent.StudentId = 99999999; // Assign an ID as if it was added to the database
            });

            // Act
            var result = await controller.CreateStudent(validStudent);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
            Assert.Equal(nameof(controller.GetStudent), createdAtActionResult.ActionName);
            // You can add more assertions based on your specific requirements  
        }

        [Fact]
        public async Task GetStudent_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<Students>>();
            var controller = new StudentsController(mockRepository.Object);
            var existingStudent = new Students
            {
                StudentId = 1,
                StudentName = "John Doe",
                StudentClassId = 1
            };

            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingStudent);

            // Act
            var result = await controller.GetStudent(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result); // Check if the outer result is OkObjectResult
            var student = Assert.IsType<Students>(okResult.Value); // Check if the Value property is of type Students
           
            Assert.Equal(1, student.StudentId);
            Assert.Equal("John Doe", student.StudentName);
            Assert.Equal(1, student.StudentClassId);
        }

        [Fact]
        public async Task GetStudent_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<Students>>();
            var controller = new StudentsController(mockRepository.Object);

            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Students)null);

            // Act
            var result = await controller.GetStudent(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result); // Check the Result property of ActionResult
        }

        [Fact]
        public async Task UpdateStudent_ExistingId_ReturnsNoContentResult()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<Students>>();
            var controller = new StudentsController(mockRepository.Object);

            var existingStudent = new Students
            {
                StudentId = 1,
                StudentName = "John Doe",
                StudentClassId = 1
            };

            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingStudent);

            var updatedStudent = new Students
            {
                StudentId = 1,
                StudentName = "Updated John Doe",
                StudentClassId = 2
            };

            // Act
            var result = await controller.UpdateStudent(1, updatedStudent);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(existingStudent.StudentName, "Updated John Doe"); // Check if the entity is updated
            Assert.Equal(existingStudent.StudentClassId, 2);
        }

        [Fact]
        public async Task UpdateStudent_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<Students>>();
            var controller = new StudentsController(mockRepository.Object);

            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Students)null);

            var updatedStudent = new Students
            {
                StudentId = 1,
                StudentName = "Updated John Doe",
                StudentClassId = 2
            };

            // Act
            var result = await controller.UpdateStudent(1, updatedStudent);

            // Assert
            var IsResultNotFound = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteStudent_ExistingId_ReturnsNoContentResult()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<Students>>();
            var controller = new StudentsController(mockRepository.Object);

            var existingStudent = new Students
            {
                StudentId = 1,
                StudentName = "John Doe",
                StudentClassId = 1
            };

            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingStudent);

            // Act
            var result = await controller.DeleteStudent(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteStudent_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<Students>>();
            var controller = new StudentsController(mockRepository.Object);

            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Students)null);

            // Act
            var result = await controller.DeleteStudent(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result); // Change to NotFoundObjectResult
        }

    }
}
