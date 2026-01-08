using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.BLL.ErrorModel
{
    public class Result<T>
    {
        public readonly object GuestImage;

        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;
        public T? Data { get; set; }

        public Result(bool success, T? data, string? error)
        {
            Success = success;
            Data = data;
            Error = error;
        }
        public static Result<T> SuccessResult(T data)
        {
            return new Result<T>(true, data, string.Empty);
        }
        public static Result<T> FailResult(string error)
        {
            return new Result<T>(false, default, error);
        }

        internal static async Task<Result<IList<Room>>> SuccessResult(Task<IList<Room>> rooms)
        {
            throw new NotImplementedException();
        }
    }
}
