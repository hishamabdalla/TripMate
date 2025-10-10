namespace Tripmate.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
        public NotFoundException(string Entity, string id) : base(($"{Entity} with ID {id}  not found"))
        {

        }

    }
}
