Цей проєкт являє собою API для управління конференц-залами та їх бронювання. Включає функціонал додавання залів, редагування їхніх даних, видалення, пошуку доступних залів і здійснення бронювання.
Основні можливості
Додавання конференц-залів: Введення даних про зал (назва, місткість, послуги, ціна за годину).
Редагування інформації про зал: Зміна параметрів залу.
Видалення конференц-залу: Видалення залу за унікальним ID.
Пошук доступних залів: Перевірка доступності залів на певну дату і час.
Бронювання залу: Бронювання залу із зазначенням послуг, розрахунок загальної вартості оренди.

Технологии
ASP.NET Core
Entity Framework Core (PostgreSQL)
Automapper

Налаштування та запуск проєкту

Вимоги:
.NET 6 SDK
PostgreSQL

Кроки для запуску: 
1.Клонуйте репозиторій: git clone https://github.com/your-repo-url.git
2. Налаштуйте підключення до бази даних у файлі appsettings.json.
3. Застосуйте міграції бази даних: dotnet ef database update
4.Запустіть додаток: dotnet run
API буде доступний за адресою http://localhost:5134

API Методи:
1. Додавання конференц-залу:
URL: /api/halls/add
Метод: POST
Опис: Створює новий зал із зазначеними параметрами (назва, місткість, послуги, базова ціна оренди за годину).
 - Вхідні дані: 
 {
    "name": "Зал А",
    "capacity": 50,
    "hourlyRate": 2000,
    "serviceIds": [1, 2]
}
 - Вихідні дані:
   {
    "$id": "1",
    "id": 7,
    "message": "Зал успiшно створений"
}
   
2.Редагування інформації про зал
URL: /api/halls/{id}
Метод: PUT
Опис: Оновлює інформацію про зал за його унікальним ID.
 - Вхідні дані:
   {
    "name": "Зал 52",
    "capacity": 150,
    "hourlyRate": 2300,
    "serviceIds": [1, 2]
  }
 
 -  Вихідні дані: 
   {
    "$id": "1",
    "message": "Зал успiшно оновлений"
  }

3. Видалення конференц-залу:
URL: /api/halls/{id}
Метод: DELETE
Опис: Видаляє конференц-зал за його унікальним ID.   
 - Вихідні дані: 
   {
    "$id": "1",
    "message": "Зал успiшно видалений"
  }
   
4. Пошук доступних залів:
URL: /api/booking/search-available-halls
Метод: POST
Опис: Дозволяє знайти вільні зали на зазначену дату і час.
 - Вхідні дані:
{
  "date": "2024-09-21",
  "startTime": "09:00:00",
  "endTime": "10:00:00",
  "requiredCapacity": 50
}
   
 - Вихідні дані:
  {
    "$id": "1",
    "$values": [
        {
            "$id": "2",
            "id": 6,
            "name": "Зал M",
            "capacity": 50,
            "basePricePerHour": 2000
        },
        {
            "$id": "3",
            "id": 3,
            "name": "Зал 52",
            "capacity": 50,
            "basePricePerHour": 3000
        },
        {
            "$id": "4",
            "id": 5,
            "name": "Зал 22",
            "capacity": 500,
            "basePricePerHour": 3000
        }
    ]
}
   
5. Бронювання залу:
URL: /api/booking/create
Метод: POST
Опис: Дозволяє забронювати зал із зазначеними параметрами (дата, час, тривалість, послуги).   
 - Вхідні дані:
   {
    "hallId": 1,
    "date": "2024-09-21",
    "startTime": "09:00:00",
    "duration": 3,
    "serviceIds": [1, 2]
}
 - Вихідні дані:
   {
    "$id": "1",
    "bookingId": 12,
    "totalPrice": 6200.0
}
   
