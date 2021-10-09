using System.Data;

namespace API.Manager.Infrastracture
{
    public class CommandParameter
    {
        public virtual string Name { get; set; }
        public virtual object Value { get; set; }
        public virtual DbType Type { get; set; }  
    }
}
