using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace SimpleAssemblyExplorer
{
    public class Evaluator
    {
        public static object Eval(string statement)
        {
            return _evaluatorType.InvokeMember(
                        "Eval",
                        BindingFlags.InvokeMethod,
                        null,
                        _evaluator,
                        new object[] { statement }
                     );
        }

        static Evaluator()
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("JScript");

            CompilerParameters parameters;
            parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            CompilerResults results;
            results = provider.CompileAssemblyFromSource(parameters, _jscriptSource);

            Assembly assembly = results.CompiledAssembly;
            _evaluatorType = assembly.GetType("Evaluator");

            _evaluator = Activator.CreateInstance(_evaluatorType);
        }

        private static object _evaluator = null;
        private static Type _evaluatorType = null;

        private static readonly string _jscriptSource =

            @"class Evaluator
              {
                  public function Eval(expr : String) : String 
                  { 
                     return eval(expr); 
                  }
              }";
    }
}