using Hotel.Domain.Interfaces;
using Hotel.Domain.Models;
using Hotel.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests;


    public class DeparturesControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            var fakeDepartures = new List<Departure>
            {
                new Departure { Id = 1, Code = "MSK", Name = "Из Москвы" },
                new Departure { Id = 2, Code = "SPB", Name = "Из Петербурга" }
            };

            mockService.Setup(s => s.GetAllAsync())
                       .ReturnsAsync(fakeDepartures);

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var departures = Assert.IsType<List<Departure>>(okResult.Value);
            Assert.Equal(2, departures.Count);

            mockService.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_OnException_Returns500()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            mockService.Setup(s => s.GetAllAsync())
                       .ThrowsAsync(new Exception("Test exception"));

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetAll();

            // Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objResult.StatusCode);
            Assert.Equal("Internal Server Error", objResult.Value);

            // Проверяем логирование через базовый метод Log(...)
            mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error in GetAll Departures")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
            ), Times.Once);
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            var fakeDep = new Departure { Id = 10, Code = "KZN", Name = "Из Казани" };

            mockService.Setup(s => s.GetByIdAsync(10))
                       .ReturnsAsync(fakeDep);

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetById(10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var departure = Assert.IsType<Departure>(okResult.Value);
            Assert.Equal(10, departure.Id);
        }

        [Fact]
        public async Task GetById_NotFound_Returns404()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            mockService.Setup(s => s.GetByIdAsync(777))
                       .ReturnsAsync((Departure)null);

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetById(777);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Departure not found.", notFound.Value);
        }

        [Fact]
        public async Task GetById_OnException_Returns500()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            mockService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                       .ThrowsAsync(new Exception("Test exception"));

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetById(1);

            // Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objResult.StatusCode);
            Assert.Equal("Internal Server Error", objResult.Value);

            mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error in GetById Departures")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
            ), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnsCreatedWithNewId()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            var newDeparture = new Departure { Code = "EKB", Name = "Из Екатеринбурга" };
            mockService.Setup(s => s.CreateAsync(It.IsAny<Departure>()))
                       .ReturnsAsync(123);

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.Create(newDeparture);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(DeparturesController.GetById), createdResult.ActionName);
            Assert.Equal(123, createdResult.RouteValues["id"]);

            var returnedValue = Assert.IsType<int>(createdResult.Value);
            Assert.Equal(123, returnedValue);
        }

        [Fact]
        public async Task Create_OnException_Returns500()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            var departure = new Departure { Code = "NSK", Name = "Из Новосибирска" };
            mockService.Setup(s => s.CreateAsync(It.IsAny<Departure>()))
                       .ThrowsAsync(new Exception("Test exception"));

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.Create(departure);

            // Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objResult.StatusCode);
            Assert.Equal("Internal Server Error", objResult.Value);

            mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error in Create Departure")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
            ), Times.Once);
        }

        [Fact]
        public async Task Update_BadRequest_WhenIdMismatch()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);
            var departure = new Departure { Id = 5, Code = "XYZ" };

            // Act
            var result = await controller.Update(10, departure);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("URL id and model id do not match.", badRequest.Value);
        }

        [Fact]
        public async Task Update_NotFound_WhenFalseReturned()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            mockService.Setup(s => s.UpdateAsync(It.IsAny<Departure>()))
                       .ReturnsAsync(false);

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);
            var departure = new Departure { Id = 10, Code = "Valid" };

            // Act
            var result = await controller.Update(10, departure);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Departure not found.", notFound.Value);
        }

        [Fact]
        public async Task Update_NoContent_WhenTrueReturned()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            mockService.Setup(s => s.UpdateAsync(It.IsAny<Departure>()))
                       .ReturnsAsync(true);

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);
            var departure = new Departure { Id = 10, Code = "Valid" };

            // Act
            var result = await controller.Update(10, departure);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_OnException_Returns500()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            mockService.Setup(s => s.UpdateAsync(It.IsAny<Departure>()))
                       .ThrowsAsync(new Exception("Test exception"));

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.Update(1, new Departure { Id = 1 });

            // Assert
            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
            Assert.Equal("Internal Server Error", objResult.Value);

            mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error in Update Departure")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
            ), Times.Once);
        }

        [Fact]
        public async Task Delete_NotFound_WhenFalseReturned()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            mockService.Setup(s => s.DeleteAsync(99))
                       .ReturnsAsync(false);

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.Delete(99);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Departure not found.", notFound.Value);
        }

        [Fact]
        public async Task Delete_NoContent_WhenTrueReturned()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            mockService.Setup(s => s.DeleteAsync(99))
                       .ReturnsAsync(true);

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.Delete(99);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_OnException_Returns500()
        {
            // Arrange
            var mockService = new Mock<IDepartureService>();
            var mockLogger = new Mock<ILogger<DeparturesController>>();

            mockService.Setup(s => s.DeleteAsync(It.IsAny<int>()))
                       .ThrowsAsync(new Exception("Test exception"));

            var controller = new DeparturesController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.Delete(123);

            // Assert
            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objResult.StatusCode);
            Assert.Equal("Internal Server Error", objResult.Value);

            mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error in Delete Departure")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
            ), Times.Once);
        }
    }

