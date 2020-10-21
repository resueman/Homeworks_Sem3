using System;

namespace MyThreadPool
{
    /// <summary>
    /// Interface for asynchronous operations that will be evaluated by the thread pool 
    /// </summary>
    /// <typeparam name="TResult">Type of asynchronous operation result</typeparam>
    public interface IMyTask<TResult>
    {
        /// <summary>
        /// Determines whether the task is completed
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Returns the result of task evaluation if it is ready, 
        /// otherwise blocks the calling thread until the end of evaluation
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Creates new task based on result of previous task
        /// </summary>
        /// <typeparam name="TNewResult">Type of new task result</typeparam>
        /// <param name="continuation">Function which will be applied to the result of a given task</param>
        /// <returns>New task for execution</returns>
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
    }
}
