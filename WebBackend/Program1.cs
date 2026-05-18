using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebBackend.Dao;
using WebBackend.Service;
namespace WebBackend
{
     class Program1
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var totalTaskService = new TotalTasksService(configuration);
            var subTaskService = new SubTasksService(configuration);

            // 测试 TotalTaskService
            var newTotalTask = new TotalTask
            {
                ProcessCardId = 1,
                BatchNumber = "Batch001",
                WorkpieceCount = 100,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };

            await totalTaskService.AddTotalTaskAsync(newTotalTask);
            Console.WriteLine("TotalTask added.");

            var totalTasks = await totalTaskService.GetAllTotalTasksAsync();
            foreach (var task in totalTasks)
            {
                Console.WriteLine($"TotalTask ID: {task.Id}, BatchNumber: {task.BatchNumber}");
            }

            // 测试 SubTaskService
            var newSubTask = new SubTask
            {
                TotalTaskId = 1,
                ProcessCardId = 1,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };

            await subTaskService.AddSubTaskAsync(newSubTask);
            Console.WriteLine("SubTask added.");

            var subTasks = await subTaskService.GetAllSubTasksAsync();
            foreach (var task in subTasks)
            {
                Console.WriteLine($"SubTask ID: {task.Id}, ProcessCardId: {task.ProcessCardId}");
            }
        }
    }
}
