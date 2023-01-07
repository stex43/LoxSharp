namespace Loxsharp;

internal sealed class Parser
{
    private readonly List<Token> tokens;

    private int current;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    private Expression Expression()
    {
        return this.Equality();
    }

    private Expression Equality()
    {
        var expression = this.Comparison();

        while (Match(TokenType.EqualEqual, TokenType.BangEqual))
        {
            var tokenOperator = this.Previous();
            var right = this.Comparison();
            expression = new Expression.Binary(expression, tokenOperator, right);
        }

        return expression;
    }

    private Expression Comparison()
    {
        var expression = this.Term();

        while (this.Match(TokenType.Less, TokenType.LessEqual, TokenType.Greater, TokenType.GreaterEqual))
        {
            var tokenOperator = this.Previous();
            var right = this.Term();
            expression = new Expression.Binary(expression, tokenOperator, right);
        }

        return expression;
    }

    private Expression Term()
    {
        var expression = this.Factor();

        while (this.Match(TokenType.Plus, TokenType.Minus))
        {
            var tokenOperator = this.Previous();
            var right = this.Factor();
            expression = new Expression.Binary(expression, tokenOperator, right);
        }

        return expression;
    }

    private Expression Factor()
    {
        var expression = this.Unary();

        while (this.Match(TokenType.Slash, TokenType.Star))
        {
            var tokenOperator = this.Previous();
            var right = this.Unary();
            expression = new Expression.Binary(expression, tokenOperator, right);
        }

        return expression;
    }

    private Expression Unary()
    {
        if (this.Match(TokenType.Bang, TokenType.Minus))
        {
            var tokenOperator = this.Previous();
            var expression = this.Unary();
            return new Expression.Unary(tokenOperator, expression);
        }

        return this.Primary();
    }

    private Expression Primary()
    {
        if (this.Match(TokenType.False))
        {
            return new Expression.Literal(false);
        }

        if (this.Match(TokenType.True))
        {
            return new Expression.Literal(true);
        }

        if (this.Match(TokenType.Nil))
        {
            return new Expression.Literal(null);
        }

        if (this.Match(TokenType.Number, TokenType.String))
        {
            return new Expression.Literal(this.Previous().Literal);
        }

        if (this.Match(TokenType.LeftParen))
        {
            var expression = this.Expression();
            this.Consume(TokenType.RightParen, "Expect ')' after expression.");
            return new Expression.Grouping(expression);
        }
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var tokenType in types)
        {
            if (this.Check(tokenType))
            {
                this.Advance();
                return true;
            }
        }

        return false;
    }

    private Token Advance()
    {
        return this.tokens[this.current++];
    }

    private bool Check(TokenType tokenType)
    {
        if (this.IsAtEnd())
        {
            return false;
        }

        return this.Peek().Type == tokenType;
    }

    private Token Peek()
    {
        return this.tokens[this.current];
    }

    private Token Previous()
    {
        return this.tokens[this.current - 1];
    }

    private bool IsAtEnd()
    {
        return this.Peek().Type == TokenType.Eof;
    }
}