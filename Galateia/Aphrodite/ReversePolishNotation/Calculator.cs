using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aphrodite.ReversePolishNotation
{
    public class Calculator
    {
        private readonly Dictionary<string, FunctionParam> _functions = new Dictionary<string, FunctionParam>();
        private readonly Dictionary<string, double> _variables = new Dictionary<string, double>();

        /// <summary>
        ///     変数・関数の定義，代入操作を含めて，入力文字列を処理します
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>計算結果がある場合は数値，ない場合はNaNが返ります</returns>
        public double Process(string input)
        {
            double result;
            if (TryProcess(input, out result))
                return result;

            throw new ArgumentException("Invalid input");
        }

        public bool TryProcess(string input, out double result)
        {
            result = double.NaN;

            var regex =
                new Regex(
                    @"\s*(?<lhs>(?<id>\w[\w\d]*)\s*(?<args>\(\s*(?<arg>\w[\w\d]*)(\s*,\s*(?<arg>\w[\w\d]*))*\s*\))?\s*=)?\s*(?<def>.*)");
            Match m = regex.Match(input);
            if (!m.Success)
            {
                result = double.NaN;
                return false;
            }

            if (m.Groups["id"].Captures.Count == 1)
            {
                string id = m.Groups["id"].Captures[0].Value;
                if (m.Groups["def"].Captures.Count != 1)
                    throw new ArgumentException("No value to assign.");
                Notation rhs = Notation.Parse(m.Groups["def"].Captures[0].Value);

                if (m.Groups["args"].Captures.Count == 1)
                {
                    // 関数の場合
                    var param = new FunctionParam(rhs, from Capture c in m.Groups["arg"].Captures
                        select c.Value);
                    if (_functions.ContainsKey(id))
                        _functions[id] = param;
                    else
                        _functions.Add(id, param);
                    result = double.NaN;
                }
                else
                {
                    // 変数の場合
                    double r = Calculate(rhs, _variables, _functions);
                    if (_variables.ContainsKey(id))
                        _variables[id] = r;
                    else
                        _variables.Add(id, r);
                    result = r;
                }
            }
            else
            {
                try
                {
                    result = Calculate(Notation.Parse(input), _variables, _functions);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     逆ポーランド記法の式を計算します
        /// </summary>
        /// <param name="notation">逆ポーランド記法で表現された式</param>
        /// <param name="variables">変数リスト．</param>
        /// <param name="functions">関数リスト．</param>
        /// <returns>計算結果</returns>
        private static double Calculate(Notation notation, Dictionary<string, double> variables,
            Dictionary<string, FunctionParam> functions)
        {
            var stack = new Stack<IToken>();
            foreach (IToken token in notation.Tokens())
            {
                switch (token.Type)
                {
                    case TokenTypes.Constant:
                        stack.Push(token);
                        break;
                    case TokenTypes.Variable:
                        stack.Push(token);
                        break;
                    case TokenTypes.UnaryOperator:
                    {
                        double r = GetValue(stack.Pop(), variables);
                        switch (((UnaryOperator) token).Operation)
                        {
                            case UnaryOperator.Operations.Plus:
                                stack.Push(new Constant(r));
                                break;
                            case UnaryOperator.Operations.Minus:
                                stack.Push(new Constant(-r));
                                break;
                            default:
                                throw new InvalidOperationException("Invalid unary operator");
                        }
                    }
                        break;
                    case TokenTypes.BinaryOperator:
                    {
                        double r = GetValue(stack.Pop(), variables);
                        IToken lToken = stack.Pop();
                        double l = GetValue(lToken, variables);
                        switch (((BinaryOperator) token).Operation)
                        {
                            case BinaryOperator.Operations.Addition:
                                stack.Push(new Constant(l + r));
                                break;
                            case BinaryOperator.Operations.Subtraction:
                                stack.Push(new Constant(l - r));
                                break;
                            case BinaryOperator.Operations.Multiplication:
                                stack.Push(new Constant(l*r));
                                break;
                            case BinaryOperator.Operations.Division:
                                stack.Push(new Constant(l/r));
                                break;
                            case BinaryOperator.Operations.Power:
                                stack.Push(new Constant(Math.Pow(l, r)));
                                break;
                            case BinaryOperator.Operations.Modulo:
                                stack.Push(new Constant(l%r));
                                break;
                            default:
                                throw new InvalidOperationException("Invalid binary operator");
                        }
                    }
                        break;
                    case TokenTypes.Function:
                    {
                        var f = (Function) token;
                        var args = new double[f.NumberOfArguments];
                        for (int i = f.NumberOfArguments - 1; i >= 0; i--)
                            args[i] = GetValue(stack.Pop(), variables);
                        stack.Push(new Constant(CallFunction(f, args, variables, functions)));
                    }
                        break;
                    default:
                        throw new InvalidOperationException("Invalid token");
                }
            }

            if (stack.Count != 1)
                throw new InvalidOperationException("Invalid notation");

            return GetValue(stack.Pop(), variables);
        }

        private static double GetValue(IToken token, Dictionary<string, double> variables)
        {
            switch (token.Type)
            {
                case TokenTypes.Constant:
                    return ((Constant) token).Value;
                case TokenTypes.Variable:
                    return variables[((Variable) token).Token];
                default:
                    throw new InvalidOperationException("Invalid token");
            }
        }

        private static double CallFunction(Function f, double[] args, Dictionary<string, double> variables,
            Dictionary<string, FunctionParam> functions)
        {
            switch (f.Token.ToLower())
            {
                case "abs":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Abs(args[0]);
                case "acos":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Acos(args[0]);
                case "acosh":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Log(args[0] + Math.Sqrt(args[0]*args[0] - 1));
                case "asin":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Asin(args[0]);
                case "asinh":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Log(args[0] + Math.Sqrt(args[0]*args[0] + 1));
                case "atan":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Atan(args[0]);
                case "atan2":
                    if (f.NumberOfArguments != 2)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Atan2(args[0], args[1]);
                case "atanh":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return (Math.Log(1 + args[0]) - Math.Log(1 - args[0]))/2;
                case "ceiling":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Ceiling(args[0]);
                case "cos":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Cos(args[0]);
                case "cosh":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Cosh(args[0]);
                case "erf":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Erf(args[0]);
                case "erfc":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return 1 - Erf(args[0]);
                case "exp":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Exp(args[0]);
                case "floor":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Floor(args[0]);
                case "log":
                    switch (f.NumberOfArguments)
                    {
                        case 1:
                            return Math.Log(args[0]);
                        case 2:
                            return Math.Log(args[0], args[1]);
                        default:
                            throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                        f.NumberOfArguments + " arguments.");
                    }
                case "log10":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Log10(args[0]);
                case "max":
                    if (f.NumberOfArguments != 2)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Max(args[0], args[1]);
                case "min":
                    if (f.NumberOfArguments != 2)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Min(args[0], args[1]);
                case "round":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Round(args[0]);
                case "sign":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Sign(args[0]);
                case "sin":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Sin(args[0]);
                case "sinh":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Sinh(args[0]);
                case "sqrt":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Sqrt(args[0]);
                case "tan":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Tan(args[0]);
                case "tanh":
                    if (f.NumberOfArguments != 1)
                        throw new ArgumentException("No overload for function " + f.Token + " takes " +
                                                    f.NumberOfArguments + " arguments.");
                    return Math.Tanh(args[0]);
                default:
                    if (!functions.ContainsKey(f.Token)) throw new InvalidOperationException("No function " + f.Token);
                    FunctionParam fp = functions[f.Token];
                    if (f.NumberOfArguments != fp.ArgumentNames.Length)
                        throw new ArgumentException("User defined function " + f.Token + " takes " +
                                                    fp.ArgumentNames.Length + " arguments.");
                    var arguments = new Dictionary<string, double>();
                    for (int i = 0; i < args.Length; i++)
                        arguments.Add(fp.ArgumentNames[i], args[i]);
                    return Calculate(fp.Notation.Substitute(arguments), variables, functions);
            }
        }

        private static double Erf(double x)
        {
            // constants
            const double a1 = 0.254829592;
            const double a2 = -0.284496736;
            const double a3 = 1.421413741;
            const double a4 = -1.453152027;
            const double a5 = 1.061405429;
            const double p = 0.3275911;

            // Save the sign of x
            int sign = (x < 0) ? -1 : 1;
            x = (x < 0) ? -x : x;

            // Handbook of Mathematical Functions by Abramowitz and Stegun : formula 7.1.26
            double t = 1.0/(1.0 + p*x);
            double y = 1.0 - (((((a5*t + a4)*t) + a3)*t + a2)*t + a1)*t*Math.Exp(-x*x);

            return sign*y;
        }

        private class FunctionParam
        {
            public readonly string[] ArgumentNames;
            public readonly Notation Notation;

            public FunctionParam(Notation notation, IEnumerable<string> argumentNames)
            {
                Notation = notation;
                ArgumentNames = argumentNames.ToArray();
            }
        }
    }
}