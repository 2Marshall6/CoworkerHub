using AutoMapper;
using CoworkerHub.Application.DTOs.Desk;
using CoworkerHub.Application.Exceptions;
using CoworkerHub.Application.Interfaces;
using CoworkerHub.Application.Services;
using CoworkerHub.Domain.Entities;
using CoworkerHub.Domain.Enums;
using Moq;
using Xunit;

namespace CoworkerHub.Tests.Unit.Services
{
    public class DeskServiceTests
    {
        private readonly Mock<IDeskRepository> _mockDeskRepository;
        private readonly Mock<IWorkspaceRepository> _mockWorkspaceRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DeskService _deskService;

        public DeskServiceTests()
        {
            _mockDeskRepository = new Mock<IDeskRepository>();
            _mockWorkspaceRepository = new Mock<IWorkspaceRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();

            _deskService = new DeskService(
                _mockDeskRepository.Object,
                _mockWorkspaceRepository.Object,
                _mockUnitOfWork.Object,
                _mockMapper.Object
            );
        }

        #region CreateDeskAsync Tests

        [Fact]
        public async Task CreateDeskAsync_WithValidData_ReturnsDeskDTO()
        {
            // Arrange
            var createModel = new CreateDeskDTO
            {
                Name = "Desk A1",
                Floor = 1,
                Capacity = 1,
                PricePerHour = 15m,
                WorkspaceId = 1
            };

            var workspace = new Workspace { Id = 1, Name = "Workspace 1" };

            var desk = new Desk
            {
                Id = 1,
                Name = createModel.Name,
                Floor = createModel.Floor,
                Capacity = createModel.Capacity,
                PricePerHour = createModel.PricePerHour,
                WorkspaceId = createModel.WorkspaceId,
                Status = DeskStatus.Available
            };

            var deskDTO = new DeskDTO
            {
                Id = 1,
                Name = createModel.Name,
                Floor = createModel.Floor,
                Capacity = createModel.Capacity,
                PricePerHour = createModel.PricePerHour
            };

            _mockWorkspaceRepository.Setup(x => x.GetWorkspaceByIdAsync(1, CancellationToken.None))
                .ReturnsAsync(workspace);
            _mockMapper.Setup(x => x.Map<Desk>(createModel))
                .Returns(desk);
            _mockMapper.Setup(x => x.Map<DeskDTO>(desk))
                .Returns(deskDTO);

            // Act
            var result = await _deskService.CreateDeskAsync(createModel, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createModel.Name, result.Name);
            Assert.Equal(createModel.PricePerHour, result.PricePerHour);
            _mockDeskRepository.Verify(x => x.AddDesk(It.IsAny<Desk>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateDeskAsync_WithNonExistentWorkspace_ThrowsNotFoundException()
        {
            // Arrange
            var createModel = new CreateDeskDTO
            {
                Name = "Desk A1",
                Floor = 1,
                Capacity = 1,
                PricePerHour = 15m,
                WorkspaceId = 999
            };

            _mockWorkspaceRepository.Setup(x => x.GetWorkspaceByIdAsync(999, CancellationToken.None))
                .ReturnsAsync((Workspace)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _deskService.CreateDeskAsync(createModel, CancellationToken.None)
            );

            Assert.Contains("Workspace with ID 999 not found", exception.Message);
        }

        [Fact]
        public async Task CreateDeskAsync_SetsStatusToAvailable()
        {
            // Arrange
            var createModel = new CreateDeskDTO
            {
                Name = "Desk A1",
                Floor = 1,
                Capacity = 1,
                PricePerHour = 15m,
                WorkspaceId = 1
            };

            var workspace = new Workspace { Id = 1, Name = "Workspace 1" };

            var desk = new Desk
            {
                Id = 1,
                Name = createModel.Name,
                Status = DeskStatus.Available
            };

            var deskDTO = new DeskDTO { Id = 1, Name = createModel.Name };

            _mockWorkspaceRepository.Setup(x => x.GetWorkspaceByIdAsync(1, CancellationToken.None))
                .ReturnsAsync(workspace);
            _mockMapper.Setup(x => x.Map<Desk>(createModel))
                .Returns(desk);
            _mockMapper.Setup(x => x.Map<DeskDTO>(desk))
                .Returns(deskDTO);

            // Act
            await _deskService.CreateDeskAsync(createModel, CancellationToken.None);

            // Assert
            Assert.Equal(DeskStatus.Available, desk.Status);
        }

        #endregion

        #region GetDeskByIdAsync Tests

        [Fact]
        public async Task GetDeskByIdAsync_WithValidId_ReturnsDeskDTO()
        {
            // Arrange
            var desk = new Desk
            {
                Id = 1,
                Name = "Desk A1",
                Floor = 1,
                PricePerHour = 15m
            };

            var deskDTO = new DeskDTO
            {
                Id = 1,
                Name = "Desk A1",
                Floor = 1,
                PricePerHour = 15m
            };

            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(1, CancellationToken.None))
                .ReturnsAsync(desk);
            _mockMapper.Setup(x => x.Map<DeskDTO>(desk))
                .Returns(deskDTO);

            // Act
            var result = await _deskService.GetDeskByIdAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Desk A1", result.Name);
        }

        [Fact]
        public async Task GetDeskByIdAsync_WithNonExistentId_ThrowsNotFoundException()
        {
            // Arrange
            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(999, CancellationToken.None))
                .ReturnsAsync((Desk)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _deskService.GetDeskByIdAsync(999, CancellationToken.None)
            );

            Assert.Contains("Desk with ID 999 not found", exception.Message);
        }

        #endregion

        #region UpdateDeskAsync Tests

        [Fact]
        public async Task UpdateDeskAsync_WithValidData_UpdatesDesk()
        {
            // Arrange
            var updateModel = new UpdateDeskDTO
            {
                Name = "Updated Desk",
                PricePerHour = 20m
            };

            var desk = new Desk
            {
                Id = 1,
                Name = "Old Desk",
                PricePerHour = 15m
            };

            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(1, CancellationToken.None))
                .ReturnsAsync(desk);
            _mockMapper.Setup(x => x.Map(updateModel, desk))
                .Callback<UpdateDeskDTO, Desk>((src, dest) =>
                {
                    dest.Name = src.Name;
                    dest.PricePerHour = src.PricePerHour;
                });

            // Act
            await _deskService.UpdateDeskAsync(1, updateModel, CancellationToken.None);

            // Assert
            Assert.Equal("Updated Desk", desk.Name);
            Assert.Equal(20m, desk.PricePerHour);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateDeskAsync_WithNonExistentId_ThrowsNotFoundException()
        {
            // Arrange
            var updateModel = new UpdateDeskDTO { Name = "Updated Desk" };

            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(999, CancellationToken.None))
                .ReturnsAsync((Desk)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _deskService.UpdateDeskAsync(999, updateModel, CancellationToken.None)
            );

            Assert.Contains("Desk with ID 999 not found", exception.Message);
        }

