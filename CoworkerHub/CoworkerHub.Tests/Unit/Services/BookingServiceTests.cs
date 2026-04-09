using AutoMapper;
using CoworkerHub.Application.DTOs.Booking;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Application.Services;
using CoworkerHub.Domain.Entities;
using CoworkerHub.Domain.Enums;
using Moq;
using Xunit;

namespace CoworkerHub.Tests.Unit.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<IDeskRepository> _mockDeskRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockDeskRepository = new Mock<IDeskRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();

            _bookingService = new BookingService(
                _mockBookingRepository.Object,
                _mockDeskRepository.Object,
                _mockUnitOfWork.Object,
                _mockMapper.Object
            );
        }

        #region CreateBookingAsync Tests

        [Fact]
        public async Task CreateBookingAsync_WithValidData_ReturnsBookingDTO()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createModel = new CreateBookingDTO
            {
                DeskId = 1,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2)
            };

            var desk = new Desk
            {
                Id = 1,
                Name = "Desk 1",
                PricePerHour = 10m,
                Status = DeskStatus.Available,
                WorkspaceId = 1
            };

            var booking = new Booking
            {
                Id = 1,
                DeskId = 1,
                UserId = userId,
                StartTime = createModel.StartTime,
                EndTime = createModel.EndTime,
                TotalPrice = 10m,
                Status = BookingStatus.Pending
            };

            var bookingDTO = new BookingDTO
            {
                Id = 1,
                DeskId = 1,
                StartTime = createModel.StartTime,
                EndTime = createModel.EndTime,
                TotalPrice = 10m
            };

            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(1, CancellationToken.None))
                .ReturnsAsync(desk);
            _mockBookingRepository.Setup(x => x.HasOverlappingBookingsAsync(1, createModel.StartTime, createModel.EndTime, CancellationToken.None))
                .ReturnsAsync(false);
            _mockMapper.Setup(x => x.Map<BookingDTO>(It.IsAny<Booking>()))
                .Returns(bookingDTO);

            // Act
            var result = await _bookingService.CreateBookingAsync(userId, createModel, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookingDTO.DeskId, result.DeskId);
            Assert.Equal(bookingDTO.TotalPrice, result.TotalPrice);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_WithNonExistentDesk_ThrowsNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createModel = new CreateBookingDTO
            {
                DeskId = 999,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2)
            };

            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(999, CancellationToken.None))
                .ReturnsAsync((Desk)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _bookingService.CreateBookingAsync(userId, createModel, CancellationToken.None)
            );

            Assert.Contains("Desk with ID 999 not found", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_WithMaintenanceDesk_ThrowsAppValidationException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createModel = new CreateBookingDTO
            {
                DeskId = 1,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2)
            };

            var desk = new Desk
            {
                Id = 1,
                Name = "Desk 1",
                Status = DeskStatus.Maintenance,
                WorkspaceId = 1
            };

            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(1, CancellationToken.None))
                .ReturnsAsync(desk);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppValidationException>(
                () => _bookingService.CreateBookingAsync(userId, createModel, CancellationToken.None)
            );

            Assert.Contains("under repair", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_WithOverlappingBooking_ThrowsAlreadyExistsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createModel = new CreateBookingDTO
            {
                DeskId = 1,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2)
            };

            var desk = new Desk
            {
                Id = 1,
                Name = "Desk 1",
                PricePerHour = 10m,
                Status = DeskStatus.Available,
                WorkspaceId = 1
            };

            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(1, CancellationToken.None))
                .ReturnsAsync(desk);
            _mockBookingRepository.Setup(x => x.HasOverlappingBookingsAsync(1, createModel.StartTime, createModel.EndTime, CancellationToken.None))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AlreadyExistsException>(
                () => _bookingService.CreateBookingAsync(userId, createModel, CancellationToken.None)
            );

            Assert.Contains("already booked", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_CalculatesPriceCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddHours(5);

            var createModel = new CreateBookingDTO
            {
                DeskId = 1,
                StartTime = startTime,
                EndTime = endTime
            };

            var desk = new Desk
            {
                Id = 1,
                Name = "Desk 1",
                PricePerHour = 20m,
                Status = DeskStatus.Available,
                WorkspaceId = 1
            };

            var bookingDTO = new BookingDTO
            {
                Id = 1,
                DeskId = 1,
                StartTime = startTime,
                EndTime = endTime,
                TotalPrice = 100m  // 5 часов * 20m
            };

            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(1, CancellationToken.None))
                .ReturnsAsync(desk);
            _mockBookingRepository.Setup(x => x.HasOverlappingBookingsAsync(1, startTime, endTime, CancellationToken.None))
                .ReturnsAsync(false);
            _mockMapper.Setup(x => x.Map<BookingDTO>(It.IsAny<Booking>()))
                .Returns(bookingDTO);

            // Act
            var result = await _bookingService.CreateBookingAsync(userId, createModel, CancellationToken.None);

            // Assert
            Assert.Equal(100m, result.TotalPrice);
        }

        #endregion

        #region GetMyBookingsAsync Tests

        [Fact]
        public async Task GetMyBookingsAsync_WithValidUserId_ReturnsBookings()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookings = new List<Booking>
            {
                new Booking { Id = 1, UserId = userId, DeskId = 1, Status = BookingStatus.Pending },
                new Booking { Id = 2, UserId = userId, DeskId = 2, Status = BookingStatus.Confirmed }
            };

            var bookingDTOs = new List<BookingDTO>
            {
                new BookingDTO { Id = 1, DeskId = 1 },
                new BookingDTO { Id = 2, DeskId = 2 }
            };

            _mockBookingRepository.Setup(x => x.GetBookingsByUserIdAsync(userId, CancellationToken.None))
                .ReturnsAsync(bookings);
            _mockMapper.Setup(x => x.Map<List<BookingDTO>>(bookings))
                .Returns(bookingDTOs);

            // Act
            var result = await _bookingService.GetMyBookingsAsync(userId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal(2, result[1].Id);
        }

        [Fact]
        public async Task GetMyBookingsAsync_WithNoBookings_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var bookings = new List<Booking>();
            var bookingDTOs = new List<BookingDTO>();

            _mockBookingRepository.Setup(x => x.GetBookingsByUserIdAsync(userId, CancellationToken.None))
                .ReturnsAsync(bookings);
            _mockMapper.Setup(x => x.Map<List<BookingDTO>>(bookings))
                .Returns(bookingDTOs);

            // Act
            var result = await _bookingService.GetMyBookingsAsync(userId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion
    }
}
