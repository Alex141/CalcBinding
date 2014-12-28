using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CalcBinding.Inverse
{
    /// <summary>
    /// Validate and inverse expression of one parameter
    /// </summary>
    public class Inverter
    {
        private const string RES = "({0})";

        private static readonly ExpressionFuncsDictionary<ExpressionType> inversedFuncs = new ExpressionFuncsDictionary<ExpressionType> 
        {
            // res = a+c or c+a => a = res - c
            {ExpressionType.Add, ConstantPlace.Wherever, constant => RES + "-" + constant},
            // res = c-a => a = c - res         
            {ExpressionType.Subtract, ConstantPlace.Left, constant => constant + "-" + RES},
            // res = a-c => a = res + c         
            {ExpressionType.Subtract, ConstantPlace.Right, constant => RES + "+" + constant},
            // res = c*a or a*c => a = res / c  
            {ExpressionType.Multiply, ConstantPlace.Wherever, constant => RES + "/" + constant},
            // res = c/a => a = c / res         
            {ExpressionType.Divide, ConstantPlace.Left, constant => constant + "/" + RES},
            // res = a/c => a = res*c           
            {ExpressionType.Divide, ConstantPlace.Right, constant => RES + "*" + constant},
        };
        
        private static readonly ExpressionFuncsDictionary<String> inversedMathFuncs = new ExpressionFuncsDictionary<string>
        {
            // res = Math.Sin(a) => a = Math.Asin(res)
            {"Math.Sin", ConstantPlace.Wherever, dummy => "Math.Asin" + RES},
            // res = Math.Asin(a) => a = Math.Sin(res)
            {"Math.Asin", ConstantPlace.Wherever, dummy => "Math.Sin" + RES},
            
            // res = Math.Cos(a) => a = Math.Acos(res)
            {"Math.Cos", ConstantPlace.Wherever, dummy => "Math.Acos" + RES},
            // res = Math.Acos(a) => a = Math.Cos(res)
            {"Math.Acos", ConstantPlace.Wherever, dummy => "Math.Cos" + RES},
            
            // res = Math.Tan(a) => a = Math.atan(res)
            {"Math.Tan", ConstantPlace.Wherever, dummy => "Math.Atan" + RES},
            // res = Math.Atan(a) => a = Math.Tan(res)
            {"Math.Atan", ConstantPlace.Wherever, dummy => "Math.Tan" + RES},

            // res = Math.Pow(c, a) => a = Math.Pow(res, 1/c)
            {"Math.Pow", ConstantPlace.Left, constant => "Math.Log(" + RES + ", " + constant + ")"},
            // res = Math.Pow(a, c) => a = Math.Pow(res, 1/c)
            {"Math.Pow", ConstantPlace.Right, constant => "Math.Pow(" + RES + ", 1.0/" + constant + ")"},

            // res = Math.Log(c, a) => a = Math.Pow(c, 1/res)
            {"Math.Log", ConstantPlace.Left, constant => "Math.Pow(" + constant + ", 1.0/" + RES + ")"},
            // res = Math.Log(a, c) => a = Math.Pow(c, res)
            {"Math.Log", ConstantPlace.Right, constant => "Math.Pow(" + constant + ", " + RES + ")"},

        };

        private static readonly ExpressionFuncsDictionary<String> inversedMath = new ExpressionFuncsDictionary<String>
        {
            // res = Sin(a) => a = Asin(res)
            {"Sin", ConstantPlace.Wherever, (dummy) => "Asin" + RES}
        };

        /// <summary>
        /// Inverse expression of one parameter
        /// </summary>
        /// <param name="expression">Expression Y=F(X)</param>
        /// <param name="parameter">Type and name of Y parameter</param>
        /// <returns>Inverted expression X = F_back(Y)</returns>
        public Lambda InverseExpression(Expression expression, ParameterExpression parameter)
        {
            var recInfo = new RecursiveInfo();
            InverseExpressionInternal(expression, recInfo);

            if (recInfo.FoundedParamName == null)
                throw new InverseException("Parameter was not found in expression!");

            recInfo.InvertedExp = String.Format(recInfo.InvertedExp, parameter.Name);

            var res = new Interpreter().Parse(recInfo.InvertedExp, new Parameter(parameter.Name, parameter.Type));
            Trace.WriteLine(res.ExpressionText);
            return res;
        }

        //public Expression InverseExpression(Expression expression, ParameterExpression parameter)
        //{
        //    return InverseException()
        //}

        /// <summary>
        /// Generate inversed expression tree from original expression tree of one parameter 
        /// using recursion
        /// </summary>
        /// <param name="expr">Original expression</param>
        /// <param name="recInfo">Out expression</param>
        /// <returns>NodeType - const or variable</returns>
        private NodeType InverseExpressionInternal(Expression expr, RecursiveInfo recInfo)
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
                            recInfo.InvertedExp = RES;
                            return NodeType.Variable;
                        }

                        if (recInfo.FoundedParamName == parameter.Name)
                            throw new InverseException(String.Format("Variable {0} is defined more than one time!"));
                        else
                            throw new InverseException(String.Format("More than one variables are defined in expression: {0} and {1}", recInfo.FoundedParamName, parameter.Name));
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
                case ExpressionType.Call:
                    {
                        var methodExpr = expr as MethodCallExpression;
                        
                        if (!inversedMathFuncs.ContainsKey(methodExpr.Method.ToString()))
                        {
                            throw new InverseException(String.Format("Unsupported method call expression: {0}", expr));
                        }

                        var leftOperandType = InverseExpressionInternal(methodExpr.Arguments[0], recInfo);
                        NodeType? rightOperandType = null;
                        Expression leftOperand, rightOperand = null;

                        leftOperand = methodExpr.Arguments[0];

                        if (methodExpr.Arguments.Count == 2)
                        {
                            rightOperandType = InverseExpressionInternal(methodExpr.Arguments[1], recInfo);
                            rightOperand = methodExpr.Arguments[1];
                        }

                        string inversedRes = null;
                        if (leftOperandType == NodeType.Variable)
                            inversedRes = inversedMathFuncs[methodExpr.Method.ToString(), ConstantPlace.Left](rightOperand);
                        else
                            if (rightOperandType.HasValue && rightOperandType.Value == NodeType.Variable)
                                inversedRes = inversedMathFuncs[methodExpr.Method.ToString(), ConstantPlace.Right](leftOperand);

                        if (inversedRes != null)
                            recInfo.InvertedExp = String.Format(recInfo.InvertedExp, inversedRes);

                        return inversedRes == null ? NodeType.Constant : NodeType.Variable;
                    }
                default:
                    throw new InverseException(String.Format("Unsupported expression: {0}", expr));
            }
        }

        #region Types for recursion func work
		
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

        private delegate String FuncExpressionDelegate(Expression constant);    
        
        /// <summary>
        /// Dictionary for inversed funcs static initialize
        /// </summary>
        private class ExpressionFuncsDictionary<T> : Dictionary<T, ConstantPlace, FuncExpressionDelegate>
        {
            public override FuncExpressionDelegate this[T key1, ConstantPlace key2]
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
        
        #endregion    
    }
}
