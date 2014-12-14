using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    /// <summary>
    /// Validate and inverse expression of one parameter
    /// </summary>
    public class Inverter
    {
        private const string RES = "{0}";

        private static readonly ExpressionFuncsDictionary inversedFuncs = new ExpressionFuncsDictionary 
        {
            // res = a+c or c+a => a = res - c
            {ExpressionType.Add, ConstantPlace.Wherever, constant => RES + "-" + constant},
            // res = c-a => a = c - res         
            {ExpressionType.Subtract, ConstantPlace.Left, constant => constant + "-" + RES},
            // res = a-c => a = res + c         
            {ExpressionType.Subtract, ConstantPlace.Right, constant => RES + constant},
            // res = c*a or a*c => a = res / c  
            {ExpressionType.Multiply, ConstantPlace.Wherever, constant => RES + "/" + constant},
            // res = c/a => a = c / res         
            {ExpressionType.Divide, ConstantPlace.Left, constant => constant + "/" + RES},
            // res = a/c => a = res*c           
            {ExpressionType.Divide, ConstantPlace.Right, constant => RES + "*" + constant},
        };

        /// <summary>
        /// Inverse expression of one parameter
        /// </summary>
        /// <param name="expression">Expression Y=F(X)</param>
        /// <param name="parameter">Type and name of Y parameter</param>
        /// <returns>Inverted expression X = F_back(Y)</returns>
        public static Expression InverseExpression(Expression expression, ParameterExpression parameter)
        {
            var recInfo = new RecursiveInfo();
            InverseExpressionInternal(expression, recInfo);

            recInfo.InvertedExp = String.Format(recInfo.InvertedExp, parameter.Name);

            return new Interpreter().Parse(recInfo.InvertedExp, new Parameter(parameter.Name, parameter.Type)).Expression;
        }

        /// <summary>
        /// Generate inversed expression tree by original expression tree of one parameter using recursion
        /// </summary>
        /// <param name="expr">Original expression</param>
        /// <param name="recInfo">Out expression</param>
        /// <returns>NodeType - const or variable</returns>
        private static NodeType InverseExpressionInternal(Expression expr, RecursiveInfo recInfo)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                    {
                        var binExp = expr as BinaryExpression;

                        var leftOperandType = InverseExpressionInternal(binExp.Left, recInfo);
                        var rightOperandType = InverseExpressionInternal(binExp.Right, recInfo);

                        var nodeType = (leftOperandType == NodeType.Variable || rightOperandType == NodeType.Variable)
                                        ? NodeType.Variable
                                        : NodeType.Constant;

                        if (nodeType == NodeType.Variable)
                        {
                            var constantPlace = leftOperandType == NodeType.Constant ? ConstantPlace.Left : ConstantPlace.Right;
                            var constant = leftOperandType == NodeType.Constant ? binExp.Left : binExp.Right;
                            recInfo.InvertedExp = String.Format(recInfo.InvertedExp, inversedFuncs[expr.NodeType, constantPlace](constant));
                        }

                        return nodeType;
                    }
                case ExpressionType.Parameter:
                    {
                        var parameter = expr as ParameterExpression;

                        if (recInfo.FoundedParamName == null)
                        {
                            recInfo.FoundedParamName = parameter.Name;
                            return NodeType.Variable;
                        }

                        if (recInfo.FoundedParamName == parameter.Name)
                            throw new Exception(String.Format("Variable {0} is defined more than one time!"));
                        else
                            throw new Exception(String.Format("More than one variables are defined in expression: {0} and {1}", recInfo.FoundedParamName, parameter.Name));
                    }

                case ExpressionType.Constant:
                    {
                        var constant = expr as ConstantExpression;

                        return NodeType.Constant;
                    }
                case ExpressionType.Convert:
                    {
                        var convertExpr = expr as UnaryExpression;
                        var operandType = InverseExpressionInternal(convertExpr.Operand, recInfo);
                        return operandType;
                    }
                default:
                    throw new Exception(String.Format("Данное выражение не знаем как распарсить: {0}, ошибка", expr));
            }
        }

        #region For recursion func work
		
        internal enum NodeType
        {
            Variable,
            Constant
        }

        internal enum ConstantPlace
        {
            Left,
            Right,
            Wherever
        }

        private class RecursiveInfo
        {
            public string FoundedParamName;
            public string InvertedExp;
        }
	    
        #endregion    

        /// <summary>
        /// Dictionary for inversed funcs static initialize
        /// </summary>
        private class ExpressionFuncsDictionary : Dictionary<ExpressionType, ConstantPlace, FuncExpressionDelegate>
        {
            public override FuncExpressionDelegate this[ExpressionType key1, ConstantPlace key2]
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
    }
}
