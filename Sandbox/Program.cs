
using System.Diagnostics;
using ScrubJay.Text.Builders;

int id = Random.Shared.Next();

var text = TextBuilder.New
    .AppendLine($"{1234:N0}")
    .AppendLine($"{4567:N0}")
    .AppendLine($"{6789:N0}")
    .ToStringAndDispose();

Console.WriteLine(text);


Debugger.Break();
Console.ReadLine();
return 0;
