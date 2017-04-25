using System;
using System.Threading;
using System.Threading.Tasks;

namespace async_demo
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        public static async Task MainAsync(string[] args)
        {
            await Task.Delay(1000);
            Console.WriteLine("Hello World!");

            TaskQueue taskList = new TaskQueue();
            var task1=  taskList.Enqueue<string>(() => sayHelloAsync("say"));
            var task2 =  taskList.Enqueue<string>(() => sayHelloAsync("some"));
            var task3 =  taskList.Enqueue<string>(() => sayHelloAsync("thing"));
            var task4 =  taskList.Enqueue<string>(() => sayHelloAsync("end"));
            var result = await Task.WhenAll(task1,task2 ,task3,task4 );
            foreach (var item in result)
            {
                Console.WriteLine(string.Format("return : {0}", item));
            }
        }



        public static async Task<string> sayHelloAsync(string str)
        {
            await Task.Delay(0);
            Console.WriteLine(string.Format("Hello {0}", str));
            return str;
        }
    }
    public class TaskQueue
    {
        private SemaphoreSlim semaphore;
        public TaskQueue()
        {
            semaphore = new SemaphoreSlim(1);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            await semaphore.WaitAsync();
            try
            {
                return await taskGenerator();
            }
            finally
            {
                semaphore.Release();
            }
        }
        public async Task Enqueue(Func<Task> taskGenerator)
        {
            await semaphore.WaitAsync();
            try
            {
                await taskGenerator();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }



}
