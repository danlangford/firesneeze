using System;
using UnityEngine;

public class GlossaryEntry : ScriptableObject
{
    [Tooltip("the main body of text when tapped in the rules panel")]
    public StrRefType Body;
    [Tooltip("whether the hyper-linking should be case-sensitive")]
    public bool CaseSensitive = true;
    [Tooltip("the category in the rules panel this entry should be put under")]
    public GlossaryCategory Category;
    [Tooltip("entries that cause a See Also at the end of the Body : ")]
    public GlossaryEntry[] LinkedEntries;
    [Tooltip("the title in the main body, the hyperlink key, and the link in the left underneath the categories")]
    public StrRefType Title;
}

