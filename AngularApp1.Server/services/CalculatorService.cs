using AngularApp1.Server.mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Data;
using System.Numerics;
using System.Collections.Concurrent;

namespace AngularApp1.Server.services
{
    public class CalculatorService
    {
        private readonly IMongoCollection<CalculatorOperation> _operationsCollection;
        private readonly ConcurrentDictionary<BigInteger, BigInteger> _factorialCache;

        public CalculatorService(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _operationsCollection = database.GetCollection<CalculatorOperation>("Operations");
            _factorialCache = new ConcurrentDictionary<BigInteger, BigInteger>();
        }

        public async Task SaveOperationAsync(CalculatorOperation operation)
        {
            await _operationsCollection.InsertOneAsync(operation);
        }

        public async Task<List<CalculatorOperation>> GetOperationsAsync()
        {
            return await _operationsCollection.Find(op => true).ToListAsync();
        }

        public string CalculateResult(string expression)
        {
            if (expression.Contains("!"))
            {
                return CalculateFactorial(expression).ToString();
            }
            else
            {
                return EvaluateExpression(expression).ToString();
            }
        }

        private BigInteger CalculateFactorial(string expression)
        {
            var numberStr = expression.TrimEnd('!');
            if (BigInteger.TryParse(numberStr, out BigInteger number))
            {
                return FactorialCalculator.PrimeSwingFactorial(number);
            }
            throw new ArgumentException("Invalid factorial expression");
        }

        private BigInteger OptimizedFactorial(BigInteger n)
        {
            if (n <= 1) return BigInteger.One;

            if (_factorialCache.ContainsKey(n))
            {
                return _factorialCache[n];
            }

            BigInteger result = n * OptimizedFactorial(n - 1);
            _factorialCache[n] = result;
            return result;
        }

        private double EvaluateExpression(string expression)
        {
            var dataTable = new DataTable();
            return Convert.ToDouble(dataTable.Compute(expression, string.Empty));
        }

        public string ProcessAndSaveOperation(string expression)
        {
            string result = CalculateResult(expression);

            var operation = new CalculatorOperation
            {
                Operation = expression,
                Result = result,
                Date = DateTime.UtcNow
            };

            SaveOperationAsync(operation).GetAwaiter().GetResult();

            return result;
        }
    }
}
