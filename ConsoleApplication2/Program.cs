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

    enum ConstantPlace
    {
        Left,
        Right,
        Wherever
    }

    public class Dictionary<TKey1, TKey2, TValue>: 
        Dictionary<TKey1, Dictionary<TKey2, TValue>> 
    { 
        public new void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            if (!ContainsKey(key1))
                Add(key1, new Dictionary<TKey2, TValue>());

            this[key1].Add(key2, value);
        }

    }

    class ExpressionFuncsDictionary:Dictionary<ExpressionType, ConstantPlace, FuncExpressionDelegate>
    {
        public FuncExpressionDelegate this[ExpressionType key1, ConstantPlace key2]
        {
            get
            {
                var dict = this[key1];

                if (dict.ContainsKey(key2))
                    return dict[key2];

                if (dict.ContainsKey(ConstantPlace.Wherever))
                    return dict[ConstantPlace.Wherever];

                return dict[key2];
            }
        }
    }

    public delegate Func<Expression, Expression> FuncExpressionDelegate(Expression constant);

    class Program
    {
        static String ParamName;

        static ExpressionFuncsDictionary inverseFuncs = new ExpressionFuncsDictionary 
        {
            /* Path = a+c or c+a => a = Path - c */
            {ExpressionType.Add, ConstantPlace.Wherever, constant => (res => Expression.Subtract(res, constant))},
            /* Path = c-a => a = c - Path */
            {ExpressionType.Subtract, ConstantPlace.Left, constant => (res => Expression.Subtract(constant, res))},
            {ExpressionType.Subtract, ConstantPlace.Right, constant => (res => Expression.Add(res, constant))},
            {ExpressionType.Multiply, ConstantPlace.Wherever, constant => (res => Expression.Divide(res, constant))},
            {ExpressionType.Divide, ConstantPlace.Left, constant => (res => Expression.Divide(constant, res))},
            {ExpressionType.Divide, ConstantPlace.Right, constant => (res => Expression.Multiply(res, constant))},
            {ExpressionType.Convert, ConstantPlace.Wherever, (operand) => (res => Expression.Convert(res, operand.Type))},
        };

        static Stack<Func<Expression, Expression>> inverseFuncsStack = new Stack<Func<Expression, Expression>>();

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
             * и A должно быть только в 1 месте.
             * 
             * типы, которые мы вообще пропускаем:
             * ConstantExpression
             * 
             * Для строк тоже можно придумать. Только для строк у нас всего 1 операция - + и 
             * обратная к ней будет чтение по шаблону, как в С это было. Можно и регулярочкой
             */

            /* Идея: можно в CalcBinding предоставить пользователю самому написать обратную функцию, если 
             * разбор функции не получился
             * Label Content = "{c:Binding Path=Pow(A), Обр.функция=Sqrt(Path)}" />
             */

            /*
             * a+c -> r - c
             * 
             * 
             */

            /* template
             * 
             *func, varPos (left, right) -> { 
             */

            /* недостаток генерирования expression:
         
             -за одну рекурсию не могу генерировать (из за того что выходящий expression нельзя менять, а это нам не годится
             -из-за этого дублируются switch и условия. Можно наверное как-то избавиться от этого с помощью класса visitor, 
             * или нельзя, ну получится по моему не читаемо
             -с Convert есть проблемы, мне кажется что потом мы получим от него трудности. А может не получим
             */ 

            /* достоинства генерирования expression:
             * -не работаем со строками
             */
            #region примеры
            var exprList = new List<Expression<Func<int, double>>>() 
            {
                //+
                (a) => a + 2,
                (a) => 4 + a,
                (a) => a + 2 + 3 + 5 + 2,
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

                (a) => (int)((double)5.0 / (17-a)),
 
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
                    var inverseExpression = InverseExpression(expr.Body);

                    Console.WriteLine("Выражение годится, обратная функция: {0}", inverseExpression);
                    //, тип узла: " + ((nodeType == NodeType.Constant) ? "константа" : "переменная"));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.ReadLine();
        }
 
        
        public static Expression InverseExpression(Expression expr)
        {
            ParamName = null;
            InverseExpressionInternal(expr);
            
            Expression res = Expression.Parameter(typeof(int), "Path");

            while (inverseFuncsStack.Count > 0)
            {
                res = inverseFuncsStack.Pop()(res);
            }
            return res;
        }

        private static NodeType InverseExpressionInternal(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                {
                    var binExp = expr as BinaryExpression;

                    var leftOperandType = InverseExpressionInternal(binExp.Left);
                    var rightOperandType = InverseExpressionInternal(binExp.Right);

                    var nodeType = (leftOperandType == NodeType.Variable || rightOperandType == NodeType.Variable) 
                                    ? NodeType.Variable 
                                    : NodeType.Constant;
                    
                    if (nodeType == NodeType.Variable)
                    {
                        var constantPlace = leftOperandType == NodeType.Constant ? ConstantPlace.Left : ConstantPlace.Right;
                        var constant = leftOperandType == NodeType.Constant ? binExp.Left : binExp.Right;
                        inverseFuncsStack.Push(inverseFuncs[expr.NodeType, constantPlace](constant));
                    }

                    return nodeType;
                }
                case ExpressionType.Parameter:
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

                case ExpressionType.Constant:
                {
                    var constant = expr as ConstantExpression;

                    return NodeType.Constant;
                }
                case ExpressionType.Convert:
                {
                    var convertExpr = expr as UnaryExpression;
                    var operandType = InverseExpressionInternal(convertExpr.Operand);

                    if (operandType == NodeType.Variable)
                    {
                        //Func<Expression, Expression> inverseFunc = (operand) => Expression.Convert(operand, convertExpr.Operand.Type);
                        inverseFuncsStack.Push(inverseFuncs[ExpressionType.Convert, ConstantPlace.Wherever](convertExpr.Operand));
                    }
                    return operandType;
                }
                default:
                    throw new Exception(String.Format("Данное выражение не знаем как распарсить: {0}, ошибка", expr));
            }
        }
    }
}
