
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using ScrubJay.Text.Dumping;

var thing = (147, "TJ");

var d= Dump.Value(thing);

Console.WriteLine(d);
Debugger.Break();
Console.ReadLine();
return 0;