        #endregion

        #region DeleteDeskAsync Tests

        [Fact]
        public async Task DeleteDeskAsync_WithValidId_DeletesDesk()
        {
            // Arrange
            var desk = new Desk { Id = 1, Name = "Desk A1" };

            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(1, CancellationToken.None))
                .ReturnsAsync(desk);

            // Act
            await _deskService.DeleteDeskAsync(1, CancellationToken.None);

            // Assert
            _mockDeskRepository.Verify(x => x.DeleteDesk(desk), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDeskAsync_WithNonExistentId_ThrowsNotFoundException()
        {
            // Arrange
            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(999, CancellationToken.None))
                .ReturnsAsync((Desk)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _deskService.DeleteDeskAsync(999, CancellationToken.None)
            );

            Assert.Contains("Desk with ID 999 not found", exception.Message);
        }

        #endregion

        #region ChangeDeskStatusAsync Tests

        [Fact]
        public async Task ChangeDeskStatusAsync_WithValidData_ChangesStatus()
        {
            // Arrange
            var desk = new Desk
            {
                Id = 1,
                Name = "Desk A1",
                Status = DeskStatus.Available
            };

            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(1, CancellationToken.None))
                .ReturnsAsync(desk);

            // Act
            await _deskService.ChangeDeskStatusAsync(1, DeskStatus.Maintenance, CancellationToken.None);

            // Assert
            Assert.Equal(DeskStatus.Maintenance, desk.Status);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ChangeDeskStatusAsync_WithNonExistentId_ThrowsNotFoundException()
        {
            // Arrange
            _mockDeskRepository.Setup(x => x.GetDeskByIdAsync(999, CancellationToken.None))
                .ReturnsAsync((Desk)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _deskService.ChangeDeskStatusAsync(999, DeskStatus.Maintenance, CancellationToken.None)
            );

            Assert.Contains("Desk with ID 999 not found", exception.Message);
        }

        #endregion
    }
}
