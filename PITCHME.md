# Тестирование

---

## Тестовые заглушки (_Test doubles_)

- **Dummy**
- **Fake**
- **Stub**
- **Mock**
- **Spy**

---

### **Dummy**

```C#
internal class DummyLogger : ILogger
{
    public void Log(string message) {}
}

[Fact]
public void TestWithDummy()
{
    ILogger dummy = new DummyLogger();
    Calculator calculator = new Calculator(dummy);

    int result = calculator.Sum(1, 5, -1);

    Assert.Equal(5, result);
}
```

---

### **Fake**

---

### Схожесть **Stub** и **Mock**

1. Environment
2. Проверки (_assertions_)

---

### **Stub** и **Mock**

```C#
[Fact]
public void TestOrderIsFilledIfEnoughInWarehouse()
{
    Warehouse warehouse = new Warehouse { { "1", 5 }, { "2", 6 } };
    Order order = new Order
    {
        Amount = 5,
        Product = "1"
    };

    order.Fill(warehouse);

    Assert.True(order.IsFilled);
    Assert.Equal(0, warehouse["1"]);
}

[Fact]
public void TestOrderDoesNotRemoveIfNotEnough()
{
    Warehouse warehouse = new Warehouse { { "1", 5 }, { "2", 6 } };
    Order order = new Order
    {
        Amount = 6,
        Product = "1"
    };

    order.Fill(warehouse);

    Assert.False(order.IsFilled);
    Assert.Equal(5, warehouse["1"]);
}
```

---

### **Stub** и **Mock**

- Проверка состояния
- `Assert`-ы

---

### **Stub** и **Mock**

```C#
[Fact]
public void TestOrderIsFilledIfEnoughInWarehouse()
{
    Mock<IWarehouse> warehouseMock = new Mock<IWarehouse>();
    warehouseMock
        .Setup(warehouse => warehouse.IsHave("1", 5))
        .Returns(true);
    Order order = new Order
    {
        Amount = 5,
        Product = "1"
    };

    order.Fill(warehouseMock.Object);

    Assert.True(order.IsFilled);
    warehouseMock.Verify(warehouse => warehouse.IsHave("1", 5), Times.Once);
    warehouseMock.Verify(warehouse => warehouse.IsHave(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
    warehouseMock.Verify(warehouse => warehouse.Take("1", 5), Times.Once);
    warehouseMock.Verify(warehouse => warehouse.Take(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
}

[Fact]
public void TestOrderDoesNotRemoveIfNotEnough()
{
    Mock<IWarehouse> warehouseMock = new Mock<IWarehouse>();
    warehouseMock
        .Setup(warehouse => warehouse.IsHave("1", 6))
        .Returns(false);
    Order order = new Order
    {
        Amount = 6,
        Product = "1"
    };

    order.Fill(warehouseMock.Object);

    Assert.False(order.IsFilled);
    warehouseMock.Verify(warehouse => warehouse.IsHave("1", 6), Times.Once);
    warehouseMock.Verify(warehouse => warehouse.IsHave(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
    warehouseMock.Verify(warehouse => warehouse.Take(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
}
```

---

### **Stub** и **Mock**

- Определение поведения **Mock**-объекта
- Проверка поведения
- Нет `Assert`-ов

---

### Различия **Stub** и **Mock**

- **Mock** - это про проверку поведения (_behavior verification_), а **Stub** - про проверку состояния (_state verification_).
- **Mock** сам производит assert-ы и может выкидывать исключения, в то время как состояние **Stub** проверяет сам программист.

---

### **Spy**

---

### xUnit.net

```C#
using Xunit;

namespace XUnitTestProject
{
    public class XunitTests
    {
        [Fact]
        public void TestSimple()
        {
            int num = 1 + 2;

            Assert.Equal(3, num);
        }
    }
}
```

---

### xUnit.net. `Assert`-ы

```
Assert.True() Failure
Expected: True
Actual:   False
```

---

### xUnit.net. Аргументы `Assert`-ов

- Порядок
- Строковое сообщение

---

### xUnit.net. `Theory`

- Новый тест для каждого набора
  - - Долго
  - - Неподдерживаемо
- Цикл
  - - Отслеживание упавщего набора
  - - Сколько прошло?
- `Theory`

---

### xUnit.net. `Theory`

```C#
[Theory]
[InlineData(1, 2, 3)]
[InlineData(0, 0, 0)]
[InlineData(0, int.MaxValue, int.MaxValue)]
[InlineData(int.MinValue, int.MaxValue, -1)]
[InlineData(int.MaxValue, int.MaxValue, -2)]
[InlineData(int.MinValue, int.MinValue, 0)]
public void TestTheorySimple(int a, int b, int expected)
{
    int num = a + b;

    Assert.Equal(expected, num);
}
```

---

### xUnit.net. `MemberData`

```C#
public static List<object[]> testParams = new List<object[]>
{
    new object[] {1, 2, 3},
    new object[] {0, 0, 0},
    new object[] {0, int.MaxValue, int.MaxValue},
    new object[] {int.MinValue, int.MaxValue, -1},
    new object[] {int.MaxValue, int.MaxValue, -2},
    new object[] {int.MinValue, int.MinValue, 0},
};

[Theory]
[MemberData(nameof(testParams))]
public void TestTheoryMember(int a, int b, int expected)
{
    int num = a + b;

    Assert.Equal(expected, num);
}
```

---

### xUnit.net. `MemberData`

```C#
public static IEnumerable<object[]> generateTestParams()
{
    yield return new object[] { 1, 2, 3 };
    yield return new object[] { 0, 0, 0 };
    yield return new object[] { 0, int.MaxValue, int.MaxValue };
    yield return new object[] { int.MinValue, int.MaxValue, -1 };
    yield return new object[] { int.MaxValue, int.MaxValue, -2 };
    yield return new object[] { int.MinValue, int.MinValue, 0 };
}

[Theory]
[MemberData(nameof(generateTestParams))]
public void TestTheoryMemberGenerator(int a, int b, int expected)
{
    int num = a + b;

    Assert.Equal(expected, num);
}
```

---

### xUnit.net. `ClassData`

```C#
public class TestsParamsGenerator : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1, 2, 3 };
        yield return new object[] { 0, 0, 0 };
        yield return new object[] { 0, int.MaxValue, int.MaxValue };
        yield return new object[] { int.MinValue, int.MaxValue, -1 };
        yield return new object[] { int.MaxValue, int.MaxValue, -2 };
        yield return new object[] { int.MinValue, int.MinValue, 0 };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[Theory]
[ClassData(typeof(TestsParamsGenerator))]
public void TestTheoryClass(int a, int b, int expected)
{
    int num = a + b;

    Assert.Equal(expected, num);
}
```

---

### xUnit.net. `Assert.Throws()`

- `ExpectedException`
  - - Трекинг, где упало
  - - Тест прекращается
- `Assert.Throws()`

---

### xUnit.net. `Assert.Throws()`

```C#
namespace XUnitTestProject
{
    [Fact]
    public void TestException()
    {
        string a = null;

        NullReferenceException throwedException =
            Assert.Throws<NullReferenceException>(
                () => a.Insert(1, "asd")
            );

        Assert.Equal(nameof(XUnitTestProject), throwedException.Source);
    }
}
```

---
