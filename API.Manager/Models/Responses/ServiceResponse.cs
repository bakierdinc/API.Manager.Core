namespace API.Manager.Models.Responses
{
    public class ServiceResponse
    {
        public virtual int Id { get; set; }
        public virtual string Project { get; set; }
        public virtual string Controller { get; set; }
        public virtual string Method { get; set; }
        public virtual string MethodType { get; set; }
        public virtual string IsServiceable { get; set; }
    }
}
