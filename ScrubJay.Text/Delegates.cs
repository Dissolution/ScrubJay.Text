#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
global using text = System.ReadOnlySpan<char>;
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.


namespace ScrubJay.Text;

public delegate void TextAction(text text);
public delegate void TextAction<in T1>(text text, T1 arg1);
public delegate void TextAction<in T1, in T2>(text text, T1 arg1, T2 arg2);
public delegate void TextAction<in T1, in T2, in T3>(text text, T1 arg1, T2 arg2, T3 arg3);
public delegate void TextAction<in T1, in T2, in T3, in T4>(text text, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
public delegate void TextAction<in T1, in T2, in T3, in T4, in T5>(text text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
public delegate void TextAction<in T1, in T2, in T3, in T4, in T5, in T6>(text text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
public delegate void TextAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(text text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
public delegate void TextAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(text text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

public delegate TResult TextFunc<out TResult>(text text);
public delegate TResult TextFunc<in T1, out TResult>(text text, T1 arg1);
public delegate TResult TextFunc<in T1, in T2, out TResult>(text text, T1 arg1, T2 arg2);
public delegate TResult TextFunc<in T1, in T2, in T3, out TResult>(text text, T1 arg1, T2 arg2, T3 arg3);
public delegate TResult TextFunc<in T1, in T2, in T3, in T4, out TResult>(text text, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
public delegate TResult TextFunc<in T1, in T2, in T3, in T4, in T5, out TResult>(text text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
public delegate TResult TextFunc<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>(text text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
public delegate TResult TextFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TResult>(text text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
public delegate TResult TextFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TResult>(text text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);