using Backend.Domain.Modals;

namespace Backend.Payloads
{
    public class MutationReturnPayload
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }

    }
}
