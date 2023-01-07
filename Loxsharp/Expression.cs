using Loxsharp;

internal abstract class Expression
{
   internal sealed class Binary : Expression
   {
       public Binary (Expression Left, Token TokenOperator, Expression Right)
       {
           this.Left = Left;
           this.TokenOperator = TokenOperator;
           this.Right = Right;
       }

      public Expression Left { get; }

      public Token TokenOperator { get; }

      public Expression Right { get; }
   }

   internal sealed class Grouping : Expression
   {
       public Grouping (Expression Expression)
       {
           this.Expression = Expression;
       }

      public Expression Expression { get; }
   }

   internal sealed class Literal : Expression
   {
       public Literal (object Value)
       {
           this.Value = Value;
       }

      public object Value { get; }
   }

   internal sealed class Unary : Expression
   {
       public Unary (Token TokenOperator, Expression Right)
       {
           this.TokenOperator = TokenOperator;
           this.Right = Right;
       }

      public Token TokenOperator { get; }

      public Expression Right { get; }
   }

}
