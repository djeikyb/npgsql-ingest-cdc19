using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Xunit;

namespace Test;

public class UnitTest1
{
    const string tsv = @"col1	col2	col3
ab	cd	ef
gh	ij	kl
mn	op	qr
";

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    public void Test1(int bufferSize)
    {
        var col1 = new List<string>();
        var col2 = new List<string>();
        var col3 = new List<string>();

        var reader = new StringReader(tsv);

        Span<char> buffer = stackalloc char[bufferSize];

        var maxFieldSize = 1024;
        Span<char> field = stackalloc char[maxFieldSize];
        field.Clear();

        var fieldLength = 0;
        var colIdx = 0;

        const char delim = '\t';
        const char newline = '\n';

        int readLen;
        while ((readLen = reader.ReadBlock(buffer)) > 0)
        {
            for (var i = 0; i < readLen; i++)
            {
                var c = buffer[i];

                if (c == delim || c == newline)
                {
                    var s = new string(field[..fieldLength]);

                    // column specific assignment stuff goes here
                    switch (colIdx)
                    {
                        case 0:
                            col1.Add(s);
                            break;
                        case 1:
                            col2.Add(s);
                            break;
                        case 2:
                            col3.Add(s);
                            break;
                        default: throw new Exception($"Unexpected column index: {colIdx}.");
                    }

                    // reset field buffer
                    fieldLength = 0;

                    // shift the column index
                    if (c == delim)
                    {
                        // increase if delimiter
                        colIdx += 1;
                    }
                    else
                    {
                        // reset to zero if line separator
                        colIdx = 0;
                    }
                }
                else
                {
                    field[fieldLength] = c;
                    fieldLength += 1;
                }
            }
        }

        col1.Should().Equal(new List<string> { "col1", "ab", "gh", "mn" });
        col2.Should().Equal(new List<string> { "col2", "cd", "ij", "op" });
        col3.Should().Equal(new List<string> { "col3", "ef", "kl", "qr" });
    }
}
