namespace RivieraStudio.Magic.Services.Core.Libs.Colorful
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    #if !NET40
    public class TaskQueue
    {
        private readonly SemaphoreSlim _semaphore;

        public TaskQueue()
        {
            this._semaphore = new SemaphoreSlim(1);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            await this._semaphore.WaitAsync();
            try
            {
                return await taskGenerator();
            }
            finally
            {
                this._semaphore.Release();
            }
        }

        public async Task Enqueue(Func<Task> taskGenerator)
        {
            await this._semaphore.WaitAsync();
            try
            {
                await taskGenerator();
            }
            finally
            {
                this._semaphore.Release();
            }
        }
    }
    #endif
}