using System;
using System.Linq;
using Domain.Data;
using Domain.Models;

namespace AutoRepairWorkshopConsole
{
    internal class Program
    {
        static AutoRepairWorkshopContext context = new AutoRepairWorkshopContext();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Меню:");
                Console.WriteLine("1. Выборка всех данных из таблицы (отношение 'один')");
                Console.WriteLine("2. Выборка данных из таблицы с фильтрацией (отношение 'один')");
                Console.WriteLine("3. Группировка данных с итогом (отношение 'многие')");
                Console.WriteLine("4. Выборка данных из двух таблиц ('один-ко-многим')");
                Console.WriteLine("5. Выборка данных из двух таблиц с фильтрацией ('один-ко-многим')");
                Console.WriteLine("6. Вставка данных в таблицу (отношение 'один')");
                Console.WriteLine("7. Вставка данных в таблицу (отношение 'многие')");
                Console.WriteLine("8. Удаление данных из таблицы (отношение 'один')");
                Console.WriteLine("9. Удаление данных из таблицы (отношение 'многие')");
                Console.WriteLine("10. Обновление данных с условием");
                Console.WriteLine("0. Выход");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        SelectAllFromOwner();
                        break;
                    case "2":
                        SelectOwnerWithFilter();
                        break;
                    case "3":
                        GroupByService();
                        break;
                    case "4":
                        SelectCarWithRepairOrder();
                        break;
                    case "5":
                        SelectFilteredCarWithRepairOrder();
                        break;
                    case "6":
                        InsertOwner();
                        break;
                    case "7":
                        InsertRepairOrder();
                        break;
                    case "8":
                        DeleteOwner();
                        break;
                    case "9":
                        DeleteRepairOrder();
                        break;
                    case "10":
                        UpdateCar();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор, попробуйте снова.");
                        break;
                }
            }
        }

        // 3.2.1 Выборка всех данных из таблицы Owner (отношение "один")
        static void SelectAllFromOwner()
        {
            var owners = context.Owners.ToList();
            foreach (var owner in owners)
            {
                Console.WriteLine(owner.FullName);
            }
        }

        // 3.2.2 Выборка данных из таблицы Owner с фильтрацией
        static void SelectOwnerWithFilter()
        {
            Console.WriteLine("Введите имя владельца для фильтрации:");
            string name = Console.ReadLine();

            var owners = context.Owners
                .Where(o => o.FullName.Contains(name))
                .ToList();

            foreach (var owner in owners)
            {
                Console.WriteLine(owner.FullName);
            }
        }

        // 3.2.3 Группировка по Service с подсчетом количества сервисов
        static void GroupByService()
        {
            var serviceGroup = context.Services
                .GroupBy(s => (int)s.Price/100)
                .Select(g => new { ServiceName = g.Key, Count = g.Count() })
                .ToList();

            foreach (var group in serviceGroup)
            {
                Console.WriteLine($"{group.ServiceName}: {group.Count}");
            }
        }

        // 3.2.4 Выборка данных из таблиц Car и RepairOrder (связь один-ко-многим)
        static void SelectCarWithRepairOrder()
        {
            var query = context.Cars
                .Select(c => new { c.LicensePlate, Orders = c.RepairOrders.Select(ro => ro.OrderId) })
                .ToList();

            foreach (var car in query)
            {
                Console.WriteLine($"{car.LicensePlate}: {string.Join(", ", car.Orders)}");
            }
        }

        // 3.2.5 Выборка данных из Car и RepairOrder с фильтрацией по году выпуска
        static void SelectFilteredCarWithRepairOrder()
        {
            Console.WriteLine("Введите год выпуска для фильтрации:");
            int year = int.Parse(Console.ReadLine());

            var query = context.Cars
                .Where(c => c.YearOfProduction == year)
                .Select(c => new { c.LicensePlate, Orders = c.RepairOrders.Select(ro => ro.OrderId) })
                .ToList();

            foreach (var car in query)
            {
                Console.WriteLine($"{car.LicensePlate}: {string.Join(", ", car.Orders)}");
            }
        }

        // 3.2.6 Вставка данных в таблицу Owner (отношение "один")
        static void InsertOwner()
        {
            Console.WriteLine("Введите данные для нового владельца:");
            Console.Write("Номер водительского удостоверения: ");
            string license = Console.ReadLine();
            Console.Write("ФИО: ");
            string fullName = Console.ReadLine();

            Owner newOwner = new Owner { DriverLicenseNumber = license, FullName = fullName };
            context.Owners.Add(newOwner);
            context.SaveChanges();
            Console.WriteLine("Владелец добавлен.");
        }

        // 3.2.7 Вставка данных в таблицу RepairOrder (отношение "многие")
        static void InsertRepairOrder()
        {
            Console.WriteLine("Введите данные для нового заказа на ремонт:");
            Console.Write("ID автомобиля: ");
            int carId = int.Parse(Console.ReadLine());
            Console.Write("ID механика: ");
            int mechanicId = int.Parse(Console.ReadLine());

            RepairOrder newOrder = new RepairOrder { CarId = carId, MechanicId = mechanicId, OrderDate = DateOnly.FromDateTime(DateTime.Now) };
            context.RepairOrders.Add(newOrder);
            context.SaveChanges();
            Console.WriteLine("Заказ на ремонт добавлен.");
        }

        // 3.2.8 Удаление данных из таблицы Owner (отношение "один")
        static void DeleteOwner()
        {
            Console.Write("Введите ID владельца для удаления: ");
            int ownerId = int.Parse(Console.ReadLine());

            var owner = context.Owners.Find(ownerId);
            if (owner != null)
            {
                context.Owners.Remove(owner);
                context.SaveChanges();
                Console.WriteLine("Владелец удален.");
            }
            else
            {
                Console.WriteLine("Владелец не найден.");
            }
        }

        // 3.2.9 Удаление данных из таблицы RepairOrder (отношение "многие")
        static void DeleteRepairOrder()
        {
            Console.Write("Введите ID заказа на ремонт для удаления: ");
            int orderId = int.Parse(Console.ReadLine());

            var order = context.RepairOrders.Find(orderId);
            if (order != null)
            {
                context.RepairOrders.Remove(order);
                context.SaveChanges();
                Console.WriteLine("Заказ на ремонт удален.");
            }
            else
            {
                Console.WriteLine("Заказ не найден.");
            }
        }

        // 3.2.10 Обновление данных в таблице Car
        static void UpdateCar()
        {
            Console.Write("Введите ID автомобиля для обновления: ");
            int carId = int.Parse(Console.ReadLine());

            var car = context.Cars.Find(carId);
            if (car != null)
            {
                Console.Write("Введите новый цвет автомобиля: ");
                string newColor = Console.ReadLine();

                car.Color = newColor;
                context.SaveChanges();
                Console.WriteLine("Цвет автомобиля обновлен.");
            }
            else
            {
                Console.WriteLine("Автомобиль не найден.");
            }
        }
    }
}
