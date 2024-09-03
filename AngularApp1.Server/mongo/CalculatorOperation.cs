namespace AngularApp1.Server.mongo
{
    public class CalculatorOperation
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public string Operation { get; set; }         
        public string Result { get; set; }             
        public DateTime Date { get; set; } = DateTime.UtcNow;  
    }
}
