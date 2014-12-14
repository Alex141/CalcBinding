using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    public delegate String FuncExpressionDelegate(Expression constant);

    class Program
    {
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
                    var inversedExpression = Inverter.InverseExpression(expr.Body, Expression.Parameter(typeof(double), "Path"));

                    Console.WriteLine("Выражение годится, обратная функция: {0}", inversedExpression);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.ReadLine();
        }
    }
}
