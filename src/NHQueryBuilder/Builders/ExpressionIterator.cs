using System.Linq.Expressions;

namespace NHQueryBuilder.Builders
{
    internal class ExpressionIterator
    {
        private IList<string> _properties;
        private int position = 0;
        private string _expression;

        public ExpressionIterator(Expression expression)
        {
            if (expression != null)
            {
                if (expression.GetType().BaseType == typeof(MethodCallExpression))
                {
                    MethodCallExpression mExp = expression as MethodCallExpression;
                    Process(mExp.Arguments.First().ToString());
                }
                else
                    Process(expression.ToString());
            }
        }

        public ExpressionIterator(string body)
        {
            Process(body);
        }

        private void Process(string body)
        {
            if (body.Contains("Convert("))
            {
                body = body.Replace("Convert(", "");
                
                int commaIndex = body.IndexOf(",");
                if (commaIndex > -1)
                    body = body.Substring(0, commaIndex);
                else
                    body = body.Substring(0, body.Length - 1);
            }
            else if (body.Contains("("))
            {
                body = body.Substring(body.IndexOf("(", StringComparison.Ordinal) + 1); // Start from '('
                body = body.Substring(0, body.Length - 1); // Until ')'

                if (body.Contains(","))
                {
                    var list = body.Split(',');
                    _properties = new List<string>();
                    foreach (var item in list)
                        _properties.Add(item.Split('.').Skip(1).FirstOrDefault());

                    return;
                }
            }

            _properties = body.Split('.').Skip(1).ToList();
            _expression = body;
        }

        public string Expression => _expression;

        public string Property => string.Join(".", _expression.Split('.').Skip(1));

        public string Alias => _expression.Split('.').First();

        public bool HasNext()
        {
            return position < _properties.Count;
        }

        public string Next()
        {
            return _properties.ElementAt(position++);
        }

        public string Previous()
        {
            return _properties.ElementAt(position--);
        }
    }
}
