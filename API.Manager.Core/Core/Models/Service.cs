namespace API.Manager.Core.Models
{
    public class Service
    {
        public virtual int Id { get; set; }
        public virtual string Channel { get; set; }
        public virtual string Project { get; set; }
        public virtual string Controller { get; set; }
        public virtual string Method { get; set; }
        public virtual string MethodType { get; set; }
        public virtual bool IsServiceable { get; set; }
    }
}
