using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Models
{
    // Generic data container that represent the success or failure of an operation
    // Behaves similar to a DTO or a wrapper model
    internal class Result<T>
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
        public T Data { get; }
        
        private Result(bool isSuccess, T data, string errorMessage)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
        }

        // Factory method for success
        public static Result<T> Success(T data)
        {
            return new Result<T>(true, data, null);
        }

        // Factory method for failure
        public static Result<T> Failure(string errorMsg)
        {
            return new Result<T>(false, default, errorMsg);
        }

    }
}
