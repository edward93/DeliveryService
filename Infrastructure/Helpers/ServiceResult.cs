using System.Collections.Generic;
using System.Linq;
using DAL.Enums;

namespace Infrastructure.Helpers
{
    /// <summary>
    /// This class is an implementation of the options pattern
    /// </summary>
    public class ServiceResult
    {
        public ServiceResult()
        {
            Messages = new List<Message<MessageType, string>>();
        }
        public bool Success { get; set; }
        public object Data { get; set; }
        public List<Message<MessageType, string>> Messages { get; set; }

        public string DisplayMessage ()
        {
            return Messages.Aggregate("",
                (current, message) =>
                    current +
                    (message.Key == MessageType.Error
                        ? $"Error: {message.Value}"
                        : message.Key == MessageType.Info ? $"Info: {message.Value}" : $"Warning: {message.Value}"));
        }
    }
}