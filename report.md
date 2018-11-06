# Тестирование

Я хочу немного поговорить о тестировании, зачем оно надо и как упростить себе жизнь.

## Введение

Небольшой исторический экскурс, который плавно перетекает в

### А зачем оно надо

Которое совмещенно с 

### Виды и классификации тестирования

Говорю задачу и сразу о виде тестирования, который этим занимается

  * Модульное
  * Интеграционное
  * Тестирование системы
  * ...
  
Все, что до этого момента не долго, не более 5 минут.

Это всё, конечно, занимательно, но повляется волне закономерный вопрос: <быдлоГолос>И шо?</быдлоГолос> Так вот, переходим ближе к практике

## Подходы

Сравнение, плюсы, минусы:
  * Ручное
  * Обычные автотесты
  * behavior-автотесты
  * отсутствие оных, гыгы
  
Обязательна статистика затраченного времени при разном уровне тестирования (показать, что без тестов айайай).

## Автотесты (80% времени)

Тут можно чуть кодяру пописать. Посмотреть, что сейчас популярно, что развивается, рассказать-показать.

Надо как-то упомянуть модульное тестирование и перейти на

### Тестовые заглушки (*Doubles*)

Это какие-то специальные объекты, которые создаються для каждого отдельного теста (или группы). Есть несколько разных видов:

  * **Dummy** - объект-пустышка, который никак не относиться к тесту, но нужен для заполнения списка параметров.

    ```C#
    internal class DummyLogger : ILogger
    {
        public void Log(string message) { }
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

  * **Fake** - объект, который, в отличае от **Dummy**, имеет полноценную реализацию, но, обычно, она делается с упором в скорость написания и не подходит для исользования в продакшене. Например, чтобы не развертывать отдельную базу данных для тестов и не очищать её потом, можно использовать in-memory базу данных.

  * **Stub** и **Mock** - очень похожи и их легко спутать. Оба служат тестовым окружением для цели теста. Оба, в отличае от **Dummy** и **Fake** для проверки, коррекетно ли работает цель теста. Но между ними есть 2 принципиальных различия, которые сейчас покажу на примере. Представим следующую задачу: Есть склад с продуктами и заказы на продукты. Если мы заказываем продукт, который есть на складе в нужном нам количестве, то заказ проходит успешно, а количество товаров на складе уменьшается. Если товара нет или мы запрашиваем больше, чем есть, то заказ не проходит. Давайте посмотрим на тесты для этой задачи.

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

  Тесты составлены по паттерну AAA (Arrange, Act, Assert): в начале создаем склад с товарами и заказ, после выполняем тест, а в конце проверяем результат. Обращаю внимание на то, что в конце теста проверяется состояние в котором оказались тестовые объекты. 
  Теперь перепишем тест немного иначе:

    ```C#
    [Fact]
    public void TestOrderIsFilledIfEnoughInWarehouse()
    {
        Mock<Warehouse> warehouseMock = new Mock<Warehouse>();
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
        Mock<Warehouse> warehouseMock = new Mock<Warehouse>();
        warehouseMock
            .Setup(warehouse => warehouse.IsHave("1", 6))
            .Returns(true);
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

  Общая схема абсолютно такая же, но детали отличаются:
    1. В начале вместо содания и заполнения объекта склада, создаеться mock-объект и определяется его поведение. 
    2. В конце проверяется не состояние склада, а действия, которые были над ним выполнены.
    3. Мы для проверки склада не сделали ни одного assert-а. Вместо этого вызываtтся метод `Verify`, в которой передается не ожидаемые данные, а ожидаемое поведение. 

  Как нетрудно догадаться, во втором примере мы использовали **Mock** в качестве тестового склада , а в первом методом исключения - **Stub**. Исходя из вышенаписанного можно выделить два ключевых различия между **Mock** и **Stub**:
  
    1. **Mock** - это про проверку поведения (*behavior verification*), а **Stub** - про проверку состояния (*state verification*).
    2. **Mock** сам производит assert-ы и может выкидывать исключения, в то время как **Stub** проверяет программист.

  * **Spy** - это объект-обертка над обычным

Внимание на обЬяснение, в чем отличие между ними, на SO есть несколько вопросов по этому. Martin Fowler.

Заглушки закончились.

### Тестирование асинхронных сценариев

Тут про тестирование последовательности действий. Раскажу про ошибку с сохранением данных в парсере. Можно даже тот код показать. И код теста, в котором с помощью мока или стаба проверяется поледовательность вызовов.

конец Тестирование асинхронных сценариев

В течении главы про автотесты побольше кода. И на разных фремворках. Можно даже попробовать каждый пример написать на всех вариантах. Потом в конце сделать сравнение:
  * xUnit.NET
  * NUnit
  * MSTest вряд ли, может упомяну

Behavior фреймворки:
  * NSpec
  * MSpec
  * SpecFlow
  * BDDfy
  * ApprovalTests
  * SpecsFor
  * LightBDD
  
Еще сказать (и сравнить) о mock фреймворках:
  * Moq
  * Mock в NUnit
  * FakeItEasy
  * NSubstitute 

[Respawn][RespawnGithub]


!Attention. MyTested.AspNetCore.Mvc. Бомба. Про неё рассказать, показать. Обязательно и много.

Рассказать, возможно, про NHamcrest, зачем, почему. FluentAssertions.

Прошерстить github, попробовать найти прикольные штуки, которые только развиваются.

Можно еще упомянть (но не более) про CI/CD.

А когда писать тесты? Надо придумать переход плавный или переместить следующий раздел в другое место.

## Тесты до кода или нет. TDD и BDD

Сравнение 
  * До написания кода
  * После
  
Какие-нибудь исследования найти.
  
## Тестирование API

## Список литературы

  * https://habr.com/post/275013/
  * http://docs.mytestedasp.net/tutorial/gettingstarted.html
  * https://github.com/ivaylokenov/MyTested.AspNetCore.Mvc

[RespawnGithub]: https://github.com/jbogard/Respawn

Вытянуть руку с микрофоном, уронить его, и уйти в закат.