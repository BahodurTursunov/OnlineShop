//namespace BaseLibrary.DTOs
//{
//    public class ApiResponse<T>
//    {
//        public bool Success { get; set; }
//        public T? Data { get; set; }
//        public string? Message { get; set; }

//        public ApiResponse(bool success, T? data = default, string? message = null)
//        {
//            Success = success;
//            Data = data;
//            Message = message;
//        }

//        public static ApiResponse<T> SuccessResponse(T data)
//        {
//            return new ApiResponse<T>(true, data);
//            // TODO : посмотреть что выходит в мессаже 
//        }

//        public static ApiResponse<T> ErrorResponse(string message)
//        {
//            return new ApiResponse<T>(false, default, message);
//        }

//        public static ApiResponse<T> FailureResponse(string message)
//        {
//            return new ApiResponse<T>(false, default, message);
//        }
//    }
//}
