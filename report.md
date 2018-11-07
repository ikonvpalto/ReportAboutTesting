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

Ура, автотесты, практика. Не тут то было. В начале опять теория. Поговорим про

### Тестовые заглушки (*Test doubles*)

Почему? Потому, что они везде. Модульные тесты так вообще без заглушек никуда. 

Сразу хочу сказать, что все термины ниже имеют разные определения в разных источниках (сколько источников, столько и оределений, ужас). Я брал большую часть у [Мартина Фаулера][TestDoubleFowler] и одно из [другого места][SpyDefinition] (мне оно показалось логичнее; может я не до конца понял Фаулера). Так что при использовании какой-то либы для тестирования обязательно посмотрите, какие определения у них.

Тестовые заглушки (*Test doubles*) - Это общий термин для группы объектов, которые служат для замены production-сущностей при тестировании. Самый простой пример - использование локальной базы данных. Всего их 5: **Dummy**, **Fake**, **Stub**, **Mock**, **Spy**.

  * **Dummy** - объект-пустышка, единственной целью которого является заолнения списка параметров. 

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

  * **Fake** - тестовая заглушка, которая, в отличае от **Dummy**, имеет полноценную реализацию, но, обычно, очень упрощенную в угоду скорости написания, почему не подходит для исользования на проде. Пример выше с локальной базой данных - это оно.

  * **Stub** и **Mock** - очень похожие понятия. Во-первых, оба являются тестовыми заглушками, которые служат окружением (*environment*) для цели теста. Во-вторых, если **Dummy** и **Fake** мы поматросили и бросили, то **Stub** и **Mock** также используются для проверок, пройден тест или нет (корректно работает цель или нет). Это что общего, тепер различия, которые сейчас покажу на примере. Представим следующую задачу: Есть склад с продуктами и заказы на продукты. Если мы заказываем продукт, который есть на складе в нужном нам количестве, то заказ проходит успешно, а количество товаров на складе уменьшается. Если товара нет или мы запрашиваем больше, чем есть, то заказ не проходит. Давайте посмотрим на тесты для этой задачи:

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

  Общая схема абсолютно такая же, но детали отличаются:

    1. В начале вместо содания и заполнения объекта склада, создаеться mock-объект и определяется его поведение. 
    2. В конце проверяется не состояние склада, а действия, которые были над ним выполнены.
    3. Мы для проверки склада не сделали ни одного assert-а. Вместо этого вызываtтся метод `Verify`, в которой передается не ожидаемые данные, а ожидаемое поведение. 

  Как нетрудно догадаться, во втором примере мы использовали **Mock** в качестве тестового склада , а в первом методом исключения - **Stub**. Исходя из вышенаписанного можно выделить два ключевых различия между **Mock** и **Stub**:

    1. **Mock** - это про проверку поведения (*behavior verification*), а **Stub** - про проверку состояния (*state verification*).
    2. **Mock** сам производит assert-ы и может выкидывать исключения, в то время как состояние **Stub** проверяет сам программист.

  * **Spy** - это объект-обертка, в который оборачиваются production-сущности, и который записывает 

Кстати, в тестах выше я использовал 2 библиотеки: xUnit.net и Moq. Давайте рассмотрим, для начала xUnit.net поближе

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

Максимально простой тест. Создаём тестовый класс, который не надо помечать аттрибутами и который не наследуется от каких-то библиотечны классов. 

### Moq

Ососбенности:

  * Мокируемый класс\интерфейс\... должен быть публичным
  * Член, для которого определяется поведение, должен быть `virtual` 

### Тестирование асинхронных сценариев

Тут про тестирование последовательности действий. Раскажу про ошибку с сохранением данных в парсере. Можно даже тот код показать. И код теста, в котором с помощью мока или стаба проверяется поледовательность вызовов.

конец Тестирование асинхронных сценариев

В течении главы про автотесты побольше кода. И на разных фремворках. Можно даже попробовать каждый пример написать на всех вариантах. Потом в конце сделать сравнение:
  * xUnit.net
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
[TestDoubleFowler]: https://martinfowler.com/bliki/TestDouble.html
[SpyDefinition]: https://javapointers.com/tutorial/difference-between-spy-and-mock-in-mockito/

Вытянуть руку с микрофоном, уронить его, и уйти в закат.