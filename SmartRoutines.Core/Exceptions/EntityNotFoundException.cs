using System;

namespace SmartRoutines.Core.Exceptions
{
    public class EntityNotFoundException : SmartRoutinesException
    {
        public string EntityName { get; }
        public object Key { get; }

        public EntityNotFoundException(string entityName, object key)
            : base($"The entity '{entityName}' with ID ({key}) was not found.")
        {
            EntityName = entityName;
            Key = key;
        }

        public EntityNotFoundException(string entityName, object key, Exception inner)
            : base($"The entity '{entityName}' with ID ({key}) was not found.", inner)
        {
            EntityName = entityName;
            Key = key;
        }
    }
}
