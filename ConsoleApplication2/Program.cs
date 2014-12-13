using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    enum NodeType
    {
        Variable,
        Constant
    }

    class Program
    {
        static String ParamName;

        static void Main(string[] args)
        {
            /* типы выражений, который мы сможем изменить
             *  BinaryExpression:
             *  Add, Divide, Multiply, Power, Subtract
             * 
             * UnaryExpression:
             * Not
             * 
             * MethodCallExpression:
             * только для Type = Math и методов:
             * Pow, Sin, Cos, Tag, Cotag, log
             * 
             * типы, которые мы вообще пропускаем:
             * ConstantExpression
             * и A должно быть только в 1 месте.
             * 
             * Для строк тоже можно придумать. Только для строк у нас всего 1 операция - + и 
             * обратная к ней будет чтение по шаблону, как в С это было. Можно и регулярочкой
             */

            /* Идея: можно в CalcBinding предоставить пользователю самому написать обратную функцию, если 
             * разбор функции не получился
             * Label Content = "{c:Binding Path=Pow(A), Обр.функция=Sqrt(Path)}" />
             */

            #region примеры
            var exprList = new List<Expression<Func<int, int>>>() 
            {
                //+
                (a) => a + 2,
                (a) => 4 + a,
                
                //-
                (a) => a - 5,
                (a) => 4 - a,
               
                //*
                (a) => a * 3,
                (a) => 3 * a,

                // /
                (a) => a / 7,
                (a) => 3 / a,

                //complex
                (a) => ((a + 2 - 5 * 3) / (2 - 7) - 3) * 2 / 3,

                //constant complex
                (a) => 2 / 5 * 4 + 5 - (6 + 3) * 2 / 4,

                //bad samples
                (a) => a % 2,
                (a) => (int)Math.Max(a, 4)
            };
            #endregion

            foreach (var expr in exprList)
            {
                Console.WriteLine("Выражение: {0}", expr);
                Console.Write("Ответ: ");

                try
                {
                    var nodeType = ExpressionProcessStart(expr.Body);

                    Console.WriteLine("Выражение годится, тип узла: " + ((nodeType == NodeType.Constant) ? "константа" : "переменная"));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.ReadLine();
        }
 
        static NodeType ExpressionProcessStart(Expression expr)
        {
            ParamName = null;

            return ExpressionProcess(expr);
        }

        static NodeType ExpressionProcess(Expression expr)
        {
            if (expr is BinaryExpression)
            {
                var binExp = expr as BinaryExpression;
                var leftNodeType = ExpressionProcess(binExp.Left);
                var rightNodeType = ExpressionProcess(binExp.Right);

                if (leftNodeType == NodeType.Variable || rightNodeType == NodeType.Variable)
                    return NodeType.Variable;

                return NodeType.Constant;
            }
            
            if (expr is ParameterExpression)
            {
                var parameter = expr as ParameterExpression;
                if (ParamName == null)
                {
                    ParamName = parameter.Name;
                    return NodeType.Variable;
                }
                    
                if (ParamName == parameter.Name)
                    throw new Exception(String.Format("переменная {0} определена более 1 раза, ошибка"));
                else
                    throw new Exception(String.Format("определено как минимум 2 переменных: {0} и {1}, ошибка", ParamName, parameter.Name));
            }

            if (expr is ConstantExpression)
            {
                var constant = expr as ConstantExpression;

                return NodeType.Constant;
            }

            throw new Exception(String.Format("Данное выражение не знаем как распарсить: {0}, ошибка", expr));
        }
    }
}
