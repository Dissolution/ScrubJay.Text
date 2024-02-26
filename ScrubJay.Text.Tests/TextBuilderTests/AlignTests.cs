﻿
using ScrubJay.Comparison;
using ScrubJay.Memory;

namespace ScrubJay.Text.Tests.TextBuilderTests;

public class AlignTests
{
    [Fact]
    public void CanAlignChar()
    {
        using var textBuilder = new TextBuilder();

        Assert.Throws<ArgumentOutOfRangeException>(() => textBuilder.Align('a', 0, Alignment.Center));
        textBuilder.Align('b', 1, Alignment.Left);
        Assert.Equal('b', textBuilder[0]);
        textBuilder.Align('c', 1, Alignment.Right);
        Assert.Equal('c', textBuilder[1]);
        textBuilder.Align('d', 1, Alignment.Center);
        Assert.Equal('d', textBuilder[2]);

        textBuilder.Align('e', 2, Alignment.Left);
        Assert.Equal("e ", textBuilder[^2..].ToString());
        textBuilder.Align('f', 2, Alignment.Right);
        Assert.Equal(" f", textBuilder[^2..].ToString());
        textBuilder.Align('g', 2, Alignment.Center);
        Assert.Equal("g ", textBuilder[^2..].ToString());
        textBuilder.Align('h', 2, Alignment.Center | Alignment.Right);
        Assert.Equal(" h", textBuilder[^2..].ToString());
        
        textBuilder.Align('i', 3, Alignment.Left);
        Assert.Equal("i  ", textBuilder[^3..].ToString());
        textBuilder.Align('j', 3, Alignment.Right);
        Assert.Equal("  j", textBuilder[^3..].ToString());
        textBuilder.Align('k', 3, Alignment.Center);
        Assert.Equal(" k ", textBuilder[^3..].ToString());
    }

    [Fact]
    public void CanAlignText()
    {
        using var textBuilder = new TextBuilder();

        var testStrings = new string?[] { (string?)null, string.Empty, "1", "22" };
        var testWidths = new int[] { 0, 1, 2, 3, 4, 5, 6 };
        
        foreach (string? testString in testStrings)
        foreach (int testWidth in testWidths)
        {
            int len = testString?.Length ?? 0;
            if (len > testWidth)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => textBuilder.Align(testString, testWidth, Alignment.Center));
            }
            else
            {
                ReadOnlySpan<char> testText = testString.AsSpan();
                Span<char> wrote;
                
                // Left
                textBuilder.Align(testString, testWidth, Alignment.Left);
                wrote = textBuilder.AsSpan()[^testWidth..];
                Assert.Equal(testWidth, wrote.Length);
                Assert.True(SpanExtensions.StartsWith(wrote, testText));
                
                // Right
                textBuilder.Align(testString, testWidth, Alignment.Right);
                wrote = textBuilder.AsSpan()[^testWidth..];
                Assert.Equal(testWidth, wrote.Length);
                Assert.True(wrote.EndsWith(testText));
                
                // Center
                int spaces = testWidth - len;

                foreach (Alignment alignment in new[] { Alignment.Center, Alignment.Center | Alignment.Left, Alignment.Center | Alignment.Right })
                {
                    textBuilder.Align(testString, testWidth, alignment);
                    wrote = textBuilder.AsSpan()[^testWidth..];
                    var reader = new SpanEnumerator<char>(wrote);
                    Assert.Equal(testWidth, reader.UnenumeratedItems.Length);
                    
                    var frontSpaces = reader.TakeWhile(static ch => ch == ' ');
                    
                    var text = reader.TakeUntil(' ');
                    Assert.Equal(len, text.Length);
                    Assert.True(Relate.Equal.Sequence(text,testText));
                    
                    var backSpaces = reader.TakeWhile(static ch => ch == ' ');
                    Assert.Equal(0, reader.UnenumeratedItems.Length);

                    Assert.Equal(spaces, (frontSpaces.Length + backSpaces.Length));
                    
                    if (len > 0)
                    {
                        if (spaces % 2 == 0)
                        {
                            Assert.Equal(backSpaces.Length, frontSpaces.Length);
                        }
                        else if (alignment.HasFlag(Alignment.Right))
                        {
                            Assert.True(frontSpaces.Length > backSpaces.Length);
                        }
                        else
                        {
                            Assert.True(frontSpaces.Length < backSpaces.Length);
                        }
                    }
                    else
                    {
                        Assert.Equal(testWidth, frontSpaces.Length);
                    }
                }
            }
        }
    }
}