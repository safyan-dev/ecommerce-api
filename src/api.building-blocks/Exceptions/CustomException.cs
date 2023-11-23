namespace api.building.Exceptions
{
    public class CustomException: Exception
    {
        public CustomException(string message, params string[] errors): base(message)
        {
            ErrorMessages = errors;
        }

        public IEnumerable<string> ErrorMessages { get; protected set; }
    }
}
