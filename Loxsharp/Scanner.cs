using System.Globalization;

namespace Loxsharp;

internal sealed class Scanner
{
    private readonly string source;
    private readonly List<Token> tokens = new();

    private int start;
    private int current;
    private int line = 1;

    private static readonly Dictionary<string, TokenType> ReservedWords = new()
    {
        { "and", TokenType.And },
        { "class", TokenType.Class },
        { "else", TokenType.Else },
        { "false", TokenType.False },
        { "fun", TokenType.Fun },
        { "for", TokenType.For },
        { "if", TokenType.If },
        { "nil", TokenType.Nil },
        { "or", TokenType.Or },
        { "print", TokenType.Print },
        { "return", TokenType.Return },
        { "super", TokenType.Super },
        { "this", TokenType.This },
        { "true", TokenType.True },
        { "var", TokenType.Var },
        { "while", TokenType.While }
    };

    public Scanner(string source)
    {
        this.source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!this.IsAtEnd())
        {
            this.start = this.current;
            this.ScanToken();
        }
        
        this.tokens.Add(new Token(TokenType.Eof, "", null, this.line));

        return this.tokens;
    }

    private void ScanToken()
    {
        var character = this.Advance();
        switch (character)
        {
            case '(':
                this.AddToken(TokenType.LeftParen);
                break;

            case ')':
                this.AddToken(TokenType.RightParen);
                break;

            case '{':
                this.AddToken(TokenType.LeftBrace);
                break;

            case '}':
                this.AddToken(TokenType.RightBrace);
                break;

            case ',':
                this.AddToken(TokenType.Comma);
                break;

            case '.':
                this.AddToken(TokenType.Dot);
                break;

            case '-':
                this.AddToken(TokenType.Minus);
                break;

            case '+':
                this.AddToken(TokenType.Plus);
                break;

            case ';':
                this.AddToken(TokenType.Semicolon);
                break;

            case '*':
                this.AddToken(TokenType.Star);
                break;
            
            case '!':
                this.AddToken(this.Match('=') ? TokenType.BangEqual : TokenType.Bang);
                break;
            
            case '=':
                this.AddToken(this.Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                break;
            
            case '<':
                this.AddToken(this.Match('=') ? TokenType.LessEqual : TokenType.Less);
                break;
            
            case '>':
                this.AddToken(this.Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                break;
            
            case '/':
                if (this.Match('/'))
                {
                    while (!this.IsAtEnd() && this.Peek() != '\n')
                    {
                        this.Advance();
                    }
                    
                    break;
                }

                this.AddToken(TokenType.Slash);
                break;
            
            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                this.line++;
                break;
            
            case '"':
                this.ScanString();
                break;

            default:
                if (char.IsDigit(character))
                {
                    this.ScanNumber();
                    break;
                }

                if (IsAlpha(character))
                {
                    this.ScanIdentifier();
                    break;
                }
                
                Lox.Error(this.line, $"Unexpected character. {character}");
                break;
        }
    }

    private char Advance()
    {
        return this.source[this.current++];
    }

    private char Peek()
    {
        return this.IsAtEnd() ? '\0' : this.source[this.current];
    }

    private char NextPeek()
    {
        if (this.current + 1 >= this.source.Length)
        {
            return '\0';
        }

        return this.source[this.current + 1];
    }
    
    private bool Match(char expected)
    {
        if (this.IsAtEnd() || this.source[this.current] != expected)
        {
            return false;
        }

        this.current++;
        return true;
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        var text = this.source.Substring(this.start, this.current - this.start);
        this.tokens.Add(new Token(type, text, literal, this.line));
    }  

    private void ScanString()
    {
        while (!this.IsAtEnd() && this.Peek() != '"')
        {
            if (this.Peek() == '\n')
            {
                this.line++;
            }

            this.Advance();
        }

        if (this.IsAtEnd())
        {
            Lox.Error(this.line, "Unterminated string.");
            return;
        }

        this.Advance();

        var value = this.source.Substring(this.start + 1, this.current - this.start - 2);
        this.AddToken(TokenType.String, value);
    }

    private void ScanNumber()
    {
        while (char.IsDigit(this.Peek()))
        {
            this.Advance();
        }

        if (this.Peek() == '.' && char.IsDigit(this.NextPeek()))
        {
            this.Advance();
        }
        
        while (char.IsDigit(this.Peek()))
        {
            this.Advance();
        }

        var stringValue = this.source.Substring(this.start, this.current - this.start);
        this.AddToken(TokenType.Number, double.Parse(stringValue, CultureInfo.InvariantCulture));
    }
    
    private void ScanIdentifier()
    {
        while (IsAlphaNumeric(this.Peek()))
        {
            this.Advance();
        }

        var identifier = this.source.Substring(this.start, this.current - this.start);

        if (!ReservedWords.TryGetValue(identifier, out var type))
        {
            type = TokenType.Identifier;
        }

        this.AddToken(type);
    }

    private static bool IsAlpha(char character)
    {
        return char.IsLetter(character) || character == '_';
    }

    private static bool IsAlphaNumeric(char character)
    {
        return char.IsLetter(character) || char.IsDigit(character) || character == '_';
    }

    private bool IsAtEnd() 
    {
        return this.current >= this.source.Length;
    }
}