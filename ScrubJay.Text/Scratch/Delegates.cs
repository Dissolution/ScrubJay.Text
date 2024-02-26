namespace ScrubJay.Text.Scratch;

public delegate void Build(TextFormatter formatter);
public delegate void BuildWithText(TextFormatter formatter, text text);
public delegate void BuildWithValue<in T>(TextFormatter formatter, T value);
public delegate void BuildWithIndexedValue<in T>(TextFormatter formatter, T value, int index);