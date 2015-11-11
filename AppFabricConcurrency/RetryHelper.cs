using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationServer.Caching;

namespace CacheAPISample
{
    /// <summary>
    /// Helper class to retry the functionality
    /// </summary>
    public static class RetryHelper
    {
        /// <summary>
        /// Executes the action with the retry logic
        /// </summary>
        /// <param name="retryAction">Action to be executed</param>
        /// <param name="canContinue">Checks if the exception can informational log or exception handling log </param>
        /// <param name="buildLogEntry">Function delegate to return the log entry</param>
        /// <param name="maxRetryCount">Maximum number of the retry</param>
        /// <param name="canThrowException">True or false whether it can throw exception or not.</param>
        /// <param name="delay">Delay in retry</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "It is required to catch the generic exception here")]
        public static void Execute(Action retryAction, Predicate<Exception> canContinue, int maxRetryCount = 3, bool canThrowException = true, int delay = 200)
        {
            int retryCount = 0;

            //Iteration for the retry starts
            while (retryCount < maxRetryCount)
            {
                try
                {
                    // Invokes the delegate action
                    retryAction();
                    return;
                }

                catch (Exception ex)
                {
                    retryCount += 1;

                    var canRetryContinue = canContinue(ex);

                    //Logs the exception 
                    //LogExceptions(canRetryContinue, ex, retryCount, buildLogEntry, maxRetryCount, canThrowException);

                    //Get out of the loop if cannot be continued.
                    if (!canRetryContinue)
                    {
                        return;
                    }
                }

                //Adding the delay to the retry.
                System.Threading.Tasks.Task.Delay(delay);
            }
        }

        /// <summary>
        /// Executes the retry function
        /// </summary>
        /// <typeparam name="TResult">Type that is returned</typeparam>
        /// <param name="retryFunction">Retry function</param>
        /// <param name="canContinue">Condition to check whether the exception are logged as is or informational</param>
        /// <param name="buildLogEntry">Builds the log entry</param>
        /// <param name="maxRetryCount">Maximum retry count</param>
        /// <param name="canThrowException">True or False Whether it can throw the exception or not</param>
        /// <param name="delay">Delay interval to retry</param>
        /// <returns>Type that the retry function returns</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "It is required to catch the generic exception here")]
        public static TResult Execute<TResult>(Func<TResult> retryFunction, Predicate<Exception> canContinue, int maxRetryCount = 3, bool canThrowException = true, int delay = 200)
        {
            int retryCount = 0;
            TResult returnType = default(TResult);

            //Iteration for retry starts
            while (retryCount < maxRetryCount)
            {
                try
                {
                    // Invokes the delegate function
                    returnType = retryFunction();
                    return returnType; // returns the type
                }

                catch (Exception ex)
                {
                    retryCount += 1;

                    var canRetryContinue = canContinue(ex);

                    //Logs the exception
                    //LogExceptions(canRetryContinue, ex, retryCount, buildLogEntry, maxRetryCount, canThrowException);

                    //Get out of the loop if cannot be continued.
                    if (!canRetryContinue)
                    {
                        return returnType;
                    }
                }

                //Adding the delays
                System.Threading.Tasks.Task.Delay(delay);
            }

            return returnType;
        }
    }
}