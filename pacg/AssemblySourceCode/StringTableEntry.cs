using System;
using System.Collections;
using System.Xml;

public class StringTableEntry
{
    public string DefaultText = string.Empty;
    public string FemaleText = string.Empty;
    public uint Package;
    public int StringID = -1;

    public StringTableEntry(XmlNode parentNode)
    {
        IEnumerator enumerator = parentNode.ChildNodes.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                XmlNode current = (XmlNode) enumerator.Current;
                if (current.Name == "ID")
                {
                    int result = -1;
                    if (int.TryParse(current.InnerText, out result))
                    {
                        this.StringID = result;
                    }
                }
                else
                {
                    if (current.Name == "DefaultText")
                    {
                        this.DefaultText = current.InnerText;
                        continue;
                    }
                    if (current.Name == "FemaleText")
                    {
                        this.FemaleText = current.InnerText;
                    }
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
    }
}

