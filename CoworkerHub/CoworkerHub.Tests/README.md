# CoworkerHub.Tests

Unit тесты для CoworkerHub проекта.

## Запуск тестов

### Всех тестов
```bash
dotnet test
```

### Конкретного тестового класса
```bash
dotnet test --filter "ClassName=BookingServiceTests"
```

### С логированием
```bash
dotnet test --logger "console;verbosity=detailed"
```

### С покрытием кода
```bash
dotnet test /p:CollectCoverageMetrics=True
```

## Структура тестов

```
CoworkerHub.Tests/
├── Unit/
│   ├── Services/
│   │   ├── BookingServiceTests.cs    (7 тестов)
│   │   ├── DeskServiceTests.cs       (9 тестов)
│   │   ├── UserServiceTests.cs       (13 тестов)
│   │   └── TokenServiceTests.cs      (10 тестов)
│   └── Validations/
│       └── ValidatorTests.cs         (11 тестов)
└── README.md
```

## Покрытие

- **BookingService**: 7 тестов
  - CreateBookingAsync (5 сценариев)
  - GetMyBookingsAsync (2 сценария)

- **DeskService**: 9 тестов
  - CreateDeskAsync (3 сценария)
  - GetDeskByIdAsync (2 сценария)
  - UpdateDeskAsync (2 сценария)
  - DeleteDeskAsync (1 сценарий)
  - ChangeDeskStatusAsync (1 сценарий)

- **UserService**: 13 тестов
  - RegisterUserAsync (2 сценария)
  - LoginUserAsync (3 сценария)
  - RefreshTokensAsync (3 сценария)
  - ChangePasswordAsync (2 сценария)
  - AssignRoleAsync (3 сценария)

- **TokenService**: 10 тестов
  - GenerateJwt (4 сценария)
  - GenerateRefreshToken (2 сценария)
  - ValidateRefreshToken (4 сценария)

- **Validators**: 11 тестов
  - CreateWorkspaceValidator (5 тестов)
  - GetWorkspacesListValidator (6 тестов)

**Всього: 50 unit тестов**

## Используемые инструменты

- **xUnit** - фреймворк для тестирования
- **Moq** - библиотека для мокирования зависимостей
- **AutoMapper** - для маппирования объектов
- **FluentValidation** - для валидации

## Примеры запуска

### Запустить все тесты BookingService
```bash
dotnet test --filter "ClassName=BookingServiceTests"
```

### Запустить конкретный тест
```bash
dotnet test --filter "DisplayName~CreateBookingAsync_WithValidData_ReturnsBookingDTO"
```

### Запустить с выводом в консоль
```bash
dotnet test -v n
```

## Best Practices

Все тесты следуют **AAA Pattern** (Arrange-Act-Assert):

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange - подготовка данных и моков
    var input = new CreateBookingDTO { ... };
    _mockRepository.Setup(x => x.GetAsync(It.IsAny<int>()))
        .ReturnsAsync(expectedValue);
    
    // Act - выполнение тестируемого метода
    var result = await _service.CreateAsync(input);
    
    // Assert - проверка результата
    Assert.NotNull(result);
    Assert.Equal(expectedValue.Id, result.Id);
}
```

## Рекомендации

- Каждый тест должен быть **независимым**
- Один тест - одна проверка
- Использовать **факт** для детерминированных тестов
- Использовать **тео́рию** (Theory) для параметризованных тестов
