using System;
using System.Text;

namespace Reflex.Generator.Injector
{
    public class CodeStringBuilder
    {
        public StringBuilder Builder { get; }

        byte Indentation;
        bool OnNewLine;

        public BlockSegment CodeBlock() => new BlockSegment(this, ("{", "}"));
        public BlockSegment ArrayBlock() => new BlockSegment(this, ("[", "]"));
        public struct BlockSegment : IDisposable
        {
            readonly CodeStringBuilder Builder;

            (string Start, string End) Markers;

            public void Dispose()
            {
                Builder.Indentation -= 1;
                Builder.Write(Markers.End);
                Builder.Newline();
            }

            public BlockSegment(CodeStringBuilder Builder, (string Start, string End) Markers)
            {
                this.Builder = Builder;
                this.Markers = Markers;

                Builder.Newline();
                Builder.Write(Markers.Start);
                Builder.Newline();
                Builder.Indentation += 1;
            }
        }

        public void StartBlock(string symbol = "{")
        {
            Newline();
            Write(symbol);
            Newline();
            Indent();
        }
        public void EndBlock(string symbol = "}")
        {
            Unindent();
            Write(symbol);
            Newline();
        }

        public InlineBlock StringDeclaration() => new InlineBlock(this, ("\"", "\""));
        public InlineBlock Parameters() => new InlineBlock(this, ("(", ")"));
        public InlineBlock GenericArguments() => new InlineBlock(this, ("<", ">"));
        public struct InlineBlock : IDisposable
        {
            readonly CodeStringBuilder Builder;
            readonly (string Start, string End) Markers;

            public void Dispose()
            {
                Builder.Write(Markers.End);
            }

            public InlineBlock(CodeStringBuilder Builder, (string Start, string End) Markers)
            {
                this.Builder = Builder;
                this.Markers = Markers;

                Builder.Write(Markers.Start);
            }
        }

        public void Write<T>(T input)
        {
            if (OnNewLine)
            {
                OnNewLine = false;

                Builder.Append('\t', Indentation);
            }

            var text = input.ToString();

            Builder.Append(text);
        }

        public void Space() => Write(" ");

        public void EndLine(string terminator = ";")
        {
            Write(terminator);
            Newline();
        }

        public void Newline() => Newline(1);
        public void Newline(byte count)
        {
            for (byte i = 0; i < count; i++)
                Builder.AppendLine();

            OnNewLine = true;
        }

        public void Indent() => Indentation += 1;
        public void Unindent() => Indentation -= 1;

        public void UseNamespace(string name)
        {
            Write("using ");

            Write(name);

            EndLine();
        }

        public void Comment<T>(T content)
        {
            Write("//");
            Write(content);
        }

        public override string ToString() => Builder.ToString();

        public CodeStringBuilder() : this(0) { }
        public CodeStringBuilder(int capacity)
        {
            Builder = new StringBuilder(capacity);
        }
    }
}