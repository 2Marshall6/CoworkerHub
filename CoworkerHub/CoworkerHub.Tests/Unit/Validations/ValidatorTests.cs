using CoworkerHub.Application.DTOs.Workspace;
using CoworkerHub.Application.Validations;
using FluentValidation;
using Xunit;

namespace CoworkerHub.Tests.Unit.Validations
{
    public class CreateWorkspaceValidatorTests
    {
        private readonly CreateWorkspaceValidator _validator;

        public CreateWorkspaceValidatorTests()
        {
            _validator = new CreateWorkspaceValidator();
        }

        [Fact]
        public async Task Validate_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var model = new CreateWorkspaceDTO
            {
                Name = "Open Office",
                Description = "A spacious open office workspace"
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task Validate_WithEmptyName_ReturnsFails()
        {
            // Arrange
            var model = new CreateWorkspaceDTO
            {
                Name = "",
                Description = "Valid description"
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateWorkspaceDTO.Name));
        }

        [Fact]
        public async Task Validate_WithNullName_ReturnsFails()
        {
            // Arrange
            var model = new CreateWorkspaceDTO
            {
                Name = null,
                Description = "Valid description"
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateWorkspaceDTO.Name));
        }

        [Fact]
        public async Task Validate_WithShortDescription_ReturnsFails()
        {
            // Arrange
            var model = new CreateWorkspaceDTO
            {
                Name = "Valid Name",
                Description = "ab"  // Less than 3 characters
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == nameof(CreateWorkspaceDTO.Description));
        }

        [Fact]
        public async Task Validate_WithMinimumValidDescription_ReturnsSuccess()
        {
            // Arrange
            var model = new CreateWorkspaceDTO
            {
                Name = "Valid Name",
                Description = "abc"  // Exactly 3 characters
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.True(result.IsValid);
        }
    }

    public class GetWorkspacesListValidatorTests
    {
        private readonly GetWorkspacesListValidator _validator;

        public GetWorkspacesListValidatorTests()
        {
            _validator = new GetWorkspacesListValidator();
        }

        [Fact]
        public async Task Validate_WithValidPagination_ReturnsSuccess()
        {
            // Arrange
            var model = new GetWorkspacesListDTO
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Validate_WithZeroPageNumber_ReturnsFails()
        {
            // Arrange
            var model = new GetWorkspacesListDTO
            {
                PageNumber = 0,
                PageSize = 10
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == nameof(GetWorkspacesListDTO.PageNumber));
        }

        [Fact]
        public async Task Validate_WithNegativePageNumber_ReturnsFails()
        {
            // Arrange
            var model = new GetWorkspacesListDTO
            {
                PageNumber = -1,
                PageSize = 10
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Validate_WithZeroPageSize_ReturnsFails()
        {
            // Arrange
            var model = new GetWorkspacesListDTO
            {
                PageNumber = 1,
                PageSize = 0
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == nameof(GetWorkspacesListDTO.PageSize));
        }

        [Fact]
        public async Task Validate_WithPageSizeGreaterThan1000_ReturnsFails()
        {
            // Arrange
            var model = new GetWorkspacesListDTO
            {
                PageNumber = 1,
                PageSize = 1001
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == nameof(GetWorkspacesListDTO.PageSize));
        }

        [Fact]
        public async Task Validate_WithMaxPageSize_ReturnsSuccess()
        {
            // Arrange
            var model = new GetWorkspacesListDTO
            {
                PageNumber = 1,
                PageSize = 999  // Just under 1000
            };

            // Act
            var result = await _validator.ValidateAsync(model);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
