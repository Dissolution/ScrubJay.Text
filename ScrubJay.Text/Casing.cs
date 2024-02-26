namespace ScrubJay.Text;

public enum Casing
{
    Default = 0,
    
    /// <summary>
    /// Lowercase<br/>
    /// <c>thisvar -> thisvar</c><br/>
    /// <c>thisVar -> thisvar</c><br/>
    /// <c>ThisVar -> thisvar</c><br/>
    /// <c>THISVAR -> thisvar</c><br/>
    /// </summary>
    Lower,
    
    /// <summary>
    /// Uppercase<br/>
    /// <c>thisvar -> THISVAR</c><br/>
    /// <c>thisVar -> THISVAR</c><br/>
    /// <c>ThisVar -> THISVAR</c><br/>
    /// <c>THISVAR -> THISVAR</c><br/>
    /// </summary>
    Upper,
    
    /// <summary>
    /// Camel case<br/>
    /// <c>thisvar -> thisvar</c><br/>
    /// <c>thisVar -> thisVar</c><br/>
    /// <c>ThisVar -> thisVar</c><br/>
    /// <c>THISVAR -> tHISVAR</c><br/>
    /// </summary>
    Camel,
    
    /// <summary>
    /// Pascal case<br/>
    /// <c>thisvar -> Thisvar</c><br/>
    /// <c>thisVar -> ThisVar</c><br/>
    /// <c>ThisVar -> ThisVar</c><br/>
    /// <c>THISVAR -> THISVAR</c><br/>
    /// </summary>
    Pascal,
}